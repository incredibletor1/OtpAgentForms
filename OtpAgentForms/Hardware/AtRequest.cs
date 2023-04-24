using OtpAgentForms.Models;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace OtpAgentForms.Hardware
{
    /// <summary>
    /// At commands requests helper
    /// </summary>
    public static class AtRequest
    {
        /// <summary>
        /// AT
        /// </summary>        
        public static string AT() => "AT";
        
        /// <summary>
        /// Get model name
        /// </summary>        
        public static string GetModelName() => "AT+GMM";

        /// <summary>
        /// Get sim card status
        /// </summary>
        public static string GetSimCardStatus() => "AT+CPIN?";  
        
        /// <summary>
        /// Get serial number
        /// </summary>
        public static string GetSerialNumber() => "AT+CGSN";

        /// <summary>
        /// Get imei
        /// </summary>
        public static string GetImei() => "AT+GSN";

        /// <summary>
        /// Get signal quality
        /// </summary>
        public static string GetSignalQuality() => "AT+CSQ";   
        
        /// <summary>
        /// Get imsi
        /// </summary>        
        public static string GetImsi() => "AT+CIMI";

        /// <summary>
        /// Send ussd
        /// </summary>
        public static string SenUssd(string ussdRequest, int broadcastDataCoding = 15, int indicatesControl = 1) =>        
            $"AT+CUSD={indicatesControl},\"{ussdRequest}\",{broadcastDataCoding}";

        /// <summary>
        /// Set te character
        /// </summary>
        public static string SetTeCharacter(TeCharacter teCharacter) =>
            $"AT+CSCS=\"{teCharacter}\"";

        /// <summary>
        /// Read sms
        /// </summary>
        public static string ReadSms(int position) =>
            $"AT+CMGR={position}";

        /// <summary>
        /// Read all sms
        /// </summary>
        public static string ReadAllSms() =>
            $"AT+CMGL=\"ALL\"";

        /// <summary>
        /// Delete sms
        /// </summary>
        public static string DeleteSms(int position) =>
            $"AT+CMGD={position}";

        /// <summary>
        /// Enable phone functions
        /// </summary>
        public static string EnablePhone() =>
            $"AT+CFUN=1";

        /// <summary>
        /// Set text mode
        /// </summary>
        public static string SetTextMode() =>
            $"AT+CMGF=1";

        /// <summary>
        /// Enable new sms indication
        /// </summary>
        public static string EnableNewSmsIndication() =>
            $"AT+CNMI=2,1";

        /// <summary>
        /// Set internal sms memory
        /// </summary>
        public static string SetInternalSmsMemory() =>
            $"AT+CPMS=\"MT\",\"MT\",\"MT\"";

        /// <summary>
        /// Set internal phonebook memory
        /// </summary>
        public static string SetInternalPhonebookMemory() =>
            $"AT+CPBS=\"ME\"";

        
        /// <summary>
        /// Enable signal quality echo
        /// </summary>
        public static string EnableCsqEcho() =>
            $"AT+QEXTUNSOL=\"SQ\",1";

        /// <summary>
        /// Read phonebook
        /// </summary>
        public static string ReadPhoneBook(int position) =>
            $"AT+CPBR={position}";

        /// <summary>
        /// Delete phonebook
        /// </summary>
        public static string DeletePhoneBook(int position) =>
            $"AT+CPBW={position}";

        /// <summary>
        /// Write phonebook
        /// </summary>
        public static string WritePhoneBook(int position, string number, string name) =>
            $"AT+CPBW={position},\"{number}\",129,\"{name}\"";

        /// <summary>
        /// Set imei
        /// </summary>
        /// <param name="imei">new imei</param>        
        public static string SetImei(string imei) =>
            $"AT+EGMR=1,7,\"{imei}\"";
    }
}