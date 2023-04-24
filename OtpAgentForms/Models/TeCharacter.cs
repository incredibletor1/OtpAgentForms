namespace OtpAgentForms.Models
{
    /// <summary>
    /// TE Character
    /// </summary>
    public enum TeCharacter
    {
        /// <summary>
        /// GSM default alphabet
        /// </summary>
        GSM,

        /// <summary>
        /// Character strings consist only of hexadecimal numbers from 00 to FF
        /// </summary>
        HEX,

        /// <summary>
        /// International reference alphabet
        /// </summary>
        IRA,

        /// <summary>
        /// PC character set Code
        /// </summary>
        PCCP437,

        /// <summary>
        /// UCS2 alphabet
        /// </summary>
        UCS2
    }
}