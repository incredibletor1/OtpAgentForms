using Otp.Agent.Settings;
using OtpAgentForms.Api;
using OtpAgentForms.Attributes;
using OtpAgentForms.Connector;
using OtpAgentForms.Dto.Provider.Models;
using OtpAgentForms.Dto.SimCard.Models;
using OtpAgentForms.Hardware;
using OtpAgentForms.Hardware.Commands;
using OtpAgentForms.Hardware.Commands.Base;
using OtpAgentForms.Helpers;
using OtpAgentForms.Logging;
using OtpAgentForms.Models;
using OTPAgent.Helpers;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data;
using OtpAgentForms.Dto.Modem.Models;

namespace OtpAgentForms.Modem
{
    /// <summary>
    /// Modem object
    /// <see cref="IModem"/>
    /// </summary>
    internal class Modem : IModem, IDisposable
    {
        const string phonebookSerialName = "PortNum";
        const int serialPhonebookPosition = 8;
        const int updateModemIntervalSec = 10;

        #region Modem properties 
        /// <inheritdoc/>
        public Guid? Id { get; private set; } = null;

        /// <inheritdoc/>
        public string PortName { get; private set; }

        /// <inheritdoc/>
        public int Row { get; set; }

        /// <inheritdoc/>
        public string Model { get; private set; }

        /// <inheritdoc/>
        public string SerialNumber { get; private set; }

        /// <inheritdoc/>
        public ulong Imei { get; private set; }

        /// <inheritdoc/>
        public ulong Imsi { get; private set; }

        /// <inheritdoc/>
        public SimCardStatus SimCardStatus { get; private set; } = SimCardStatus.NOTREADY;

        /// <inheritdoc/>
        public uint Rssi { get; private set; }

        /// <inheritdoc/>
        public SimCard SimCard { get; private set; }

        /// <inheritdoc/>
        public int ModemErrorCount { get; private set; }

        /// <inheritdoc/>
        public string Error { get; set; }
        #endregion

        /// <inheritdoc/>
        public event ModemEventDelegate OnFailure;

        /// <summary>
        public event ModemEventDelegate OnChanged;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Api connector
        /// </summary>
        private readonly IApiConnector _apiConnector;

        /// <summary>
        /// Modem connector
        /// </summary>
        private readonly IModemConnector _serialPortService;

        /// <summary>
        /// Cancellation token
        /// </summary>
        private readonly CancellationToken _cancellation;

        /// <summary>
        /// Cancellation token source
        /// </summary>
        private readonly CancellationTokenSource _tokenSource;

        /// <summary>
        /// Health check task
        /// </summary>
        private Task healthCheck = null;

        /// <summary>
        /// Sms sender task
        /// </summary>
        private Task smsSender = null;

        /// <summary>
        /// Global form
        /// </summary>
        private Form1 globalForm = null;

        /// <summary>
        /// sms list to send 
        /// </summary>
        private readonly ConcurrentQueue<int> _smsList;

        private List<string> errorPorts = new List<string>();

        /// <summary>
        /// Is modem initialized flag
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// Init modem
        /// </summary>     
        public Modem(ILogger logger, IApiConnector apiConnector, IModemConnector serialPortService)
        {
            _logger = logger;
            _apiConnector = apiConnector;
            _serialPortService = serialPortService;
            _smsList = new();
            _tokenSource = new();
            _cancellation = _tokenSource.Token;
        }

        /// <inheritdoc/>
        public void SetId()
        {
            if (Id == null)
            {
                Id = Guid.NewGuid();
            }
        }

