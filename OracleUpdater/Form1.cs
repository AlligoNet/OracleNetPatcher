using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace OracleUpdater
{
    public partial class UpdaterForm : Form
    {
        private string downloadServer = "REDACTED";

        public UpdaterForm()
        {
            InitializeComponent();

            /* Kill Main Client */
            Process[] activeProcesses = Process.GetProcesses();
            foreach( Process currProc in activeProcesses)
            {
                string procName = currProc.ProcessName.ToLower();
                if( procName.CompareTo("oracleclient") == 0)
                {
                    currProc.Kill();
                    break;
                }
            }

            // Update OracleNet Client
            UpdateOracleNet();
        }

        private void UpdateOracleNet()
        {
            //Remove Backup if already exists
            File.Delete("OracleLauncher_old.exe");

            //Rename old connector - REEBABLE AFTER DEBUG
            File.Move("OracleLauncher.exe", "OracleLauncher_old.exe");

            //Download Client Update
            WebClient downloader = new WebClient();
            //downloader.DownloadDataCompleted += UpdateDownloaded;
            downloader.DownloadFileCompleted += UpdateDownloaded;
            downloader.DownloadProgressChanged += UpdateProgressChanged;
            downloader.DownloadFileAsync(new System.Uri(downloadServer), "OracleLauncher.exe");
        }

        // Runs as file is downloading
        void UpdateProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgressBar.Value = e.ProgressPercentage;
        }

        // Runs when file download is complete
        private void UpdateDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            // Reopen OracleClient.exe
            Process.Start("OracleLauncher.exe");

            MessageBox.Show("Update Completed");

            // Kill Self
            Application.Exit();
        }
    }
}
