using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class ParameterManager
    {
        private static string _mdbFilePath { get; set; }
        /// <summary>
        /// 数据库文件
        /// </summary>
        public static string MDBFilePath { get { return _mdbFilePath; }set { _mdbFilePath = value; } }
        private static string _district { get; set; }
        /// <summary>
        /// 行政区名称
        /// </summary>
        public static string District { get { return _district; }set { _district = value; } }
        private static string _code { get; set; }
        /// <summary>
        /// 行政区代码
        /// </summary>
        public static string Code { get { return _code; }set { _code = value; } }
        private static OleDbConnection _connection { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static OleDbConnection Connection { get { return _connection; } }
        private static IWorkspace _workspace { get; set; }
        /// <summary>
        /// ArcGIS工作空间
        /// </summary>
        public static IWorkspace Workspace { get { return _workspace; } }

        private static List<string> _featureClassNames { get; set; }
        /// <summary>
        /// 空间矢量要素类列表
        /// </summary>
        public static List<string> FeatureClassNames { get { return _featureClassNames == null ? _featureClassNames = XmlManager.Get("/Tables/Table[@IsSpace='true']", "Name", XmlEnum.Field) : _featureClassNames; } }
        private static ISpatialReference _currentSpatialReference { get; set; }
        /// <summary>
        /// 要求的坐标系统
        /// </summary>
        public static ISpatialReference CurrentSpatialReference { get { return _currentSpatialReference == null ? _currentSpatialReference = SpatialReferenceManager.Get40SpatialReference() : _currentSpatialReference; } }
        private static List<string> _tableNames { get; set; }
        /// <summary>
        /// 数据库要求的表
        /// </summary>
        public static List<string> TableNames { get { return _tableNames == null ? _tableNames = XmlManager.Get("/Tables/Table", "Name", XmlEnum.Field) : _tableNames; } }
        private static List<string> _childrenFolder { get; set; }
        /// <summary>
        /// 质检子目录
        /// </summary>
        public static List<string> ChildrenFolder { get { return _childrenFolder == null ? _childrenFolder = XmlManager.Get("/Folders/Folder", "Name", XmlEnum.DataTree) : _childrenFolder; } }

        
    }
}
