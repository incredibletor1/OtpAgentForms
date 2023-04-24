using Microsoft.Extensions.Configuration;
using OtpAgentForms.Settings;

namespace Otp.Agent.Settings
{
    /// <summary>
    /// Application settings
    /// </summary>
    internal sealed class AppSettings
    {
        /// <summary>
        /// Api settings
        /// </summary>
        public GeneralSettings General { get; set; } = new ();

        /// <summary>
        /// Logging settings
        /// </summary>
        public LoggingSettings Logging { get; set; }

        private static AppSettings _appSettings;

        /// <summary>
        /// Init app settings
        /// </summary>
        public AppSettings()
        {
            _appSettings = this;
        }

        /// <summary>
        /// Currens application settings
        /// </summary>
        public static AppSettings Current
        {
            get
            {
                _appSettings ??= GetCurrentSettings();
                _appSettings.Logging ??= new LoggingSettings();
                return _appSettings;
            }
        }

        /// <summary>
        /// Get current settings
        /// </summary>
        /// <returns><see cref="AppSettings"/></returns>
        private static AppSettings GetCurrentSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddIniFile($"conf.ini", optional: false);

            IConfigurationRoot configuration = builder.Build();

            var settings = new AppSettings();
            configuration.Bind(settings);
            return settings;         
        }
    }
}