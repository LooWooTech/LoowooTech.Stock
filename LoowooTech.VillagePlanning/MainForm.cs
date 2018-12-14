using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.WorkBench;
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
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private IWorkBench2 _workBench;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadForm.Instance.Close();
            LoadRules();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dialog = new DataForm();
            dialog.ShowDialog(this);
        }

        private void LoadRules()
        {
            this.treeView1.Nodes.Clear();
            var ruleNodes = ParameterManager2.ConfigXml.SelectNodes("/Config/Rules/Rule");
            if (ruleNodes.Count > 0)
            {
                for(var i = 0; i < ruleNodes.Count; i++)
                {
                    var node = ruleNodes[i];
                    var childNode = treeView1.Nodes.Add(node.Attributes["ID"].Value, string.Format("[{0}]{1}", node.Attributes["ID"].Value, node.Attributes["Title"].Value));
                    childNode.Tag = node.Attributes["ID"].Value;
                    childNode.ImageIndex = 0;
                    childNode.Checked = true;
                }
            }
        }

        private List<int> GetRuleIds(TreeNodeCollection nodes)
        {
            var list = new List<int>();
            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                {
                    if (node.Nodes.Count == 0)
                    {
                        list.Add(int.Parse(node.Tag.ToString()));
                    }
                    else
                    {
                        list.AddRange(GetRuleIds(node.Nodes));
                    }
                }
            }

            return list;
        }

        private void LoadResults(List<Question2> questions)
        {
            listView1.Items.Clear();
            foreach(var q in questions)
            {
                var item = listView1.Items.Add(new ListViewItem(new string[] {
                    q.Code,q.Name,q.CheckProject.ToString(),
                    q.TableName,q.BSM,q.Description
                }));
            }
        }

        private void StartCheckButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(ParameterManager2.Folder) ||System.IO.Directory.Exists(ParameterManager2.Folder) == false)
            {
                MessageBox.Show("请指定有效的村规划数据库成果路径之后，点击‘开始质检’");
                return;
            }
            lblOperator.Text = "正在进行质检.....";

            _workBench = new WorkBench3();
            _workBench.IDs = GetRuleIds(treeView1.Nodes);

            var form = new ProgressForm(_workBench);
            form.ShowDialog();
            if (form.StopRequested == false)
            {
                LoadResults(_workBench.Results);
                MessageBox.Show("已经完成质检", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            lblOperator.Text = "就绪";

        }
    }
}
