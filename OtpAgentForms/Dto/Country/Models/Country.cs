namespace OtpAgentForms.Dto.Country.Models
{
    /// <summary>
    /// Country model
    /// </summary>
    public sealed class Country
    {
        /// <summary>
        /// Country id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Country name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Country code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Country phone code
        /// </summary>
        public string PhoneCode { get; set; }

        /// <summary>
        /// Is country active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Country create date
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Is country deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Country deleted date. NULL if not deleted
        /// </summary>
        public DateTime? DateDeleted { get; set; }
    }
}