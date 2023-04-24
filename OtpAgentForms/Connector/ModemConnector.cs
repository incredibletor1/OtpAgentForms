using OtpAgentForms.Helpers;
using OtpAgentForms.Logging;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace OtpAgentForms.Connector
{
    /// <summary>
    /// Serial port service implementation
    /// <see cref="IModemConnector"/>
    /// </summary>
    internal sealed class ModemConnector : IModemConnector, IDisposable
    {
        /// <summary>
        /// Modem event
        /// </summary>
        public event ModemEventDelegate ModemEvent;

        /// <summary>
        /// Command sender
        /// </summary>
        private Task _sender = null;

        /// <summary>
        /// Serial port name
        /// </summary>
        private string PortName;

        /// <summary>
        /// Serial port object
        /// </summary>
        private SerialPort _serialPort;

        /// <summary>
        /// Serial messages queue
        /// </summary>
        private ConcurrentDictionary<Guid, ModemMessage> _messageQueue = new();
        private Guid? _lastCommandId = null;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Cancellation token
        /// </summary>
        private readonly CancellationToken _cancellation;

        /// <summary>
        /// Cancellation token source
        /// </summary>
        private readonly CancellationTokenSource _tokenSource;

        /// <summary>
        /// Global form
        /// </summary>
        private Form1 globalForm = null;

        /// <summary>
        /// Init connector
        /// </summary>
        public ModemConnector(ILogger logger)
        {
            _logger = logger;
            _tokenSource = new();
            _cancellation = _tokenSource.Token;
        }

        /// <inheritdoc/>
        public bool Start(string portName, int baudRate, Form form)
        {
            try
            {
                globalForm = (Form1)form;
                // init serial port
                PortName = portName;
                _serialPort = new(PortName)
                {
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    DtrEnable = false,
                    RtsEnable = false,
                    Handshake = Handshake.None,
                    BaudRate = baudRate,
                    ReceivedBytesThreshold = 1
                };
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                _serialPort.Open();

                // start serial command sender
                _sender = new Task(() => SerialPortSender(), TaskCreationOptions.LongRunning);
                _sender.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public string DoRequest(string request, int timeout = 5)
        {
            // cancellation
            CancellationTokenSource cts = new();
            cts.CancelAfter(timeout * 1000);
            CancellationToken ct = cts.Token;

            // build modem message
            var data = new ModemMessage(request);
            try
            {
                // add request to queue
                _messageQueue.TryAdd(data.MessageId, data);

                // waiting for a request to be received
                while (!data.ResponseHasBeenReceived)
                {
                    // if timeout
                    if (ct.IsCancellationRequested)
                    {
                        var e = DateTime.Now;
                        ct.ThrowIfCancellationRequested();
                    }
                    Task.Delay(5, ct).Wait(ct);
                }
                // get response
                return _messageQueue[data.MessageId].Response;
            }
            catch (OperationCanceledException)
            {
                // timeout exception
                globalForm.ChangeGrid(globalForm.GetRowIndex(PortName), 6, $"Unable to execute command {request}. Request timeout.");
                _logger.WriteError($"Unable to execute command {request}. Request timeout.", PortName);
                return null;
            }
            catch(Exception ex)
            {
                globalForm.ChangeGrid(globalForm.GetRowIndex(PortName), 6, $"Unable to execute command {request}: {ex.Message}");
                _logger.WriteError($"Unable to execute command {request}: {ex.Message}", PortName);
                return null;
            }
            finally
            {
                //lock (_locker)
                //{
                // release resources
                if (_lastCommandId != null && _lastCommandId == data.MessageId)
                {
                    _lastCommandId = null;
                }
                _messageQueue?.TryRemove(data.MessageId, out var _);
                //}
                cts.Dispose();
            }
        }


        string line = string.Empty;


        /// <summary>
        /// Received data handler
        /// </summary>
        /// <param name="sender">Serial port</param>
        /// <param name="e"><see cref="SerialDataReceivedEventArgs"/></param>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            char lineChar;
            Regex newMessage = new(@"^\r\n\+\w+:.*\r\n$");
            Regex responseStatusPattern = new("ERROR(\r\n)$|OK(\r\n)$|ERROR:\\s\\d+(\r\n)$");
            string[] ignoreCallback = { "Call Ready" };
            SerialPort sp = (SerialPort)sender;

            try
            {
                while (sp.IsOpen && sp.BytesToRead > 0)
                {
                    var readByte = sp.ReadByte();
                    lineChar = Convert.ToChar(readByte);
                    line += lineChar;
                    if (line.Length <= 2 || lineChar != '\n')
                    {
                        continue;
                    }
                    if (ignoreCallback.Contains(line.ClearCLRF()))
                    {
                        line = string.Empty;
                        continue;
                    }

                    //lock (_locker)
                    //{
                    if (_lastCommandId != null)
                    {
                        if (_messageQueue.TryGetValue((Guid)_lastCommandId, out var lastCommand)
                            && lastCommand.Request == line.ClearCLRF())
                        {
                            _logger.WriteSerial($"{line.ClearCLRF()}", PortName);
                            line = string.Empty;
                            continue;
                        }

                        if (lastCommand != null && !lastCommand.ResponseHasBeenReceived && responseStatusPattern.IsMatch(line))
                        {
                            _logger.WriteSerial($"{line.ClearCLRF()}", PortName);
                            lastCommand.Response = line;
                            lastCommand.ResponseHasBeenReceived = true;
                            _lastCommandId = null;
                            line = string.Empty;
                            continue;
                        }
                    }

                    if (newMessage.IsMatch(line))
                    {
                        var req = line;
                        _logger.WriteSerial($"{line.ClearCLRF()}", PortName);
                        Task.Run(() => ModemEvent?.Invoke(req));
                        line = string.Empty;
                        continue;
                    }
                    //}
                }
            }
            catch
            {

            }
            
        }

        /// <summary>
        /// This method send commands to modem from queue 
        /// </summary>
        private void SerialPortSender()
        {
            while (!_cancellation.IsCancellationRequested)
            {
                try
                {
                    if (_lastCommandId.HasValue || !_messageQueue.Any())
                    {
                        Task.Delay(50, _cancellation).Wait(_cancellation);
                        continue;
                    }

                    // get first command ready to send
                    var toSend = _messageQueue.Values.Where(x => !x.RequestHasBeenSend).FirstOrDefault();
                    if (toSend == null)
                    {
                        continue;
                    }

                    //lock (_locker)
                    //{
                    _serialPort.WriteLine(toSend.Request);
                    _lastCommandId = toSend.MessageId;

                    // mark as sent
                    toSend.RequestHasBeenSend = true;
                    //}
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch(Exception ex)
                {
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _tokenSource.Cancel();
            _sender?.Wait();
            _serialPort?.Dispose();
            _sender = null;
            _serialPort = null;
            _messageQueue.Clear();
            _messageQueue = null;
            GC.Collect();
        }
    }
}