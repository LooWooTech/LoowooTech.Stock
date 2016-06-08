using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class MdbClass
    {

        /// <summary>
        /// 获取数据库中的所有表名
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<string> GetTables(OleDbConnection connection)
        {
            var list = new List<string>();
            if (connection != null)
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var Tables = connection.GetSchema("TABLE");
                foreach(DataRow row in Tables.Rows)
                {
                    if (row[3].ToString().ToUpper() == "TABLE")
                    {
                        list.Add(row[2].ToString());
                    }
                }

            }
            return list;
        }

        private static List<string> GetTablesTwo(OleDbConnection connection)
        {
            var list = new List<string>();
            if (connection != null)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                DataTable shemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });
                var m = shemaTable.Columns.IndexOf("TABLE_NAME");
                for(var i = 0; i < shemaTable.Rows.Count; i++)
                {
                    var dataRow = shemaTable.Rows[i];
                    list.Add(dataRow.ItemArray.GetValue(m).ToString());
                }
            }
            return list;
        }

        public static List<Field> GetFields(OleDbConnection connection,string tableName)
        {
            var list = new List<Field>();
            if (connection != null)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                var table = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName, null });
                var m = table.Columns.IndexOf("COLUMN_NAME");
                var n = table.Columns.IndexOf("DATA_TYPE");
                var a = table.Columns.IndexOf("NUMERIC_PRECISION");
                var b = table.Columns.IndexOf("CHARACTER_MAXIMUM_LENGTH");
                var length = 0;
                for(var i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    list.Add(new Field()
                    {
                        Name = row.ItemArray.GetValue(m).ToString(),
                        Title=row.ItemArray.GetValue(a).ToString(),
                        Length=int.TryParse(row.ItemArray.GetValue(b).ToString(),out length)?length:0,
                        Type=(FieldType)Enum.Parse(typeof(FieldType),row.ItemArray.GetValue(n).ToString()),
                    });
                }
            }
            return list;
        }
    }
}
