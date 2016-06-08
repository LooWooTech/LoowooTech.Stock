using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueNullTool:ITool
    {
        /// <summary>
        /// 表
        /// </summary>
        public string TableName { get; set; }
        
        public string[] CheckFields { get; set; }
        public string Key { get; set; }
        public string WhereCaluse { get; set; }
        public string ID { get; set; }
        public bool Is_Nullable { get; set; }//true  为空  false  必填
        public List<string> Messages { get; set; }
        
        public string Name
        {
            get
            {
                var sb = new StringBuilder(string.Format("规则{0}:表‘{1}’ 当‘{2}’的时候，‘{3}’", ID, TableName, WhereCaluse, CheckFields[0]));
                for(var i = 1; i < CheckFields.Count(); i++)
                {
                    sb.AppendFormat("、‘{0}’", CheckFields[i]);
                }
                sb.Append(Is_Nullable ? "为空" : "必填");
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
                    var sb = new StringBuilder(CheckFields[0]);
                    for(var i = 1; i < CheckFields.Count(); i++)
                    {
                        sb.AppendFormat(",{0}", CheckFields[i]);
                    }
                    command.CommandText = string.Format("Select {0},{1} from {2} where {3}", sb.ToString(), Key, TableName, WhereCaluse);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for(var i = 0; i < CheckFields.Count(); i++)
                            {
                                if (Is_Nullable^string.IsNullOrEmpty(reader[i].ToString()))
                                {

                                }
                                else
                                {
                                   
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
