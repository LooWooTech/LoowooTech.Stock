using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueRangeTool2:ValueBaseTool2,IVTool
    {
        public string Name
        {
            get
            {
                return
                    string.IsNullOrEmpty(WhereClause) ?
                    string.Format("规则{0}：表【{1}】中字段【{2}】的值域范围是【{3}】", ID, TableName, CheckFieldName, string.Join(",", Values))
                    : string.Format("规则{0}：表【{1}】中字段【{2}】当【{3}】时的值域范围是【{4}】", ID, TableName, CheckFieldName, WhereClause, string.Join(",", Values));
            }
        }

        public string CheckFieldName { get; set; }
        public string[] Values { get; set; }

        public string WhereClause { get; set; }

        private string _SQL { get
            {
                var text = string.Format("SELECT {0} FROM {1} WHERE {2} NOT IN ({3})", Key, TableName, CheckFieldName, string.Join(",", Values));

                return
                    string.IsNullOrEmpty(WhereClause) ?
                    string.Format("SELECT {0},{1} FROM {2}", Key, CheckFieldName, TableName)
                    : string.Format("SELECT {0},{1} FROM {2} WHERE {3}", Key, CheckFieldName, TableName, WhereClause);
            }
        }

        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, _SQL);
            if (reader == null)
            {
                return false;
            }

            while (reader.Read())
            {
                var keyValue = reader[0].ToString().Trim();
                var checkValue = reader[0].ToString().Trim();
                if (string.IsNullOrEmpty(keyValue) == false)
                {
                    if (Values.Contains(checkValue) == false)
                    {
                        Messages2.Add(new Models.VillageMessage
                        {
                            Value = keyValue,
                            Description = string.Format("【{0}={1}】不符合【{2}】", Key, keyValue, Name),
                            WhereClause = string.Format("[{0}] = '{1}'", Key, keyValue)
                        });
                        //Messages.Add(keyValue);
                    }
                }
            }
            return true;
        }

    }
}
