namespace OtpAgentForms.Dto.Base
{
    /// <summary>
    /// Base request model
    /// </summary>
    public class BaseRequest : IRequest
    {
        /// <inheritdoc/>
        public Guid RequestId { get; set; }

        /// <summary>
        /// Init base request
        /// </summary>
        public BaseRequest()
        {
        }
    }
}