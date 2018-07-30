using LoowooTech.Stock.Models;
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
    public partial class FileViewForm : Form
    {
        private List<StockFile> _files { get; set; }
        public FileViewForm(List<StockFile> files,string text)
        {
            _files = files;
            InitializeComponent();
            this.Text = text;
        }

        private void FileViewForm_Load(object sender, EventArgs e)
        {
            this.FilecomboBox.Items.Clear();
            foreach(var item in _files)
            {
                this.FilecomboBox.Items.Add(item.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileName = this.FilecomboBox.SelectedItem.ToString();
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("请选择需要查看的文件");
            }
            var entry = _files.FirstOrDefault(j => j.FileName.ToLower() == fileName.ToLower());
            if (entry == null)
            {
                MessageBox.Show("未识别文件信息");
            }
            else
            {
                System.Diagnostics.Process.Start(entry.FullName);
            }
            Console.WriteLine(fileName);
        }
    }
}
