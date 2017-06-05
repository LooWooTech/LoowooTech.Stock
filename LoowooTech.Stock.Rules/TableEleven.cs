using LoowooTech.Stock.Common;
using LoowooTech.Stock.Tool;
using System.Linq;
using System.Data.OleDb;

namespace LoowooTech.Stock.Rules
{
    public class TableEleven:TableBase,ITable
    {
        public override string Name
        {
            get
            {
                return "检查 表11 农村存量经营性建设用地利用现状调查表基本属性结构表";
            }
        }
        public TableEleven()
        {
            _tableName = "JYXJSYD";
            _key = "BSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "11000(结构规则)" });
            list.Add(new ValueCountTool() { TableName = _tableName, ID = "11001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = _key, ID = "11002(填写规则)", Code = "3302" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010400" }, ID = "11003(填写规则)" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "11004(逻辑规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "11005(填写规则)",Code="3201" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "11006(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "11007(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "11008(填写规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "TDYT", "NSSE", "CYRS" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "11009(逻辑规则)",Is_Nullable=false });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY", "FQYY" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "11010(逻辑规则)", Is_Nullable = true });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "11011(逻辑规则)", Is_Nullable = false });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY","TDYT", "NSSE", "CYRS" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "11012(逻辑规则)", Is_Nullable = true });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "11013(逻辑规则)", Is_Nullable = false });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY", "TDYT", "NSSE", "CYRS" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "11014(逻辑规则)", Is_Nullable = true });  
        }
        public override void Check(OleDbConnection connection)
        {
            base.Check(connection);
            var gain = new GainAreaTool() { TableName = _tableName, AreaFields = new string[] { "JSYDMJ" } };
            gain.Gain(connection);
        }

    }
}
