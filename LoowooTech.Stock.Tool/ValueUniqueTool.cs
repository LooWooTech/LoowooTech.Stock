using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System.Collections.Generic;
using System.Data.OleDb;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 检查字段值  唯一
    /// </summary>
    public class ValueUniqueTool:ValueBaseTool, ITool
    {
        /// <summary>
        /// 检查字段
        /// </summary>
        public string CheckFieldName { get; set; }
        
        /// <summary>
        /// 约束字段，字段可空
        /// </summary>
        public string WhereFieldName { get; set; }
        public string Code { get; set; }

        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(WhereFieldName))
                {
                    return string.Format("规则{0}:表‘{1}’中字段‘{2}’的值为唯一值", ID, TableName, CheckFieldName);
                }

                return string.Format("规则{0}:表‘{1}’中字段‘{2}’在字段‘{3}’同一值下的值为唯一值", ID, TableName, CheckFieldName,WhereFieldName);
            }
        }

        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, string.IsNullOrEmpty(WhereFieldName) ? string.Format("Select {0} from {1}", CheckFieldName, TableName) : string.Format("Select {0},{1} from {1}", CheckFieldName, WhereFieldName, TableName));
            if (reader != null)
            {
                var temp = new List<string>();
                var str = string.Empty;
                var info = string.Empty;
                while (reader.Read())
                {
                    str = string.IsNullOrEmpty(WhereFieldName) ? string.Format("{0}:{1}", CheckFieldName, reader[0].ToString()) : string.Format("{0}:{1}并且{2}:{3}", WhereFieldName, reader[1], CheckFieldName, reader[0]);
                    if (temp.Contains(str))
                    {
                        info = string.Format("{0}  存在重复", str);
                        Messages.Add(info);
                        _questions.Add(new Question { Code = Code, Name = Name, Project = CheckProject.属性正确性, TableName = TableName, BSM = CheckFieldName, Description = info });
                    }
                    else
                    {
                        temp.Add(str);
                    }
                }
                QuestionManager.AddRange(_questions);
                return true;
            }
            return false;
        }

    }
}
