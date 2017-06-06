using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 检查
    /// </summary>
    public class ValueCombinationTool:ITool
    {
        public string TableName { get; set; }
        public string CheckField { get; set; }
        public string[] Tables { get; set; }
        public List<string> Messages { get; set; }
        public string ID { get; set; }
        public string Name {
            get
            {
                var sb = new StringBuilder(string.Format("规则{0}:字段{1}在表{2}", ID,CheckField ,Tables[0]));
                for (var i = 1; i < Tables.Count(); i++)
                {
                    sb.AppendFormat("、{0}", Tables[i]);
                }
                sb.Append("中的值不能同时存在！");
                return sb.ToString();
            }
        }

        public bool Check(OleDbConnection connection)
        {
            var dict = Search(connection);
            foreach(var entry in dict)
            {
                Messages.Add(string.Format("{0}对应的值：{1} 同时存在表：{2}", CheckField, entry.Key, entry.Value.GetMessage));
            }
            return true;
        }

        private Dictionary<string,CombinationClass> Search(OleDbConnection connection)
        {
            var dict = new Dictionary<string, CombinationClass>();
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

                foreach(var table in Tables)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = string.Format("Select {0} from {1} group by {0}", CheckField, table);
                        using (var reader = command.ExecuteReader())
                        {
                            var str = string.Empty;
                            while (reader.Read())
                            {
                                str = reader[0].ToString().Trim();
                                if (dict.ContainsKey(str))
                                {
                                    dict[str].Tables.Add(table);
                                }
                                else
                                {
                                    dict.Add(str, new CombinationClass() { Tables = new List<string>() { str } });
                                }
                            }

                        }
                    }
                }
            }
            return dict.Where(e=>e.Value.Count>1).ToDictionary(e=>e.Key,e=>e.Value);
        }
    }
}
