using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for unstructured supplementary service data command response
    /// </summary>
    public class CusdWriteResponse : BaseResponse<CusdWriteResponse>
    {
        /// <summary>
        /// Ussd response
        /// </summary>
        public string UssdResponse { get; set; }

        /// <inheritdoc/>
        public override CusdWriteResponse Parse(string rawResponse)
        {
            const string cmsError = "+CMS ERROR";
            const string cmeError = "+CME ERROR";
            const string genericError = "ERROR";

            var response = SplitResponse(rawResponse);
            if (Error != null)
            {
                return this;
            }
            if (response.Count <= 2)
            {
                Error = ErrorList.NoResponse;
                return this;
            }
            var c = response.IndexOf("+CUSD");
            var ussdResponse = response[c + 2];


            // response has status ERROR
            if (ussdResponse == genericError)
            {
                Error = ErrorList.Error;
                return this;
            }

            // response has network error
            if (ussdResponse.Contains(cmsError))
            {
                Error = GetError(ErrorType.Network, int.Parse(response[c+3]));
                return this;
            }

            // response has equipment error
            if (ussdResponse.Contains(cmeError))
            {
                Error = GetError(ErrorType.Device, int.Parse(response[c + 3]));
                return this;
            }

            UssdResponse = Screen(ussdResponse);
            return this;
        }
    }
}