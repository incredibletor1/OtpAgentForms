using System.Net;

namespace OtpAgentForms.Dto.Base
{
    /// <summary>
    /// Response interface
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Response id
        /// </summary>
        Guid ResponseId { get; init; }

        /// <summary>
        /// Response status
        /// </summary>
        ResponseStatus Status { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        int ErrorCode { get; set; }

        /// <summary>
        /// Response https status code
        /// </summary>
        HttpStatusCode StatusCode { get; set; }
    }   
}