using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 字段的值 只能填哪些值
    /// </summary>
    public class ValueRangeTool:ITool
    {
        /// <summary>
        /// 表
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 检查结果信息
        /// </summary>
        public List<string> Messages { get; set; }
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
        /// 规则ID
        /// </summary>
        public string ID { get; set; }
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
            if (connection != null)
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("select {0},{1} from {2}", CheckFieldName, Key, TableName);
                    using (var reader = command.ExecuteReader())
                    {
                        var str = string.Empty;
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
                                Messages.Add(string.Format("{0}对应的‘{1}’值不正确", reader[1].ToString(), str));
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
