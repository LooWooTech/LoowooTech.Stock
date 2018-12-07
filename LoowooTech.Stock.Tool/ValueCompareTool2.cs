using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueCompareTool2:ValueBaseTool2,IVTool
    {
        public string Name
        {
            get
            {
                var sb = new StringBuilder(string.Format("规则{0}：表【{1}】中字段【{2}】等于字段【{3}];", ID, TableName, string.Join(",", CheckLeftFieldNames), string.Join(",", CheckRightFieldNames)));
                if (Relative.HasValue)
                {
                    sb.AppendFormat("容差相对值【{0}%】;", Relative * 100);
                }
                if (Absolute.HasValue)
                {
                    sb.AppendFormat("容差绝对值【{0}】；", Absolute);
                }
                sb.AppendFormat("保留{0}位小数位", Decimals);
                return sb.ToString();
            }
        }

        public string[] CheckLeftFieldNames { get; set; }
        public string[] CheckRightFieldNames { get; set; }
        /// <summary>
        /// 相对值  百分比
        /// </summary>
        public double? Relative { get; set; }
        /// <summary>
        /// 绝对值 具体数值
        /// </summary>
        public double? Absolute { get; set; }
        public int Decimals { get; set; }

        private string _SQL
        {
            get
            {
                return string.Format("SELECT {0},{1},{2} FROM {3}", Key, string.Join(",", CheckLeftFieldNames), string.Join(",", CheckRightFieldNames), TableName);
            }
        }

        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, _SQL);
            if (reader == null)
            {
                return false;
            }

            while (reader.Read())
            {
                var keyValue = reader[0].ToString().Trim();
                if (string.IsNullOrEmpty(keyValue) == false)
                {
                    var leftValue = GetValue(reader, 1, 1 + CheckLeftFieldNames.Length);
                    var rightValue = GetValue(reader, 1 + CheckLeftFieldNames.Length, 1 + CheckLeftFieldNames.Length + CheckRightFieldNames.Length);
                    var abs = Math.Abs(leftValue - rightValue);
                    var flag = false;
                    if (Relative.HasValue)
                    {
                        var pp = abs / leftValue;
                        flag = pp < Relative.Value;
                    }
                    if (Absolute.HasValue)
                    {
                        flag = abs < Absolute.Value;
                    }
                }
            }
            return true;
        }

        private double GetValue(OleDbDataReader reader,int start,int end)
        {
            double sum = .0;
            var a = .0;
            for(var i = start; i < end; i++)
            {
                if(double.TryParse(reader[i].ToString().Trim(),out a))
                {
                    sum += Math.Round(a, Decimals);
                }
            }
            return sum;
        }
    }
}
