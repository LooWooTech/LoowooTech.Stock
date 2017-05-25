using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueFunctionTool:ITool
    {
        public string TableName { get; set; }
        public string Name
        {
            get { return string.Format("规则{0}：核对表{1}应满足{2}",ID,TableName,Functions); }
        }
        public string ID { get; set; }
        public List<string> Messages { get; set; }
        public string Functions { get; set; }
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
                    command.CommandText = string.Format("Select {0}");
                }
            }
            return false;
        }
    }
}
