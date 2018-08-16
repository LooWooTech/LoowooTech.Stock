using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Rules;
using LoowooTech.Stock.Tools;
using LoowooTech.Stock.WorkBench;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace LoowooTech.Stock
{
    public partial class AdvanceForm : Form
    {
        private string _dataPath { get; set; }
        private string _mdbPath { get; set; }
        private string _codePath { get; set; }
        private string[] _spaceArray { get; set; } = new string[] { "XZQ_XZ", "XZQ_XZC", "XZQJX", "DCDYTB" };
        
        private static readonly Dictionary<string, Type> toolDictionary = new Dictionary<string, Type>
        {
            {"ZoomIn", typeof(ControlsMapZoomInToolClass) },
            {"ZoomOut", typeof(ControlsMapZoomOutToolClass) },
            {"Globe", typeof(ControlsMapFullExtentCommandClass) },
            {"Previous", typeof(ControlsMapZoomToLastExtentBackCommandClass) },
            {"Next", typeof(ControlsMapZoomToLastExtentForwardCommandClass) },
            {"Pan", typeof(ControlsMapPanToolClass) },
            {"Identity", typeof(ControlsMapIdentifyToolClass) }
        };
        private List<StockFile> _docs { get; set; }


        public AdvanceForm()
        {
            InitializeComponent();
            simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Width = 4;
            simpleLineSymbol.Color = DisplayExtensions.GetRGBColor(255, 0, 99);
            simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            simpleMarkerSymbol.Size = 8;
            simpleMarkerSymbol.Color = DisplayExtensions.GetRGBColor(255, 0, 0);

            simpleFillSymbol = new SimpleFillSymbolClass();
            simpleFillSymbol.Style = esriSimpleFillStyle.esriSFSCross;
            simpleFillSymbol.Outline = simpleLineSymbol;
            simpleLineSymbol.Color = DisplayExtensions.GetRGBColor(255, 0, 0);
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
            LoadForm.Instance.Close();
        }
        
        private void ToolButton_Click(object sender, EventArgs e)
        {
            if (sender is RibbonButton)
            {
                var btn = sender as RibbonButton;
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
                var mdbfile = FolderExtensions.GetFiles(_dataPath, "*.mdb").FirstOrDefault();
                this.btnIdentity3.Enabled = false;
                this.btnAttributeSearch.Enabled = false;
                this.btnAddLayer.Enabled = false;
                if (mdbfile == null)
                {
                    MessageBox.Show("成果中缺失空间数据库文件，请核对！");
                }
                else
                {
                    mdbPath = mdbfile.FullName;
                    this.btnIdentity3.Enabled = true;
                }
                var folder1 = System.IO.Path.Combine(_dataPath, "1空间数据库");
                var codefile = FolderExtensions.GetFiles(folder1, "*.xls").FirstOrDefault();
                if (codefile == null)
                {
                    MessageBox.Show("成果中缺失单位代码表文件，请核对！");
                }
                else
                {
                    _codePath = codefile.FullName;
                    ExcelManager.Init(_codePath);
                    this.btnAttributeSearch.Enabled = true;
                }
                this.btnXJ.Enabled = System.IO.Directory.Exists(System.IO.Path.Combine(_dataPath, "2栅格图件", "县级成果"));
                this.btnZJ.Enabled = System.IO.Directory.Exists(System.IO.Path.Combine(_dataPath, "2栅格图件", "乡镇级成果"));
                this.btnExcel.Enabled = System.IO.Directory.Exists(System.IO.Path.Combine(_dataPath, "3统计表格"));
                var docFolder = System.IO.Path.Combine(_dataPath, "4文档报告");
                var docus = FolderExtensions.GetFiles(docFolder, "*.doc");
                this.btnWorkWord.Enabled = false;
                this.btnTechnique.Enabled = false;
                _docs = docus;

               
                foreach(var item in docus)
                {
                    if(Regex.IsMatch(item.FileName, @"^[\u4e00-\u9fa5]{3,}\(33[0-9]{4}\)农村存量建设用地调查工作报告.doc$"))
                    {
                        this.btnWorkWord.Enabled = true;
                    }

                    if(Regex.IsMatch(item.FileName, @"^[\u4e00-\u9fa5]{3,}\(33[0-9]{4}\)农村存量建设用地调查技术报告.doc$"))
                    {
                        this.btnTechnique.Enabled = true;
                    }
                }
                this.btnAddLayer.Enabled = true;
                this.dataGridView1.DataSource = null;
                this.dataGridView2.DataSource = null;
               

                _mdbPath = mdbPath;
                if(string.IsNullOrEmpty(mdbPath) == false)
                {
                    try
                    {
                        FileListHelper.LoadMapData2(_mdbPath, axMapControl1, configDoc);
                       
                        Full();
                    }
                    catch
                    {
                        MessageBox.Show("数据库格式有误或缺少必要图层", "数据库错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                var KZMJ = FolderExtensions.GetFiles2(folder1, new string[] { "*.shp" }).FirstOrDefault();
                if (KZMJ != null)
                {
                    FileListHelper.LoadKZBJ(KZMJ, axMapControl1);
                }

                var rasterfiles = FolderExtensions.GetFiles2(folder1, new string[] { "*.img"});
                foreach (var item in rasterfiles)
                {
                    FileListHelper.LoadRasterData(item, this.axMapControl1);
                }
                
            }
            
        }
        private void LoadSG(string folder,RibbonComboBox combox)
        {
            var files = FolderExtensions.GetFiles(folder, "*.jpg");
            foreach(var item in files)
            {
                combox.DropDownItems.Add(new RibbonLabel { Text = item.FileName });
            }
        }
        private void Full()
        {
            var tool = new ControlsMapFullExtentCommandClass();
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

        private void btnIdentity3_Click(object sender, EventArgs e)
        {
            var cmd = new SearchTool("调查单元图斑",this);
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = cmd as ITool;
        }


        private List<CollectTable> _tables { get; set; } = new List<CollectTable>()
        {
            new CollectTable { Name="CLZJD", Title="宅基地"},
            new CollectTable { Name="JYXJSYD",Title="经营性建设用地"},
            new CollectTable { Name="GGGL_GGFWSSYD",Title="公共管理及公共服务设施用地"},
            new CollectTable { Name="QTCLJSYD", Title="其它存量建设用地" }
        };
        

        private Dictionary<string, string> _SQLDict { get; set; } = new Dictionary<string, string>
        {
            { "CLZJD", "SELECT COUNT(*),SUM(JZZDMJ+FSYDMJ) FROM CLZJD WHERE XZCDM = '{0}' AND TBBH = '{1}'" },
            { "JYXJSYD","SELECT COUNT(*),SUM(JSYDMJ) FROM JYXJSYD WHERE XZCDM = '{0}' AND TBBH = '{1}'"},
            { "GGGL_GGFWSSYD","SELECT COUNT(*),SUM(JSYDMJ) FROM GGGL_GGFWSSYD WHERE XZCDM = '{0}' AND TBBH = '{1}'"},
            { "QTCLJSYD","SELECT COUNT(*),SUM(JSYDMJ) FROM QTCLJSYD WHERE XZCDM = '{0}' AND TBBH = '{1}'"}
        };

        private Dictionary<string,List<Models.Field>> _dict { get; set; }
        private Dictionary<string,List<Models.Field>> Dict { get { return _dict == null ? _dict = GetFields() : _dict; } }
        private Dictionary<string, List<Models.Field>> GetFields()
        {
            var dict = new Dictionary<string, List<Models.Field>>();
            foreach(var name in _SQLDict.Keys)
            {
                var fields = XmlClass.GetField(name);
                dict.Add(name, fields);
            }
            return dict;
        }
        private string _connectionString { get { return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", _mdbPath); } }
        private string _XZCDM { get; set; }
        private string _TBBH { get; set; }
        private DataTable Search(string XZCDM, string TBBH, string tableName,List<Models.Field> fields)
        {
            DataTable dataTable = new DataTable();
            foreach(var field in fields)
            {
                dataTable.Columns.Add(field.Title);
            }
            DataRow datarow = null;
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    var SQLText = string.Format("Select {0} FROM {1} WHERE XZCDM = '{2}' AND TBBH = '{3}'", string.Join(",", fields.Select(e => e.Name).ToArray()), tableName, XZCDM, TBBH);
                    command.CommandText = SQLText;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        datarow = dataTable.NewRow();
                        for(var i = 0; i < fields.Count; i++)
                        {
                            var field = fields[i];
                            var val = reader[i];
                            if (val != null)
                            {
                                datarow[field.Title] = val.ToString();
                            }
                            
                        }
                        dataTable.Rows.Add(datarow);
                    }


                }
                connection.Close();
                
            }
            return dataTable;
        }

        public void Search2(string XZCDM, string TBBH)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("字段");
            dataTable.Columns.Add("值");
            DataRow dataRow = null;
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("Select {0} FROM DCDYTB where XZCDM = '{1}' AND TBBH = '{2}'", string.Join(",", _featureValues.Select(j => j.Name).ToArray()), XZCDM, TBBH);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        for(var i = 0; i < _featureValues.Count; i++)
                        {
                            if (reader[i] == null)
                            {
                                continue;
                            }
                            var ff = _featureValues[i];
                            var value = reader[i].ToString();
                            dataRow = dataTable.NewRow();
                            dataRow["字段"] = ff.Title;
                            if(ff.Name== "DCDYLX")
                            {
                                switch (value)
                                {
                                    case "1":
                                        dataRow["值"] = "撤并型";
                                        break;
                                    case "2":
                                        dataRow["值"] = "保留型";
                                        break;
                                    case "3":
                                        dataRow["值"] = "集聚型";
                                        break;
                                }
                            }
                            else
                            {
                                dataRow["值"] = value;
                            }
                            dataTable.Rows.Add(dataRow);
                        }
                    }
                }
            }
            this.dataGridView1.DataSource = dataTable;
        }
        public void Search(string XZCDM,string TBBH)
        {
            _XZCDM = XZCDM;
            _TBBH = TBBH;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("类别");
            dataTable.Columns.Add("数量");
            dataTable.Columns.Add("建设用地面积【平方米】");
            DataRow datarow = null;
            using (var connection=new OleDbConnection(_connectionString))
            {
                connection.Open();
                foreach (var entry in _SQLDict)
                {
                    using (var command = connection.CreateCommand())
                    {
                        var sqlText = string.Format(entry.Value, XZCDM, TBBH);
                        command.CommandText = sqlText;
                        int a = 0;
                        double b = .0;
                        var reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            datarow = dataTable.NewRow();
                            var ee = _tables.FirstOrDefault(j => j.Name.ToLower() == entry.Key.ToLower());
                            if (ee != null)
                            {
                                datarow["类别"] = ee.Title;
                            }
                           
                            if (int.TryParse(reader[0].ToString(), out a))
                            {
                                datarow["数量"] = a;              
                            }
                            if(double.TryParse(reader[1].ToString(),out b))
                            {
                                datarow["建设用地面积【平方米】"] = Math.Round(b, 4);
                            }
                            dataTable.Rows.Add(datarow);
                        }

                    }
                }
             
                connection.Close();
            }
            this.dataGridView2.DataSource = dataTable;

        }
        private void btnXJ_Click(object sender, EventArgs e)
        {
            var folder = System.IO.Path.Combine(_dataPath, "2栅格图件", "县级成果");
            var files = FolderExtensions.GetFiles(folder, "*.jpg");
            var form = new FileViewForm(files,"县级成果查看");
            form.ShowDialog(this);

        }
        private void btnZJ_Click(object sender, EventArgs e)
        {
            var folder = System.IO.Path.Combine(_dataPath, "2栅格图件", "乡镇级成果");
            var files = FolderExtensions.GetFiles(folder, "*.jpg");
            var form = new FileViewForm(files, "乡镇级成果查看");
            form.ShowDialog(this);
        }
        private void btnExcel_Click(object sender, EventArgs e)
        {
            var folder = System.IO.Path.Combine(_dataPath, "3统计表格");
            var files = FolderExtensions.GetFiles(folder, "*.xls");
            var form = new FileViewForm(files, "统计表格查看");
            form.ShowDialog(this);
        }
        private void btnWorkWord_Click(object sender, EventArgs e)
        {
            foreach(var item in _docs)
            {
                if (Regex.IsMatch(item.FileName, @"^[\u4e00-\u9fa5]{3,}\(33[0-9]{4}\)农村存量建设用地调查工作报告.doc$"))
                {
                    System.Diagnostics.Process.Start(item.FullName);
                }
            }
        }
        private void btnTechnique_Click(object sender, EventArgs e)
        {
            foreach (var item in _docs)
            {
                if (Regex.IsMatch(item.FileName, @"^[\u4e00-\u9fa5]{3,}\(33[0-9]{4}\)农村存量建设用地调查技术报告.doc$"))
                {
                    System.Diagnostics.Process.Start(item.FullName);
                }
            }
        }
        public bool IsAttribute { get; set; }
        private void btnAttributeSearch_Click(object sender, EventArgs e)
        {
            var form = new SelectForm();
            form.Show(this);
            IsAttribute = true;
        }
        private List<Models.FeatureValue> _featureValues { get; set; } = new List<Models.FeatureValue>
        {
             new Models.FeatureValue { Name="XZCDM",Title="行政村代码"},
             new Models.FeatureValue { Name="XZCMC",Title="行政村名称"},
             new Models.FeatureValue { Name="TBBH",Title="图斑编号"},
             new Models.FeatureValue { Name="DCDYLX",Title="调查单元类型"},
             new Models.FeatureValue { Name="MJ",Title="面积"},
             new Models.FeatureValue { Name="BZ",Title="备注"}
        };

        public double SearchArea(string whereClause)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    var sb = new StringBuilder("Select SUM(MJ) FROM DCDYTB");
                    if (!string.IsNullOrEmpty(whereClause))
                    {
                        sb.AppendFormat(" WHERE {0}", whereClause);
                    }

                    command.CommandText = sb.ToString();
                    var obj = command.ExecuteScalar();
                    if (obj != null)
                    {
                        double a = .0;
                        if (double.TryParse(obj.ToString(), out a))
                        {
                            return a;
                        }
                    }
                }
                connection.Close();
            }

            return 0.0;
        }
        public DataTable Search(string whereClause)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("序号");
            foreach (var item in _featureValues)
            {
                dataTable.Columns.Add(item.Title);
            }
            DataRow datarow = null;
            using (var connection=new OleDbConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("Select {0} FROM DCDYTB", string.Join(",", _featureValues.Select(j => j.Name).ToArray()));
                    if (!string.IsNullOrEmpty(whereClause))
                    {
                        sb.AppendFormat(" WHERE {0}", whereClause);
                    }
                    var sqlText = sb.ToString();
                    command.CommandText = sqlText;
                    var reader = command.ExecuteReader();
                    var row = 1;
                    while (reader.Read())
                    {
                        datarow = dataTable.NewRow();
                        datarow["序号"] = row;
                        for (var i = 0; i < _featureValues.Count; i++)
                        {
                            var feature = _featureValues[i];
                            
                            if (reader[i] != null)
                            {
                                var val = reader[i].ToString();
                                if (feature.Name == "DCDYLX")
                                {
                                    switch (val)
                                    {
                                        case "1":
                                            datarow[feature.Title]= "撤并型";
                                            break;
                                        case "2":
                                            datarow[feature.Title] = "保留型";
                                            break;
                                        case "3":
                                            datarow[feature.Title] = "集聚型";
                                            break;
                                    }
                                }
                                else
                                {
                                    datarow[feature.Title] = val;
                                }
                            }
                         
                        }
                        dataTable.Rows.Add(datarow);
                        row++;
                    }

                }
                connection.Close();
            }
            return dataTable;
            //this.dataGridView1.DataSource = dataTable;
            //this.ExportExcelbutton.Enabled = true;
            //this.dataGridView2.DataSource = null;
        }

        private void dataGridView2_DoubleClick_1(object sender, EventArgs e)
        {
            int a = 0;
            var tableName = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            var count = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            if (!string.IsNullOrEmpty(tableName))
            {
                if (int.TryParse(count, out a))
                {
                    if (a > 0)
                    {
                        var rr = _tables.FirstOrDefault(j => j.Title.ToLower() == tableName.ToLower());
                        if (rr != null)
                        {

                        }
                        var dataTable = Search(_XZCDM, _TBBH, rr.Name, Dict[rr.Name]);
                        var form = new AttributeForm(dataTable, tableName);
                        form.ShowDialog(this);
                    }
                    else
                    {
                        MessageBox.Show("当前无记录！");
                    }
                }
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (IsAttribute == false)
            {
                return;
            }
            var TBBH = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            var XZCDM = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            if (!string.IsNullOrEmpty(TBBH) && !string.IsNullOrEmpty(XZCDM))
            {
                Search(XZCDM, TBBH);
                Center(TBBH, XZCDM);
            }

        }

        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        private SimpleMarkerSymbolClass simpleMarkerSymbol { get; set; }
        private SimpleFillSymbolClass simpleFillSymbol { get; set; }

        public void Center(string TBBH,string XZCDM)
        {
            var queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = string.Format("XZCDM = '{0}' AND TBBH = '{1}'", XZCDM, TBBH);
            IFeatureLayer featureLayer = null;
            for(var i = 0; i < this.axMapControl1.Map.LayerCount; i++)
            {
                var tlayer = this.axMapControl1.Map.get_Layer(i);
                if(tlayer is IFeatureLayer)
                {
                    if((tlayer as IFeatureLayer).Name == "调查单元图斑")
                    {
                        featureLayer = tlayer as IFeatureLayer;
                        break;
                    }
                }
            }
            if (featureLayer == null)
            {
                return;
            }

            IFeatureCursor featureCursor = featureLayer.FeatureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            if (feature != null)
            {
                Center(feature.Extent);
                this.axMapControl1.ActiveView.ScreenDisplay.UpdateWindow();
                Twinkle(feature.Shape);
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
        }
        private void Center(IEnvelope envelope)
        {
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

            //axMapControl1.CenterAt(point);
            //居中方法二

            envelope.Expand(2, 2, true);

            var env2 = axMapControl1.ActiveView.Extent;
            env2.CenterAt(point);
            axMapControl1.ActiveView.Extent = envelope;//env2  时  当前视图显示范围

            axMapControl1.ActiveView.Refresh();
        }

        private void Twinkle(IGeometry geo)
        {
            if (geo == null) return;

            switch (geo.GeometryType)
            {
                case esriGeometryType.esriGeometryMultipoint:
                case esriGeometryType.esriGeometryPoint:
                    axMapControl1.FlashShape(geo, 4, 300, simpleMarkerSymbol);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryLine:
                    axMapControl1.FlashShape(geo, 4, 300, simpleLineSymbol);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    axMapControl1.FlashShape(geo, 4, 300, simpleFillSymbol);
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExportExcel ex = new ExportExcel();
            ex.ExportToExcel(dataGridView1);
        }

        private void btnAddLayer_Click(object sender, EventArgs e)
        {
            var cmd = new ControlsAddDataCommandClass();
            cmd.OnCreate(this.axMapControl1.Object);
            cmd.OnClick();
        }

        private ILayer _curLayer { get; set; }

        private void PopupContextMenu(int x, int y)
        {
            var layer = _curLayer;
            mnuUp.Enabled = layer != null;
            mnuDown.Enabled = layer != null;
            mnuRemove.Enabled = layer != null;
            //mnuSymbol.Enabled = layer != null;
            contextMenuStrip1.Show(new System.Drawing.Point { X = x + 8, Y = y + 168 });
        }

        private void 符合设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curLayer == null) return;

            var PSheet = new ComPropertySheetClass();
            PSheet.HideHelpButton = true;

            ISet PSet = new SetClass();
            PSet.Add(_curLayer);
            PSheet.ClearCategoryIDs();
            PSheet.AddCategoryID(new UIDClass());

            PSheet.AddPage(new ESRI.ArcGIS.CartoUI.LayerDrawingPropertyPageClass());
            PSheet.Title = "显示属性设置";
            if (PSheet.CanEdit(PSet))
            {
                if (PSheet.EditProperties(PSet, 0))
                {
                    axTOCControl1.Refresh();
                }
            }
        }
        private void axTOCControl1_OnMouseUp_1(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {

        }

        private void axTOCControl1_OnMouseUp(object sender, ESRI.ArcGIS.Controls.ITOCControlEvents_OnMouseUpEvent e)
        {
            if (e.button == 2)
            {
                IBasicMap map = null;
                ILayer layer = null;
                object other = null;
                object index = null;
                esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;

                axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
                if (item == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    if (layer is IAnnotationSublayer)
                    {
                        _curLayer = null;
                    }
                    else
                    {
                        _curLayer = layer;
                    }
                }
                else
                {
                    _curLayer = null;
                }

                PopupContextMenu(e.x, e.y);

            }
        }

        private void mnuUP_Click(object sender, EventArgs e)
        {
            IMap pMap = this.axMapControl1.ActiveView.FocusMap;
            if (_curLayer != null)
            {

                ILayer pTempLayer;
                for (int i = 1; i < pMap.LayerCount; i++)
                {
                    pTempLayer = pMap.get_Layer(i);
                    if (pTempLayer == _curLayer)
                    {
                        pMap.MoveLayer(_curLayer, i - 1);
                        axMapControl1.ActiveView.Refresh();
                        axTOCControl1.Update();

                    }

                }
            }
        }

        private void mnuDown_Click(object sender, EventArgs e)
        {
            IMap pMap = this.axMapControl1.ActiveView.FocusMap;
            if (_curLayer != null)
            {

                ILayer pTempLayer;
                for (int i = 0; i < pMap.LayerCount - 1; i++)
                {
                    pTempLayer = pMap.get_Layer(i);
                    if (pTempLayer == _curLayer)
                    {
                        pMap.MoveLayer(_curLayer, i + 1);
                        axMapControl1.ActiveView.Refresh();
                        axTOCControl1.Update();
                        return;
                    }

                }
            }
        }

        private void mnuRemove_Click(object sender, EventArgs e)
        {
            if (_curLayer == null) return;
            axMapControl1.ActiveView.FocusMap.DeleteLayer(_curLayer);
            axMapControl1.ActiveView.Refresh();
            axTOCControl1.Update();
        }

    
    }
}
