using LoowooTech.Stock.Common;
using LoowooTech.Stock.Tool;
using System.Linq;

namespace LoowooTech.Stock.Rules
{
    public class TableSix:TableBase,ITable
    {
        public override string Name
        {
            get
            {
                return "检查 表6 行政区(村级)要素基本属性结构表";
            }
        }
        public TableSix()
        {
            _tableName = "XZQ_XZC";
            _key = "BSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "06000(结构规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600100" }, ID = "06001(填写规则)" });
            list.Add(new ValueMathTool() { TableName = _tableName, CheckFieldName = "XZQDM", Key = _key, RegexString = "33[0-9]{10}", ID = "06002(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "XZQDM", ID = "06003(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "XZQMC", ID = "06004(填写规则)" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZQDM", "XZQMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "06005(逻辑规则)" });
        }
    }
}
