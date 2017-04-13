using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public class ArcGISHeart
    {
        private string _mdbFilePath { get; set; }
        public string MDBFilePath { get { return _mdbFilePath; }set { _mdbFilePath = value; } }
        private List<string> _featureClassNames { get; set; }//mdb中的featureClass系统中要求的
        /// <summary>
        /// MDB文件中的要求的图层名称
        /// </summary>
        public List<string> FeatureClassNames { get { return _featureClassNames; }set { _featureClassNames = value; } }
        private IWorkspace _workspace { get; set; }
        private List<string> _messages { get; set; }
        private ISpatialReference _currentSpatialReference { get; set; }
        public ISpatialReference CurrentSpatialReference { get { return _currentSpatialReference == null ? _currentSpatialReference = SpatialReferenceManager.Get40SpatialReference() : _currentSpatialReference; } }

        public ArcGISHeart()
        {
            _messages = new List<string>();
        }

        public void Program()
        {
            _workspace = MDBFilePath.OpenAccessFileWorkSpace();
            if (_workspace == null)
            {
                _messages.Add("获取Access的workspace失败");
                return;
            }
            foreach(var className in FeatureClassNames)
            {
                var featureClass = _workspace.GetFeatureClass(className);
                if (featureClass == null)
                {
                    _messages.Add(string.Format("未获取图层：{0}，无法进行图层相关检查", className));
                    continue;
                }
                var spatialReference = SpatialReferenceManager.GetSpatialReference(featureClass);//检查图层坐标系
                if (spatialReference.Name.Trim() != CurrentSpatialReference.Name.Trim())
                {
                    _messages.Add(string.Format("图层：{0}不符合2201（平面坐标系是否采用‘1980 西安坐标系’、3度带、带带号，检查高程系统是否采用‘1985 国家高程基准’，检查投影方式是否采用高斯-克吕格投影）"));
                }

            }
        }
    }
}
