using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class TableEight
    {
        public TableEight()
        {
            var list = new List<ITool>();
            list.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "YSDM", Key = "MBBSM", Values = new string[] { "2008010200" }, ID = "8001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = "CLZJD", CheckFieldName = "DCBH", ID = "8002(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = "CLZJD", CheckFieldName = "TBBH", ID = "8003(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "QSLY", Key = "MBBSM", Values = new string[] { "1", "2", "3", "4" }, ID = "8004(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "JZLX", Key = "MBBSM", Values = new string[] { "1", "2", "3", "4" }, ID = "8005(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "LYZT", Key = "MBBSM", Values = new string[] { "1", "2", "3" }, ID = "8006(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "NHLX", Key = "MBBSM", Values = new string[] { "1", "2", "3", "4" }, ID = "8007(填写规则)" });
        }
    }
}
