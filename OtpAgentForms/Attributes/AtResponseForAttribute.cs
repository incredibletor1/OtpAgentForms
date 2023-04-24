namespace OtpAgentForms.Attributes
{
    /// <summary>
    /// At response for attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AtResponseForAttribute : Attribute
    {
        /// <summary>
        /// At command
        /// </summary>
        public string At { get; }

        /// <summary>
        /// Init attribute
        /// </summary>        
        public AtResponseForAttribute(string at) => At = at;
    }
}