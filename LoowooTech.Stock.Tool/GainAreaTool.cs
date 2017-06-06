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
        /// <summary>
        /// 分母
        /// </summary>
        public double Denominator { get; set; }

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
                            sum += a/Denominator;
                        }
                    }
                    val.MJ = sum;
                    List.Add(val);
                }
                if (List.Count == 0)
                {
                    var info = string.Format("获取表【{0}】中的图斑面积时，图斑面积数据量为空", TableName);
                    LogManager.Log(info);
                    QuestionManager.Add(new Question { Code = "3201", Name = "检验图斑面积", TableName = TableName, Description = info });
                }
                else
                {
                    DCDYTBManager.AddTB(List);
                }
            }
        }
    }
}
