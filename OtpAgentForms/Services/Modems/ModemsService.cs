using Microsoft.Extensions.DependencyInjection;
using Otp.Agent.Settings;
using OtpAgentForms.Api;
using OtpAgentForms.Dto.Base;
using OtpAgentForms.Logging;
using OtpAgentForms.Modem;
using OtpAgentForms.Services.Modems;
using System.Collections.Concurrent;
using System.Data;
using System.IO.Ports;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OtpAgentForms
{
    /// <summary>
    /// Modem service implementation
    /// <see cref="IModemsService"/>
    /// </summary>
    internal class ModemsService : IModemsService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Modems list
        /// Key => serial port name
        /// Value modem instance
        /// </summary>
        private readonly ConcurrentDictionary<string, IModem> _modems;

        /// <summary>
        /// Serial ports with errors
        /// Key => serial port name
        /// Value => date time next attempt
        /// </summary>
        private readonly ConcurrentDictionary<string, DateTime> _failureModems;

        /// <summary>
        /// Api connector
        /// </summary>
        private readonly IApiConnector _apiConnector;

        /// <summary>
        /// Global Form1
        /// </summary>
        private Form1 globalForm = null;

        /// <summary>
        /// Init service
        /// </summary>        
        public ModemsService(ILogger logger, IServiceProvider serviceProvider, IApiConnector apiConnector)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _apiConnector = apiConnector;
            _modems = new();
            _failureModems = new();
            UpdateTitle();
        }

        /// <inheritdoc/>
        public void Start(CancellationToken ct, Form form)
        {
            globalForm = (Form1)form;
            new Task(() => SearchPorts(ct, form), TaskCreationOptions.LongRunning).Start();            
            //Task.Run(() => SearchPorts(ct), ct);
        }

        /// <summary>
        /// Search serial ports
        /// </summary>
        private void SearchPorts(CancellationToken ct, Form form)
        {
            const int tryConnectSeconds = 10;

            try
            {
                var delete = _apiConnector.DeleteAllModems(form);
                while (delete?.Status != ResponseStatus.ResponseOk)
                {
                    // close thread
                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }

                    _logger.WriteError($"Start agent failed. Cannot delete modems from server. I'll try again in {tryConnectSeconds} seconds");
                    delete = _apiConnector.DeleteAllModems(form);
                    Task.Delay(tryConnectSeconds * 1000, ct).Wait(ct);
                }
                _logger.WriteInformation($"Delete modems from server: Success");

                while (true)
                {
                    // close thread
                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }

                    string[] ports = SerialPort.GetPortNames();
                    globalForm.ShowTotalPortsNumber(ports.Count() - 1);
                    foreach (var port in ports)
                    {
                        if (_modems.ContainsKey(port) || AppSettings.Current.General.ExcludePortList.Where(x => x.ToUpper() == port.ToUpper()).Any())
                        {
                            continue;
                        }
                        if (_failureModems.ContainsKey(port) && _failureModems[port] > DateTime.Now)
                        {
                            continue;
                        }
#if DEBUG
                        if (port != "COM25")
                        {
                            //continue;
                        }
                        if (_modems.Count >= 10)
                        {
                            //continue;
                        }
#endif
                        _logger.WriteDebug("Trying to connect to modem", port);
                        /*if (_failureModems.ContainsKey(port))
                        {
                            var form1 = (Form1)form;
                            form1.FindRowToDelete(port);
                        }*/
                        _failureModems.TryRemove(port, out var _);
                        var modem = _serviceProvider.GetRequiredService<IModem>();
                        modem.OnChanged += OnModemChanged;
                        modem.OnFailure += OnModemFailure;
                        _modems.TryAdd(port, modem);
                        //var form1 = (Form1)form;
                        //form1.AddRow(_modems.Count);
                        
                        Task.Run(() => modem. Init(port, form), ct);
                        //globalForm.ShowTotalPortsNumber();
                        Task.Delay(200, ct).Wait(ct);
                    }

                    Task.Delay(1000, ct).Wait(ct);
                }
            }
            catch (OperationCanceledException)
            {
                // dispose modems
                foreach (var modem in _modems)
                {
                    modem.Value.Dispose();
                }
                return;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// On modem failure callback
        /// </summary>
        /// <param name="portName">Failure modem port name</param>
        private void OnModemFailure(string portName)
        {
            if (!_modems.TryGetValue(portName, out var modem))
            {
                UpdateTitle();
                return;
            }
            if (modem.Id != null)
            {
                _apiConnector.DeleteModem((Guid)modem.Id, portName);
            }
            _logger.WriteInformation($"Modem deleted", portName);
            globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 2, $"");
            globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 6, $"Modem deleted");
            
            //globalForm.label1.Text = int.TryParse(globalForm.label1.Text, out number);

            if (_modems.ContainsKey(portName))
            {
                _modems[portName].Dispose();
                //Form1.ChangeGrid(_modems[portName].Row, -1, "");
                _modems.TryRemove(portName, out var _);
            }


            _failureModems.TryAdd(portName, DateTime.Now.AddSeconds(10));
            //globalForm.ChangePortsCount(_modems.Count, _failureModems.Count);
            UpdateTitle();
        }

        /// <summary>
        /// On modem changed callback
        /// </summary>
        /// <param name="portName">Failure modem port name</param>
        private void OnModemChanged(string portName)
        {
            var modem = _modems[portName];
            if (modem.Id == null)
            {
                modem.SetId();
                _logger.WriteInformation($"Modem initialized", portName);
                globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 6, $"Modem initialized");
                _logger.WriteDebug($"Modem initialized:\n" +
                    $"ID: {modem.Id} \n" +
                    $"Port name: {modem.PortName} \n" +
                    $"Model: {modem.Model} \n" +
                    $"IMEI: {modem.Imei} \n" +
                    $"SN: {modem.SerialNumber}\n" +
                    $"RSSI: {modem.Rssi} \n" +
                    $"IMSI: {modem.Imsi} \n" +
                    $"SIM status: {modem.SimCardStatus} \n" +
                    $"Phone number: {modem.SimCard?.PhoneNumber} \n", portName);
            }
            _apiConnector.AddEditModem(modem, portName);
            //globalForm.ChangePortsCount(_modems.Count, _failureModems.Count);
            UpdateTitle();
        }

        /// <summary>
        /// Update console title
        /// </summary>
        private void UpdateTitle()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }
            
            string[] ports = SerialPort.GetPortNames();

        }
    }
}