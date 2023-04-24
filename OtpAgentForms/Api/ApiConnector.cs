using OtpAgentForms.Dto.Provider.Models;
using OtpAgentForms.Dto.Provider;
using OtpAgentForms.Dto.Sms;
using OtpAgentForms.Logging;
using OtpAgentForms.Dto.SimCard.Models;
using OtpAgentForms.Dto.SimCard;
using Otp.Agent.Settings;
using System.Net.Http.Headers;
using OtpAgentForms.Dto.Base;
using System.Diagnostics;
using System.Text;
using OtpAgentForms.Dto.Modem;
using OtpAgentForms.Models;
using OtpAgentForms.Modem;
using Newtonsoft.Json;

namespace OtpAgentForms.Api
{
    /// <summary>
    /// Api connector implementation
    /// <see cref="IApiConnector"/>
    /// </summary>
    internal class ApiConnector : IApiConnector
    {
        /// <summary>
        /// Http client
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Global form
        /// </summary>
        private Form1 globalForm = null;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Init api connector        
        /// </summary>
        public ApiConnector(ILogger logger)
        {
            _logger = logger;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json));
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("User-Agent", "OTP.Agent/Connector");
            _client.DefaultRequestHeaders.Add("X-Agent", AppSettings.Current.General.AgentId.ToString());
        }

        #region Sms
        /// <summary>
        /// Controller name for sms
        /// </summary>
        private const string smsController = "Sms";

        /// <inheritdoc/>
        public AddSmsResponse AddSms(AtSms sms, Guid simCardId, string portName = null)
        {
            try
            {
                var request = new AddSmsRequest()
                {
                    RequestId = Guid.NewGuid(),
                    DateReceived = sms.DateReceived,
                    SenderNumber = sms.SourceNumber,
                    Message = sms.Message,
                    SimCardId = simCardId
                };
                return DoRequest<AddSmsResponse>(smsController, "AddSms", request);
            }
            catch (Exception ex)
            {
                //globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 4, $"Failed create sms on server: {ex.Message}");
                _logger.WriteError($"Failed create sms on server: {ex.Message}", portName);
                return null;
            }
        }
        #endregion

        #region Provider
        /// <summary>
        /// Controller name for provider
        /// </summary>
        private const string providerController = "Provider";

        /// <inheritdoc/>
        public Provider GetProviderByImsi(ulong imsi, string portName = null)
        {
            try
            {
                // if provider code not set return null
                if (imsi == 0)
                {
                    return null;
                }

                var request = new GetProviderByImsiRequest()
                {
                    RequestId = Guid.NewGuid(),
                    Imsi = imsi
                };
                var response = DoRequest<GetProviderByImsiResponse>(providerController, "GetProviderByImsi", request);
                return response?.Provider;

            }
            catch (Exception ex)
            {
                //globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 4, $"Failed create sms on server: {ex.Message}");
                _logger.WriteError($"Unable to get provider by imsi {imsi}: {ex.Message}", portName);
                return null;
            }
        }
        #endregion

        #region Sim card
        /// <summary>
        /// Controller name for sim card
        /// </summary>
        private const string simCardController = "SimCard";

        /// <inheritdoc/>
        public SimCard GetSimCard(ulong imsi, string portName = null)
        {
            try
            {
                // if imsi not set
                if (imsi == 0)
                {
                    throw new Exception("imsi not set");
                }
                // get provider from server
                var request = new GetSimCardRequest()
                {
                    Imsi = imsi
                };
                var response = DoRequest<GetSimCardResponse>(simCardController, "GetSimCard", request);
                return response?.SimCard;

            }
            catch (Exception ex)
            {
                //globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 4, $"Failed create sms on server: {ex.Message}");
                _logger.WriteError($"Unable to get sim card with imsi {imsi}: {ex.Message}", portName);
                return null;
            }
        }

        /// <inheritdoc/>
        public SimCard AddSimCard(SimCard simCard, string portName = null)
        {
            try
            {
                var request = new AddSimCardRequest()
                {
                    Imsi = simCard.Imsi,
                    PhoneNumber = simCard.PhoneNumber,
                    ProviderId = simCard.ProviderId,
                    RequestId = Guid.NewGuid(),
                };
                var response = DoRequest<AddSimCardResponse>(simCardController, "AddSimCard", request);
                return response?.SimCard ?? null;
            }
            catch (Exception ex)
            {
                //globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 4, $"Unable to add sim card: {ex.Message}");
                _logger.WriteError($"Unable to add sim card: {ex.Message}", portName);
                return null;
            }
        }
        #endregion

        #region Modem
        /// <summary>
        /// Controller name for sim card
        /// </summary>
        private const string modemController = "Modem";

        /// <inheritdoc/>
        public DeleteModemResponse DeleteModem(Guid modemId, string portName = null)
        {
            try
            {
                var request = new DeleteModemRequest()
                {
                    Id = modemId
                };
                return DoRequest<DeleteModemResponse>(modemController, "DeleteModem", request);
            }
            catch (Exception ex)
            {
                globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 6, $"Failed delete modem on server: {ex.Message}");
                _logger.WriteError($"Failed delete modem on server: {ex.Message}", portName);
                return null;
            }
        }

        /// <inheritdoc/>
        public DeleteAllModemsResponse DeleteAllModems(Form form)
        {
            try
            {
                globalForm = (Form1)form;
                var request = new DeleteAllModemsRequest();
                return DoRequest<DeleteAllModemsResponse>(modemController, "DeleteAllModems", request);
            }
            catch (Exception ex)
            {
                //globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 4, $"Failed create sms on server: {ex.Message}");
                _logger.WriteError($"Failed delete all modems on server: {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc/>
        public AddEditModemResponse AddEditModem(IModem modem, string portName = null)
        {
            try
            {
                var request = new AddEditModemRequest()
                {
                    RequestId = Guid.NewGuid(),
                    Id = modem.Id,
                    Imei = modem.Imei,
                    Model = modem.Model,
                    PortName = modem.PortName,
                    Rssi = (int)modem.Rssi,
                    SerialNumber = modem.SerialNumber,
                    SimInserted = modem.SimCardStatus == SimCardStatus.READY,
                    SimCardId = modem.SimCard?.Id,
                    ErrorMessage = modem.Error
                };
                return DoRequest<AddEditModemResponse>(modemController, "AddEditModem", request);
            }
            catch (Exception ex)
            {
                globalForm.ChangeGrid(globalForm.GetRowIndex(portName), 6, $"Failed create modem on server: {ex.Message}");
                _logger.WriteError($"Failed create modem on server: {ex.Message}", portName);
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Do request to service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private T DoRequest<T>(string controller, string action, object body = null) where T : IResponse, new()
        {
            var scheme = AppSettings.Current.General.ApiUseHttps
                ? "https"
                : "http";
            var uri = new Uri($"{scheme}://{AppSettings.Current.General.ApiHost}:{AppSettings.Current.General.ApiPort}/v1/{controller}/{action}");

            HttpResponseMessage httpResponse;
            try
            {
                if (body == null)
                {
                    var result = _client.GetAsync(uri);
                    result.Wait();
                    httpResponse = result.Result;
                }
                else
                {
                    var stringPayload = JsonConvert.SerializeObject(body);
                    using var httpContent = new StringContent(stringPayload, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);
                    var result = _client.PostAsync(uri, httpContent);
                    result.Wait();
                    httpResponse = result.Result;
                }
            }
            catch (Exception ex)
            {
                httpResponse = null;
                Debug.WriteLine($"Connector HTTP request error: {ex.Message}");
            }

            if (httpResponse != null & httpResponse?.Content != null)
            {
                var result = httpResponse.Content.ReadAsStringAsync();
                result.Wait();
                var responseContent = result.Result;
                var response = JsonConvert.DeserializeObject<T>(responseContent);
                response ??= new T();
                response.StatusCode = httpResponse.StatusCode;
                return response;
            }

            return default;
        }
    }    
}