namespace OtpAgentForms.Attributes
{
    /// <summary>
    /// At response for attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ModemHanlerForAttribute : Attribute
    {
        /// <summary>
        /// At command
        /// </summary>
        public string At { get; }

        /// <summary>
        /// Init attribute
        /// </summary>        
        public ModemHanlerForAttribute(string at) => At = at;
    }
}