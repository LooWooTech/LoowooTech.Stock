using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using NPOI.SS.UserModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
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
        private const string SM = "合计";
        /// <summary>
        /// 表格名字
        /// </summary>
        public string ExcelName { get; set; }
        public string TitleName { get; set; }
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
        
        /// <summary>
        /// 汇总增加列号
        /// </summary>
        public int? CollectIndex
        {
            get
            {
                if (XmlNode == null)
                {
                    return null;
                }
                return int.Parse(XmlNode.Attributes["CollectIndex"].Value);
            }
        }

        private OleDbConnection _connection { get; set; }
        public OleDbConnection Connection { get { return _connection; }set { _connection = value; } }
        private XmlNode _xmlNode { get; set; }
        /// <summary>
        /// 配置文件节点
        /// </summary>
        public XmlNode XmlNode
        {
            get
            {
                return _xmlNode == null ? _xmlNode = XmlManager.GetSingle(string.Format("/Tables/Excel[@Name='{0}']", ExcelName), XmlEnum.Field) : _xmlNode;
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
        /// 表格要求字段  配置文件读取的
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

        //private List<ExcelField> _excelFields { get; set; }
        ///// <summary>
        ///// Excel表格读取
        ///// </summary>
        //public List<ExcelField> ExcelFields { get { return _excelFields; } set { _excelFields = value; } }
        private List<Question> _questions { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public List<Question> Questions { get { return _questions; } }

        private ConcurrentBag<Question> _paralleQuestions { get; set; }
        public ConcurrentBag<Question> ParalleQuestions { get { return _paralleQuestions; } }
        private Dictionary<string,List<FieldValue>> _dict { get; set; }
        /// <summary>
        /// 数据库 ——获取的统计值【正确值】
        /// </summary>
        public Dictionary<string,List<FieldValue>> Dict { get { return _dict; } }
        private List<FieldValue> _accessList { get; set; }
        private Dictionary<string,List<FieldValue>> _excelDict { get; set; }
        /// <summary>
        /// excel-核对数值
        /// </summary>
        public Dictionary<string,List<FieldValue>> ExcelDict { get { return _excelDict; } }
        /// <summary>
        /// 获取到的Excel所有列表值
        /// </summary>
        private List<FieldValue> _excelList { get; set; }
        
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
                    if (XmlNode.Attributes["Regex"] != null)
                    {
                        _excelFilePath = new FileTool { Folder = Folder, Filter = "*.xls", RegexString = XmlNode.Attributes["Regex"].Value }.GetFile();
                    }
                  
                }
                System.Console.WriteLine(_excelFilePath);
                return _excelFilePath;
            }
        }
        private ISheet _sheet { get; set; }
        /// <summary>
        /// 数据起始行 表格表头所在的行号  在校验的时候，自动读取
        /// </summary>
        private int _startline { get; set; }
        private string _modelExcel { get; set; }
        /// <summary>
        /// 用于生成统计表格的模型文件路径
        /// </summary>
        public string ModelExcel
        {
            get
            {
                return string.IsNullOrEmpty(_modelExcel) || !System.IO.File.Exists(_modelExcel) ? _modelExcel = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excels", XmlNode.Attributes["Model"].Value) : _modelExcel;
            }
        }
        private int _mStartLine { get; set; }
        /// <summary>
        /// 模型Excel文件
        /// </summary>
        public int MStartLine { get { return _mStartLine; } set { _mStartLine = value; } }
        private string _saveFolder { get; set; }
        /// <summary>
        /// 生成表格的保存文件夹
        /// </summary>
        public string SaveFolder { get { return _saveFolder; }set { _saveFolder = value; } }
        private string _checkCode { get; set; }
        public string CheckCode { get { return _checkCode; }set { _checkCode = value; } }
        protected List<ITool2> Tools { get; set; }
        public ExcelBase()
        {
            _paralleQuestions = new ConcurrentBag<Question>();
            _dict = new Dictionary<string, List<FieldValue>>();
            _excelDict = new Dictionary<string, List<FieldValue>>();
            _excelList = new List<FieldValue>();
            _accessList = new List<FieldValue>();
            Tools = new List<ITool2>();
        }
        private List<ExcelField> GetFields()
        {
          
            var list = new List<ExcelField>();
            if (XmlNode != null)
            {
                var tableName = TableName;
                var nodes = XmlNode.SelectNodes("Field");
                if (nodes != null && nodes.Count > 0)
                {
                    for(var i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        var val = new ExcelField
                        {
                            TableName = tableName,
                            Name =  node.Attributes["Name"].Value,
                            Title = node.Attributes["Title"].Value,
                            Index = int.Parse(node.Attributes["Index"].Value),
                            Type = node.Attributes["Type"].Value.ToLower() == "int" ? ExcelType.Int : ExcelType.Double,
                            Compute = node.Attributes["Compute"].Value.ToLower() == "sum" ? Compute.Sum : Compute.Count,
                        };
                        if (node.Attributes["Unit"] != null)
                        {
                            val.Unit = node.Attributes["Unit"].Value.Trim();
                        }

                        if (node.Attributes["View"] != null)
                        {
                            val.View = node.Attributes["View"].Value;
                        }
                        if (node.Attributes["WhereClause"] != null)
                        {
                            val.WhereClause = node.Attributes["WhereClause"].Value;
                        }
                        if (node.Attributes["TableName"] != null)
                        {
                            val.FieldTableName = node.Attributes["TableName"].Value;
                        }
                        if (node.Attributes["Indexs"] != null)
                        {
                            var indexs = node.Attributes["Indexs"].Value;
                            if (!string.IsNullOrEmpty(indexs))
                            {
                                var temps = indexs.Split(',');
                                var res = new int[temps.Length];
                                for(var j = 0; j < temps.Length; j++)
                                {
                                    var a = 0;
                                    res[j] = int.TryParse(temps[j], out a) ? a : 0;
                                }
                                val.Indexs = res;
                            }
                        }
                        list.Add(val);
                    }
                }
            }

            return list;
        }

        private void GainExcel()
        {
            if (ExcelParameterManager.ExcelListDict.ContainsKey(ExcelName) && ExcelParameterManager.ExcelDict.ContainsKey(ExcelName))
            {
                _excelList = ExcelParameterManager.ExcelListDict[ExcelName];
                _excelDict = ExcelParameterManager.ExcelDict[ExcelName];
                return;
            }
            var info = string.Empty;
            if (!System.IO.File.Exists(ExcelFilePath))
            {
                info = string.Format("未找到统计表格：{0}，无法进行检查", ExcelFilePath);
                Console.WriteLine(info);
                _paralleQuestions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = info });
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
                                    //for (var j = 2; j < heads.Length; j++)
                                    //{
                                    //    info = heads[j];
                                    //    if (!string.IsNullOrEmpty(info))
                                    //    {
                                    //        var field = Fields.FirstOrDefault(e => e.Index == j);

                                    //        if (field == null)
                                    //        {
                                    //            flag = false;
                                    //            break;
                                    //        }
                                    //        if (field.Title.ToLower() != info.ToLower())
                                    //        {
                                    //            flag = false;
                                    //            break;
                                    //        }
                                    //    }
                                    //}
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
                            _paralleQuestions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = "未获取表格的表头，请核对数据库标准" });
                        }
                        else
                        {
                            Analyze(_sheet, _startline + Space);//读取excel文件的数据值
                        }
                    }
                    else
                    {
                        _paralleQuestions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = "无法获取Excel中的工作表" });
                    }
                    
                   

                }
                else
                {

                    _paralleQuestions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = "无法打开Excel文件" });
                }
            }
        }
        private XZC GetXZC(IRow row)
        {
            var array = row.GetCellValues(0, 2);
            return new XZC { XZCDM = array[0], XZCMC = array[1] };
            
        }

        private List<FieldValue> GetValues(IRow row)
        {
            var list = new List<FieldValue>();
            foreach(var field in Fields)
            {
                var cell = row.GetCell(field.Index);
                var temp = new FieldValue
                {
                    Index = field.Index,
                    Type = field.Type,
                    Title = field.Title
                };
                switch (cell.CellType)
                {
                    case CellType.Formula:
                        temp.Val= cell.NumericCellValue;
                        break;
                    case CellType.Numeric:
                        temp.Val= cell.NumericCellValue;
                        break;
                    default:
                        temp.Val = cell.ToString().Trim();
                        break;
                }
                list.Add(temp);
            }
            return list;
        }

        private void Analyze(ISheet sheet,int startLine)
        {
            IRow row = null;
            var info = string.Empty;
            for(var i = startLine; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null)
                {
                    break;
                }
                #region  核对乡镇代码与乡镇名称的正确性
                var xzc = GetXZC(row);
                if (string.IsNullOrEmpty(xzc.XZCDM) && string.IsNullOrEmpty(xzc.XZCMC))
                {
                    continue;
                }
                if (xzc.XZCDM == SM || xzc.XZCMC == SM)//合计
                {
                    //var totals = GetValues(row);
                    CheckTotal(row);//核对合计数值

                    break;
                }
                var entry = List.FirstOrDefault(e => e.XZCDM.ToLower() == xzc.XZCDM.ToLower());
                if (entry == null)
                {
                    info = string.Format("第{0}行：未找到行政区代码为{1}的乡镇信息，请核对",i+1, xzc.XZCDM);
                    _paralleQuestions.Add(new Question
                    {
                        Code = "6101",
                        Name = Name,
                        TableName = ExcelName,
                        Project=CheckProject.汇总表与数据库图层逻辑一致性,
                        BSM = (i + 1).ToString(),
                        Description = info
                    });
                    continue;
                }
                if (entry.XZCMC.ToLower() != xzc.XZCMC.ToLower())
                {
                    info = string.Format("第{0}行：当前乡镇代码【{1}】对应的乡镇名称【{2}】与数据库乡镇名称【{3}】不符，请核对", i + 1, xzc.XZCDM, xzc.XZCMC, entry.XZCMC);
                    _paralleQuestions.Add(new Question
                    {
                        Code = "6101",
                        Name = Name,
                        Project = CheckProject.汇总表与数据库图层逻辑一致性,
                        BSM = (i + 1).ToString(),
                        Description = info
                    });
                    continue;
                }
                #endregion


                #region
                //读取其他字段的数据值 并且核对数值的正确性
                var values = GetValues(row);
                foreach(var item in values)
                {
                    if (string.IsNullOrEmpty(item.Val.ToString()))
                    {
                        continue;
                    }
                    info = string.Empty;
                    
                    switch (item.Type)
                    {
                        case ExcelType.Double:
                            var a = .0;
                            if(!double.TryParse(item.Val.ToString(),out a))
                            {
                                info = string.Format("乡镇代码【{0}】乡镇名称【{1}】中字段【{2}】的值与要求的格式不一致", xzc.XZCDM, xzc.XZCMC, item.Title);
                            }
                            break;
                        case ExcelType.Int:
                            var b = 0;
                            if(!int.TryParse(item.Val.ToString(),out b))
                            {
                                info = string.Format("乡镇代码【{0}】乡镇名称【{1}】中字段【{2}】的值与要求的格式不一致", xzc.XZCDM, xzc.XZCMC, item.Title);
                            }
                            break;
                    }
                    if (!string.IsNullOrEmpty(info))
                    {
                        _paralleQuestions.Add(new Question
                        {
                            Code = "6101",
                            Name = Name,
                            TableName = ExcelName,
                            BSM = (i + 1).ToString(),
                            Project = CheckProject.汇总表与数据库图层逻辑一致性,
                            Description = info
                        });
                    }
                }
                #endregion

                if (_excelDict.ContainsKey(xzc.XZCDM))
                {
                    info = string.Format("乡镇代码【{0}】乡镇名称【{1}】存在重复数据，请核对", xzc.XZCDM, xzc.XZCMC);
                    _paralleQuestions.Add(new Question
                    {
                        Code = "6101",
                        Name = Name,
                        TableName = ExcelName,
                        BSM = (i + 1).ToString(),
                        Project = CheckProject.汇总表与数据库图层逻辑一致性,
                        Description = info
                    });
                }
                else
                {
                    _excelList.AddRange(values);
                    _excelDict.Add(xzc.XZCDM, values);
                }
            }

            ExcelParameterManager.AddExcel(ExcelName, _excelList, _excelDict);
        }
        /// <summary>
        /// 作用：核对表格中的合计是否正确
        /// </summary>
        /// <param name="list"></param>
        private void CheckTotal(IRow row)
        {
            var info = string.Empty;
            var list = GetValues(row);
            var dou = .0;
            var inter = 0;
            foreach(var field in list)
            {
                info = string.Empty;
                var children = _excelList.Where(e => e.Index == field.Index&& e.Val != null && !string.IsNullOrEmpty(e.Val.ToString())).ToList();
                switch (field.Type)
                {
                    case ExcelType.Double:
                        var a = children.Sum(e => double.TryParse(e.Val.ToString(),out dou)?dou:.0);
                        var b = .0;
                        if (field.Val != null&&!string.IsNullOrEmpty(field.Val.ToString()))
                        {
                            double.TryParse(field.Val.ToString(), out b);
                        }
                        if (Math.Abs(b - a) > 0.0001)
                        {
                            info = string.Format("表格合计中{0}的值【{1}】与有效值【{2}】合计容差率超过0.001,请核对!",field.Title,b,a);
                           
                        }
                        break;
                    case ExcelType.Int:
                        var c = children.Sum(e =>int.TryParse(e.Val.ToString(),out inter)?inter:0);
                        var d = 0;
                        if (field.Val != null && !string.IsNullOrEmpty(field.Val.ToString()))
                        {
                            int.TryParse(field.Val.ToString(), out d);
                        }
                        if (c != d)
                        {
                            info = string.Format("表格合计中{0}的值【{1}】与有效值【{2}】合计不相等，请核对！", field.Title,d,c);
                        }
                        break;
                }
                if (!string.IsNullOrEmpty(info))
                {
                    Console.WriteLine(info);
                    _paralleQuestions.Add(new Question
                    {
                        Code = "6102",
                        Name = Name,
                        TableName = ExcelName,
                        Project = CheckProject.表格汇总面积和数据库汇总面积一致性,
                        Description = info
                    });
                }
            }
        }

        private FieldValue GainCommon(ExcelField field,string dmWhere)
        {
            
            var a = 0;
            var b = .0;
            var val = string.Empty;
            var sb = new StringBuilder(string.Format("Select {0} from ", field.SQL));
            if (string.IsNullOrEmpty(field.View))
            {
                if (string.IsNullOrEmpty(field.FieldTableName))
                {
                    sb.Append(TableName);
                }
                else
                {
                    sb.Append(field.FieldTableName);
                }
               
            }
            else
            {
                sb.AppendFormat("({0})", field.View);
            }
            sb.AppendFormat(" Where ( {0} )", dmWhere);
            if (!string.IsNullOrEmpty(field.WhereClause))
            {
                sb.AppendFormat(" AND {0}", field.WhereClause);
            }
            Console.WriteLine("{0}:{1}", TableName, sb.ToString());
            var obj = ADOSQLHelper.ExecuteScalar(Connection, sb.ToString());
            if (obj == null)
            {
                return null; 
            }
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
            var result = new FieldValue
            {
                Index = field.Index,
                Type = field.Type,
                Title = field.Title,
                Val=val
            };
            return result;
        }

        private FieldValue GetValues(ExcelField field, List<XZC> value)
        {
            var a = 0;
            var b = .0;
            var IntSum = 0;
            var DoubleSum = .0;
            foreach(var index in field.Indexs)
            {
                var f = Fields.FirstOrDefault(e => e.Index == index);
                if (f != null)
                {
                    var val = GetValue(f, value);
                    if (val != null)
                    {
                        switch (field.Type)
                        {
                            case ExcelType.Double:
                                DoubleSum += Math.Round(double.TryParse(val.Val.ToString(), out b) ? b : .0,4);
                                break;
                            case ExcelType.Int:
                                IntSum += int.TryParse(val.Val.ToString(), out a) ? a : 0;
                                break;
                        }
                    }
                }
            }

            return new FieldValue
            {
                Index = field.Index,
                Type = field.Type,
                Title = field.Title,
                Val = field.Type == ExcelType.Double ? DoubleSum : IntSum
            };
        }
        private FieldValue GetValue(ExcelField field, List<XZC> value)
        {
            var qy = string.Join(" OR ", value.Select(e => string.Format("{0}.XZCDM = '{1}'", string.IsNullOrEmpty(field.FieldTableName) ? TableName : field.FieldTableName, e.XZCDM)).ToArray());
            var val = GainCommon(field, qy);
            return val;
        }
        private void GainAccess()
        {
            if (ExcelParameterManager.AccessListDict.ContainsKey(ExcelName) && ExcelParameterManager.AccessDict.ContainsKey(ExcelName))
            {
                _accessList = ExcelParameterManager.AccessListDict[ExcelName];
                _dict = ExcelParameterManager.AccessDict[ExcelName];
                return;
            }
            foreach(var entry in ExcelManager.Dict)
            {
                var array = entry.Key.Split(',');
                var result = new List<FieldValue>();
                var value = entry.Value;
                foreach (var field in Fields)
                {

                    var val = field.Indexs != null ? GetValues(field, value) : GetValue(field, value);
                    if (val != null)
                    {
                        result.Add(val);
                    }

                }
                _accessList.AddRange(result);
                _dict.Add(array[1], result);
            }

            ExcelParameterManager.AddAccess(ExcelName, _accessList, _dict);
        }

     

   

        private void WriteAccess()
        {
            var filePath = ModelExcel;
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                LogManager.Log("模型Excel文件不存在，无法进行生成操作");
                return;
            }

            IWorkbook workbook = filePath.OpenExcel();
            if (workbook == null)
            {
                LogManager.Log("无法打开模型Excel文件，无法进行生成操作");
                return;
            }
            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                LogManager.Log("未获取模型Excel文件中的工作表");
                return;
            }
            var head = string.Format("{0} {1}", ExcelName, Title.Replace("{Name}", District).Replace("{Code}", Code));
            sheet.GetRow(0).GetCell(0).SetCellValue(head);
            var i = MStartLine;
            var modelRow = sheet.GetRow(i);
            IRow row = null;
            var list = new List<FieldValue>();
            sheet.ShiftRows(i+1, i+5, Dict.Count - 1);
            foreach(var entry in Dict)
            {
                row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                row.Height = modelRow.Height;
                var cell = ExcelClass.GetCell(row, 0, modelRow);
                cell.SetCellValue(entry.Key);
                var item = ExcelManager.XZQ.FirstOrDefault(e => e.XZCDM == entry.Key);
                cell = ExcelClass.GetCell(row, 1, modelRow);
                if (item != null)
                {
                    cell.SetCellValue(item.XZCMC);
                }
                else
                {
                    cell.SetCellValue("未查询到名称");
                }

                list.AddRange(entry.Value);
                foreach(var field in entry.Value)
                {
                    ExcelClass.GetCell(row, field.Index, modelRow).SetCellValue(field.Value);
                }
                i++;
            }
            row = sheet.GetRow(i);
            row.Height = modelRow.Height;
            var dict = list.GroupBy(e => e.Index).ToDictionary(e => e.Key, e => e.Where(k => k.Val != null && !string.IsNullOrEmpty(k.Val.ToString())));
            foreach(var field in Fields)
            {
                var cell = row.GetCell(field.Index);
                if (!dict.ContainsKey(field.Index))
                {
                    
                    continue;
                }
                var temp = dict[field.Index];
                switch (field.Type)
                {
                    case ExcelType.Double:
                        var a = temp.Sum(e => double.Parse(e.Val.ToString()));
                        cell.SetCellValue(a);
                        break;
                    case ExcelType.Int:
                        var b = temp.Sum(e => int.Parse(e.Val.ToString()));
                        cell.SetCellValue(b);
                        break;
                }
            }
            using (var fs=new FileStream(System.IO.Path.Combine(SaveFolder, ExcelName +" "+TitleName + ".xls"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }

        }

        public void Write()
        {
            var info = string.Empty;
            if (Fields.Count == 0)
            {
                info = string.Format("配置文件FieldInfo.xml未读取{0}的节点信息，无法进行统计数据的核对！", ExcelName);
                Console.WriteLine(info);
                _paralleQuestions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = info });
            }
            else
            {
                _dict.Clear();
                GainAccess();
                WriteAccess();
            }
        }
        public virtual void Check()
        {
            var info = string.Empty;
            if (Fields.Count == 0)
            {
                info= string.Format("配置文件FieldInfo.xml未读取{0}的节点信息，无法进行统计数据的核对！", ExcelName);
                Console.WriteLine(info);
                _paralleQuestions.Add(new Question { Code = "6101", Name = Name, TableName = ExcelName, Description = info });
            }
            else
            {
                _dict.Clear();
        
                Parallel.Invoke(GainExcel, GainAccess);
                if (CheckCode == "6101")
                {
                    CheckData();
                }
                else if (CheckCode == "6201")
                {
                    CheckCollect();
                }
                
            }
        }

        private void CheckData()
        {
            var info = string.Empty;
            var dou = .0;
            if (Dict.Count == 0)
            {
                info = "未获取数据库相关核对数据";
                _paralleQuestions.Add(new Question
                {
                    Code = "6101",
                    Name = Name,
                    TableName = ExcelName,
                    Project = CheckProject.汇总表与数据库图层逻辑一致性,
                    Description = info
                });
            }
            if (ExcelDict.Count == 0)
            {
                info = "未获取Excel文件中的相关数据";
                _paralleQuestions.Add(new Question
                {
                    Code = "6101",
                    Name = Name,
                    TableName = ExcelName,
                    Project = CheckProject.汇总表与数据库图层逻辑一致性,
                    Description = info
                });
            }
            foreach(var entry in Dict)
            {
                var xzc = entry.Key;
                var access = entry.Value;
                if (!access.Any(e => e.Val != null && !string.IsNullOrEmpty(e.Val.ToString())))
                {
                    continue;
                }
                var sum = access.Where(e => e.Val != null && !string.IsNullOrEmpty(e.Val.ToString())).Sum(e => double.TryParse(e.Val.ToString(),out dou)?dou:.0);
                if (sum > 0)
                {

                }
                else
                {
                    continue;
                }

                if (!ExcelDict.ContainsKey(xzc))
                {
                    info = string.Format("表格：{0}中不存在乡镇代码【{1}】的汇总数据，请核对", ExcelName, xzc);
                    _paralleQuestions.Add(new Question
                    {
                        Code = "6101",
                        Name = Name,
                        Project = CheckProject.汇总表与数据库图层逻辑一致性,
                        TableName = ExcelName,
                        Description = info
                    });
                    continue;
                }
               
                var excels = ExcelDict[xzc];
                foreach (var value in access)
                {
                    var excel = excels.FirstOrDefault(e => e.Index == value.Index);
                    if (excel == null)
                    {
                        info = string.Format("检验乡镇代码【{0}]对应{1}的统计值时，未在Excel表格中找到，请核对", xzc, value.Title);
                        _paralleQuestions.Add(new Question
                        {
                            Code = "6101",
                            Name = Name,
                            Project = CheckProject.汇总表与数据库图层逻辑一致性,
                            TableName = ExcelName,
                            BSM = value.Title,
                            Description = info
                        });
                        continue;
                    }
                    switch (value.Type)
                    {
                        case ExcelType.Double:
                            var a = .0;
                            var b = .0;
                            double.TryParse(value.Val.ToString(), out a);
                            double.TryParse(excel.Val.ToString(), out b);
                            if (Math.Abs(a - b) > 0.001)
                            {
                                info = string.Format("乡镇代码【{0}】中{1}的值在数据库中合计值【{2}】与表格中填写的值【{3}】容差率超过0.0001,请核对", xzc,
                                    value.Title,a,b);
                                _paralleQuestions.Add(new Question
                                {
                                    Code = "6102",
                                    Name = Name,
                                    TableName = ExcelName,
                                    Project = CheckProject.表格汇总面积和数据库汇总面积一致性,
                                    Description = info
                                });
                            }
                            break;
                        case ExcelType.Int:
                            var l = 0;
                            var m = 0;
                            int.TryParse(value.Val.ToString(), out l);
                            int.TryParse(excel.Val.ToString(), out m);
                            if (l != m)
                            {
                                info = string.Format("乡镇代码【{0}】中{1}的值在数据库中合计值【{2}】与表格中填写的值【{3}】不相等，请核对", xzc,value.Title, l, m);
                                _paralleQuestions.Add(new Question
                                {
                                    Code = "6101",
                                    Name = Name,
                                    TableName = ExcelName,
                                    Project = CheckProject.汇总表与数据库图层逻辑一致性,
                                    Description = info
                                });
                            }
                            break;
                    }
                }
            }
        }
        private void CheckCollect()
        {
            var info = string.Empty;
            var dou = .0;
            var inter = 0;
            foreach(var field in Fields)
            {
                var excels = _excelList.Where(e => e.Index == field.Index&&e.Val!=null&&!string.IsNullOrEmpty(e.Val.ToString()));
                var access = _accessList.Where(e => e.Index == field.Index&&e.Val!=null&&!string.IsNullOrEmpty(e.Val.ToString()));
                var flag = false;
                switch (field.Type)
                {
                    case ExcelType.Double:
                        var a = excels.Sum(e => double.TryParse(e.Val.ToString(),out dou)?dou:.0);
                        var b = access.Sum(e => double.TryParse(e.Val.ToString(),out dou)?dou:.0);
                        flag = Math.Abs(a - b) > 0.001;
                        break;
                    case ExcelType.Int:
                        var m = excels.Sum(e => int.TryParse(e.Val.ToString(),out inter)?inter:0);
                        var n = access.Sum(e => int.TryParse(e.Val.ToString(),out inter)?inter:0);
                        flag = m != n;
                        break;
                }
                if (flag)//不想等或者容差率超了
                {
                    info = string.Format("{0}中字段：{1}汇总面积和与数据库汇总面积和不符", ExcelName, field.Title);
                    _paralleQuestions.Add(new Question { Code = "6201", Name = "检查表格数据中，各级汇总面积和数据库汇总面积的一致性", Project = CheckProject.表格汇总面积和数据库汇总面积一致性, TableName = ExcelName, BSM = field.Title, Description = info });
                }
            }
        }





        private CollectType _collectType { get; set; }
        /// <summary>
        /// 汇总类型
        /// </summary>
        public CollectType CollectType { get { return _collectType; } set { _collectType = value; } }
        private string _collectFolder { get; set; }

        public string CollectFolder { get { return _collectFolder; }set { _collectFolder = value; } }
        private List<CollectXZQ> _collectXZQ { get; set; }
        public List<CollectXZQ> CollectXZQ { get { return _collectXZQ; } set { _collectXZQ = value; } }

        private string _modelExcel2 { get; set; }

        public string ModelExcel2
        {
            get
            {
                return string.IsNullOrEmpty(_modelExcel2) || !System.IO.File.Exists(_modelExcel2) ? _modelExcel2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excels", XmlNode.Attributes["Model2"].Value) : _modelExcel2;
            }
        }

        public void CollectWrite()
        {
            var info = string.Empty;
            if (Fields.Count == 0)
            {
                info = string.Format("配置文件FieldInfo.xml未读取{0}的节点信息，无法进行汇总数据", ExcelName);
                Console.WriteLine(info);
                _paralleQuestions.Add(new Question { Code = "8888", Name = Name, TableName = ExcelName, Description = info });
            }
            else
            {
                _dict.Clear();

            }
            Dictionary<XZC, Dictionary<XZC, List<FieldValue>>> w = null;
            if (CollectType == CollectType.Excel)
            {

            }else if (CollectType == CollectType.MDB)
            {
                w= CollectMdb();
            }

            //var filePath
        }

        private Dictionary<XZC,Dictionary<XZC,List<FieldValue>>> CollectMdb()
        {
            var messages = new List<string>();
            var mdbfiles = FolderExtensions.GetFiles(CollectFolder, "*.mdb");
            var codefiles = FolderExtensions.GetFiles(CollectFolder, "*.xls");
            #region 获取数据
            var resultDict = new Dictionary<XZC, Dictionary<XZC, List<FieldValue>>>();
            foreach(var shi in CollectXZQ)
            {
                if (shi.Children != null && shi.Children.Count > 0)
                {
                    var shiResult = new Dictionary<XZC, List<FieldValue>>();
                    foreach(var qu in shi.Children)
                    {
                        var codeFile = codefiles.FirstOrDefault(e => e.XZQDM == qu.XZCDM && e.XZQMC == qu.XZCMC);
                        var mdbfile = mdbfiles.FirstOrDefault(e => e.XZQDM == qu.XZCDM && e.XZQMC == qu.XZCMC);
                        if (codeFile == null||mdbfile==null)
                        {
                            messages.Add(string.Format("未找到行政区代码【{0}】行政区名称【{1}】对应的单位代码表或者mdb矢量文件，未进行汇总，请核对", qu.XZCDM, qu.XZCMC));

                        }
                        else
                        {
                            if (Connection != null)
                            {
                                if (Connection.State == System.Data.ConnectionState.Open || Connection.State == System.Data.ConnectionState.Connecting)
                                {
                                    Connection.Close();
                                }
                               
                            }
                            Connection = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", mdbfile.FullName));
                            var dict = ExcelClass.GainXZ(codeFile.FullName);
                            var quResult = new List<FieldValue>();
                            foreach(var entry in dict)//
                            {
                                var array = entry.Key.Split(',');
                                var result = new List<FieldValue>();//每个乡镇对应每一列的值
                                var value = entry.Value;
                                foreach(var field in Fields)
                                {
                                    var val = field.Indexs != null ? GetValues(field, value) : GetValue(field, value);
                                    if (val != null)
                                    {
                                        result.Add(val);
                                    }
                                }
                                quResult.AddRange(result);
                            }
                            shiResult.Add(qu, quResult);
                            Connection.Close();
                        }
                    }

                    resultDict.Add(new XZC { XZCDM = shi.XZQDM, XZCMC = shi.XZQMC }, shiResult);
                }
            }
            #endregion

            return resultDict;


        }

        private void CollectExcel()
        {

        }
    }

    public enum CollectType
    {
        MDB,
        Excel
    }
}
