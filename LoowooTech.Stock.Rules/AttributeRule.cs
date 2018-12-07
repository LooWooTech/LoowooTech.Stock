using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class AttributeRule:ValueBaseRule2
    {
        public override string RuleName { get { return "属性正确性"; } }
        public override string ID { get { return "1303"; } }

        public override CheckProject2 CheckProject { get { return CheckProject2.属性正确性; } }

        public override void InitTool()
        {
            #region 行政区编码一致性
            _tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", CheckFieldName = "XZQDM", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCDM).ToArray(), ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueMathTool2 { TableName = "JQDLTB", CheckKeyFieldName = "XZQDM", CheckValueFieldName = "XZQMC", Key = _key, CurrentDict = ParameterManager2.XZCDict, ID = "", RelationName = "JQDLTB" });

            _tools.Add(new ValueRangeTool2 { TableName = "GHYT", CheckFieldName = "XZQDM", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCDM).ToArray(), ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueMathTool2 { TableName = "GHYT", CheckKeyFieldName = "XZQDM", CheckValueFieldName = "XZQMC", Key = _key, CurrentDict = ParameterManager2.XZCDict, ID = "", RelationName = "GHYT" });

            _tools.Add(new ValueRangeTool2 { TableName = "TDGHDL", CheckFieldName = "XZQDM", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCDM).ToArray(), ID = "", RelationName = "TDGHDL" });
            _tools.Add(new ValueMathTool2 { TableName = "TDGHDL", CheckKeyFieldName = "XZQDM", CheckValueFieldName = "XZQMC", Key = _key, CurrentDict = ParameterManager2.XZCDict, ID = "", RelationName = "TDGHDL" });

            _tools.Add(new ValueRangeTool2 { TableName = "JSYDGZQ", CheckFieldName = "XZQDM", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCDM).ToArray(), ID = "", RelationName = "JSYDGZQ" });
            #endregion


            #region 代码名称一致性检查
            _tools.Add(new ValueMathTool2 { TableName = "JQDLTB", CheckKeyFieldName = "DLDM", CheckValueFieldName = "DLMC", Key = _key, CurrentDict = ParameterManager2.GHYTDL, ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueMathTool2 { TableName = "TDGHDL", CheckKeyFieldName = "GHDLDM", CheckValueFieldName = "GHDLMC", Key = _key, CurrentDict = ParameterManager2.GHYTDL, ID = "", RelationName = "TDGHDL" });
            _tools.Add(new ValueMathTool2 { TableName = "GHYT", CheckKeyFieldName = "GHYTDM", CheckValueFieldName = "GHYTMC", Key = _key, CurrentDict = ParameterManager2.SeconGHYTs.ToDictionary(e => e.Code, e => e.Name), ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRegexTool { TableName = "GHYT", CheckFiledName = "GHYTDM", WhereClause = "CJJX = 'C1' OR CJJX = 'C2'", RegexStrings = new string[] { "^X2*$" }, ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRegexTool { TableName = "GHYT", CheckFiledName = "GHYTDM", WhereClause = "CJJX = 'E1' OR CJJX = 'E2'", RegexStrings = new string[] { "N111", "^X18$", "^X3*$" }, ID = "", RelationName = "GHYT" });

            #endregion

            #region 编号唯一性检查
            _tools.Add(new ValueUniqueTool2 { TableName = "JQDLTB", CheckFieldName = "TBBH", GroupByFieldName = "XZQDM", ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueUniqueTool2 { TableName = "TDGHDL", CheckFieldName = "TBBH", GroupByFieldName = "XZQDM", ID = "", RelationName = "TDGHDL" });
            #endregion

            #region 权属单位代码名称一致性检查



            #endregion
        }
    }
}
