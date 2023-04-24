using OtpAgentForms.Hardware.Commands.Base;
using OtpAgentForms.Models;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for enter PIN/set sim status command response
    /// </summary>
    public sealed class CpinResponse : BaseResponse<CpinResponse>
    {
        /// <summary>
        /// Sim card status
        /// </summary>
        public SimCardStatus Status { get; set; } = SimCardStatus.NOTREADY;
        public string RawResponse { get; set; }

        /// <inheritdoc/>
        public override CpinResponse Parse(string rawResponse)
        {
            RawResponse = rawResponse;
            var response = SplitResponse(rawResponse);
            if (Error != null)
            {
                return this;
            }

            string[] statusTypes = Enum.GetNames(typeof(SimCardStatus));
            var status = statusTypes.Where(x => x.ToUpper() == response[1].Replace(" ", "")).FirstOrDefault();
            if (status == null || !Enum.IsDefined(typeof(SimCardStatus), status))
            {
                Status = SimCardStatus.NOTREADY;
            }
            else
            {
                Status = (SimCardStatus)Enum.Parse(typeof(SimCardStatus), status);

            }
            return this;
        }
    }
}