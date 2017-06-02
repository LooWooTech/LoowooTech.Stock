using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LoowooTech.Stock.Rules
{
    public class ExcelBase
    {
        private const string DM = "乡镇代码";
        private const string MC = "乡镇名称";
        /// <summary>
        /// 表格名字
        /// </summary>
        public string ExcelName { get; set; }
        public int Space { get; set; }
        public string Name
        {
            get
            {
                return string.Format("统计表格：{0}检查", ExcelName);
            }
        }      
        /// <summary>
        /// 数据库表名
        /// </summary>
        private string _tableName { get; set; }
        public string TableName
        {
            get
            {
                return string.IsNullOrEmpty(_tableName) ? XmlNode!=null?XmlNode.Attributes["TableName"].Value:string.Empty : _tableName;
            }
        }
        private XmlNode _xmlNode { get; set; }
        /// <summary>
        /// 配置文件节点
        /// </summary>
        public XmlNode XmlNode
        {
            get
            {
                return _xmlNode == null ? _xmlNode = XmlManager.GetSingle(string.Format("/Tbales/Exlce[@Name='{0}']", ExcelName), XmlEnum.Field) : _xmlNode;
            }
        }
        private string _title { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    if (XmlNode != null)
                    {
                        _title = XmlNode.Attributes["Title"].Value;
                    }
                }
                return _title;
            }
        }
        private List<XZC> _list { get; set; }
        /// <summary>
        /// 行政区列表
        /// </summary>
        public List<XZC> List { get { return _list; }set { _list = value; } }
        private List<ExcelField> _fields { get; set; }
        /// <summary>
        /// 表格要求字段
        /// </summary>
        public List<ExcelField> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = GetFields();
                }
                return _fields;
            }
        }
        private List<Question> _questions { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public List<Question> Questions { get { return _questions; } }

        private ConcurrentBag<Question> _paralleQuestions { get; set; }
        public ConcurrentBag<Question> ParalleQuestions { get { return _paralleQuestions; } }
        private Dictionary<XZC,List<ExcelField>> _dict { get; set; }
        /// <summary>
        /// 正确值
        /// </summary>
        public Dictionary<XZC,List<ExcelField>> Dict { get { return _dict; } }
        private string _district { get; set; }
        /// <summary>
        /// 行政区  区县名称
        /// </summary>
        public string District { get { return _district; }set { _district = value; } }
        private string _code { get; set; }
        /// <summary>
        /// 行政区 区县代码
        /// </summary>
        public string Code { get { return _code; }set { _code = value; } }
        private string _folder { get; set; }
        public string Folder { get { return _folder; }set { _folder = value; } }
        private string _excelFilePath { get; set; }
        /// <summary>
        /// 表格路径
        /// </summary>
        public string ExcelFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_excelFilePath))
                {
                    if (!string.IsNullOrEmpty(Title))
                    {
                        _excelFilePath = System.IO.Path.Combine(Folder, Title.Replace("{Name}", District).Replace("{Code}", Code));
                    }
                }
                return _excelFilePath;
            }
        }
        private ISheet _sheet { get; set; }
        /// <summary>
        /// 数据起始行
        /// </summary>
        private int _startline { get; set; }
        public ExcelBase()
        {
            _questions = new List<Question>();
            _dict = new Dictionary<XZC, List<ExcelField>>();
        }
        private List<ExcelField> GetFields()
        {
          
            var list = new List<ExcelField>();
            if (XmlNode != null)
            {
                var nodes = XmlNode.SelectNodes("/Field");
                if (nodes != null && nodes.Count > 0)
                {
                    for(var i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        var val = new ExcelField
                        {
                            Name = node.Attributes["Name"].Value,
                            Title = node.Attributes["Title"].Value,
                            Index = int.Parse(node.Attributes["Index"].Value),
                            Type = node.Attributes["Type"].Value == "Int" ? ExcelType.Int : ExcelType.Double,
                            Compute = node.Attributes["Compute"].Value == "Sum" ? Compute.Sum : Compute.Count
                        };
                        try
                        {
                            val.Unit = node.Attributes["Unit"].Value.Trim();
                        }
                        catch
                        {

                        }
                        list.Add(val);
                    }
                }
            }

            return list;
        }

        protected void Add(Question question)
        {
            _questions.Add(question);
        }

        private void CheckExcel()
        {
            var info = string.Empty;
            if (!System.IO.File.Exists(ExcelFilePath))
            {
                info = string.Format("未找到统计表格：{0}，无法进行检查", ExcelFilePath);
                Console.WriteLine(info);
                _questions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = info });
            }
            else
            {
                var workbook = ExcelFilePath.OpenExcel();
                if (workbook != null)
                {
                    _sheet = workbook.GetSheetAt(0);
                    if (_sheet != null)
                    {
                        var flag = false;
                        _startline = -1;
                        for (var i = 0; i <= _sheet.LastRowNum; i++)
                        {
                            var row = _sheet.GetRow(i);
                            if (row != null)
                            {
                                var heads = ExcelClass.GetCellValues(row, 0, Fields.Count + 2);
                                if (heads[0] == DM && heads[1] == MC)
                                {
                                    flag = true;
                                    #region  验证每个表头名称
                                    for (var j = 2; j < heads.Length; j++)
                                    {
                                        info = heads[j];
                                        if (!string.IsNullOrEmpty(info))
                                        {
                                            var field = Fields.FirstOrDefault(e => e.Index == j);
                                            if (field == null)
                                            {
                                                flag = false;
                                                break;
                                            }
                                            if (field.Title.ToLower() != info.ToLower())
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            if (flag)
                            {
                                _startline = i;
                                break;
                            }
                        }
                        if (_startline == -1)
                        {
                            _questions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = "未获取表格的表头，请核对数据库标准" });
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        _questions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = "无法获取Excel中的工作表" });
                    }
                    
                   

                }
                else
                {
                    _questions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = "无法打开Excel文件" });
                }
            }
        }

        private void CheckAccess(OleDbConnection connection)
        {
            var a = 0;
            var b = .0;
            var val = string.Empty;
            foreach (var xzc in List)
            {
                var result = new List<ExcelField>();
                foreach (var field in Fields)
                {
                    var obj = ADOSQLHelper.ExecuteScalar(connection, string.Format("Select {0} from {1} where XZCDM = '{2}' AND XZCMC = '{3}'", field.SQL, TableName, xzc.XZCDM, xzc.XZCMC));
                    if (field.Type == ExcelType.Double)
                    {
                        double.TryParse(obj.ToString(), out b);
                        switch (field.Unit)
                        {
                            case "亩":
                                b = b / 15;
                                break;
                            case "平方米":
                                b = b / 10000;
                                break;
                            default:
                                break;
                        }
                        val = Math.Round(b, 4).ToString();
                    }
                    else
                    {
                        int.TryParse(obj.ToString(), out a);
                        val = a.ToString();
                    }
                    field.Value = val;
                    result.Add(field);
                }
                _dict.Add(xzc, result);
            }
        }

        public virtual void Check(OleDbConnection connection)
        {
            var info = string.Empty;
            if (Fields.Count == 0)
            {
                info= string.Format("配置文件FieldInfo.xml未读取{0}的节点信息，无法进行统计数据的核对！", ExcelName);
                Console.WriteLine(info);
                _questions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = info });
            }
            else
            {
                _dict.Clear();
                
                CheckExcel();//对excel 文件进行读取并打开
                CheckAccess(connection);//获取数据库统计数值信息
            }
        }
    }
}
