using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
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
    public partial class Form1 : DMSkin.Main
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dmLabel1_Click(object sender, EventArgs e)
        {
            this.metroTextBox1.Text = DialogClass.OpenFile("数据库文件|*.mdb", "打开数据库文件");
        }

        private void dmButton1_Click(object sender, EventArgs e)
        {
            var heart = new HeartClass() { mdbFilePath = this.metroTextBox1.Text };
            heart.Beat();
        }
    }
}
