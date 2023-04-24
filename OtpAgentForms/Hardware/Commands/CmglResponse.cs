using OtpAgentForms.Hardware.Commands.Base;
using OTPAgent.Helpers;
using OtpAgentForms.Models;
using System.Globalization;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for read SMS message command response
    /// </summary>
    internal sealed class CmglResponse : BaseResponse<CmglResponse>
    {
        /// <summary>
        /// List sms messages
        /// </summary>
        public List<AtSms> SMS { get; set; }

        /// <inheritdoc/>
        public override CmglResponse Parse(string rawResponse)
        {
            SplitResponse(rawResponse);
            if (Error != null)
            {
                return this;
            }
            SMS = new();
            List<string> responseData = rawResponse
                .Replace("\r\nOK\r\n", "")
                .Split("+CMGL:", StringSplitOptions.RemoveEmptyEntries).ToList();
            string[] sep = { "\r\n", "," };
            CultureInfo provider = CultureInfo.InvariantCulture;

            foreach (string row in responseData)
            {
                var message = row.Split(sep, StringSplitOptions.None);
                if (message.Length > 2)
                {
                    // parse message id to uint
                    if (!int.TryParse(Screen(message[0]), out var intId))
                    {
                        continue;
                    }
                    int id = intId;

                    // parse date to datetime
                    if (!DateTime.TryParseExact(Screen(message[4]), "yyyy/MM/dd HH:mm:ss+ff", provider, DateTimeStyles.None, out var dateTime))
                    {
                        continue;
                    }
                    var smsDateTime = dateTime;

                    SMS.Add(
                        new AtSms()
                        {
                            Id = id,
                            Status = Screen(message[1]),
                            SourceNumber = Ucs2.ToString(Screen(message[2])).Replace("+", ""),
                            DateReceived = smsDateTime,
                            Message = Ucs2.ToString(Screen(message[5])).Replace("\n", " ")
                        }
                    );
                }
            }
            return this;
        }
    }
}
