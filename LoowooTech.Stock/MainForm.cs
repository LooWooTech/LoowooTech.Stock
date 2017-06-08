using ESRI.ArcGIS.Controls;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.WorkBench;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

namespace LoowooTech.Stock
{
    public partial class MainForm : Form
    {
        private string _dataPath;
        private string _mdbPath;
        private IWorkBench _workBench;
        
        private static readonly Dictionary<string, Type> toolDictionary = new Dictionary<string, Type>
        {
            {"ZoomIn", typeof(ControlsMapZoomInToolClass) },
            {"ZoomOut", typeof(ControlsMapZoomOutToolClass) },
            {"Globe", typeof(ControlsMapFullExtentCommandClass) },
            {"Previous", typeof(ControlsMapZoomToLastExtentBackCommandClass) },
            {"Next", typeof(ControlsMapZoomToLastExtentForwardCommandClass) },
            {"Pan", typeof(ControlsMapZoomToLastExtentBackCommandClass) },
            {"Identity", typeof(ControlsMapIdentifyToolClass) }
        };

        static MainForm()
        {
            
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private XmlNode ConfigDocument
        {
            get
            {
                var doc = new XmlDocument();
                doc.Load(string.Format("{0}\\Config.xml", Application.StartupPath));
                return doc.SelectSingleNode("Config");
            }
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            RuleHelper.LoadRules(treeView1, ConfigDocument);
        }


        private void ToolButton_Click(object sender, EventArgs e)
        {
            if (sender is Control)
            {
                var btn = sender as Control;
                if (btn.Tag != null && toolDictionary.ContainsKey(btn.Tag.ToString()))
                {
                    var type = toolDictionary[btn.Tag.ToString()];
                    var o = Activator.CreateInstance(type);
                    var cmd = o as ESRI.ArcGIS.SystemUI.ICommand;
                    if (o != null)
                    {
                        cmd.OnCreate(axMapControl1.Object);
                        var tool = o as ESRI.ArcGIS.SystemUI.ITool;
                        if (tool != null)
                        {
                            axMapControl1.CurrentTool = tool;
                        }
                        else
                        {
                            cmd.OnClick();
                        }
                    }
                }
            }
        }


        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if(sender is Control)
            {
                var btn = sender as Control;
                if(btn.Tag != null && toolDictionary.ContainsKey(btn.Tag.ToString()))
                {
                    var type = toolDictionary[btn.Tag.ToString()];
                    var o = Activator.CreateInstance(type);
                    var cmd = o as ESRI.ArcGIS.SystemUI.ICommand;
                    if (o != null)
                    {
                        cmd.OnCreate(axMapControl1.Object);
                        var tool = o as ESRI.ArcGIS.SystemUI.ITool;
                        if(tool != null)
                        {
                            axMapControl1.CurrentTool = tool;
                        }
                        else
                        {
                            cmd.OnClick();
                        }
                    }
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog { Description = "请选择数据成果所在文件夹" };
            if(dialog.ShowDialog()== DialogResult.OK)
            {
                _dataPath = dialog.SelectedPath;
                lblPath.Text = _dataPath;
                var configDoc = ConfigDocument;
               
                var mdbPath = string.Empty;
                if (FileListHelper.LoadFileList(_dataPath, configDoc, treeView2, ref mdbPath) == false)
                {
                    MessageBox.Show("成果中缺少必要的文件夹或文件，请检查文件列表。", "缺少文件", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tabControl2.SelectTab(1);
                }
                btnStart.Enabled = true;

                _mdbPath = mdbPath;
                if(string.IsNullOrEmpty(mdbPath) == false)
                {
                    try
                    {
                        FileListHelper.LoadMapData(_mdbPath, axMapControl1, configDoc);
                    }
                    catch
                    {
                        MessageBox.Show("数据库格式有误或缺少必要图层", "数据库错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            
        }

        private void treeView2_DoubleClick(object sender, EventArgs e)
        {
            var selectNode = treeView2.SelectedNode;
            if (selectNode != null && selectNode.Tag != null)
            {
                var pInfo = new ProcessStartInfo();
                pInfo.UseShellExecute = true;

                pInfo.FileName = selectNode.Tag.ToString();
                Process.Start(pInfo);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(_mdbPath) || string.IsNullOrEmpty(_mdbPath))
            {
                MessageBox.Show("您还没有指定数据文件所在目录或目录中缺失数据库文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ids = new List<int>();
            RuleHelper.GetCheckedRuleIDs(treeView1.Nodes, ids);
            _workBench.RulsIds = ids;
            btnStart.Enabled = false;
            var form = new ProgressForm(_workBench);
            form.ShowDialog();
            if(form.StopRequested == false)
            {
                RuleHelper.UpdateCheckState(treeView1.Nodes, form.Results);
                LoadResults(_workBench.Results);
                btnExport.Enabled = true;
                MessageBox.Show("已经完成质检", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            btnStart.Enabled = true;
        }

        private void LoadResults(List<Question> questions)
        {
            listView1.Items.Clear();
            foreach (var q in questions)
            {
                var item = listView1.Items.Add(new ListViewItem(new string[]
                {
                    q.Code, q.Name, q.Project.ToString(),
                    q.TableName, q.BSM, q.Description, q.Remark
                }));
            }
        }
    }
    
   
}
