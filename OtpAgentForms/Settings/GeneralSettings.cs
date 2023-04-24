namespace OtpAgentForms.Settings
{
    /// <summary>
    /// Dto for general settings
    /// </summary>
    internal class GeneralSettings
    {
        /// <summary>
        /// Api ip address or domain
        /// </summary>
        public string ApiHost { get; set; } = "api.otp.hayes.su";

        /// <summary>
        /// Api port
        /// </summary>
        public int ApiPort { get; set; } = 443;

        /// <summary>
        /// if false - use http. if true - use https
        /// </summary>
        public bool ApiUseHttps { get; set; } = true;

        /// <summary>
        /// Download port settings url 
        /// </summary> 
        public string PortSettingsUrl { get; set; }

        /// <summary>
        /// Delay after Numberphone error 
        /// </summary> 
        public int NumberphoneDelay { get; set; }

        /// <summary>
        /// Agent id
        /// </summary>
        public Guid AgentId { get; set; }

        /// <summary>
        /// Set new generated imei after modem detected
        /// </summary>
        public bool ChangeImei { get; set; }

        public bool DisablePhonebookWrite { get; set; } = false;

        /// <summary>
        /// Serial port names separated by coma
        /// Modem lookup will not be performed on these ports
        /// </summary>
        public string ExcludePortNames { get; set; } = string.Empty;

        /// <summary>
        /// The maximum number of errors allowed before destroying the modem
        /// </summary>
        public int ErrorCount { get; set; } = 3;

        /// <summary>
        /// Serial port names
        /// Modem lookup will not be performed on these ports
        /// </summary>
        public string[] ExcludePortList
        {
            get
            {
                return ExcludePortNames
                            .Trim()
                            .Split(',', StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}