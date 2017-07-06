using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.WorkBench;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LoowooTech.Stock
{
    public partial class MainForm : Form
    {
        private string _dataPath;
        private string _mdbPath;
        private IWorkBench _workBench;
        private string[] _spaceArray = new string[] { "XZQ_XZ", "XZQ_XZC", "XZQJX", "DCDYTB" };
        
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
        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        private SimpleMarkerSymbolClass simpleMarkerSymbol { get; set; }

        public MainForm()
        {
         
            InitializeComponent();

            simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Width = 4;
            simpleLineSymbol.Color = GetRGBColor(255, 0, 99);
            simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            simpleMarkerSymbol.Size = 8;
            simpleMarkerSymbol.Color = GetRGBColor(255, 0, 0);

        }
        public  IRgbColor GetRGBColor(int Red, int Green, int Blue, byte Alpha = 255)
        {
            IRgbColor color = new RgbColorClass();
            color.Red = Red;
            color.Green = Green;
            color.Blue = Blue;
            color.Transparency = Alpha;
            return color;
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
                        Full();
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

            lblOperator.Text = "正在进行质检...";
            //var ids = new List<int>() { 11, 1201, 2101, 2201, 3101, 3201, 3301, 3401, 4101, 4201, 4301, 5101, 6101, 6201 };
            var ids = new List<int>();
            RuleHelper.GetCheckedRuleIDs(treeView1.Nodes, ids);
            _workBench = new WorkBench2();
            _workBench.RulsIds = ids;
            _workBench.Folder = _dataPath;
            btnStart.Enabled = false;
            btnExport.Enabled = false;
            var form = new ProgressForm(_workBench);
            form.ShowDialog();
            if(form.StopRequested == false)
            {
                RuleHelper.UpdateCheckState(treeView1.Nodes, form.Results);
                LoadResults(_workBench.Results);
                btnExport.Enabled = true;
                MessageBox.Show("已经完成质检", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //if (MessageBox.Show("质检完成，是否需要自动生成一份统计表格？", "质检提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            //{
            //    var dialog = new FolderBrowserDialog { Description = "请选择统计表格保存的文件夹" };
            //    if (dialog.ShowDialog() == DialogResult.OK)
            //    {
            //        var saveFolder = dialog.SelectedPath;
            //        _workBench.Write(saveFolder);
            //    }
            //}
            lblOperator.Text = "就绪";
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
                    q.TableName, q.BSM, q.Description, q.Remark,q.WhereClause
                }));
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "Excel文件(*.xls)|*.xls", Title = "请选择质检结果导出文件" };
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(_workBench.ReportPath, dialog.FileName);
                    MessageBox.Show("导出质检结果文件成功", "导出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("导出文件时出现错误:" +ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                
            }
        }

        private void btnGlobe2_Click(object sender, EventArgs e)
        {
            Full();
        }

        private void Full()
        {
            var tool = new ControlsMapFullExtentCommandClass();
            tool.OnCreate(this.axMapControl1.Object);
            tool.OnClick();
        }

        private void btnPan2_Click(object sender, EventArgs e)
        {
            var tool = new ControlsMapPanToolClass();
            tool.OnCreate(this.axMapControl1.Object);
            tool.OnClick();
        }



        private void Center(IFeature feature)
        {
            var envelope = feature.Shape.Envelope;
            IPoint point = new PointClass();
            try
            {
                point.PutCoords((envelope.XMin + envelope.XMax) / 2, (envelope.YMin + envelope.YMax) / 2);
            }
            catch
            {
                envelope = axMapControl1.ActiveView.Extent;
                point.PutCoords((envelope.XMin + envelope.XMax) / 2, (envelope.YMin + envelope.YMax) / 2);
            }
            envelope.Expand(2, 2, true);

            var env2 = axMapControl1.ActiveView.Extent;
            env2.CenterAt(point);
            axMapControl1.ActiveView.Extent = envelope;//env2  时  当前视图显示范围

            axMapControl1.ActiveView.Refresh();
        }

        private void Twinkle(IFeature feature)
        {
            if (feature.Shape == null) return;
            switch (feature.Shape.GeometryType)
            {
                case esriGeometryType.esriGeometryMultipoint:
                case esriGeometryType.esriGeometryPoint:
                    axMapControl1.FlashShape(feature.Shape, 4, 300, simpleMarkerSymbol);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryLine:
                    axMapControl1.FlashShape(feature.Shape, 4, 300, simpleLineSymbol);
                    break;
            }

        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                var whereClause = this.listView1.SelectedItems[0].SubItems[7].Text;
                var className = this.listView1.SelectedItems[0].SubItems[3].Text;
                if (string.IsNullOrEmpty(whereClause))
                {
                    //MessageBox.Show("当前记录不支持定位显示");
                    return;
                }
                if (!_spaceArray.Contains(className))
                {
                    MessageBox.Show("暂不支持非空间数据查看");
                    return;
                }
                var featureClass = ParameterManager.Workspace.GetFeatureClass(className);
                if (featureClass == null)
                {
                    MessageBox.Show("获取要素类失败......");
                    return;
                }
                var feature = ArcGISManager.Search(featureClass, whereClause);
                if (feature != null)
                {
                    this.tabControl2.SelectedIndex = 0;
                    Center(feature);
                    Twinkle(feature);

                }
                
            }
        }

        private void btnIdentity2_Click(object sender, EventArgs e)
        {
            var tool = new ControlsMapIdentifyToolClass();
            tool.OnCreate(this.axMapControl1.Object);
            tool.OnClick();
        }
    }
    
   
}
