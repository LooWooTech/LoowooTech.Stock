using System.Data.OleDb;
using LoowooTech.Stock.Tool;
using LoowooTech.Stock.Common;
using System.Linq;

namespace LoowooTech.Stock.Rules
{
    public class TableFive:TableBase,ITable
    {
        
        public override string Name { get
            {
                return "检查表5 行政区(乡镇)要素基本属性结构表";
            } }
        public TableFive()
        {
            _tableName = "XZQ_XZ";
            _key = "BSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "05000(结构规则)" });
            list.Add(new ValueCountTool() { TableName = _tableName, ID = "05001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = _key, ID = "05002(填写规则)",Code="3302" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600100" }, ID = "05003(填写规则)" });
            list.Add(new ValueMathTool() { TableName = _tableName, CheckFieldName = "XZQDM", Key = _key, RegexString = "33[0-9]{7}", ID = "05004(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "XZQDM", ID = "05005(填写规则)",Code="3301" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "XZQMC", ID = "05006(填写规则)",Code="3301" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZQDM", "XZQMC" }, Split = "/", Values = ExcelManager.XZQ.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "05007(逻辑规则)",Code="3301" });
            
        }
    }
}
