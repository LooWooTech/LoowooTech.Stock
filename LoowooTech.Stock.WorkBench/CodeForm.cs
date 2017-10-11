using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Stock.WorkBench
{
    public partial class CodeForm : Form
    {
        private Dictionary<string,List<XZC>> _dict { get; set; }

        public Dictionary<string,List<XZC>> Dict { get { return _dict; }set { _dict = value; } }

        
        public CodeForm()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void InitValues()
        {
            this.treeView1.Nodes.Clear();
            foreach(var entry in Dict)
            {
                var node = new TreeNode(entry.Key);
                node.Nodes.AddRange(entry.Value.Select(e => new TreeNode(string.Format("{0},{1}", e.XZCMC, e.XZCDM))).ToArray());
                this.treeView1.Nodes.Add(node);
            }

            if (Dict.Count == 0)
            {
                this.OKbutton.Enabled = false;
            }
            //this.treeView1.ExpandAll();
        }

        private void CodeForm_Load(object sender, EventArgs e)
        {
            InitValues();
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            if (Dict.Count == 0)
            {
                MessageBox.Show("未读取到单位代码表，无法点击‘确定’继续质检");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