        /// <inheritdoc/>
        public void Init(string portName, Form form)
        {
            try
            {
                globalForm = (Form1)form;

                // check port name
                if (string.IsNullOrEmpty(portName))
                {
                    throw new ArgumentNullException("Port name not set");
                }
                PortName = portName;
                
                var form1 = (Form1)form;
                if (!form1.dataTable.Rows.Contains(portName))
                {
                    Row = form1.AddRow(portName);
                }
                else
                {
                    Row = form1.GetRowIndex(portName);
                }

                // Detect modem baud rate
                var baudRate = DetectSpeed();
                if (baudRate == 0)
                {
                    throw new Exception("Unable to detect modem baud rate");
                }
                _logger.WriteInformation($"Find modem with baudrate {baudRate}", PortName);
                form1.ChangeGrid(Row, 6, $"Find modem with baudrate {baudRate}");


                // build serial port 
                var serialPort = new SerialPort(PortName)
                {
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    DtrEnable = false,
                    RtsEnable = false,
                    Handshake = Handshake.None,
                    BaudRate = baudRate,
                };
                serialPort.Open();

                // base init
                serialPort.WriteLine("AT&F");
                serialPort.WriteLine("ATE1");
                Task.Delay(1000).Wait();
                serialPort.ReadExisting();
                serialPort.Close();
                serialPort.Dispose();

                // run serial port service
                var run = _serialPortService.Start(PortName, baudRate, form);
                if (!run)
                {
                    throw new Exception("Serial port connector start filed");
                }
                _serialPortService.ModemEvent += ReceivedHandler;

                // change imei
                if (AppSettings.Current.General.ChangeImei)
                {
                    var newImei = ImeiHelper.GenerateRandomImei();
                    if (!string.IsNullOrEmpty(newImei))
                    {
                        _serialPortService.DoRequest(AtRequest.SetImei(newImei));
                        _logger.WriteInformation($"Set new imei: {newImei}", PortName);
                        form1.ChangeGrid(Row, 6, $"Set new imei: {newImei}");
                    }
                }

                _serialPortService.DoRequest(AtRequest.EnablePhone());
                _serialPortService.DoRequest(AtRequest.SetTextMode());
                _serialPortService.DoRequest(AtRequest.SetTeCharacter(TeCharacter.UCS2));

                _serialPortService.DoRequest(AtRequest.SetInternalSmsMemory());
                _serialPortService.DoRequest(AtRequest.SetInternalPhonebookMemory());
                _serialPortService.DoRequest(AtRequest.EnableNewSmsIndication());
                _serialPortService.DoRequest(AtRequest.EnableCsqEcho());                
                //_serialPortService.DoRequest(AtRequest.GetSimCardStatus());
                //_serialPortService.DoRequest(AtRequest.GetSimCardStatus());

                // get modem model
                var gmmResponse = SendCommand<GmmResponse>(AtRequest.GetModelName());                
                if (gmmResponse == null || gmmResponse.Error != null)
                {
                    throw new Exception("Unable get modem model name");
                }
                Model = gmmResponse.Model;
                //form1.ChangeGrid(Row, 1, Model);

                // get modem imei
                var gsnResponse = SendCommand<GsnResponse>(AtRequest.GetImei());
                if (gsnResponse == null || gsnResponse.Error != null)
                {
                    throw new Exception("Unable get modem imei");                    
                }
                Imei = gsnResponse.Imei;
                //form1.ChangeGrid(Row, 1, Model);

                // get signal quality
                var csqResponse = _serialPortService.DoRequest(AtRequest.GetSignalQuality());
                SignalQualityHandler(csqResponse);
                form1.ChangeGrid(Row, 3, Rssi.ToString());
                // get serial number from phonebook
                SerialNumber = GetSerialNumber(Imei);
                if (string.IsNullOrEmpty(SerialNumber) || SerialNumber == "0")
                {
                    throw new Exception($"Unable get modem serial number");
                }

                if (form1.portSerialNumberInfo.ContainsKey(SerialNumber))
                {
                    var (hubName, portNumber) = form1.portSerialNumberInfo[SerialNumber];
                    form1.ChangeGrid(Row, 4, hubName);
                    form1.ChangeGrid(Row, 5, portNumber);
                }
                else
                {
                    form1.ChangeGrid(Row, 4, "UNKNOWN");
                    form1.ChangeGrid(Row, 5, "UNKNOWN");
                }
                //form1.ChangeGrid(Row, 2, SerialNumber);

                // init modem
                var simCardStatusResponse = _serialPortService.DoRequest(AtRequest.GetSimCardStatus());
                SimCardHandler(simCardStatusResponse);
                
                // is sim card ready
                if (SimCardStatus != SimCardStatus.READY)
                {
                    throw new Exception($"Unable start modem without sim card. Please insert sim card");
                }
                if (SimCard == null)
                {
                    throw new Exception($"Unable to detect sim card. Please check sim card");
                }
                form1.ChangeGrid(Row, 2, SimCard.PhoneNumber);
                form1.ChangeGrid(Row, 1, SimCard.Provider.Name);


                // read sms
                var sms = SendCommand<CmglResponse>(AtRequest.ReadAllSms());
                if (sms != null && sms.SMS != null)
                {
                    foreach (var newSms in sms.SMS)
                    {
                        _smsList.Enqueue(newSms.Id);
                    }
                }
                initialized = true;
                // add modem to server
                OnChanged?.Invoke(PortName);

                // start sms sender task 
                smsSender = new Task(() => SmsSender());
                smsSender.Start();

                // start health check
                healthCheck = new Task(() => ModemHealthCheck(form));
                healthCheck.Start();
                return;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.Message, PortName);
                OnFailure?.Invoke(PortName);
            }
        }

