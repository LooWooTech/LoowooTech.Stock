using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 字段的值 只能填哪些值
    /// </summary>
    public class ValueRangeTool:ValueBaseTool, ITool
    {
        
        /// <summary>
        /// 检查字段名
        /// </summary>
        public string CheckFieldName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值  范围
        /// </summary>
        public string[] Values { get; set; }
        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name { get
            {
                var sb = new StringBuilder(string.Format("规则{0}:表‘{1}’中字段‘{2}’值范围‘{3}’", ID, TableName, CheckFieldName,Values[0]));
                for(var i = 1; i < Values.Length; i++)
                {
                    sb.AppendFormat("或‘{0}’", Values[i]);
                }
                return sb.ToString();
            }
        }

        public string[] WhereFields { get; set; }
        public List<string> WhereList { get; set; }
        public string Split { get; set; }

      
        private bool CheckWhere(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, string.Format("Select {0},{1},{2} from {3}", CheckFieldName, Key, string.Join(",", WhereFields), TableName));
            if (reader != null)
            {
                var str = string.Empty;
                var error = string.Empty;
                var array = new string[WhereFields.Length];
                while (reader.Read())
                {
                    str = reader[0].ToString().Trim();
                    for(var i = 0; i < WhereFields.Length; i++)
                    {
                        array[i] = reader[i + 2].ToString();
                    }
                    var key = string.Join(Split, array);
                    if (WhereList.Contains(key))
                    {
                        if (!Values.Contains(str))
                        {
                            error = string.Format("{0}对应的‘{1}’不正确", reader[1].ToString(), str);
                            _questions.Add(new Question { Code = "3201", Name = Name, Project = CheckProject.值符合性, TableName = TableName, BSM = reader[1].ToString(), Description = error });
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private bool CheckNoWhere(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, string.Format("select {0},{1} from {2}", CheckFieldName, Key, TableName));
            if (reader != null)
            {
                var str = string.Empty;
                var error = string.Empty;
                Messages = new List<string>();
                while (reader.Read())
                {
                    str = reader[0].ToString().Trim();
                    if (Values.Contains(str))
                    {
                        continue;
                    }
                    else
                    {
                        error = string.Format("{0}对应的‘{1}’值不正确", reader[1].ToString(), str);
                        Messages.Add(error);
                        _questions.Add(new Question() { Code = "3201", Name = Name, Project = CheckProject.值符合性, TableName = TableName, BSM = reader[1].ToString(), Description = error });
                    }
                }
                QuestionManager.AddRange(_questions);
                return true;
            }
            return false;
        }
        public bool Check(OleDbConnection connection)
        {
            if (WhereFields != null)
            {
                return CheckWhere(connection);
            }
            return CheckNoWhere(connection);
        }
    }
}
