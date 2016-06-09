using System.Data.OleDb;
using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class TableFive:TableBase
    {
        public TableFive()
        {
            _tableName = "XZQ";
            _key = "BSM";
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600100" }, ID = "05001(填写规则)" });
            list.Add(new ValueMathTool() { TableName = _tableName, CheckFieldName = "XZQDM", Key = _key, RegexString = "33[0-9]{10}", ID = "05002(填写规则)" });
        }
        public void Check(OleDbConnection connection)
        {
           
        }
    }
}
