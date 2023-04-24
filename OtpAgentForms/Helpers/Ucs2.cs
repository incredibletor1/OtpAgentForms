using System.Text;

namespace OTPAgent.Helpers
{
    /// <summary>
    /// Ucs2 converter
    /// </summary>
    public static class Ucs2
    {
        /// <summary>
        /// Convert unicode to UCS2
        /// </summary>
        /// <param name="strMessage">Unicode string</param>
        /// <returns>UCS2 string</returns>
        public static String ToUCS2(String strMessage)
        {
            byte[] ba = Encoding.BigEndianUnicode.GetBytes(strMessage);
            String strHex = BitConverter.ToString(ba);
            strHex = strHex.Replace("-", "");
            return strHex;
        }

        /// <summary>
        /// Convert UCS2 to unicode string
        /// </summary>
        /// <param name="strHex">UCS2 string</param>
        /// <returns>Unicode string</returns>
        public static String ToString(String strHex)
        {
            byte[] ba = HexStr2HexBytes(strHex);
            return HexBytes2UnicodeStr(ba);
        }

        private static String HexBytes2UnicodeStr(byte[] ba)
        {
            var strMessage = Encoding.BigEndianUnicode.GetString(ba, 0, ba.Length);
            return strMessage;
        }

        private static byte[] HexStr2HexBytes(String strHex)
        {
            strHex = strHex.Replace(" ", "");
            int nNumberChars = strHex.Length / 2;
            byte[] aBytes = new byte[nNumberChars];
            using (var sr = new StringReader(strHex))
            {
                for (int i = 0; i < nNumberChars; i++)
                    aBytes[i] = Convert.ToByte(new String(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return aBytes;
        }
    }
}