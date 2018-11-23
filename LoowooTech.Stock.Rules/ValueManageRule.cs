using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ValueManageRule:ValueBaseRule2
    {
        public override string RuleName { get { return "值符合性"; } }
        public override string ID { get { return "1302"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.值符合性; } }
        public override void InitTool()
        {
            #region  要素代码
            _tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2003010100" }, ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueRangeTool2 { TableName = "GHYT", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2003020710" }, ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool2 { TableName = "TDGHDL", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2003020210" }, ID = "", RelationName = "TDGHDL" });
            _tools.Add(new ValueRangeTool2 { TableName = "JSYDGZQ", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2003020420" }, ID = "", RelationName = "JSYDGZQ" });
            #endregion

            #region 权属性质
            _tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", CheckFieldName = "QSXZ", Key = _key, Values = ParameterManager2.QSXZ.Select(e => e.Key).ToArray(), ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueRangeTool2 { TableName = "TDGHDL", CheckFieldName = "QSXZ", Key = _key, Values = ParameterManager2.QSXZ.Select(e => e.Key).ToArray(), ID = "", RelationName = "TDGHDL" });
            #endregion

            #region 坡度级别
            _tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", CheckFieldName = "PDJB", Key = _key, Values = ParameterManager2.PDJB.Select(e => e.Key).ToArray(), ID = "", RelationName = "JQDLTB" });
            #endregion

            #region 地类备注
            _tools.Add(new ValueNullTool { TableName = "JQDLTB", CheckFields = new string[] { "PZWH" }, Key = _key, WhereCaluse = "DLBZ = '10'", Is_Nullable = false, ID = "", RelationName = "JQDLTB" });
            #endregion



        }
    }
}
