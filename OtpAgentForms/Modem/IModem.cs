using OtpAgentForms.Connector;
using OtpAgentForms.Dto.SimCard.Models;
using OtpAgentForms.Models;
using System.Data;

namespace OtpAgentForms.Modem
{
    /// <summary>
    /// Modem interface
    /// </summary>
    internal interface  IModem
    {
        /// <summary>
        /// On modem failure event
        /// </summary>
        event ModemEventDelegate OnFailure;

        /// <summary>
        /// On modem changed event
        /// </summary>
        event ModemEventDelegate OnChanged;

        /// <summary>
        /// Modem id
        /// </summary>
        Guid? Id { get; }

        /// <summary>
        /// Serial port name
        /// </summary>
        string PortName { get; }

        /// <summary>
        /// Row in table
        /// </summary>
        int Row { get; set; }

        /// <summary>
        /// Modem model name
        /// </summary>
        string Model { get; }

        /// <summary>
        /// Modem serial number
        /// </summary>
        string SerialNumber { get; }

        /// <summary>
        /// Modem imei
        /// </summary>
        ulong Imei { get; }

        /// <summary>
        /// Sim card imsi
        /// </summary>
        ulong Imsi { get; }

        /// <summary>
        /// Current sim card status
        /// </summary>
        SimCardStatus SimCardStatus { get; }

        /// <summary>
        /// Current rssi level
        /// </summary>
        uint Rssi { get; }

        /// <summary>
        /// Sim card
        /// </summary>
        SimCard SimCard { get; }

        /// <summary>
        /// Modem error message
        /// </summary>
        string Error { get; }

        /// <summary>
        /// Modem errors counter
        /// </summary>
        int ModemErrorCount { get; }

        /// <summary>
        /// Set modem id
        /// </summary>
        void SetId();

        /// <summary>
        /// Init modem
        /// </summary>
        void Init(string portName, Form form);

        /// <summary>
        /// Dispose modem
        /// </summary>
        void Dispose();
    }
}
