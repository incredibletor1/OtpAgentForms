using System.Text.RegularExpressions;

namespace OtpAgentForms.Helpers
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Clear CLRF symbols
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>output string</returns>
        public static string ClearCLRF(this string s)
        {
            Regex pattern = new(@"\r\n");
            return pattern.Replace(s, "");
        }

        /// <summary>
        /// Replace string
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="separators">Separators array</param>
        /// <param name="newVal">New walue</param>
        /// <returns>Result string</returns>
        public static string Replace(this string s, char[] separators, string newVal)
        {
            string[] temp;

            temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(newVal, temp);
        }
    }
}