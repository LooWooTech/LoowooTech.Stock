using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class GainAreaTool
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 面积字段
        /// </summary>
        public string[] AreaFields { get; set; }
        public List<TB> List { get; set; }

        public void Gain(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, string.Format("select XZCDM,XZCMC,TBBH,{0} from {1}", string.Join(",", AreaFields), TableName));
            if (reader != null)
            {
                List = new List<TB>();
                while (reader.Read())
                {
                    var val = new TB
                    {
                        XZCDM = reader[0].ToString(),
                        XZCMC = reader[1].ToString(),
                        TBBH = reader[2].ToString()
                    };
                    var sum = .0;
                    for (var i = 3; i < AreaFields.Length + 3; i++)
                    {
                        var a = .0;
                        if (double.TryParse(reader[i].ToString(), out a))
                        {
                            sum += a;
                        }
                    }
                    val.MJ = sum;
                    List.Add(val);
                }
                if (List.Count > 0)
                {
                    DCDYTBManager.AddTB(TableName, List);
                }
            }
        }
    }
}
