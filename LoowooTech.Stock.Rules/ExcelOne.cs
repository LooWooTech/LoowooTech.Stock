using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelOne:ExcelBase,IExcel
    {
        public ExcelOne()
        {
            ExcelName = "表1";
            Space = 0;
        }
        public override void Check(OleDbConnection connection)
        {
            var info = string.Empty;

            base.Check(connection);
            if (Dict == null || Dict.Count == 0)
            {
                info = string.Format("未读取到表：{0}中的统计数据", TableName);
                Add(new Models.Question { Code = "6101", Name = Name, TableName = ExcelName, Description = info });
                return;
            }
           


        }
    }
}
