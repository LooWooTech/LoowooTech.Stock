using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 作用：指定某几个字段，查询是否符合值
    /// 作者：汪建龙
    /// 编写时间：2017年5月4日13:55:52
    /// </summary>
    public class ValueCurrectTool:ITool
    {
        public string TableName { get; set; }
        public string ID { get; set; }
        public List<string> Messages { get; set; }
        public string[] Fields { get; set; }
        public string Split { get; set; }
        public List<string> Values { get; set; }
        public string Name
        {
            get
            {

                return string.Format("规则{0}：表‘{1}’中字段‘{2}’组成的值相互对应");
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
                    command.CommandText = string.Format("Select {0} from {1}",string.Join(",",Fields), TableName);
                    using (var reader = command.ExecuteReader())
                    {
                        var array = new string[Fields.Length];
                        while (reader.Read())
                        {
                            array = new string[Fields.Length];
                            for(var i = 0; i < Fields.Length; i++)
                            {
                                array[i] = Fields[i].ToString();
                            }
                            var val = string.Join(Split, array);
                            if (!Values.Contains(val))
                            {
                                Messages.Add(string.Format("不存在：{0}", val));
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
