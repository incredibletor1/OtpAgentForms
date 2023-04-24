using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using Otp.Agent.Settings;
using OtpAgentForms.Helpers;
using OtpAgentForms.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace OtpAgentForms.Logging
{
    /// <summary>
    /// Logger implementation
    /// <see cref="ILogger"/>
    /// </summary>
    internal class Logger : ILogger, IDisposable
    {
        const string logFile = "agent.log";

        /// <summary>
        /// Temp storage for console log
        /// </summary>
        private static List<ConsoleMessage> consoleLogMessages = new List<ConsoleMessage>();      

        /// <summary>
        /// Log messages queue
        /// </summary>
        private readonly ConcurrentQueue<LogMessage> _logQueue;

        private StreamWriter _agentLog;

        private Task _logSender;

        /// <summary>
        /// Cancellation
        /// </summary>
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _ct;

        /// <summary>
        /// Locker object
        /// </summary>
        private object _locker = new();

        /// <summary>
        /// App settings
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// Init logger
        /// </summary>
        public Logger()
        {
            _settings = AppSettings.Current;
            _logQueue = new();
            _cts = new CancellationTokenSource();
            _agentLog = new(logFile, true);
            _ct = _cts.Token;
            _logSender = new Task(() => LogQueue(), TaskCreationOptions.LongRunning);
            _logSender.Start();
        }

        /// <inheritdoc/>
        public void WriteDebug(string message, string portName = null)
        {
            if (AppSettings.Current.Logging.Debug)
            {
                var log = new LogMessage()
                {
                    Level = LogLevel.Debug,
                    PortName = portName,
                    Message = message
                };
                _logQueue.Enqueue(log);
            }
        }

        /// <inheritdoc/>
        public void WriteInformation(string message, string portName = null)
        {
            var log = new LogMessage()
            {
                Level = LogLevel.Information,
                PortName = portName,
                Message = message
            };
            _logQueue.Enqueue(log);
        }

        /// <inheritdoc/>
        public void WriteError(string message, string portName = null)
        {
            var log = new LogMessage()
            {
                Level = LogLevel.Error,
                PortName = portName,
                Message = message
            };
            _logQueue.Enqueue(log);
        }

        /// <inheritdoc/>
        public void WriteSerial(string message, string portName = null)
        {
            var log = new LogMessage()
            {
                Level = LogLevel.Serial,
                PortName = portName,
                Message = message
            };
            _logQueue.Enqueue(log);
        }

        /// <summary>
        /// Log queue thread
        /// </summary>        
        private void LogQueue()
        {
            while (!_ct.IsCancellationRequested)
            {
                try
                {
                    // if task cancelled
                    if (_ct.IsCancellationRequested)
                    {
                        _ct.ThrowIfCancellationRequested();
                    }

                    // if queue empty
                    if (!_logQueue.TryDequeue(out var message))
                    {
                        Task.Delay(10, _ct).Wait();
                        continue;
                    }

                    // build log message
                    var messageString = $"{message.Timestamp} [{message.Level.ToString().ToUpper()}]";
                    if (!string.IsNullOrEmpty(message.PortName))
                    {
                        messageString += $" {message.PortName}";
                    }
                    messageString += $" {message.Message}";

                    // add log to console
                    //WriteConsole(message);

                    // add log to file
                    _agentLog.WriteLine(messageString);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        private void WriteConsole(LogMessage message)
        {
            if (!_settings.Logging.Serial & message.Level == LogLevel.Serial)
            {
                return;
            }

            var messageString = string.Empty;
            var (x, y) = Console.GetCursorPosition();

            if (string.IsNullOrEmpty(message.PortName))
            {
                messageString = $"{message.Timestamp} [{message.Level.ToString().ToUpper()}] {message.Message}";
            }
            else
            {
                messageString = $"{message.Timestamp} [{message.Level.ToString().ToUpper()}] {message.PortName} {message.Message}";
            }

            var consoleMessage = new ConsoleMessage()
            {
                Message = messageString,
                PosX = x,
                PosY = y
            };

            var oldConsoleMessage = GetConsoleMessageByPortName(consoleLogMessages, message.PortName);
            if (consoleLogMessages.Count < 1 || oldConsoleMessage is null)
            {
                consoleLogMessages.Add(consoleMessage);
                WriteCurrentLogMessage(consoleMessage.Message);
                return;
            }

            Console.SetCursorPosition(oldConsoleMessage.PosX, oldConsoleMessage.PosY);
            for (int i = 0; i < oldConsoleMessage.Message.Length; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(oldConsoleMessage.PosX, oldConsoleMessage.PosY);
            consoleMessage.PosX = oldConsoleMessage.PosX;
            consoleMessage.PosY = oldConsoleMessage.PosY;
            var consoleLogStringIndex = consoleLogMessages.IndexOf(oldConsoleMessage);
            consoleLogMessages[consoleLogStringIndex] = consoleMessage;
            WriteCurrentLogMessage(consoleMessage.Message);
            Console.SetCursorPosition(x, y);
        }

        /// <summary>
        /// Write message to console
        /// </summary>
        /// <param name="currentLogMessage">Log message</param>
        /// <returns></returns>
        private void WriteCurrentLogMessage(string currentLogMessage)
        {
            if (currentLogMessage.IndexOf(LogLevel.Information.ToString().ToUpper()) != -1)
            {
                WriteConsoleColorized(LogLevel.Information, currentLogMessage);
            }
            else if (currentLogMessage.IndexOf(LogLevel.Error.ToString().ToUpper()) != -1)
            {
                WriteConsoleColorized(LogLevel.Error, currentLogMessage);
            }
            else if (currentLogMessage.IndexOf(LogLevel.Debug.ToString().ToUpper()) != -1)
            {
                WriteConsoleColorized(LogLevel.Debug, currentLogMessage);
            }
            else if (currentLogMessage.IndexOf(LogLevel.Serial.ToString().ToUpper()) != -1)
            {
                WriteConsoleColorized(LogLevel.Serial, currentLogMessage);
            }
            else
            {
                Console.Write(currentLogMessage + '\n');
            }
        }

        /// <summary>
        /// Get console message object by PortName
        /// </summary>
        /// <param name="consoleMessages">All log messages</param>
        /// <param name="portName">Port name</param>
        /// <returns>Console message object</returns>
        private ConsoleMessage GetConsoleMessageByPortName(IEnumerable<ConsoleMessage> consoleMessages, string portName)
        {
            if (portName is null)
            {
                return null;
            }

            foreach (var consoleMessage in consoleMessages)
            {
                if (consoleMessage.Message.IndexOf(portName) != -1)
                {
                    return consoleMessage;
                }
            }

            return null;
        }

        /// <summary>
        /// Write to console with loglevel color
        /// </summary>
        /// <param name="currentLogMessage">log message</param>
        /// <param name="level">log level</param>
        /// <returns></returns>
        private static void WriteConsoleColorized(LogLevel level, string currentLogMessage)
        {
            var splitedCurrentLogMessage = currentLogMessage.Split(level.ToString().ToUpper());
            Console.Write(splitedCurrentLogMessage[0]);
            Console.ForegroundColor = GetConsoleColor(level);
            Console.Write(level.ToString().ToUpper());
            Console.ResetColor();
            Console.Write(splitedCurrentLogMessage[1] + '\n');
        }

        private static ConsoleColor GetConsoleColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Serial => ConsoleColor.Cyan,
                LogLevel.Debug => ConsoleColor.Blue,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Information => ConsoleColor.Green,
                _ => ConsoleColor.White

            };
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _agentLog.Dispose();
        }
    }
}