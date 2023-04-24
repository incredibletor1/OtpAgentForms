namespace OtpAgentForms.Hardware.Commands.Base
{
    public interface IAtResponse<T>
    {
        /// <summary>
        /// Response error
        /// is null - response success
        /// </summary>
        public ErrorList? Error { get; set; }

        /// <summary>
        /// Parse response to model T
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Model T</returns>
        public T Parse(string response);
    }
}
