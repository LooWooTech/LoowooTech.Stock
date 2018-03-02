using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Xml;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class ParameterManager
    {
        static string Collect = "3统计表格";
        private static string _folder { get; set; }
        /// <summary>
        /// 质检路径
        /// </summary>
        public static string Folder { get { return _folder; }set { _folder = value; } }
        private static string _mdbFilePath { get; set; }
        /// <summary>
        /// 数据库文件
        /// </summary>
        public static string MDBFilePath { get { return _mdbFilePath; }set { _mdbFilePath = value; } }
        private static string _codeFilePath { get; set; }
        /// <summary>
        /// 单位代码表
        /// </summary>
        public static string CodeFilePath { get { return _codeFilePath; }set { _codeFilePath = value; } }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectionString { get { return  string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", _mdbFilePath); } }
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
        public static OleDbConnection Connection { get { return _connection == null ? _connection = new OleDbConnection(ConnectionString) : _connection; } }
        private static IWorkspace _workspace { get; set; }
        /// <summary>
        /// ArcGIS工作空间
        /// </summary>
        public static IWorkspace Workspace { get { return _workspace == null ? _workspace = MDBFilePath.OpenAccessFileWorkSpace() : _workspace; } }

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
        public static List<string> ChildrenFolder { get { return _childrenFolder == null ? _childrenFolder = GetChildFolder() : _childrenFolder; } }

        private static List<string> _childrenFiles { get; set; }
        
        public static List<string> ChildrenFiles { get { return _childrenFiles == null ? _childrenFiles = GetChildrenFiles() : _childrenFiles; } }

        private static List<CollectXZQ> _collectXZQ { get; set; }

        public static List<CollectXZQ> CollectXZQ { get { return _collectXZQ == null ? _collectXZQ = GetCollectXZQ() : _collectXZQ; } }

        private static List<CollectXZQ> GetCollectXZQ()
        {
            var list = new List<CollectXZQ>();
            var nodes = XmlManager.GetList("/Tables/Citys/Citys", XmlEnum.Field);
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    var model = new CollectXZQ
                    {
                        XZQDM = node.Attributes["Code"].Value,
                        XZQMC = node.Attributes["Name"].Value
                    };
                    var children = node.SelectNodes("City");
                    if (children != null && children.Count > 0)
                    {
                        var result = new List<XZC>();
                        for(var j = 0; j < children.Count; j++)
                        {
                            var child = children[j];
                            var entry = new XZC
                            {
                                XZCDM = child.Attributes["Code"].Value,
                                XZCMC = child.Attributes["Name"].Value
                            };
                            result.Add(entry);
                        }
                        model.Children = result.OrderBy(e => e.XZCDM).ToList();
                    }
                    list.Add(model);
                }
            }
            return list.OrderBy(e=>e.XZQDM).ToList();
        }

        private static List<string> GetChildrenFiles()
        {
            var list = new List<string>();
            var nodes = XmlManager.GetList("/Folders/Folder", XmlEnum.DataTree);
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    list.AddRange(GetChildrenFiles(node, Folder));
                }
            }
            return list;
        }
        
        private static List<string> GetChildrenFiles(XmlNode node,string path)
        {
            var list = new List<string>();
            var name = System.IO.Path.Combine(path, node.Attributes["Name"].Value);
            var nodes = node.SelectNodes("File");
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var no = nodes[i];
                    var a = no.Attributes["Name"].Value;
                    list.Add(System.IO.Path.Combine(name, a));
                }
            }
            nodes = node.SelectNodes("Folder");
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    list.AddRange(GetChildrenFiles(nodes[i], name));
                }
            }
            return list;
        }

        private static List<string> GetChildFolder()
        {
            var list = new List<string>();
            var nodes = XmlManager.GetList("/Folders/Folder", XmlEnum.DataTree);
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    list.AddRange(GetChildFolder(node,Folder));
                }
            }
            return list;
        }
        private static List<string> GetChildFolder(XmlNode node,string path)
        {
            var list = new List<string>();
            var str = System.IO.Path.Combine(path, node.Attributes["Name"].Value);
            list.Add(str);
            var nodes = node.SelectNodes("/Folder");
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var child = nodes[i];
                    list.AddRange(GetChildFolder(child,str));
                }
            }
            return list;
            
        }

        private static List<string> _TopoFeatures { get; set; }
        /// <summary>
        /// 需要进行拓扑检查的图层
        /// </summary>
        public static List<string> TopoFeatures { get { return _TopoFeatures == null ? _TopoFeatures = XmlManager.Get("/Tables/Table[@Topo='true']", "Name", XmlEnum.Field) : _TopoFeatures; } }
        
        private static string _collectFolder { get; set; }
        /// <summary>
        /// 统计表格路径
        /// </summary>
        public static string CollectFolder { get { return string.IsNullOrEmpty(_collectFolder) ? _collectFolder = System.IO.Path.Combine(Folder, Collect) : _collectFolder; } }
        public static void Init(string folder)
        {
            _folder = folder;
            //Folder = _folder;
            _connection = null;
            _workspace = null;
            _tableNames = null;
            _childrenFolder = null;
            _TopoFeatures = null;
            _collectFolder = string.Empty;
            _childrenFiles = null;
            _featureClassNames = null;
            _code = null;
            _district = null;
            _codeFilePath = null;
            _mdbFilePath = null;
        }
    }
}
