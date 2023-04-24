using System.Net;

namespace OtpAgentForms.Dto.Base
{
    /// <summary>
    /// Base response model
    /// </summary>
    public class BaseResponse : IResponse
    {
        /// <inheritdoc/>
        public Guid ResponseId { get; init; }

        /// <inheritdoc/>
        public ResponseStatus Status { get; set; }

        /// <inheritdoc/>
        public int ErrorCode { get; set; }

        /// <inheritdoc/>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Init base response
        /// </summary>
        public BaseResponse()
        {
            ResponseId = Guid.NewGuid();
            Status = ResponseStatus.ResponseOk;
            ErrorCode = 0;
            StatusCode = HttpStatusCode.OK;
        }
    }
}