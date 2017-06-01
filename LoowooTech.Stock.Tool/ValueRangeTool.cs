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
        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, string.Format("select {0},{1} from {2}", CheckFieldName, Key, TableName));
            if (reader != null)
            {
                var str = string.Empty;
                var error = string.Empty;
                Messages = new List<string>();
                while (reader.Read())
                {
                    str = reader[0].ToString();
                    if (Values.Contains(str))
                    {
                        continue;
                    }
                    else
                    {
                        error = string.Format("{0}对应的‘{1}’值不正确", reader[1].ToString(), str);
                        Messages.Add(error);
                        _questions.Add(new Question() { Code = "3201", Name =Name, Project = CheckProject.值符合性, TableName = TableName, BSM = reader[1].ToString(), Description = error });
                    }
                }
                QuestionManager.AddRange(_questions);
                return true;
            }
            return false;
        }
    }
}
