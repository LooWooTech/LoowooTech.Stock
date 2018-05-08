using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class GatherTool
    {
        private readonly object _syncRoot = new object();

        private Dictionary<CollectTable,List<ExcelField>> _dict { get; set; }
        public Dictionary<CollectTable,List<ExcelField>> Dict { get { return _dict; } set { _dict = value; } }

        private string _mdbFile { get; set; }
        public string MdbFile { get { return _mdbFile; }set { _mdbFile = value; } }
        private string _XZQDM { get; set; }
        public string XZQDM { get { return _XZQDM; } set { _XZQDM = value; } }
        private string _XZQMC { get; set; }
        public string XZQMC { get { return _XZQMC; } set { _XZQMC = value; } }



        private string _connectString { get { return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MdbFile); } }

        private OleDbConnection _connection { get; set; }
        public OleDbConnection Connection { get { return _connection == null ? _connection = new OleDbConnection(_connectString) : _connection; } }


        private List<Collect2> _collects { get; set; } = new List<Collect2>();
        public List<Collect2> Collect2 { get { return _collects; } }

        public void Program()
        {
            foreach(var entry in Dict)
            {
                var results = Program(entry.Key, entry.Value);
                _collects.Add(new Collect2 { XZQDM = XZQDM, XZQMC = XZQMC, Table = entry.Key, Values = results });
            }
            if (Connection.State == System.Data.ConnectionState.Connecting)
            {
                Connection.Close();
            }
            Connection.Close();
        }

        private List<FieldValue> Program(CollectTable table,List<ExcelField> fields)
        {
            List<FieldValue> results = new List<FieldValue>();
            foreach(var field in fields)
            {
                var val = field.Indexs != null ? GetValues(field, fields, table.TableName) : GetValue(field, table.TableName);
                if (val != null)
                {
                    results.Add(val);
                }
            }
            return results;
        }

        private FieldValue GetValues(ExcelField field, List<ExcelField> fields, string tableName)
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
                    var val = f.Indexs!=null?GetValues(f,fields,tableName): GetValue(f, tableName);
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

        private FieldValue GetValue(ExcelField field,string tableName)
        {
            return GainCommon(field, tableName);
        }

        private FieldValue GainCommon(ExcelField field, string tableName)
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
            if (!string.IsNullOrEmpty(field.WhereClause))
            {
                sb.AppendFormat(" Where {0}", field.WhereClause);
            }
            Console.WriteLine("{0}:{1}-{2}", tableName, sb.ToString(),field.Index);
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
