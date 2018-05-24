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
    public partial class MergeMDBForm : Form
    {
        private delegate void ShowInfoDelegate(string message);
        private Thread _thread { get; set; }
        private IMerge _merge { get; set; }

        public bool StopRequested { get; set; }
        public MergeMDBForm()
        {
            InitializeComponent();
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
        private void WorkBench_OnProgramProcess(object sender, CollectProgressEventArgs e)
        {
            ShowInfo(e.Message);
            e.Cancel = StopRequested;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.SourcesBox.Text = DialogClass.SelectFolder(this.SourcesBox.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.SaveFileBox.Text = DialogClass.SaveFile("MBD文件|*.mdb", "合成mdb文件");
        }

        private void Mergebutton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.SourcesBox.Text))
            {
                MessageBox.Show("请选择矢量数据文件夹");
                return;
            }
            if (string.IsNullOrEmpty(this.SaveFileBox.Text))
            {
                MessageBox.Show("请选择保存文件路径");
                return;
            }

            this.Mergebutton.Enabled = false;

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
                    _thread = new Thread(Run);
                    _thread.IsBackground = true;
                    _thread.Start();
                }
                else
                {
                    _thread.Join();
                }
            }

            this.Mergebutton.Enabled = true;

            
        }

        private void Run()
        {
            _merge = new MergeTool();
            _merge.SourceFolder = this.SourcesBox.Text;
            _merge.SaveFile = this.SaveFileBox.Text;
            _merge.OnProgramProcess += WorkBench_OnProgramProcess;
            _merge.Program();
        }
    }
}
