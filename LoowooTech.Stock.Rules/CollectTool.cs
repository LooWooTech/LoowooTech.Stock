using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 对某一个区县 进行表1、表2的数据读取生成
    /// </summary>
    public class CollectTool
    {
        private string _mdbFile { get; set; }
        /// <summary>
        /// mdb文件路径
        /// </summary>
        public string MdbFile { get { return _mdbFile; }set { _mdbFile = value; } }
        private string _codeFile { get; set; }
        /// <summary>
        /// 单位代码表路径
        /// </summary>
        public string CodeFile { get { return _codeFile; }set { _codeFile = value; } }
        private Dictionary<CollectTable,List<ExcelField>> _tableFieldDict { get; set; }
        /// <summary>
        /// 表格对应字段列表信息
        /// </summary>
        public Dictionary<CollectTable,List<ExcelField>> TableFieldDict { get { return _tableFieldDict; }set { _tableFieldDict = value; } }
        private string _XZQDM { get; set; }
        public string XZQDM { get { return _XZQDM; }set { _XZQDM = value; } }
        private string _XZQMC { get; set; }
        public string XZQMC { get { return _XZQMC; }set { _XZQMC = value; } }




        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public  string ConnectionString { get { return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MdbFile); } }
        private OleDbConnection _connection { get; set; }
        /// <summary>
        /// mdb数据库连接符
        /// </summary>
        public OleDbConnection Connection { get { return _connection == null ? _connection = new OleDbConnection(ConnectionString) : _connection; } }
        private Dictionary<string,List<XZC>> _xzcDict { get; set; }
        /// <summary>
        /// 从Excel中读取的行政村单位代码表字典  键：行政村名称，行政村代码 组成 值：该乡镇下面的行政村列表
        /// </summary>
        public Dictionary<string,List<XZC>> XZCDict { get { return _xzcDict; } }

        private Dictionary<CollectTable, Dictionary<string, List<FieldValue>>> _result { get; set; } = new Dictionary<CollectTable, Dictionary<string, List<FieldValue>>>();
        /// <summary>
        /// 数据结果
        /// </summary>
        public Dictionary<CollectTable,Dictionary<string,List<FieldValue>>> Result { get { return _result; } }

        private List<Collect> _resullt2 { get; set; } = new List<Collect>();
        public List<Collect> Result2 { get { return _resullt2; } }

        private readonly object _syncRoot = new object();

        private void Init()
        {
            _xzcDict = ExcelClass.GainXZ(CodeFile);
        }

        public void Program()
        {
            Init();

            //var TableDict = new Dictionary<CollectTable, Dictionary<string, List<FieldValue>>>();//存储表1 表2 对应的每个乡镇对应的字段值列表   表1（表2、表3、表4、表5、表6）----乡镇名称乡镇代码----字段对应的值
            //foreach(var tableInfo in TableFieldDict)//所有表
            //{

            //    var XZDict = new Dictionary<string, List<FieldValue>>();//乡镇对应的字段值列表
            //    foreach(var entry in XZCDict)//当前行政区 乡镇信息
            //    {
            //        var result = new List<FieldValue>();

            //        var value = entry.Value;
            //        foreach(var field in tableInfo.Value)//某一张表格的字段列表
            //        {
            //            var val = field.Indexs != null ? GetValues(field, value, tableInfo.Value, tableInfo.Key.TableName) : GetValue(field, value, tableInfo.Key.TableName);
            //            if (val != null)
            //            {
            //                result.Add(val);
            //            }
            //        }

            //        XZDict.Add(entry.Key, result);
            //    }
            //    TableDict.Add(tableInfo.Key, XZDict);

            //}
            //_result = TableDict;


            Parallel.ForEach(TableFieldDict, entry =>
            {
                var result = Program(entry.Key, entry.Value);
                AddResult(entry.Key, result);
            });
            Connection.Close();

        }
        private void AddResult(CollectTable table,Dictionary<string,List<FieldValue>> result)
        {
            lock (_syncRoot)
            {
                _result.Add(table, result);
                _resullt2.Add(new Collect { XZQDM = XZQDM, XZQMC = XZQMC, Table = table,Values=result });
            }
        }

        private Dictionary<string,List<FieldValue>> Program(CollectTable table,List<ExcelField> fields)
        {
            var XZDict = new Dictionary<string, List<FieldValue>>();//乡镇对应的字段值列表
            foreach (var entry in XZCDict)//当前行政区 乡镇信息
            {
                var result = new List<FieldValue>();

                var value = entry.Value;
                foreach (var field in fields)//某一张表格的字段列表
                {
                    var val = field.Indexs != null ? GetValues(field, value, fields, table.TableName) : GetValue(field, value, table.TableName);
                    if (val != null)
                    {
                        result.Add(val);
                    }
                }

                XZDict.Add(entry.Key, result);
            }
            return XZDict;
        }
        private FieldValue GetValues(ExcelField field, List<XZC> value,List<ExcelField> fields,string tableName)
        {
            var a = 0;
            var b = .0;
            var IntSum = 0;
            var DoubleSum = .0;
            foreach (var index in field.Indexs)
            {
                var f = fields.FirstOrDefault(e => e.Index == index);
                if (f != null)
                {
                    var val = GetValue(f, value,tableName);
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

            return new FieldValue
            {
                Index = field.Index,
                Type = field.Type,
                Title = field.Title,
                Val = field.Type == ExcelType.Double ? DoubleSum : IntSum
            };
        }
        private FieldValue GetValue(ExcelField field, List<XZC> value,string tableName)
        {
            var qy = string.Join(" OR ", value.Select(e => string.Format("{0}.XZCDM = '{1}'", string.IsNullOrEmpty(field.FieldTableName) ? tableName : field.FieldTableName, e.XZCDM)).ToArray());
            var val = GainCommon(field, qy,tableName);
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
            Console.WriteLine("{0}:{1}", tableName, sb.ToString());
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

        private void Program(List<ExcelField> fields)
        {

        }
    }
}
