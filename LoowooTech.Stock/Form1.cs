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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dmLabel1_Click(object sender, EventArgs e)
        {
            //this.metroTextBox1.Text = DialogClass.OpenFile("数据库文件|*.mdb", "打开数据库文件");
        }

        private void dmButton1_Click(object sender, EventArgs e)
        {
            //var heart = new HeartClass() { mdbFilePath = this.metroTextBox1.Text };
            //heart.Beat();
        }

        private void FolderButton_Click(object sender, EventArgs e)
        {
            this.folderText.Text = DialogClass.SelectFolder();

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.folderText.Text))
            {
                MessageBox.Show("请选择质检路径");
            }
            var heart = new Heart(this.folderText.Text);
            heart.Program();
        }
    }
}
