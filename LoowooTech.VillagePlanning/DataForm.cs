using LoowooTech.Stock.ArcGISTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.VillagePlanning
{
    public partial class DataForm : Form
    {
        public DataForm()
        {
            InitializeComponent();
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DataForm_Load(object sender, EventArgs e)
        {
            this.CUNtextBox.Text = ParameterManager2.Folder;
            this.XIANGtextBox.Text = ParameterManager2.XGH;
            this.DLTBtextBox.Text = ParameterManager2.TDLYXZ;
            this.GDtextBox.Text = ParameterManager2.GD;
            this.NZYtextBox.Text = ParameterManager2.NZY;
        }

        private void CUNbutton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog { Description = this.CUNtextBox.Text,SelectedPath=this.CUNtextBox.Text };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.CUNtextBox.Text = dialog.SelectedPath;
            }

        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            ParameterManager2.Folder = this.CUNtextBox.Text;
            ParameterManager2.XGH = this.XIANGtextBox.Text;
            ParameterManager2.TDLYXZ = this.DLTBtextBox.Text;
            ParameterManager2.GD = this.GDtextBox.Text;
            ParameterManager2.NZY = this.NZYtextBox.Text;
            this.Close();
        }

        private void DLTBbutton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog { Title = "选择变更调查数据库成果" ,FileName=this.DLTBtextBox.Text,Filter="Personal GDBDatabase|*.mdb"};
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.DLTBtextBox.Text = dialog.FileName;
            }
        }

        private void XIANGbutton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog { Title = "选择乡规划数据库成果文件", FileName = this.XIANGtextBox.Text, Filter = "Personal GDBDatabase|*.mdb" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.XIANGtextBox.Text = dialog.FileName;
            }
        }

        private void GDbutton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog { Title = "选择新增耕地数据库成果文件", FileName = this.GDtextBox.Text, Filter = "Personal GDBDatabase|*.mdb" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.GDtextBox.Text = dialog.FileName;
            }
        }

        private void NZYbutton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog { Title = "选择农转用数据库成果", FileName = this.NZYtextBox.Text, Filter = "Personal GDBDatabase|*.mdb" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.NZYtextBox.Text = dialog.FileName;
            }
        }
    }
}
