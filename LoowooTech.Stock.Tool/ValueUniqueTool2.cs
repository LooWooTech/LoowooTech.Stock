using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueUniqueTool2:ValueBaseTool2,IVTool
    {
        public string Name
        {
            get
            {
                return string.Format("规则{0}：表【{1}】中字段【{2}】在字段【{3}】相同值下为唯一值;", ID, TableName, CheckFieldName, GroupByFieldName);
            }
        }
        public string CheckFieldName { get; set; }
        public string GroupByFieldName { get; set; }

        private string _SQL
        {
            get
            {
                return string.Format("SELECT {0},{1},{2} FROM {3}", Key, CheckFieldName, GroupByFieldName, TableName);
            }
        }
        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, _SQL);
            if (reader == null)
            {
                return false;
            }
            var list = new List<string>();
            while (reader.Read())
            {
                var keyValue = reader[0].ToString().Trim();

                if (string.IsNullOrEmpty(keyValue) == false)
                {
                    var str = string.Format("{0}:{1}", reader[2].ToString().Trim(), reader[1].ToString().Trim());
                    if (list.Contains(str))
                    {
                        Messages2.Add(new Models.VillageMessage
                        {
                            Value = keyValue,
                            CheckValue = str,
                            Description = string.Format("【{0} ={1}】对应的数据不符合【{2}】", Key, keyValue, Name),
                            WhereClause = string.Format("[{0}] = '{1}'", Key, keyValue)
                        });
                    }
                    else
                    {
                        list.Add(str);
                    }
                  
                }
            }
            return true;
        }
    }
}
