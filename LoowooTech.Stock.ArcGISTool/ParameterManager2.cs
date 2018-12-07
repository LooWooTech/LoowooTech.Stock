using LoowooTech.Stock.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LoowooTech.Stock.Common;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class ParameterManager2
    {
        private static string _folder { get; set; }
        /// <summary>
        /// 质检目录
        /// </summary>
        public static string Folder { get { return _folder; } set { _folder = value; } }

        private static XmlDocument _configXml { get; set; }

        static ParameterManager2()
        {
            _configXml = new XmlDocument();
            _configXml.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.xml"));
        }

        public static void Init(string folder)
        {
            _folder = folder;
            _folders = null;
            _Files = null;
            _connection = null;
            _workspace = null;
            _initFolder = null;
        }

        private static string _initFolder { get; set; }

        /// <summary>
        /// 初始化路径
        /// </summary>
        public static string InitFolder { get { return string.IsNullOrEmpty(_initFolder) ? _initFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TEMPS", DateTime.Now.Date.ToString("yyyy-MM-dd-HH-mm-ss")) : _initFolder; } }

        private static string _country { get; set; }

        /// <summary>
        /// 区县名称
        /// </summary>
        public static string Country { get { return _country; } }

        private static string _village { get; set; }
        /// <summary>
        /// 乡镇名称
        /// </summary>
        public static string Village { get { return _village; } }
        /// <summary>
        /// 分析行政区县 乡镇
        /// </summary>
        public static bool AnalyzeDistrict()
        {
            var info = new DirectoryInfo(Folder);
            var str = info.Name.Replace("村土地利用规划数据库成果", "");
            foreach(var entry in XZCList)
            {
                str = str.Replace(entry.XZCMC, "");
            }
            var array = str.Replace("县", ",").Split(',');
            if (array.Length == 2)
            {
                _country = array[0];
                _village = array[1];
                return true;
            }

            return false;

        }

        public static string XZQExcelFilePath { get { return System.IO.Path.Combine(Folder, "6.其他数据", "行政区划代码表.xls"); } }
        private static List<XZC> _XZCList { get; set; }
        public static List<XZC> XZCList { get { return _XZCList; } }
        private static Dictionary<string,string> _xzcDict { get; set; }

        public static Dictionary<string,string> XZCDict { get { return XZCList.ToDictionary(e => e.XZCDM, e => e.XZCMC); } }

        /// <summary>
        /// 行政村列表 字符串格式
        /// </summary>
        public static string XZCString { get
            {
                var sb = new StringBuilder();
                foreach(var xzc in XZCList)
                {
                    sb.Append(xzc.XZCMC);
                }
                return sb.ToString();
            } }

        /// <summary>
        /// 分析 行政村列表
        /// </summary>
        /// <returns></returns>
        public static bool AnalyzeXZQ()
        {
            if (System.IO.File.Exists(XZQExcelFilePath) == false)
            {
                return false;
            }
            IWorkbook workbook = XZQExcelFilePath.OpenExcel();
            if (workbook == null)
            {
                return false;
            }
            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                return false;
            }
            var start = false;
            var list = new List<XZC>();
            for(var i = 0; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null)
                {
                    var cell1 = row.GetCell(0);
                    var cell2 = row.GetCell(1);
                    if (cell1 != null && cell2 != null)
                    {
                        if (start == true)
                        {
                            var xzqmc = cell1.ToString();
                            var xzqdm = cell2.ToString();
                            if (string.IsNullOrEmpty(xzqmc) == false && string.IsNullOrEmpty(xzqdm) == false)
                            {
                                list.Add(new XZC
                                {
                                    XZCMC = xzqmc,
                                    XZCDM = xzqdm
                                });
                            }
                        }
                        else
                        {
                            if (cell1.ToString().ToLower() == "行政区名称".ToLower() && cell2.ToString().ToLower() == "行政区代码".ToLower())
                            {
                                start = true;
                            }
                        }
                       
                    }
                }
            }

            if (list.Count > 0)
            {
                _XZCList = list;
                return true;
            }

            return false;
        }



        private static List<string> _folders { get; set; }
        /// <summary>
        /// 村规划成果下面的文件夹列表
        /// </summary>
        public static List<string> Folders { get { return _folders == null ? _folders = GetFolders() : _folders; } }
        private static List<string> GetFolders()
        {
            var list = new List<string>();
            var nodes = _configXml.SelectNodes("/Config/Folders/Folder");
            if (nodes != null)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    list.AddRange(GetChildFolder(node, Folder));
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
                    list.AddRange(GetChildFolder(child, str));
                }
            }
            return list;
        }
        private static List<string> _Files { get; set; }
        /// <summary>
        /// 质检成果应该存在的相关文件
        /// </summary>
        public static List<string> Files { get { return _Files == null ? _Files = GetFiles() : _Files; } }
        private static List<string> GetFiles()
        {
            var list = new List<string>();
            var nodes = _configXml.SelectNodes("/Config/Folders/Folder");
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    list.AddRange(GetChildFiles(node, Folder));
                }
            }
            return list;
        }
        private static List<string> GetChildFiles(XmlNode node,string path)
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
                    list.AddRange(GetChildFiles(nodes[i], name));
                }
            }

            return list;
        }

        /// <summary>
        /// 矢量数据文件
        /// </summary>
        public static string MDBFilePath { get { return System.IO.Path.Combine(Folder, "3.规划图形数据", string.Format("{0}村土地利用规划空间数据库.mdb",XZCString)); } }


        private static string _TDLYXZ { get; set; }

        /// <summary>
        /// 土地利用现状数据库文件
        /// </summary>
        public static string TDLYXZ { get { return _TDLYXZ; } set { _TDLYXZ = value; } }

        private static string _XGH { get; set; }
        /// <summary>
        /// 乡规划数据库文件路径
        /// </summary>
        public static string XGH { get { return _XGH; } set { _XGH = value; } }

        private static string _connectString { get { return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MDBFilePath); } }

        private static OleDbConnection _connection { get; set; }
        public static OleDbConnection Connection { get { return _connection == null ? _connection = new OleDbConnection(_connectString) : _connection; } }

        private static List<VillageTable> _tables { get; set; }
        /// <summary>
        /// 数据库文件中应该存在表格 以及对应的字段信息
        /// </summary>
        public static List<VillageTable> Tables { get { return _tables == null ? _tables = GetTables() : _tables; } }

        private static List<VillageTable> GetTables()
        {
            var list = new List<VillageTable>();
            var nodes = _configXml.SelectNodes("/Config/Tables/Table");
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    list.Add(GetTable(node));
                }
            }

            return list;
        }
        private static VillageTable GetTable(XmlNode node)
        {
            var table = new VillageTable
            {
                Name = node.Attributes["Name"].Value,
                Title = node.Attributes["Title"].Value,
                IsSpace = node.Attributes["IsSpace"].Value == "true" ? true : false,
                Topo = node.Attributes["Topo"].Value == "true" ? true : false,
                Fields=new List<VillageField>()
            };

            var nodes = node.SelectNodes("Field");
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var child = nodes[i];
                    var field = new VillageField
                    {
                        Index = int.Parse(child.Attributes["Index"].Value),
                        Name = child.Attributes["Name"].Value,
                        Title = child.Attributes["Title"].Value,
                        Type = child.Attributes["Type"].Value
                    };
                    if (child.Attributes["Length"] != null && string.IsNullOrEmpty(child.Attributes["Length"].Value) == false)
                    {
                        field.Length = int.Parse(child.Attributes["Length"].Value);
                    }
                    table.Fields.Add(field);
                }
            }

            return table;
        }

        private static IWorkspace _workspace { get; set; }
        /// <summary>
        /// 矢量数据库  workspace  能不用 就不用 千万不要调用
        /// </summary>
        public static IWorkspace WorkSpace { get { return _workspace == null ? _workspace = MDBFilePath.OpenAccessFileWorkSpace() : _workspace; } }

        private static ISpatialReference _spatialReference { get; set; }
        /// <summary>
        /// 2000坐标系
        /// </summary>
        public static ISpatialReference SpatialReference { get { return _spatialReference == null ? _spatialReference = SpatialReferenceManager.Get2000SpatialReference() : _spatialReference; } }

        private static Dictionary<string, string> _GHYTDL { get; set; } = new Dictionary<string, string>
        {
            { "131","生态林"},{ "31","水域"},{"32","自然保留地" },
            { "111","水田"},{"113","旱地"},
            { "12","园地"},{"132","商品林"},{"15","其他农用地"},
            { "151","设施农用地"},{"152","农村道路"},{"153","坑塘水面"},{"154","农田水利用地"},{"155","田坎"},
            { "2121","宅基地"},{"2122","公共服务设施用地"},{"2123","基础设施用地"},{"2125","景观与绿化用地"},
            { "2126","村内交通用地"},{"2124","经营性建设用地"},
            { "221","对外交通用地"},{"226","水利设施用地"},{"213","采矿用地"},{"231","风景名胜用地"},{"232","特殊用地"},
            { "211","城镇建设用地"}
        };

        /// <summary>
        /// 村土地利用规划地类代码名称对照表
        /// </summary>
        public static Dictionary<string,string> GHYTDL { get { return _GHYTDL; } }

        

        private static Dictionary<string, string> _JSZHLX { get; set; } = new Dictionary<string, string>
        {
            {"A","可调整地类"},
            { "B","已验收开发复垦整理地块但现状未变更"},
            { "C","已批农转用（含违法补办）或已办土地使用权证，但地类与变更调查不一致"},
            { "E","风景名胜及特殊用地"},
            { "F","现状城镇建成区范围内建设用地"},
            { "G","现状为村庄，实地调查为污染性企业、垃圾填埋场、危险品仓库"},
            {"H","比例尺精度转换、1：1万变更零物、线物上图"},
            {"I","与实地不相符"},
            {"J","农村居民点用地内部细分"}
        };

        /// <summary>
        /// 分类处理编号字段属性取值代码表
        /// </summary>
        public static Dictionary<string,string> JSZHLX { get { return _JSZHLX; } }

        private static Dictionary<string, string> _DLBZ { get; set; } = new Dictionary<string, string>
        {
            {"10","批而未用" },
            {"20","违法用地" },
            {"30","可调整耕地" },
            {"99","其他" }
        };

        /// <summary>
        /// 地类备注取值代码表
        /// </summary>
        public static Dictionary<string,string> DLBZ { get { return _DLBZ; } }

        private static Dictionary<string, string> _QSXZ { get; set; } = new Dictionary<string, string>
        {
            {"10","国有土地所有权" },
            { "20","国有土地使用权"},
            { "30","集体土地所有权"},
            { "31","村民小组"},
            { "32","村集体经济组织"},
            { "33","乡集体经济组织"},
            { "34","其它农民集体经济组织"},
            { "40","集体土地使用权"}
        };
        /// <summary>
        /// 权属性质
        /// </summary>
        public static Dictionary<string,string> QSXZ { get { return _QSXZ; } }

        private static Dictionary<string, string> _PDJB { get; set; } = new Dictionary<string, string>
        {
            { "1","<=2"},
            { "2","(2~6)"},
            { "3","(6~15)"},
            { "4","(15~25)"},
            { "5",">=25"}
        };
        
        /// <summary>
        /// 坡度级别
        /// </summary>
        public static Dictionary<string,string> PDJB { get { return _PDJB; } }

        private static List<GHYT> _GHYTs { get; set; }
        /// <summary>
        /// 规划用途代码  编码
        /// </summary>
        public static List<GHYT> GHYTs { get { return _GHYTs == null ? _GHYTs = GetGHYTs() : _GHYTs; } }
        private static List<GHYT> _secondGHYTs { get; set; }
        public static List<GHYT> SeconGHYTs { get { return _secondGHYTs == null ? _secondGHYTs = GetSecondGHYT() : _secondGHYTs; } }
        private static List<GHYT> GetGHYTs()
        {
            var list = new List<GHYT>();
            var nodes = _configXml.SelectNodes("/Config/GHYTs/GHYT");
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    list.Add(GetGHYT(node));
                }
            }

            return list;
        }

        private static GHYT GetGHYT(XmlNode node)
        {
            var ghyt = new GHYT
            {
                Name = node.Attributes["Name"].Value,
                Code = node.Attributes["Code"].Value,
                GHYTs = new List<GHYT>()
            };
            var nodes = node.SelectNodes("GHYTDM");
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var child = nodes[i];
                    var entry = new GHYT
                    {
                        Name = child.Attributes["Name"].Value,
                        Code = child.Attributes["Code"].Value
                    };
                    if (child.Attributes["BM"] != null)
                    {
                        entry.BM = child.Attributes["BM"].Value;
                    }
                    ghyt.GHYTs.Add(entry);
                }
            }


            return ghyt;
        }

        public static List<GHYT> GetSecondGHYT()
        {
            var list = new List<GHYT>();
            foreach(var item in GHYTs)
            {
                if(item.GHYTs!=null&& item.GHYTs.Count > 0)
                {
                    list.AddRange(item.GHYTs);
                }
            }
            return list;
        }

        private static Dictionary<string, string> _JSYDGZQ { get; set; } = new Dictionary<string, string>
        {
            {"010","允许建设区" },{"021","有条件建设区"},
            {"030","限制建设用地区" },{"040","禁止建设用地区"},
            {"041","自然保护区核心区" },{"042","森林公园"},
            {"043","地质公园" },{"044","列入省级以上保护名录野生动植物自然栖息地"},
            {"045","水源保护核心区" },{"046","主要河湖蓄滞洪区"},
            {"047","地质灾害高危险地区" },{"048","永久基本农田示范区"},
            {"049","其他保护区" }
        };
        /// <summary>
        /// 建设用地管制区类型代码表
        /// </summary>
        public static Dictionary<string,string> JSYDGZQ { get { return _JSYDGZQ; } }
        private static double? _absolute { get; set; }
        /// <summary>
        /// 绝对值 具体数值
        /// </summary>
        public static double? Absolute { get { return _absolute.HasValue ? _absolute.Value : _absolute = GetValue("/Config/Calculator/Area", "Absolute"); } }
        private static  double? _relative { get; set; }
        /// <summary>
        /// 相对值  百分比
        /// </summary>
        public static double? Relative { get { return _relative.HasValue ? _relative.Value : _relative = GetValue("/Config/Calculator/Area", "Relative"); } }

        private static string _tolerance { get; set; }
        public static string Tolerance { get { return string.IsNullOrEmpty(_tolerance) ? _tolerance = GetString("/Config/Calculator/Area", "Tolerance") : _tolerance; } }
        private static double? GetValue(string path,string attributeName)
        {
            var str = GetString(path, attributeName);
            if (string.IsNullOrEmpty(str) == false)
            {
                var a = .0;
                if ( double.TryParse(str, out a))
                {
                    return a;
                }
            }
            return null;
        }
        private static string GetString(string path,string attributeName)
        {
            var node = _configXml.SelectSingleNode(path);
            if (node != null)
            {
                if (node.Attributes[attributeName] != null)
                {
                    return node.Attributes[attributeName].Value;
                }
            }
            return string.Empty;
        }

    }
}
