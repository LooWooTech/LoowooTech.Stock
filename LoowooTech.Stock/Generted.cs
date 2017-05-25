using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock
{
    public class Generted
    {
        private System.Diagnostics.Process _process { get; set; }
        private UpdateProgressDelegate progressDelegate;
        public void Run(UpdateProgressDelegate progressDeletate)
        {
            this.progressDelegate = progressDeletate;
            _process = new System.Diagnostics.Process();
            _process.StartInfo.FileName = "";
            _process.StartInfo.Arguments = string.Format("{0} {1} {3}");
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.OutputDataReceived += WorkerProcess_OutputDataReceived;
            _process.Start();
            _process.BeginOutputReadLine();
        }

        public void WorkerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_process == sender)
            {
                progressDelegate(string.Format("{0}", e.Data));
            }
        }
    }
}
