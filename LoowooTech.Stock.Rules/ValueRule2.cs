using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ValueRule2:IRule
    {
        public string RuleName { get { return "值符合性"; } }
        public string ID { get { return "1302"; } }
        public bool Space { get { return false; } }

        private List<ITool> _tools { get; set; } = new List<ITool>();
        private string _key { get; set; } = "BSM";

        private void InitTool()
        {
            #region 基期地类图斑要素属性结构表
            _tools.Add(new ValueCountTool { TableName = "JQDLTB" });

            _tools.Add(new ValueUniqueTool { TableName = "JQDLTB", CheckFieldName = "TBBH", WhereFieldName = "XZQDM", ID = "", Code = "" });
            _tools.Add(new ValueRangeTool { TableName = "JQDLTB", CheckFieldName = "XZQDM", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCDM).ToArray(), ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueRangeTool { TableName = "JQDLTB", CheckFieldName = "DLMC", Key = _key, Values = ParameterManager2.GHYTDL.Select(e => e.Value).ToArray(), ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueRangeTool { TableName = "JQDLTB", CheckFieldName = "DLBZ", Key = _key, Values = ParameterManager2.DLBZ.Select(e => e.Key).ToArray(), ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueRangeTool { TableName = "JQDLTB", CheckFieldName = "XZQMC", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCMC).ToArray(), ID = "", RelationName = "JQDLTB" });
           
           
           
            _tools.Add(new ValueRangeTool { TableName = "JQDLTB", CheckFieldName = "ZHLX", Key = _key, Values = ParameterManager2.JSZHLX.Select(e => e.Key).ToArray(), ID = "", RelationName = "JQDLTB" });


            #endregion

            #region 规划用途要素属性结构表

            _tools.Add(new ValueCountTool { TableName = "GHYT" });
       
            _tools.Add(new ValueRangeTool { TableName = "GHYT", CheckFieldName = "XZQDM", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCDM).ToArray(), ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool { TableName = "GHYT", CheckFieldName = "XZQMC", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCMC).ToArray(), ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool { TableName = "GHYT", CheckFieldName = "GHYTDM", Key = _key, Values = ParameterManager2.SeconGHYTs.Select(e => e.Code).ToArray(), ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool { TableName = "GHYT", CheckFieldName = "GHYTBM", Key = _key, Values = ParameterManager2.SeconGHYTs.Where(e => !string.IsNullOrEmpty(e.BM)).Select(e => e.BM).ToArray(), ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool { TableName = "GHYT", CheckFieldName = "GHYTMC", Key = _key, Values = ParameterManager2.SeconGHYTs.Select(e => e.Name).ToArray(), ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool { TableName = "GHYT", CheckFieldName = "JNLZ", Key = _key, Values = new string[] { "1", "2" }, ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool { TableName = "GHYT", CheckFieldName = "XZNTLX", Key = _key, Values = new string[] { "01", "02", "03", "04" }, ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool { TableName = "GHYT", CheckFieldName = "CJJX", Key = _key, Values = new string[] { "C1", "C2", "E1", "E2" }, ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool { TableName = "GHYT", CheckFieldName = "JBNTZBQ", Key = _key, Values = new string[] { "ZB" } });//需要添加条件
            #endregion

            #region 土地规划地类要素属性结构表

            _tools.Add(new ValueCountTool { TableName = "TDGHDL" });

            _tools.Add(new ValueRangeTool { TableName = "TDGHDL", CheckFieldName = "GHDLMC", Key = _key, Values = ParameterManager2.GHYTDL.Select(e => e.Value).ToArray(), ID = "", RelationName = "TDGHDL" });
            _tools.Add(new ValueRangeTool { TableName = "TDGHDL", CheckFieldName = "XZQDM", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCDM).ToArray(), ID = "", RelationName = "TDGHDL" });
            _tools.Add(new ValueRangeTool { TableName = "TDGHDL", CheckFieldName = "GHDLDM", Key = _key, Values = ParameterManager2.GHYTDL.Select(e => e.Key).ToArray(), ID = "", RelationName = "TDGHDL" });
            _tools.Add(new ValueRangeTool { TableName = "TDGHDL", CheckFieldName = "XZQMC", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCMC).ToArray(), ID = "", RelationName = "TDGHDL" });


            #endregion

            #region 建设用地管制区属性结构描述表

            _tools.Add(new ValueRangeTool { TableName = "JSYDGZQ" });

            _tools.Add(new ValueRangeTool { TableName = "JSYDGZQ", CheckFieldName = "GZQLXDM",Key=_key,Values=ParameterManager2.JSYDGZQ.Select(e=>e.Key).ToArray(),ID="",RelationName="JSYDGZQ" });
            _tools.Add(new ValueRangeTool { TableName = "JSYDGZQ", CheckFieldName = "XZQDM", Key = _key, Values = ParameterManager2.XZCList.Select(e => e.XZCDM).ToArray(), ID = "", RelationName = "JSYDGZQ" });
            #endregion
        }
        public void Check()
        {
            InitTool();

            foreach(var tool in _tools)
            {

            }
        }
    }
}
