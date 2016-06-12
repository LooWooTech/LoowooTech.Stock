using System.Data.OleDb;
using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class TableFive:TableBase,ITable
    {
        
        public override string Name { get
            {
                return "检查表5 行政区要素基本属性结构表";
            } }
        public TableFive()
        {
            _tableName = "XZQ";
            _key = "BSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "05000(结构规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600100" }, ID = "05001(填写规则)" });
            list.Add(new ValueMathTool() { TableName = _tableName, CheckFieldName = "XZQDM", Key = _key, RegexString = "33[0-9]{10}", ID = "05002(填写规则)" });
        }
    }
}
