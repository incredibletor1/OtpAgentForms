namespace OtpAgentForms.Hardware.Commands.Base
{
    /// <summary>
    /// Base at command response
    /// </summary>
    public abstract class BaseResponse<T> : IAtResponse<T>
    {
        /// <inheritdoc/>
        public ErrorList? Error { get; set; }

        /// <inheritdoc/>
        public abstract T Parse(string response);

        /// <summary>
        /// Split raw response to list. Check for errors
        /// </summary>
        /// <param name="rawRespone">Response string</param>
        /// <returns>List parameters</returns>
        protected List<string> SplitResponse(string rawRespone)
        {
            const string cmsError = "+CMS ERROR";
            const string cmeError = "+CME ERROR";
            const string genericError = "ERROR";

            // Empty response
            if (string.IsNullOrEmpty(rawRespone))
            {
                Error = ErrorList.NoResponse;
                return null;

            }
            string[] sep = { ":", ",", "\r\n" };
            List<string> resposeData = rawRespone.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToList();

            // response has status ERROR
            if (resposeData.Last() == genericError)
            {
                Error = ErrorList.Error;
            }

            // response has network error
            if (resposeData[0].Contains(cmsError))
            {
                Error = GetError(ErrorType.Network, int.Parse(resposeData[1]));
            }

            // response has equipment error
            if (resposeData[0].Contains(cmeError))
            {
                Error = GetError(ErrorType.Device, int.Parse(resposeData[1]));
            }
            return resposeData;
        }

        /// <summary>
        /// Remove \ and " fron string
        /// </summary>
        /// <param name="src">Source string</param>
        /// <returns>Screened string</returns>
        protected string Screen(string src)
        {
            return src.Trim().Replace("\"", "");
        }

        /// <summary>
        /// Get error
        /// </summary>
        /// <param name="type"><see cref="ErrorType"/></param>
        /// <param name="code">Error code</param>
        /// <returns><see cref="ErrorList"/></returns>
        protected static ErrorList GetError(ErrorType type, int code)
        {
            return (ErrorList)((int)type | code);
        }
    }
}