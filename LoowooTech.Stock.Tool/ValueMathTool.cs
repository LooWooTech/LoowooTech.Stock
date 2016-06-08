using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 字段
    /// </summary>
    public class ValueMathTool:ITool
    {
        /// <summary>
        /// 表
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 检查字段
        /// </summary>
        public string CheckFieldName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 规则ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 匹配正则表达式
        /// </summary>
        public string RegexString { get; set; }
        public List<string> Messages { get; set; }
        public string Name
        {
            get
            {
                return string.Format("规则{0}:表‘{1}’中字段‘{2}’的值格式‘{3}’",ID,TableName,CheckFieldName,RegexString);
            }
        }

        public bool Check(OleDbConnection connection)
        {
            if (connection != null)
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("Select {0},{1} from {2}", CheckFieldName, Key, TableName);
                    using (var reader = command.ExecuteReader())
                    {
                        Messages = new List<string>();
                        var str = string.Empty;
                        while (reader.Read())
                        {
                            str = reader[0].ToString();
                            if (Regex.IsMatch(str, RegexString))
                            {
                                continue;
                            }
                            else
                            {
                                Messages.Add(string.Format("{0}对应的值不正确，请核对", reader[1].ToString()));
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
