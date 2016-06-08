using LoowooTech.Stock.Common;
using LoowooTech.Stock.Rules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Stock
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = DialogClass.OpenFile("数据库文件|*.mdb", "打开mdb");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CESHI.Program(this.textBox1.Text);
        }
    }
}
