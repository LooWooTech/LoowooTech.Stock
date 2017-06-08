using LoowooTech.Stock.WorkBench;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Windows.Forms;

namespace LoowooTech.Stock
{
    public partial class ProgressForm : Form
    {
        private delegate void ShowInfoDelegate(string message);
        
        private IWorkBench _workBench;

        private bool _canExit;

        private Thread _thread;
        
        public Dictionary<string, ProgressResultTypeEnum> Results { get; private set; }
        
        public bool StopRequested { get; private set; }
        public ProgressForm()
        {
            InitializeComponent();
            Results = new Dictionary<string, ProgressResultTypeEnum>();
        }

        public ProgressForm(IWorkBench workbench):base()
        {
            _workBench = workbench;
            _workBench.OnProgramProcess += WorkBench_OnProgramProcess;
        }

        private void ShowInfo(string message)
        {
            if(listBox1.InvokeRequired)
            {
                listBox1.Invoke(new ShowInfoDelegate(ShowInfo), message);
            }
            else
            {
                listBox1.Items.Add(string.Format("[{0:HH:mm:ss]{1}", DateTime.Now, message));
            }
        }

        private void WorkBench_OnProgramProcess(object sender, ProgressEventArgs e)
        {
            ShowInfo(e.Message);
            lock (Results)
            {
                Results.Add(e.Code, e.Result);
            }
            e.Cancel = StopRequested;
        }

        private void ProgressForm_Load(object sender, System.EventArgs e)
        {
            var timer = new System.Windows.Forms.Timer{ Interval = 500 };
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

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_canExit) return;
            MessageBox.Show("质检过程中无法关闭窗口，请等待质检结束或中断质检。", "关闭窗口", MessageBoxButtons.OK, MessageBoxIcon.Information);
            e.Cancel = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("当前操作将中断质检并可能丢失部分结果，是否确认中断？", "是否中断", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                StopRequested = true;
            }
        }
    }
}
