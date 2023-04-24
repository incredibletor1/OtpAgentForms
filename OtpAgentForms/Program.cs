using Microsoft.Extensions.DependencyInjection;
using Otp.Agent.Settings;
using OtpAgentForms.Api;
using OtpAgentForms.Connector;
using OtpAgentForms.Helpers;
using OtpAgentForms.Logging;
using OtpAgentForms.Modem;
using OtpAgentForms.Services.Modems;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OtpAgentForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var s = "07916629031167F24406D0C1E41400082221105173858288050003CC03030E2D0E340E1900200E2A0E210E310E040E230E1F0E230E350E230E310E1A0E040E300E410E190E190E170E310E190E170E3500200E040E250E340E010E400E250E2200200068007400740070003A002F002F006D002E006100690073002E0063006F002E00740068002F006D007900410049005300570065006C0063006F006D0065";
            //Console.WriteLine(Ucs2.ToString(s));


            // check configuration file
            AppSettings configuration = null;
            try
            {
                configuration = AppSettings.Current;
                if (configuration.General.AgentId == Guid.Empty)
                {
                    MessageBox.Show("Configuration file error. Ñheck your conf.ini for errors");
                    Environment.Exit(1);
                }

            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Configuration file not found");
                Environment.Exit(1);
            }
            catch
            {
                MessageBox.Show("Configuration file error. Ñheck your conf.ini for errors");
                Environment.Exit(1);
            }

            Process[] vsProcs = Process.GetProcessesByName("cmd");
            if (vsProcs.Any())
            {
                var maxTime = vsProcs.Max(p => p.StartTime);
                vsProcs.First(p => p.StartTime == maxTime).Kill();
            }

            // read tac
            ImeiHelper.ReadTac();

            // configure services
            var services = new ServiceCollection();

            // logger as singleton
            services.AddSingleton<ILogger, Logger>();

            // api connector as transient
            services.AddTransient<IApiConnector, ApiConnector>();

            // modem connector as transient
            services.AddTransient<IModemConnector, ModemConnector>();

            // modem service as singleton
            services.AddSingleton<IModemsService, ModemsService>();

            // modem object as singleton
            services.AddTransient<IModem, Modem.Modem>();

            var serviceProvider = services.BuildServiceProvider();

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            IModemsService modemService = serviceProvider.GetService<IModemsService>();
                
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1(modemService, ct));
            //ct.WaitHandle.WaitOne();

            //Environment.Exit(0);
        }
    }
}