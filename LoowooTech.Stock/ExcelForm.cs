using LoowooTech.Stock.Common;
using LoowooTech.Stock.Rules;
using LoowooTech.Stock.Tool;
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
    public partial class ExcelForm : Form
    {
        private delegate void ShowInfoDelegate(string message);
        private Thread _thread { get; set; }
        private ICollect2 _collect { get; set; }
        public bool StopRequested { get; set; }

        public ExcelForm()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.SavetextBox.Text = DialogClass.SelectFolder(this.SavetextBox.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ResulttextBox.Text = DialogClass.SelectFolder(this.ResulttextBox.Text);
        }

        private void ExcelForm_Load(object sender, EventArgs e)
        {
            this.ExcelcheckedListBox.Items.Clear();
            var excels = Arguments.TableFieldDict;
            var collects = Arguments.CollectTableDict;
            foreach(var entry in excels)
            {
                this.ExcelcheckedListBox.Items.Add(entry.Key.Name + "、" + entry.Key.Title.Replace("{Name}({Code})", ""), true);
            }
            foreach(var entry in collects)
            {
                this.ExcelcheckedListBox.Items.Add(entry.Key.Name + "、" + entry.Key.Title, true);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ResulttextBox.Text) || !System.IO.Directory.Exists(this.ResulttextBox.Text))
            {
                MessageBox.Show("请输入有效正确的区县农村存量建设用地调查数据成果路径");
                return;
            }

            if (string.IsNullOrEmpty(this.SavetextBox.Text))
            {
                MessageBox.Show("请选择正确的保存路径");
                return;
            }

            this.button3.Enabled = false;
            this.button4.Enabled = false;

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

            this.button3.Enabled = true;
            this.button4.Enabled = true;


        }

        private void Run()
        {
            _collect = new CollectExcelTool2();
            _collect.ResultFolder = this.ResulttextBox.Text;
            _collect.SaveFolder = this.SavetextBox.Text;
            _collect.Excels = GetCollectTables();
            _collect.OnProgramProcess += WorkBench_OnProgramProcess;
            _collect.Program();


        }



        private string[] GetCollectTables()
        {
            var list = new List<string>();
            for(var i = 0; i < this.ExcelcheckedListBox.Items.Count; i++)
            {
                if (this.ExcelcheckedListBox.GetItemChecked(i))
                {
                    var val = this.ExcelcheckedListBox.GetItemText(this.ExcelcheckedListBox.Items[i]);
                    var key= val.Split('、')[0];
                    list.Add(key);
                }
            }

            return list.ToArray();
        }
    }
}
