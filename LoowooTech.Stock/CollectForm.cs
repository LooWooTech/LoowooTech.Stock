using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Rules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LoowooTech.Stock
{
    public partial class CollectForm : Form
    {
        private delegate void ShowInfoDelegate(string message);
        private Thread _thread { get; set; }
        private ICollect _collect { get; set; }

        public bool StopRequested { get; set; }
        public CollectType CollectType { get; set; }
        public CollectForm()
        {
            InitializeComponent();
            this.Text = string.Format("{0}数据汇总", CollectType.GetDescription());
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.SourceBox.Text = DialogClass.SelectFolder();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.SaveBox.Text = DialogClass.SelectFolder();
        }
        private void WorkBench_OnProgramProcess(object sender,CollectProgressEventArgs e)
        {
            ShowInfo(e.Message);
            e.Cancel = StopRequested;
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

        private void Startbutton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.SourceBox.Text)||!System.IO.Directory.Exists(this.SourceBox.Text))
            {
                MessageBox.Show("请输入正确的数据路径");
                return;
            }

            if (string.IsNullOrEmpty(this.SaveBox.Text)||!System.IO.Directory.Exists(this.SaveBox.Text))
            {
                MessageBox.Show("请选择正确的保存路径");
                return;
            }
            this.Startbutton.Enabled = false;
            this.Closebutton.Enabled = false;
            Run();
            //if (_thread == null)
            //{
            //    _thread = new Thread(Run);
            //    _thread.IsBackground = true;
            //    _thread.Start();
            //}
            //else
            //{
            //    if (_thread.IsAlive == false)
            //    {

            //    }
            //}

            this.Startbutton.Enabled = true;
            this.Closebutton.Enabled = true;
        }
        private void Run()
        {
            _collect = new CollectDataTool();
            _collect.CollectExcelType = this.radioButton1.Checked == true ? CollectExcelType.Excel1 : CollectExcelType.Excel2;
            _collect.SourceFolder = this.SourceBox.Text;
            _collect.SaveFolder = this.SaveBox.Text;
            _collect.CollectType = CollectType;
            _collect.CollectXZQ = ParameterManager.CollectXZQ;
            _collect.OnProgramProcess += WorkBench_OnProgramProcess;
            _collect.Program();
        }
    }
}
