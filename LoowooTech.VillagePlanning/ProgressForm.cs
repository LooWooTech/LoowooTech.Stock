using LoowooTech.Stock.WorkBench;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LoowooTech.VillagePlanning
{
    public partial class ProgressForm : Form
    {
        private delegate void ShowInfoDelegate(string message);

        private Thread _thread;

        private bool _canExit;

        private IWorkBench2 _workBench;
        public bool StopRequested { get; private set; }
        public ProgressForm()
        {
            InitializeComponent();
        }

        public ProgressForm(IWorkBench2 workbench) : base()
        {
            _workBench = workbench;
            _workBench.OnProgramProcess += WorkBench_OnProgramProcess;
            InitializeComponent();

        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            var timer = new System.Windows.Forms.Timer { Interval = 500 };
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, System.EventArgs e)
        {
            if (_thread == null)
            {
                _thread = new Thread(Run);
                _thread.IsBackground = true;
                _thread.Start();
            }
            else
            {
                if (_thread.IsAlive == false)
                {
                    _canExit = true;
                    Close();
                }
            }
        }

        private void Run()
        {
            _workBench.Program();
        }

        private void ShowInfo(string message)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke(new ShowInfoDelegate(ShowInfo), message);
            }
            else
            {
                listBox1.Items.Add(string.Format("[{0:HH:mm:ss}]{1}", DateTime.Now, message));
            }
        }

        private void WorkBench_OnProgramProcess(object sender, ProgressEventArgs e)
        {
            ShowInfo(e.Message);
            e.Cancel = StopRequested;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("当前操作将中断质检并可能丢失部分结果，是否确认中断？", "是否中断", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                StopRequested = true;
            }
        }

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_canExit) return;
            MessageBox.Show("质检过程中无法关闭窗口，请等待质检结束或中断质检。", "关闭窗口", MessageBoxButtons.OK, MessageBoxIcon.Information);
            e.Cancel = true;
        }
    }
}
