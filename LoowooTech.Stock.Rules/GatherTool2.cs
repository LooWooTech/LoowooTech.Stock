using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class GatherTool2
    {
        private Dictionary<CollectTable,List<ExcelField>> _dict { get; set; }
        public Dictionary<CollectTable,List<ExcelField>> Dict { get { return _dict; } set { _dict = value; } }

        private string _mdbfile { get; set; }
        public string MdbFile { get { return _mdbfile; } set { _mdbfile = value; } }
        private string _codefile { get; set; }
        public string CodeFile { get { return _codefile; } set { _codefile = value; } }
        private string _XZQDM { get; set; }
        public string XZQDM { get { return _XZQDM; } set { _XZQDM = value; } }
        private string _XZQMC { get; set; }
        public string XZQMC { get { return _XZQMC; } set { _XZQMC = value; } }
        private string _saveFolder { get; set; }
        public string SaveFolder { get { return _saveFolder; } set { _saveFolder = value; } }


        private string _connectionString { get { return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", _mdbfile); } }
        private OleDbConnection _connection { get; set; }
        public OleDbConnection Connection { get { return _connection == null ? _connection = new OleDbConnection(_connectionString) : _connection; } }



        public void Program()
        {
            ExcelManager.Init(CodeFile);//读取单位代码表中的乡镇 以及村列表

            foreach(var entry in Dict)//表格
            {
                var gone = new GatherOne
                {
                    Table = entry.Key,
                    Fields = entry.Value,
                    SaveFolder = SaveFolder,
                    Dict = ExcelManager.Dict,
                    Connection = Connection,
                    XZQDM=XZQDM,
                    XZQMC=XZQMC
                };
                var t = new Thread(gone.Program);
                t.IsBackground = true;
                t.Start();
                
                //Program(entry.Key, entry.Value);

            }
        }
        private List<FieldValue> _results { get; set; }

        private readonly object _syncRoot = new object();

        private void Program(CollectTable table,List<ExcelField> fields)
        {
            var resultdict = new Dictionary<string, List<FieldValue>>();
            var gg = new List<Gather>();
            Console.WriteLine(table.Name);
            #region  获取数据
            foreach (var item in ExcelManager.Dict)//乡镇
            {
                _results = new List<FieldValue>();
                var array = item.Key.Split(',');
                var value = item.Value;
                gg.Add(new Gather { Table = table, Fields = fields, Connection = Connection, XZC = item.Value, XZCDM = array[1] });
            }

            Parallel.ForEach(gg, g =>
            {
                g.Program();
            });

            foreach(var item in gg)
            {
                resultdict.Add(item.XZCDM, item.Results);
            }
            
            #endregion

            #region  写数据
            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Excels",table.Model);
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                LogManager.Log("模型Excel文件不存在，无法进行生成操作！");
                return;
            }

            IWorkbook workbook = filePath.OpenExcel();
            if (workbook == null)
            {
                LogManager.Log("未获取模型Excel文件，无法进行生成操作！");
                return;
            }
            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                LogManager.Log("未获取模型Excel文件中的工作表");
                return;
            }

            var modelrow = sheet.GetRow(table.StartIndex);
            IRow row = null;
            sheet.ShiftRows(table.StartIndex + 1, table.StartIndex + 5, resultdict.Count - 1);
            var i = table.StartIndex;
            var list = new List<FieldValue>();
            foreach(var entry in resultdict)
            {
                row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                i++;
                row.Height = modelrow.Height;
                var cell = ExcelClass.GetCell(row, 0, modelrow);
                cell.SetCellValue(entry.Key);
                var item = ExcelManager.XZQ.FirstOrDefault(e => e.XZCDM == entry.Key);
                cell = ExcelClass.GetCell(row, 1, modelrow);
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
                    ExcelClass.GetCell(row, field.Index-2, modelrow).SetCellValue(field.Value);
                }
            }

            row = sheet.GetRow(i);
            row.Height = modelrow.Height;
            var dict2 = list.GroupBy(e => e.Index).ToDictionary(e => e.Key, e => e.Where(k => k.Val != null && !string.IsNullOrEmpty(k.Val.ToString())));
            foreach(var field in fields)
            {
                var cell = ExcelClass.GetCell(row, field.Index-2, modelrow);
                if (!dict2.ContainsKey(field.Index))
                {
                    continue;
                }
                var temp = dict2[field.Index];
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
            using (var fs=new FileStream(System.IO.Path.Combine(SaveFolder, table.Name + "" + table.Title + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".xlsx"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }


            #endregion
        }
        private FieldValue GetValues(ExcelField field, List<XZC> value,List<ExcelField> fields,string tableName)
        {
            var item = _results.FirstOrDefault(e => e.Index == field.Index);
            if (item != null)
            {
                return item;
            }
            var a = 0;
            var b = .0;
            var IntSum = 0;
            var DoubleSum = .0;
            foreach (var index in field.Indexs)
            {
                var f = fields.FirstOrDefault(e => e.Index == index);
                if (f != null)
                {
                    var val = f.Indexs!=null?GetValues(f,value,fields,tableName): GetValue(f, value,tableName);
                    if (val != null)
                    {
                        switch (field.Type)
                        {
                            case ExcelType.Double:
                                DoubleSum += Math.Round(double.TryParse(val.Val.ToString(), out b) ? b : .0, 4);
                                break;
                            case ExcelType.Int:
                                IntSum += int.TryParse(val.Val.ToString(), out a) ? a : 0;
                                break;
                        }
                    }
                }
            }

            var ee= new FieldValue
            {
                Index = field.Index,
                Type = field.Type,
                Title = field.Title,
                Val = field.Type == ExcelType.Double ? DoubleSum : IntSum
            };
            _results.Add(ee);

            return ee;
        }
        private FieldValue GetValue(ExcelField field, List<XZC> value,string tableName)
        {
            var item = _results.FirstOrDefault(e => e.Index == field.Index);
            if (item != null)
            {
                return item;
            }
            var qy = string.Join(" OR ", value.Select(e => string.Format("{0}.XZCDM = '{1}'", string.IsNullOrEmpty(field.FieldTableName) ? tableName : field.FieldTableName, e.XZCDM)).ToArray());
            var val = GainCommon(field, qy,tableName);
            if (val != null)
            {
                _results.Add(val);
            }
  
            return val;
        }

        private FieldValue GainCommon(ExcelField field, string dmWhere,string tableName)
        {

            var a = 0;
            var b = .0;
            var val = string.Empty;
            var sb = new StringBuilder(string.Format("Select {0} from ", field.SQL));
            if (string.IsNullOrEmpty(field.View))
            {
                if (string.IsNullOrEmpty(field.FieldTableName))
                {
                    sb.Append(tableName);
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
            Console.WriteLine("{0}-{1}:{2}",field.Index, tableName, sb.ToString());
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
                Val = val
            };
            return result;
        }
    }


    public class GatherOne
    {
        private string _XZQDM { get; set; }
        public string XZQDM { get { return _XZQDM; } set { _XZQDM = value; } }
        private string _XZQMC { get; set; }
        public string XZQMC { get { return _XZQMC; } set { _XZQMC = value; } }
        private CollectTable _table { get; set; }
        public CollectTable Table { get { return _table; } set { _table = value; } }
        private List<ExcelField> _fields { get; set; }
        public List<ExcelField> Fields { get { return _fields; } set { _fields = value; } }
        private string _saveFolder { get; set; }
        public string SaveFolder { get { return _saveFolder; } set { _saveFolder = value; } }
        private Dictionary<string,List<XZC>> _dict { get; set; }
        public Dictionary<string,List<XZC>> Dict { get { return _dict; } set { _dict = value; } }

        private OleDbConnection _connection { get; set; }
        public OleDbConnection Connection { get { return _connection; } set { _connection = value; } }

        public void Program()
        {
            var resultdict = new Dictionary<string, List<FieldValue>>();
            var gg = new List<Gather>();
            Console.WriteLine(Table.Name);
            #region  获取数据
            foreach (var item in Dict)//乡镇
            {
                var array = item.Key.Split(',');
                var value = item.Value;
                gg.Add(new Gather { Table = Table, Fields = Fields, Connection = Connection, XZC = item.Value, XZCDM = array[1] });
            }

            Parallel.ForEach(gg, g =>
            {
                g.Program();
            });

            foreach (var item in gg)
            {
                resultdict.Add(item.XZCDM, item.Results);
            }

            #endregion
            #region  写数据
            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excels", Table.Model);
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                LogManager.Log("模型Excel文件不存在，无法进行生成操作！");
                return;
            }

            IWorkbook workbook = filePath.OpenExcel();
            if (workbook == null)
            {
                LogManager.Log("未获取模型Excel文件，无法进行生成操作！");
                return;
            }
            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                LogManager.Log("未获取模型Excel文件中的工作表");
                return;
            }

            var modelrow = sheet.GetRow(Table.StartIndex);
            IRow row = null;
            sheet.ShiftRows(Table.StartIndex + 1, Table.StartIndex + 5, resultdict.Count - 1);
            var i = Table.StartIndex;
            var list = new List<FieldValue>();
            foreach (var entry in resultdict)
            {
                row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                i++;
                row.Height = modelrow.Height;
                var cell = ExcelClass.GetCell(row, 0, modelrow);
                cell.SetCellValue(entry.Key);
                var item = ExcelManager.XZQ.FirstOrDefault(e => e.XZCDM == entry.Key);
                cell = ExcelClass.GetCell(row, 1, modelrow);
                if (item != null)
                {
                    cell.SetCellValue(item.XZCMC);
                }
                else
                {
                    cell.SetCellValue("未查询到名称");
                }
                list.AddRange(entry.Value);
                foreach (var field in entry.Value)
                {
                    ExcelClass.GetCell(row, field.Index - 2, modelrow).SetCellValue(field.Value);
                }
            }

            row = sheet.GetRow(i);
            row.Height = modelrow.Height;
            var dict2 = list.GroupBy(e => e.Index).ToDictionary(e => e.Key, e => e.Where(k => k.Val != null && !string.IsNullOrEmpty(k.Val.ToString())));
            foreach (var field in Fields)
            {
                var cell = ExcelClass.GetCell(row, field.Index - 2, modelrow);
                if (!dict2.ContainsKey(field.Index))
                {
                    continue;
                }
                var temp = dict2[field.Index];
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
            using (var fs = new FileStream(System.IO.Path.Combine(SaveFolder,XZQMC+"("+XZQDM+")"+ Table.Name + "" + Table.Title + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".xlsx"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }


#endregion
        }

    }


    public class Gather
    {
        private CollectTable _table { get; set; }
        public CollectTable Table { get { return _table; } set { _table = value; } }
        private List<ExcelField> _fields { get; set; }
        /// <summary>
        /// 表格中所有字段信息
        /// </summary>
        public List<ExcelField> Fields { get { return _fields; } set { _fields = value; } }
        private OleDbConnection _connection { get; set; }
        public OleDbConnection Connection { get { return _connection; }set { _connection = value; } }
        private List<XZC> _xzc { get; set; }
        /// <summary>
        /// 行政村列表
        /// </summary>
        public List<XZC> XZC { get { return _xzc; } set { _xzc = value; } }

        private string _XZCDM { get; set; }
        /// <summary>
        /// 乡镇代码
        /// </summary>
        public string XZCDM { get { return _XZCDM; } set { _XZCDM = value; } }



        private List<FieldValue> _results { get; set; } = new List<FieldValue>();

        public List<FieldValue> Results { get { return _results; } }

        public void Program()
        {
            foreach(var field in Fields)
            {
                var val = field.Indexs != null ? GetValues(field, XZC, Fields, Table.TableName) : GetValue(field, XZC, Table.TableName);
                if (val != null && _results.Any(e => e.Index == val.Index) == false)
                {
                    _results.Add(val);
                }
            }
        }

        private FieldValue GetValues(ExcelField field, List<XZC> value, List<ExcelField> fields, string tableName)
        {
            var item = _results.FirstOrDefault(e => e.Index == field.Index);
            if (item != null)
            {
                return item;
            }
            var a = 0;
            var b = .0;
            var IntSum = 0;
            var DoubleSum = .0;
            foreach (var index in field.Indexs)
            {
                var f = fields.FirstOrDefault(e => e.Index == index);
                if (f != null)
                {
                    var val = f.Indexs != null ? GetValues(f, value, fields, tableName) : GetValue(f, value, tableName);
                    if (val != null)
                    {
                        switch (field.Type)
                        {
                            case ExcelType.Double:
                                DoubleSum += Math.Round(double.TryParse(val.Val.ToString(), out b) ? b : .0, 4);
                                break;
                            case ExcelType.Int:
                                IntSum += int.TryParse(val.Val.ToString(), out a) ? a : 0;
                                break;
                        }
                    }
                }
            }

            var ee = new FieldValue
            {
                Index = field.Index,
                Type = field.Type,
                Title = field.Title,
                Val = field.Type == ExcelType.Double ? DoubleSum : IntSum
            };
            _results.Add(ee);

            return ee;
        }
        private FieldValue GetValue(ExcelField field, List<XZC> value, string tableName)
        {
            var item = _results.FirstOrDefault(e => e.Index == field.Index);
            if (item != null)
            {
                return item;
            }
            var qy = string.Join(" OR ", value.Select(e => string.Format("{0}.XZCDM = '{1}'", string.IsNullOrEmpty(field.FieldTableName) ? tableName : field.FieldTableName, e.XZCDM)).ToArray());
            var val = GainCommon(field, qy, tableName);
            if (val != null)
            {
                _results.Add(val);
            }

            return val;
        }

        private FieldValue GainCommon(ExcelField field, string dmWhere, string tableName)
        {

            var a = 0;
            var b = .0;
            var val = string.Empty;
            var sb = new StringBuilder(string.Format("Select {0} from ", field.SQL));
            if (string.IsNullOrEmpty(field.View))
            {
                if (string.IsNullOrEmpty(field.FieldTableName))
                {
                    sb.Append(tableName);
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
            Console.WriteLine("{0}-{1}:{2}", field.Index, tableName, sb.ToString());
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
                Val = val
            };
            return result;
        }
    }
}
