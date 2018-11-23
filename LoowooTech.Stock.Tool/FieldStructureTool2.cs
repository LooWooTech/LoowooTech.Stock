using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class FieldStructureTool2
    {
        public VillageTable CheckTable { get; set; }
        private List<string> _lacks { get; set; } = new List<string>();
        private List<string> _errors { get; set; } = new List<string>();

        public string Message { get
            {
                var sb = new StringBuilder();
                if (_lacks.Count > 0)
                {
                    sb.AppendFormat("缺少字段：{0}；", string.Join(",", _lacks.ToArray()));
                }
                if (_errors.Count > 0)
                {
                    sb.AppendFormat("字段类型或者长度不符：{0}；", string.Join(",", _errors.ToArray()));
                }
                return sb.ToString();
            } }
        public void Check(OleDbConnection connection)
        {
            if (connection != null)
            {
                if (connection.State == System.Data.ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var dict = new Dictionary<string, Field>();
                var table = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, CheckTable.Name, null });
                var m = table.Columns.IndexOf("COLUMN_NAME");
                var n = table.Columns.IndexOf("NUMERIC_PRECISION");
                var l = table.Columns.IndexOf("CHARACTER_MAXIMUM_LENGTH");
                var a = table.Columns.IndexOf("DATA_TYPE");
                var length = 0;

                for (var i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var name = row.ItemArray.GetValue(m).ToString().Trim();
                    if (!dict.ContainsKey(name))
                    {
                        var field = new Field()
                        {
                            Name = name,
                            Title = row.ItemArray.GetValue(n).ToString().Trim(),
                            Length = int.TryParse(row.ItemArray.GetValue(l).ToString().Trim(), out length) ? length : int.TryParse(row.ItemArray.GetValue(n).ToString().Trim(), out length) ? length : 0,
                            Type = (FieldType)Enum.Parse(typeof(FieldType), row.ItemArray.GetValue(a).ToString().Trim())
                        };
                        dict.Add(name, field);
                    }
                }
                foreach(var field in CheckTable.Fields)
                {
                    if (dict.ContainsKey(field.Name))
                    {
                        var currentfield = dict[field.Name];
                        if (currentfield.Type == FieldType.NChar)
                        {
                            currentfield.Type = FieldType.Char;
                        }
                        if (currentfield.Type == FieldType.Real)
                        {
                            currentfield.Type = FieldType.Float;
                        }
                        if (currentfield.Type == FieldType.SmallInt)
                        {
                            currentfield.Type = FieldType.Int;
                        }
                        if (currentfield.Type == FieldType.DateTime || currentfield.Type == FieldType.SmallDateTime)
                        {
                            currentfield.Type = FieldType.Date;
                        }

                        if (field.Type != currentfield.Type.ToString() || field.Name != currentfield.Name || (field.Type == "Char" ? field.Length != currentfield.Length : false))
                        {
                            _errors.Add(field.Name);
                        }
                    }
                    else
                    {
                        _lacks.Add(field.Name);
                    }
                }



            }
        }
    }
}
