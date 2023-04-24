namespace OtpAgentForms.Dto.Base
{
    /// <summary>
    /// Request interface
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Unique request id
        /// </summary>
        public Guid RequestId { get; }
    }
}