using LoowooTech.Stock.Common;
using LoowooTech.Stock.Tool;
using System.Linq;
using System.Data.OleDb;

namespace LoowooTech.Stock.Rules
{
    public class TableTen:TableBase,ITable
    {
        public override string Name
        {
            get
            {
                return "检查 表10 存量宅基地要素基本属性结构表";
            }
        }
        public TableTen()
        {
            _tableName = "CLZJD";
            _key = "BSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "10000(结构规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010300" }, ID = "10001(填写规则)" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZCDM", "XZCMC" }, Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), Split = "/", ID = "10002(逻辑规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "10003(填写规则)" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "10004(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "10005(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "FKDYJ", Key = _key, Values = new string[] { "1", "2", "" }, ID = "10006(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "10007(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "KZYY", Key = _key, Values = new string[] { "1", "2", "3", "4", "5", "" }, ID = "10008(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "FQYY", Key = _key, Values = new string[] { "1", "2", "3", "4", "" }, ID = "10009(填写规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY", "FQYY", "QTYY" }, Key = _key, WhereCaluse = "LYZT='1'", Is_Nullable = true, ID = "10010(逻辑规则)" });//为空
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", Is_Nullable = false, ID = "10011(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='2'", Is_Nullable = true, ID = "10012(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LJZT='3'", Is_Nullable = false, ID = "10013(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LJZT='3'", Is_Nullable = true, ID = "10014(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "QTYY" }, Key = _key, WhereCaluse = "KZYY='5'||FQYY='4'", Is_Nullable = false, ID = "10015(逻辑规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "SFWFKD", Key = _key, Values = new string[] { "是", "否", "" }, ID = "10016(填写规则)" });
        }

        public override void Check(OleDbConnection connection)
        {
            base.Check(connection);
            var gain = new GainAreaTool() { TableName = _tableName, AreaFields = new string[] { "JZZDMJ", "FSYDMJ" } };
            gain.Gain(connection);

        }

    }
}
