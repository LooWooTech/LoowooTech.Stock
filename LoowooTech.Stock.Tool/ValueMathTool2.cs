using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueMathTool2:ValueBaseTool2,IVTool
    {
        public string Name
        {
            get
            {
                return string.Format("规则{0}：表【{1}】中字段【{2}】与字段【{3}】相匹配",ID,TableName,CheckKeyFieldName,CheckValueFieldName);
            }
        }

        public string CheckKeyFieldName { get; set; }
        public string CheckValueFieldName { get; set; }
        public Dictionary<string,string> CurrentDict { get; set; }

        private string _SQL
        {
            get
            {
                return string.Format("SELECT {0},{1},{2} FROM {3}", Key, CheckKeyFieldName, CheckValueFieldName, TableName);
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
                if (string.IsNullOrEmpty(keyValue) == false)
                {
                    var value1 = reader[1].ToString().Trim();
                    var value2 = reader[2].ToString().Trim();
                    if (string.IsNullOrEmpty(value1) == false && string.IsNullOrEmpty(value2)==false)
                    {
                        if (CurrentDict.ContainsKey(value1))
                        {
                            if (CurrentDict[value1].ToLower() != value2.ToLower())
                            {
                                Messages2.Add(new Models.VillageMessage
                                {
                                    Value = keyValue,
                                    CheckValue=value1+";"+value2,
                                    Description = string.Format("【{0}={1}】对应的数据不符合【{2}】", Key, keyValue, Name),
                                    WhereClause = string.Format("[{0}] = '{1}'", Key, keyValue)
                                });
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
