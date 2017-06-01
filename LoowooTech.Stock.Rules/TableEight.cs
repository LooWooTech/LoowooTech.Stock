using LoowooTech.Stock.Common;
using LoowooTech.Stock.Tool;
using System.Linq;
using System.Data.OleDb;

namespace LoowooTech.Stock.Rules
{
    public class TableEight:TableBase,ITable
    {
        public override string Name
        {
            get
            {
                return "检查 表8 调查单元图斑要素基本属性结构表";
            }
        }
        public TableEight()
        {
            _tableName = "DCDYTB";
            _key = "BSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "08000(结构规则)" });
            list.Add(new ValueCountTool() { TableName = _tableName, ID = "08001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = _key, ID = "08002(填写规则)",Code="3302" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010100" }, ID = "08003(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "TBBH",WhereFieldName="XZCDM", ID = "08004(填写规则)" ,Code=""});
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "DCDYLX", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "08005(填写规则)" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "08006(逻辑规则)",Code="3301" });
        }

        public override void Check(OleDbConnection connection)
        {
            base.Check(connection);
            DCDYTBManager.Init(connection);//获取图斑编号、调查单元类型
        }
    }
}