        /// <summary>
        /// get serial number from phonebook
        /// </summary>                
        private string GetSerialNumber(ulong imei)
        {
            var phonebookItemName = Ucs2.ToUCS2(phonebookSerialName);
            var phonebookItem = SendCommand<CpbrResponse>(AtRequest.ReadPhoneBook(serialPhonebookPosition));
            if (phonebookItem == null || phonebookItem.Error != null)
            {
                return null;
            }
            if (phonebookItem.Number != null && !string.IsNullOrEmpty(phonebookItem.Name) && phonebookItem.Name == phonebookItemName)
            {
                return phonebookItem.Number;
            }

            if (AppSettings.Current.General.DisablePhonebookWrite)
            {
                return null;
            }

            SendCommand<CpbwResponse>(AtRequest.DeletePhoneBook(serialPhonebookPosition));
            var wrireResponse = SendCommand<CpbwResponse>(AtRequest.WritePhoneBook(serialPhonebookPosition, imei.ToString(), phonebookItemName));
            if (wrireResponse == null || wrireResponse.Error != null)
            {
                return null;
            }
            return imei.ToString();
        }

        /// <summary>
        /// Received message handler
        /// </summary>        
        private void ReceivedHandler(string line)
        {
            Regex responseHeaderPettern = new(@"^\r\n\+\w+:");
            var commandHeader = responseHeaderPettern.Matches(line)
                .FirstOrDefault()
                .ToString()
                .ClearCLRF();

            var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            var handlers = methods.Where(x => x.GetCustomAttributes(typeof(ModemHanlerForAttribute)).Any());
            var handler = handlers.FirstOrDefault(x => x.GetCustomAttribute<ModemHanlerForAttribute>().At == commandHeader);
            handler?.Invoke(this, parameters: new[] { line });
        }

        #region Handlers
        /// <summary>
        /// Handler for +CMTI received
        /// </summary>
        [ModemHanlerFor("+CMTI:")]
        private void NewSmsHandler(string message)
        {
            if(!initialized)
            {
                return;
            }
            try
            {
                var response = new CmtiResponse().Parse(message);
                if (response == null || response.Error != null)
                {
                    ModemErrorCount++;
                    return;
                }
                if (response.Position == 0)
                {
                    return;
                }
                _logger.WriteDebug($"Received new sms. Position={response.Position} Memory={response.MemType}", PortName);
                _smsList.Enqueue(response.Position);
                var smsResponse = SendCommand<CmgrResponse>(AtRequest.ReadSms(response.Position));
                if (smsResponse == null || smsResponse.Error != null || smsResponse.SMS == null)
                {
                    _logger.WriteError($"Unable to read sms from memory. Position={response.Position} Memory={response.MemType}", PortName);
                    globalForm.ChangeGrid(Row, 6, $"Unable to read sms from memory. Position={response.Position} Memory={response.MemType}");
                    ModemErrorCount++;
                    return;
                }
                _logger.WriteInformation($"Received new sms from {smsResponse.SMS.SourceNumber}: {smsResponse.SMS.Message}", PortName);
                globalForm.ChangeGrid(Row, 6, $"Received new sms from {smsResponse.SMS.SourceNumber}: {smsResponse.SMS.Message}");
            }
            catch(Exception ex)
            {
                _logger.WriteInformation(ex.Message, PortName);
            }
        }

