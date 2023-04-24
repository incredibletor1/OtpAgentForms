using System.Data;

namespace OtpAgentForms.Services.Modems
{
    /// <summary>
    /// Modem service interface
    /// </summary>
    public interface IModemsService
    {
        void Start(CancellationToken ct, Form form);
    }
}
