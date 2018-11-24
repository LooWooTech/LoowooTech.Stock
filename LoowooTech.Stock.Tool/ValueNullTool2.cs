using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoowooTech.Stock.Tool
{
    public class ValueNullTool2:ValueBaseTool2,IVTool
    {
        public string Name
        {
            get
            {
                var str= string.IsNullOrEmpty(WhereClause) ?
                    string.Format("规则{0}：表【{1}】中字段【{2}】{3}", ID, TableName, string.Join(",", CheckFields), Is_Nullable ? "为空" : "必填")
                    : string.Format("规则{0}：当【{1}】时，表【{2}】字段【{3}】{4}", ID, WhereClause, TableName, string.Join(",", CheckFields), Is_Nullable ? "为空" : "必填");
                if (string.IsNullOrEmpty(RegexString) == false)
                {
                    str += string.Format("并且满足【{0}】", RegexString);
                }
                return str;
            }
        }
        public bool Is_Nullable { get; set; }//true  为空  false  必填
        public string WhereClause { get; set; }
        public string[] CheckFields { get; set; }

        public string RegexString { get; set; }

        private string _SQL
        {
            get
            {
                return string.IsNullOrEmpty(WhereClause) ?
                    string.Format("SELECT {0},{1} FROM {2}", Key, string.Join(",", CheckFields), TableName)
                    : string.Format("SELECT {0},{1} FROM {2} WHERE {3}", Key, string.Join(",", CheckFields), TableName, WhereClause);
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
                    var temp = new List<string>();
                    for(var i = 0; i < CheckFields.Count(); i++)
                    {
                        var a = reader[i + 1].ToString().Trim();
                        if (string.IsNullOrEmpty(RegexString) == false&&Is_Nullable==false)
                        {
                            if (Regex.IsMatch(a, RegexString) == false)
                            {
                                temp.Add(CheckFields[i]);
                            }
                        }
                        else
                        {
                            if (Is_Nullable ^ string.IsNullOrEmpty(a))
                            {
                                temp.Add(CheckFields[i]);
                            }
                        }
   

                        
                    }
                    if (temp.Count > 0)
                    {
                        Messages2.Add(new Models.VillageMessage
                        {
                            Value = keyValue,
                            Description = string.Format("【{0} = {1}】对应的字段【{2}】值不符合【{3}】", Key, keyValue, string.Join(",", temp.ToArray()), Name),
                            WhereClause = string.Format("[{0}] = '{1}'", Key, keyValue)
                        });
                    }
                }
            }
            return true;
        }
    }
}