        /// <summary>
        /// Handler for +CSQN received
        /// </summary>
        [ModemHanlerFor("+CSQN:")]
        private void SignalQualityHandler(string message)
        {
            var response = new CsqResponse().Parse(message);
            if (response == null || response.Error != null)
            {
                ModemErrorCount++;
                return;
            }
            Rssi = response.Rssi;
        }

        /// <summary>
        /// Handler for +CPIN received
        /// </summary>
        [ModemHanlerFor("+CPIN:")]
        private void SimCardHandler(string message)
        {
            try
            {
                var response = new CpinResponse().Parse(message);
                if (response == null || response.Error != null && response.Error != ErrorList.SimNotInserted)
                {
                    throw new Exception("Unable to get current sim card status");
                }
                SimCardStatus = response.Status;

                if(SimCardStatus == SimCardStatus.NOTREADY)
                {
                    _logger.WriteInformation($"sim card removed", PortName);
                    globalForm.ChangeGrid(Row, 6, $"sim card removed");
                }

                Imsi = 0;
                SimCard = null;
                if (SimCardStatus == SimCardStatus.READY)
                {
                    globalForm.ChangeGrid(Row, 6, $"Sim card inserted");
                    _logger.WriteInformation($"Sim card inserted", PortName);
                    // get imsi
                    //var cimiResponse = SendCommand<CimiResponse>(AtRequest.GetImsi());
                    //if (cimiResponse == null || cimiResponse.Error != null)
                    //{
                    //    throw new Exception("Unable to get sim card imsi");
                    //}
                    //Imsi = cimiResponse.Imsi;
                    //SimCard = BuildSimCard();

                    for (var i = 0; i <= 10; i++)
                    {
                        try
                        {
                            SimCard = GetSimCard();
                            Error = null;
                        }
                        catch (Exception ex)
                        {
                            Error = ex.Message;
                            globalForm.ChangeGrid(Row, 6, $"Sim card change error:  {ex.Message}");
                            _logger.WriteError($"Sim card change error:  {ex.Message}", PortName);
                            Task.Delay(5000).Wait();
                        }
                        finally
                        {
                            if (string.IsNullOrEmpty(Error))
                            {
                                i = 11;
                            }
                            //form.ChangeGrid(Row, 2, SimCard.PhoneNumber);
                            OnChanged?.Invoke(PortName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                globalForm.ChangeGrid(Row, 6, $"Sim card change error:  {ex.Message}");
                _logger.WriteError($"Sim card change error:  {ex.Message}", PortName);
                OnFailure?.Invoke(PortName);                
                return;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private SimCard GetSimCard()
        {
            // get imsi
            var cimiResponse = SendCommand<CimiResponse>(AtRequest.GetImsi());
            if (cimiResponse == null || cimiResponse.Error != null)
            {
                throw new Exception("Unable to get sim card imsi");
            }
            Imsi = cimiResponse.Imsi;
            return BuildSimCard();
        }

        /// <summary>
        /// Send command to modem
        /// </summary>
        /// <typeparam name="T">Type of response</typeparam>
        /// <param name="command">AT command</param>
        /// <param name="timeoutSec">Timeout in sec. Default value - 3</param>
        /// <returns>Response T</returns>
        private T SendCommand<T>(string command, int timeoutSec = 5) where T : IAtResponse<T>, new()
        {
            var response = _serialPortService.DoRequest(command, timeoutSec);
            return response != null
                ? new T().Parse(response)
                : default;
        }

        /// <summary>
        /// Sms sender process
        /// </summary>
        private void SmsSender()
        {
            while (true)
            {
                try
                {
                    if (SimCard == null)
                    {
                        Task.Delay(1000, _cancellation).Wait(_cancellation);
                        continue;
                    }

                    if (_cancellation.IsCancellationRequested)
                    {
                        _cancellation.ThrowIfCancellationRequested();
                    }


                    // if queue empty
                    if (!_smsList.TryPeek(out var smsPosition))
                    {
                        Task.Delay(1000, _cancellation).Wait(_cancellation);
                        continue;
                    }

                    var sms = SendCommand<CmgrResponse>(AtRequest.ReadSms(smsPosition));
                    if (sms == null || sms.Error != null || sms.SMS == null)
                    {
                        _logger.WriteError($"Unable to read sms from memory. Position={smsPosition}", PortName);
                        globalForm.ChangeGrid(Row, 6, $"Unable to read sms from memory. Position={smsPosition}");
                        ModemErrorCount++;
                        continue;
                    }

                    var response = _apiConnector.AddSms(sms.SMS, SimCard.Id);
                    if (response == null || (response.Id == null & response.ErrorCode != 400))
                    {
                        ModemErrorCount++;
                        continue;
                    }
                    _smsList.TryDequeue(out var _);
                    var delete = SendCommand<CmgdResponse>(AtRequest.DeleteSms(smsPosition));
                    if (delete == null || delete.Error != null)
                    {
                        ModemErrorCount++;
                        Task.Delay(1000, _cancellation).Wait(_cancellation);
                        continue;
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch(Exception ex)
                {
                    globalForm.ChangeGrid(Row, 6, $"Push sms error: {ex.Message}");
                    _logger.WriteError($"Push sms error: {ex.Message}", PortName);
                    Task.Delay(1000, _cancellation).Wait(_cancellation);
                }
            }
        }

        /// <summary>
        /// Check is modem online
        /// </summary>
        private void ModemHealthCheck(Form form)
        {
            while (true)
            {
                try
                {
                    Task.Delay(updateModemIntervalSec * 1000, _cancellation).Wait(_cancellation);
                    if (_cancellation.IsCancellationRequested)
                    {
                        _cancellation.ThrowIfCancellationRequested();
                    }


                    // get imsi
                    var cimiResponse = SendCommand<CimiResponse>(AtRequest.GetImsi());
                    if (cimiResponse == null || cimiResponse.Error != null)
                    {
                        globalForm.ChangeGrid(Row, 6, $"Get imsi error");
                        _logger.WriteError($"Get imsi error", PortName);
                        Task.Run(() => OnFailure?.Invoke(PortName));
                        continue;
                    }

                    if(cimiResponse.Imsi != 0 && Imsi != cimiResponse.Imsi)
                    {
                        var cpinResponse = SendCommand<CpinResponse>(AtRequest.GetSimCardStatus());
                        SimCardHandler(cpinResponse.RawResponse);

                    }

                    // check at
                    var atResponse = SendCommand<AtResponse>(AtRequest.AT(), 10);
                    if (atResponse == null || atResponse.Error != null)
                    {
                        ModemErrorCount++;
                    }
                    else
                    {
                        //ModemErrorCount = 0;
                    }

                    // check error count
                    if (ModemErrorCount >= AppSettings.Current.General.ErrorCount)
                    {
                        globalForm.ChangeGrid(Row, 6, $"The number of errors has exceeded the threshold");
                        _logger.WriteError($"The number of errors has exceeded the threshold", PortName);
                        Task.Run(() => OnFailure?.Invoke(PortName));
                        continue;
                    }

                    // update modem on server
                    OnChanged?.Invoke(PortName);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Modem baud rate detect
        /// </summary>
        /// <returns>Baud rate, or 0 if error</returns>
        private int DetectSpeed()
        {
            SerialPort serialPort = null;

            // get serial port baud rates
            var baudRates = ((int[])Enum.GetValues(typeof(SerialPortBaudRate)))
                          .OrderByDescending(x => x);

            // try connect
            foreach (var rate in baudRates)
            {
                try
                {
                    // new serial port instance
                    serialPort = new SerialPort(PortName)
                    {
                        Parity = Parity.None,
                        DataBits = 8,
                        StopBits = StopBits.One,
                        DtrEnable = false,
                        RtsEnable = false,
                        WriteTimeout = 1000,
                        ReadTimeout = 1000,
                        BaudRate = rate
                    };

                    // connect
                    serialPort.Open();
                    // sent test command
                    serialPort.WriteLine(AtRequest.AT());
                    Task.Delay(300).Wait();
                    // repeat test command
                    serialPort.WriteLine(AtRequest.AT());
                    // wait response
                    Task.Delay(1000).Wait();
                    // get response
                    var response = serialPort.ReadExisting();
                    // return baud rate if response success
                    if (!string.IsNullOrEmpty(response) && (response.Contains("AT\r\nOK\r\n") || response.Contains("\r\nOK\r\n")))
                    {
                        return rate;
                    }
                    Task.Delay(100).Wait();
                }
                catch
                {
                    return 0;
                }
                finally
                {
                    // close port if open
                    if (serialPort != null)
                    {
                        serialPort.Dispose();
                        serialPort = null;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Dispose modem
        /// </summary>
        public void Dispose()
        {
            _tokenSource.Cancel();
            _serialPortService.Dispose();
            healthCheck?.Wait();
            smsSender?.Wait();
            healthCheck = null;
            smsSender = null;
            _tokenSource.Dispose();
            GC.Collect();
        }

        /// <summary>
        /// Build sim card
        /// </summary>
        /// <returns><see cref="SimCard"/></returns>
        private SimCard BuildSimCard()
        {
            Provider provider = null;
            // check imsi
            if (Imsi == 0)
            {
                throw new Exception("Imsi can not be 0");
            }
            
            // get sim card
            var simCard = _apiConnector.GetSimCard(Imsi);
            if (simCard != null)
            {
                return simCard;
            }

            // get provider
            globalForm.ChangeGrid(Row, 6, $"New sim card");
            _logger.WriteInformation($"New sim card", PortName);
            provider = _apiConnector.GetProviderByImsi(Imsi);
            if (provider == null)
            {
                globalForm.ChangePortsCount(PortName);
                throw new Exception($"Unable to get provider. Imsi={Imsi}");                                
            }
            globalForm.ChangeGrid(Row, 6, $"Provider name is {provider.Name} ussd: {provider.UssdGetNumber}");
            _logger.WriteInformation($"Provider name is {provider.Name} ussd: {provider.UssdGetNumber}", PortName);
            if(provider.Name.ToUpper() == "AIS")
            {
                SendCommand<AtResponse>($"ATD*120;");
                Task.Delay(5000).Wait();
            }

            // get number
            globalForm.ChangeGrid(Row, 6, $"Send USSD  {provider.UssdGetNumber}");
            _logger.WriteInformation($"Send USSD  {provider.UssdGetNumber}", PortName);
            var ussd = SendCommand<CusdWriteResponse>(AtRequest.SenUssd(Ucs2.ToUCS2(provider.UssdGetNumber)), 20);
            if (ussd == null || ussd.UssdResponse == null || ussd.Error != null)
            {
                globalForm.ChangePortsCount(PortName);
                Task.Delay(AppSettings.Current.General.NumberphoneDelay * 1000).Wait();
                throw new Exception($"Failed get phone number. Imsi={Imsi}");
            }

            string convertedUssd = Ucs2.ToString(ussd.UssdResponse);
            char[] separators = new char[] { ' ', '+', '-', '(', ')' };
            convertedUssd = convertedUssd.Replace(separators, "");
            var number = Regex.Matches(convertedUssd, @"\d+")
                                        .Select(x => x.Value)
                                        .Where(x => x.Length > 8)
                                        .FirstOrDefault();

            if (string.IsNullOrEmpty(number))
            {
                globalForm.ChangePortsCount(PortName);
                Task.Delay(AppSettings.Current.General.NumberphoneDelay * 1000).Wait();
                throw new Exception($"Unable to parse ussd response {convertedUssd}");
            }

            if (number[..1] == "0")
            {
                number = number[1..];
            }

            if (!(number.Length >= 10 && Regex.IsMatch(number, @$"^{provider.Country.PhoneCode}\d+$")))
            {
                number = $"{provider.Country.PhoneCode}{number}";
            }
            if(number.Length >= 15)
            {
                globalForm.ChangePortsCount(PortName);
                throw new Exception($"Number {number} is long");
            }
            globalForm.ChangeGrid(Row, 6, $"Number parsed: {number}");
            _logger.WriteInformation($"Number parsed: {number}", PortName);
            simCard = new SimCard()
            {
                Imsi = Imsi,
                PhoneNumber = number,
                ProviderId = provider.Id,
            };

            var response = _apiConnector.AddSimCard(simCard);
            if(response == null)
            {
                globalForm.ChangePortsCount(PortName);
                throw new Exception($"Unable to add sim card. Try again");
            }
            globalForm.CheckPortsCount(PortName);
            return response;
        }
    }
}