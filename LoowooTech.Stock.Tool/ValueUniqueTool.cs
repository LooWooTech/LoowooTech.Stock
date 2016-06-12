using System.Collections.Generic;
using System.Data.OleDb;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 检查字段值  唯一
    /// </summary>
    public class ValueUniqueTool:ITool
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
        /// 规则ID
        /// </summary>
        public string ID { get; set; }
        public List<string> Messages { get; set; }

        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name
        {
            get
            {
                return string.Format("规则{0}:表‘{1}’中字段‘{2}’的值为唯一值", ID, TableName, CheckFieldName);
            }
        }

        public bool Check(OleDbConnection connection)
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
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("Select {0} from {1}", CheckFieldName, TableName);
                    using (var reader = command.ExecuteReader())
                    {
                        Messages = new List<string>();
                        var temp = new List<string>();
                        var str = string.Empty;
                        while (reader.Read())
                        {
                            str = reader[0].ToString();
                            if (temp.Contains(str))
                            {
                                Messages.Add(string.Format("值：{0}  存在重复", str));
                            }
                            else
                            {
                                temp.Add(str);
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
