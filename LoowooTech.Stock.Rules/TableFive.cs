using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class TableFive
    {
        public TableFive()
        {
            var list = new List<ITool>();
            list.Add(new ValueRangeTool() { TableName = "XZQ", CheckFieldName = "YSDM", Key = "BSM", Values = new string[] { "1000600100" }, ID = "5001(填写规则)" });
            list.Add(new ValueMathTool() { TableName = "XZQ", CheckFieldName = "XZQDM", Key = "BSM", RegexString = "33[0-9]{10}", ID = "5002(填写规则)" });
        }
        public void Check(OleDbConnection connection)
        {
           
        }
    }
}
