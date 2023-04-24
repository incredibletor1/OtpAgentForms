using CountryModel = OtpAgentForms.Dto.Country.Models.Country;

namespace OtpAgentForms.Dto.Provider.Models
{
    /// <summary>
    /// Provider model
    /// </summary>
    public sealed class Provider
    {
        /// <summary>
        /// Provider id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Provider name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ussd for get number
        /// </summary>
        public string UssdGetNumber { get; set; }

        /// <summary>
        /// Provider code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Mcc + Mnc
        /// </summary>
        public string MccMnc { get; set; }

        /// <summary>
        /// Country id
        /// </summary>
        public Guid CountryId { get; set; }

        /// <summary>
        /// Country model
        /// </summary>
        public CountryModel Country { get; set; }

        /// <summary>
        /// Is provider active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Providercreate date
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Is provider deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Provider deleted date. NULL if not deleted
        /// </summary>
        public DateTime? DateDeleted { get; set; }
    }
}