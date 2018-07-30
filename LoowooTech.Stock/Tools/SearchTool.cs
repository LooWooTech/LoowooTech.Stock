using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Stock.Tools
{
    [Guid("2D2F68E5-8D2C-47F6-92D5-02AE7CD657CA")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Info.ExpTool")]
    public sealed class SearchTool:BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            var regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            var regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IHookHelper mHookHelper;
        private string LayerName { get; set; }
        private string WhereClause { get; set; }
        private bool BusyFlag { get; set; }
        private AdvanceForm _father { get; set; }
        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        private SimpleMarkerSymbolClass simpleMarkerSymbol { get; set; }
        private SimpleFillSymbolClass simpleFillSymbol { get; set; }
        private IFeatureLayer _featureLayer { get; set; }
        public IFeatureLayer FeatureLayer
        {
            get
            {
                return _featureLayer == null ? _featureLayer = GetFeatureLayer() : _featureLayer;   
            }
        }

        private IFeatureLayer GetFeatureLayer()
        {
            for (var i = 0; i < mHookHelper.FocusMap.LayerCount; i++)
            {
                var tLayer = mHookHelper.FocusMap.get_Layer(i);
                if (tLayer is GroupLayer)
                {
                    var layer = tLayer as ICompositeLayer;
                    if (layer == null) continue;
                    for (var j = 0; j < layer.Count; j++)
                    {
                        var featureLayer = layer.get_Layer(j) as IFeatureLayer;
                        if (FeatureLayer.Name == LayerName)
                        {
                            return FeatureLayer;
                        }
                    }
                }
                else if (tLayer is IFeatureLayer)
                {
                    if ((tLayer as IFeatureLayer).Name == LayerName)
                    {
                        return tLayer as IFeatureLayer;
                    }
                }
            }

            return null;
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

        public SearchTool(string layerName,AdvanceForm father,bool Busy=false)
        {
            LayerName = layerName;
            _father = father;
            BusyFlag = Busy;
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
            simpleLineSymbol.Color= DisplayExtensions.GetRGBColor(255, 0, 0);
        }
        public override void OnCreate(object hook)
        {
            try
            {
                mHookHelper = new HookHelperClass { Hook = hook };
                if (mHookHelper.ActiveView == null)
                {
                    mHookHelper = null;
                }
            }
            catch
            {
                mHookHelper = null;
            }
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            var pt = mHookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            ShowAttribute(pt);
        }

        private void ShowAttribute(IGeometry geometry)
        {
            if (FeatureLayer != null)
            {
                IArray array = AttributeExtensions.Identity(FeatureLayer.FeatureClass, geometry, WhereClause);
                if (array != null)
                {
                    IFeatureIdentifyObj featureIdentifyObj = array.get_Element(0) as IFeatureIdentifyObj;
                    IIdentifyObj identifyObj = featureIdentifyObj as IIdentifyObj;
                    IRowIdentifyObject rowIdentifyObj = featureIdentifyObj as IRowIdentifyObject;
                    IFeature feature = rowIdentifyObj.Row as IFeature;
                    if (feature != null)
                    {
                        
                        _father.dataGridView1.DataSource = GetAttribute(feature);
                        _father.ExportExcelbutton.Enabled = true;
                        var tbbh = _featureValues.FirstOrDefault(e => e.Name == "TBBH");
                        var xzcdm = _featureValues.FirstOrDefault(e => e.Name == "XZCDM");
                        _father.Search(xzcdm.Value,tbbh.Value);

                        Twinkle(feature);
                    }
                    else
                    {
                        _father.ExportExcelbutton.Enabled = false;
                    }
                }
            }
        }

        private void Twinkle(IFeature feature)
        {
            if (feature == null) return;
            Twinkle(feature.Shape);
        }

        private void Twinkle(IGeometry geo)
        {
            if (geo == null) return;

            switch (geo.GeometryType)
            {
                case esriGeometryType.esriGeometryMultipoint:
                case esriGeometryType.esriGeometryPoint:
                    _father.axMapControl1.FlashShape(geo, 4, 300, simpleMarkerSymbol);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryLine:
                    _father.axMapControl1.FlashShape(geo, 4, 300, simpleLineSymbol);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    _father.axMapControl1.FlashShape(geo, 4, 300, simpleFillSymbol);
                    break;
            }
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {

        }

        public override bool Deactivate()
        {
            return true;
        }

        private DataTable GetAttribute(IFeature feature)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("字段");
            dataTable.Columns.Add("值");
            DataRow dataRow = null;
            foreach(var item in _featureValues)
            {
                var index = feature.Fields.FindField(item.Name);
                if (index > -1)
                {
                    var val = feature.get_Value(index);
                    if (val != null)
                    {
                        var value = val.ToString();
                        item.Value = value;
                        dataRow = dataTable.NewRow();
                        dataRow["字段"] = item.Title;
                        if (item.Name == "DCDYLX")
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

            return dataTable;
        }
    }
}
