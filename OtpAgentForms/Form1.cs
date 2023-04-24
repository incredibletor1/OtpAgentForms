using Microsoft.Extensions.DependencyInjection;
using Otp.Agent.Settings;
using OtpAgentForms.Logging;
using OtpAgentForms.Models;
using OtpAgentForms.Services.Modems;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.IO.Ports;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace OtpAgentForms
{
    public partial class Form1 : Form
    {
        public DataTable dataTable = new DataTable();
        public MainDataTable mainDataTable = new MainDataTable();
        public CancellationToken _ct;
        public IModemsService _modemService;
        public string fullDownloadPath = Environment.CurrentDirectory + @"\hub_ports.txt";
        public Dictionary<string, (string, string)> portSerialNumberInfo = new Dictionary<string, (string, string)>(2);
        public string exeName = AppDomain.CurrentDomain.FriendlyName;
        public string exePath = Assembly.GetExecutingAssembly().Location;
        public Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

        public Form1(IModemsService modemService, CancellationToken ct)
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

            _ct = ct;
            _modemService = modemService;
            dataTable = mainDataTable.DataTable();
            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[0] };
            metroGrid1.DataSource = dataTable;

            /*var version = CheckForNewVersion();
            if (version != null)
            {
                button1.Visible = true;
            }*/

            //var a = new Task(() => CheckForNewVersion());
            //a.Start();
            var aTimer = new System.Timers.Timer(60 * 60 * 1000);
            //OnTimedEvent(new object, new ElapsedEventArgs());
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Start();
            Task.Run(() =>
            {
                OnTimedEvent(null, null);
            });

            if (File.Exists(fullDownloadPath))
            {
                File.Delete(fullDownloadPath);
            }

            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.UseDefaultCredentials = true;
                    webClient.DownloadFileCompleted += Client_DownloadFileCompleted;
                    webClient.DownloadFileAsync(new Uri(AppSettings.Current.General.PortSettingsUrl), fullDownloadPath);
                };
            }
            catch (Exception ex)
            {

            }

            metroGrid1.Theme = MetroFramework.MetroThemeStyle.Dark;
            metroGrid1.Columns[0].Width = 100;
            metroGrid1.Columns[0].MinimumWidth = 60;
            metroGrid1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            metroGrid1.Columns[0].ReadOnly = true;
            metroGrid1.Columns[1].Width = 150;
            metroGrid1.Columns[1].MinimumWidth = 60;
            metroGrid1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            metroGrid1.Columns[1].ReadOnly = true;
            metroGrid1.Columns[2].Width = 150;
            metroGrid1.Columns[2].MinimumWidth = 100;
            metroGrid1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            metroGrid1.Columns[2].ReadOnly = true;
            metroGrid1.Columns[3].Width = 50;
            metroGrid1.Columns[3].MinimumWidth = 10;
            metroGrid1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            metroGrid1.Columns[3].ReadOnly = true;
            metroGrid1.Columns[4].Width = 100;
            metroGrid1.Columns[4].MinimumWidth = 60;
            metroGrid1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            metroGrid1.Columns[4].ReadOnly = true;
            metroGrid1.Columns[5].Width = 100;
            metroGrid1.Columns[5].MinimumWidth = 60;
            metroGrid1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            metroGrid1.Columns[5].ReadOnly = true;
            metroGrid1.Columns[6].Width = 350;
            metroGrid1.Columns[6].MinimumWidth = 100;
            metroGrid1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            metroGrid1.Columns[6].ReadOnly = true;

            _modemService.Start(_ct, this);            
            //_ct.WaitHandle.WaitOne();
        }

        private async Task<string> CheckForNewVersion()
        {
            var latestVersionResponse = await HttpResponse("https://analytics.otp-service.online/otp/latest_version.txt");
            var latestVersion = new Version(latestVersionResponse);

            var versionsComparer = currentVersion.CompareTo(latestVersion);
            if (versionsComparer == -1)
            {
                return latestVersion.ToString(1);
            }

            return null;
        }

        private async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var version = await CheckForNewVersion();
            if (version != null)
            {
                Invoke(() => button1.Visible = true);
            }
        }

        private void Client_DownloadFileCompleted(object? sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fullDownloadPath))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var splitedLine = line.Split(';');
                        if (splitedLine[0] == "\\N" || splitedLine[1] == "\\N" || splitedLine[2] == "\\N")
                        {
                            continue;
                        }

                        portSerialNumberInfo.Add(splitedLine[0], (splitedLine[1], splitedLine[2]));
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Download problem");
                return;
            }
        }

        public void ChangeGrid(int row, int column, string value) => Invoke(() =>
        {
            if (column == -1)
            {
                dataTable.Rows.RemoveAt(row);
                return;
            }

            DataTable dTable = dataTable;
            dTable.Rows[row][column] = value;
        });

        public int AddRow(string port) => Invoke(() =>
        {
            DataTable dTable = dataTable;
            var result = dTable.Rows.Add(port);
            return result.Table.Rows.Count - 1;
        });

        public List<string> ePorts = new List<string>();

        public void ChangePortsCount(string portName) => Invoke(() =>
        {
            var nomalPorts = SerialPort.GetPortNames();
            /*if (nomalPorts.Contains(portName) && ePorts.Contains(portName))
            {
                label3.Text = (nomalPorts.Count() - 1 - ePorts.Count()).ToString();
                label4.Text = ePorts.Count().ToString();
                return;
            }*/

            if (!nomalPorts.Contains(portName) && ePorts.Contains(portName))
            {
                ePorts.Remove(portName);
                label3.Text = (nomalPorts.Count() - 1 - ePorts.Count()).ToString();
                label4.Text = ePorts.Count().ToString();
                return;
            }

            if (nomalPorts.Contains(portName) && !ePorts.Contains(portName))
            {
                ePorts.Add(portName);
                label3.Text = (nomalPorts.Count() - 1 - ePorts.Count()).ToString();
                label4.Text = ePorts.Count().ToString();
                return;
            }
        });

        public void CheckPortsCount(string portName) => Invoke(() =>
        {
            var nomalPorts = SerialPort.GetPortNames();
            /*if (nomalPorts.Contains(portName) && ePorts.Contains(portName))
            {
                label3.Text = (nomalPorts.Count() - 1 - ePorts.Count()).ToString();
                label4.Text = ePorts.Count().ToString();
                return;
            }*/

            if (ePorts.Contains(portName))
            {
                ePorts.Remove(portName);
                label3.Text = (nomalPorts.Count() - 1 - ePorts.Count()).ToString();
                label4.Text = ePorts.Count().ToString();
                return;
            }
        });

        public void ShowTotalPortsNumber(int number) => Invoke(() =>
        {
            label6.Text = number.ToString();
        });

        public int GetRowIndex(string port) => Invoke(() =>
        {
            DataTable dTable = dataTable;
            return dTable.Rows.IndexOf(dTable.Rows.Find(port));
        });
        /*public void FindRowToDelete(string port) => Invoke(() =>
        {
            DataTable dTable = dataTable;
            dTable.PrimaryKey = new DataColumn[] { dTable.Columns[0] };

            if (dTable.Rows.Contains(port))
            {
                var row = dTable.Rows.Find(port);
                dTable.Rows.Remove(row);
            }
        });*/

        async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var latestVersionResponse = await HttpResponse("https://analytics.otp-service.online/otp/latest_version.txt");
                    var latestVersion = new Version(latestVersionResponse);

                    var versionsComparer = currentVersion.CompareTo(latestVersion);
                    if (versionsComparer == -1)
                    {
                        var latestVersionFileName = $"OtpAgentForms_v{latestVersion.ToString(1)}.zip";
                        //Cmd($"taskkill /f /im \"{exeName}.exe\" && timeout /t 1 && tar -xf OtpAgentForms_v6.zip && timeout /t 3 && \"{exePath}\"");
                        //Cmd($"taskkill /f /im \"{exeName}\" && timeout /t 1 && del \"{exePath}\" && cd /d {Environment.CurrentDirectory} && tar -xf OtpAgentForms_v6.zip && timeout /t 5 && \"{exePath}\")");
                        //Process.Start(new ProcessStartInfo { FileName = "cmd", WorkingDirectory = Environment.CurrentDirectory, Arguments = ("del  && tar -xf OtpAgentForms_v6.zip"), WindowStyle = ProcessWindowStyle.Normal }).WaitForExit();
                        if (File.Exists(Environment.CurrentDirectory + @$"\{latestVersionFileName}"))
                        {
                            File.Delete(Environment.CurrentDirectory + $@"\{latestVersionFileName}");
                        }
                        using (var stream = await client.GetStreamAsync($"https://analytics.otp-service.online/otp/OtpAgentFormsAllVersions/{latestVersionFileName}"))

                        using (var fileStream = new FileStream(Environment.CurrentDirectory + @$"\{latestVersionFileName}", FileMode.CreateNew))
                        await stream.CopyToAsync(fileStream);

                        Cmd($"tasklist && taskkill /f /im \"{exeName}.exe\" && timeout /t 1 && tar -xf {latestVersionFileName} --exclude conf.ini && timeout /t 3 && del {latestVersionFileName} && \"{exeName}.exe\" & exit");
                        //Cmd($"cd /d {Environment.CurrentDirectory} && /tar -xf OtpAgentForms_v6.zip");
                        //Cmd($"taskkill /f /im \"{exeName}\" && timeout /t 1 && del \"{exePath}\" && cd /d {Environment.CurrentDirectory} && tar -xf OtpAgentForms_v6.zip && timeout /t 5 && \"{exePath}\")");
                    }
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show("Download new version error");
            }
        }

        void Cmd(string line)
        {
            Process.Start(new ProcessStartInfo { FileName = "cmd", WorkingDirectory = Environment.CurrentDirectory, Arguments = $"/c {line}", WindowStyle = ProcessWindowStyle.Hidden });
        }

        async Task<string> HttpResponse(string line)
        {
            using (var net = new HttpClient())
            {
                var response = await net.GetAsync(line);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null; 
            }
        }
    }
}