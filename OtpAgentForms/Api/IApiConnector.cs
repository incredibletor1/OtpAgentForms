using OtpAgentForms.Dto.Modem;
using OtpAgentForms.Dto.Provider.Models;
using OtpAgentForms.Dto.SimCard.Models;
using OtpAgentForms.Dto.Sms;
using OtpAgentForms.Models;
using OtpAgentForms.Modem;

namespace OtpAgentForms.Api
{
    /// <summary>
    /// Api service interface
    /// </summary>
    internal interface IApiConnector
    {
        #region Sms
        /// <summary>
        /// Add sms to server
        /// </summary>
        /// <param name="sms"><see cref="AtSms"/></param>
        /// <param name="simCardId">Sim card id</param>
        /// <param name="portName">Serial port name</param>
        /// <returns><see cref="AddSmsResponse"/></returns>
        AddSmsResponse AddSms(AtSms sms, Guid simCardId, string portName = null);
        #endregion

        #region Provider
        /// <summary>
        /// Get provider by sim card imsi
        /// </summary>
        /// <param name="imsi">Sim card imsi</param>
        /// <param name="portName">Serial port name</param>
        /// <returns><see cref="Provider"/></returns>
        Provider GetProviderByImsi(ulong imsi, string portName = null);
        #endregion

        #region Sim card
        /// <summary>
        /// Get sim card
        /// </summary>
        /// <param name="imsi">Sim card imsi</param>
        /// <param name="portName">Serial port name</param>
        /// <returns><see cref="SimCard"/></returns>
        SimCard GetSimCard(ulong imsi, string portName = null);

        /// <summary>
        /// Add new sim card
        /// </summary>
        /// <param name="simCard"><see cref="SimCard"/></param>
        /// <param name="portName">Serial port name</param>
        /// <returns><see cref="SimCard"/></returns>
        SimCard AddSimCard(SimCard simCard, string portName = null);
        #endregion

        #region Modem
        /// <summary>
        /// Delete modem from server
        /// </summary>
        /// <param name="modemId">Modem id</param>
        /// <param name="portName">Serial port name</param>
        /// <returns></returns>
        DeleteModemResponse DeleteModem(Guid modemId, string portName = null);

        /// <summary>
        /// Delete all modems froms erver
        /// </summary>
        /// <returns><see cref="DeleteAllModemsResponse"/></returns>
        DeleteAllModemsResponse DeleteAllModems(Form form);

        /// <summary>
        /// Add or update modem
        /// </summary>
        /// <param name="modem"><see cref="Modem"/></param>
        /// <param name="portName">Serial port name</param>
        /// <returns><see cref="AddEditModemResponse"/></returns>
        AddEditModemResponse AddEditModem(IModem modem, string portName = null);
        #endregion
    }
}