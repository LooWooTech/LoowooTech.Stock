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
            #region  1、要素代码
            _tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2003010100" }, ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueRangeTool2 { TableName = "GHYT", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2003020710" }, ID = "", RelationName = "GHYT" });
            _tools.Add(new ValueRangeTool2 { TableName = "TDGHDL", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2003020210" }, ID = "", RelationName = "TDGHDL" });
            _tools.Add(new ValueRangeTool2 { TableName = "JSYDGZQ", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2003020420" }, ID = "", RelationName = "JSYDGZQ" });
            #endregion

            #region 2、权属性质
            _tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", CheckFieldName = "QSXZ", Key = _key, Values = ParameterManager2.QSXZ.Select(e => e.Key).ToArray(), ID = "", RelationName = "JQDLTB" });
            _tools.Add(new ValueRangeTool2 { TableName = "TDGHDL", CheckFieldName = "QSXZ", Key = _key, Values = ParameterManager2.QSXZ.Select(e => e.Key).ToArray(), ID = "", RelationName = "TDGHDL" });
            #endregion

            #region 3、坡度级别
            _tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", CheckFieldName = "PDJB", Key = _key, Values = ParameterManager2.PDJB.Select(e => e.Key).ToArray(), ID = "", RelationName = "JQDLTB" });
            #endregion

            #region 4、地类备注
            _tools.Add(new ValueNullTool2 { TableName = "JQDLTB", CheckFields = new string[] { "PZWH" }, Key = _key, WhereClause = "DLBZ = '10'", Is_Nullable = false, ID = "", RelationName = "JQDLTB" });
            #endregion

            #region  5、规划用途中规划用途为基本农田时 基本农田类型必须填写
            _tools.Add(new ValueRangeTool2 { TableName = "GHYT", CheckFieldName = "JNLx", Key = _key, WhereClause="GHYTDM = 'G111' OR GHYTDM = 'G112'", Values = new string[] { "1", "2" }, ID = "", RelationName = "GHYT" });
            #endregion

            #region 6、规划用户中为基本农田并且基本农田类型为2时，批准必填 并且是"浙"开头
            _tools.Add(new ValueNullTool2 { TableName = "GHYT", CheckFields = new string[] { "PZWH" }, Key = _key, WhereClause = "(GHYTDM = 'G111' OR GHYTDM = 'G112') AND JZLX = '2'", Is_Nullable = false, RegexString="^浙*$", ID = "", RelationName = "GHYT" });
            #endregion

            #region 7、规划用途编码  当规划用代码为G2*、N11*、 G11*的时候 需要填写规划用途编码
            _tools.Add(new ValueRangeTool2 { TableName = "GHYT", CheckFieldName = "GHYTBM", Key = _key, Values = ParameterManager2.SeconGHYTs.Where(e => !string.IsNullOrEmpty(e.BM)).Select(e => e.BM).ToArray(),WhereClause="GHYTDM LIKE 'G11*' OR GHYTDM LIKE 'N11*' OR GHYTDM LIEK 'G2*'" ID = "", RelationName = "GHYT" });
            #endregion
            #region 8、用途地块编号

            #endregion

            #region 9.规划用途为新增一般农田时，新增农田类型需填写 01 02 03 04
            _tools.Add(new ValueRangeTool2 { TableName = "GHYT", CheckFieldName = "XZNTLX", Key = _key, Values = new string[] { "01", "02", "03", "04" },WhereClause="GHYTDM = 'N112'", ID = "", RelationName = "GHYT" });
            #endregion

            #region 10.建设用地管制区类型代码
            _tools.Add(new ValueRangeTool2 { TableName = "JSYDGZQ", CheckFieldName = "GZQLXDM", Key = _key, Values = ParameterManager2.JSYDGZQ.Select(e => e.Key).ToArray(), ID = "", RelationName = "JSYDGZQ" });
            #endregion
        }
    }
}
