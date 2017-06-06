using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public  class RuleManager
    {
        private static List<Rule> _list { get; set; }
        /// <summary>
        /// 所有的质检规则
        /// </summary>
        public static List<Rule> List { get { return _list == null ? _list = GetRules() : _list; } }
        private static List<Rule> GetRules()
        {
            var nodes = XmlManager.GetList("/Tables/Rules/Category", XmlEnum.Field);
            if (nodes == null || nodes.Count == 0)
            {
                return null;
            }
            var list = new List<Rule>();
            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var category = node.Attributes["Name"].Value;
                if (string.IsNullOrEmpty(category))
                {
                    continue;
                }
                var children = node.SelectNodes("/Rule");
                if (children == null || children.Count == 0)
                {
                    continue;
                }
                for (var j = 0; j < children.Count; j++)
                {
                    var entry = children[j];
                    list.Add(new Rule
                    {
                        ID = entry.Attributes["ID"].Value,
                        Title = entry.Attributes["Title"].Value,
                        Category = category
                    });
                }

            }

            return list;
        }
        private  string[] _ids { get; set; }
        /// <summary>
        /// 当前需要质检的规则ID
        /// </summary>
        public  string[] IDS { get { return _ids; } set { _ids = value; } }

        private  List<ITool> _tools { get; set; }
        public  RuleManager()
        {
            var _key = "BSM";
            #region 行政区（乡镇）要素基本属性结构表
            _tools.Add(new FieldStructureTool { TableName = "XZQ_XZ", ID = "05000" });
            _tools.Add(new ValueCountTool() { TableName = "XZQ_XZ", ID = "05001" });
            _tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZ", CheckFieldName = _key, ID = "05002", Code = "3302" });
            _tools.Add(new ValueRangeTool() { TableName = "XZQ_XZ", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600100" }, ID = "05003" });
            _tools.Add(new ValueMathTool() { TableName = "XZQ_XZ", CheckFieldName = "XZQDM", Key = _key, RegexString = "33[0-9]{7}", ID = "05004" });
            _tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZ", CheckFieldName = "XZQDM", ID = "05005", Code = "3301" });
            _tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZ", CheckFieldName = "XZQMC", ID = "05006", Code = "3301" });
            _tools.Add(new ValueCurrectTool() { TableName = "XZQ_XZ", Fields = new string[] { "XZQDM", "XZQMC" }, Split = "/", Values = ExcelManager.XZQ.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "05007", Code = "3301" });
            #endregion

            #region  行政区（村级）要素基本属性结构表
            _tools.Add(new FieldStructureTool() { TableName = "XZQ_XZC", ID = "06000" });
            _tools.Add(new ValueCountTool() { TableName = "XZQ_XZC", ID = "06001" });
            _tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZC", CheckFieldName = _key, ID = "06002", Code = "3302" });
            _tools.Add(new ValueRangeTool() { TableName = "XZQ_XZC", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600100" }, ID = "06003" });
            _tools.Add(new ValueMathTool() { TableName = "XZQ_XZC", CheckFieldName = "XZQDM", Key = _key, RegexString = "33[0-9]{10}", ID = "06004" });
            _tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZC", CheckFieldName = "XZQDM", ID = "06005", Code = "3301" });
            _tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZC", CheckFieldName = "XZQMC", ID = "06006", Code = "3301" });
            _tools.Add(new ValueCurrectTool() { TableName = "XZQ_XZC", Fields = new string[] { "XZQDM", "XZQMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "06007", Code = "3301" });
            #endregion

            #region 行政区界线要素基本属性结构表
            _tools.Add(new FieldStructureTool() { TableName = "XZQJX", ID = "07000" });
            _tools.Add(new ValueCountTool() { TableName = "XZQJX", ID = "07001" });
            _tools.Add(new ValueUniqueTool() { TableName = "XZQJX", CheckFieldName = _key, ID = "07002", Code = "3302" });
            _tools.Add(new ValueRangeTool() { TableName = "XZQJX", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600200", "1000600220", "1000600230", "1000600240", "1000600250", "1000600260" }, ID = "07003" });
            _tools.Add(new ValueRangeTool() { TableName = "XZQJX", CheckFieldName = "JXLX", Key = _key, Values = new string[] { "630200", "640200", "650200", "660200", "670500" }, ID = "07004" });
            _tools.Add(new ValueRangeTool() { TableName = "XZQJX", CheckFieldName = "JXXZ", Key = _key, Values = new string[] { "600001", "600002", "600003", "600004", "600009" }, ID = "07005" });
            #endregion


            #region 调查单元图斑要素基本属性结构表
            _tools.Add(new FieldStructureTool() { TableName = "DCDYTB", ID = "08000" });
            _tools.Add(new ValueCountTool() { TableName = "DCDYTB", ID = "08001" });
            _tools.Add(new ValueUniqueTool() { TableName = "DCDYTB", CheckFieldName = _key, ID = "08002", Code = "3302" });
            _tools.Add(new ValueRangeTool() { TableName = "DCDYTB", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010100" }, ID = "08003" });
            _tools.Add(new ValueUniqueTool() { TableName = "DCDYTB", CheckFieldName = "TBBH", WhereFieldName = "XZCDM", ID = "08004", Code = "" });
            _tools.Add(new ValueRangeTool() { TableName = "DCDYTB", CheckFieldName = "DCDYLX", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "08005" });
            _tools.Add(new ValueCurrectTool() { TableName = "DCDYTB", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "08006", Code = "3301" });
            #endregion


            #region 农村存量建设用地总体情况基本属性结构表
            _tools.Add(new FieldStructureTool() { TableName = "NCCLJSYDZTQK", ID = "09000" });
            _tools.Add(new ValueCountTool() { TableName = "NCCLJSYDZTQK", ID = "09001" });
            _tools.Add(new ValueUniqueTool() { TableName = "NCCLJSYDZTQK", CheckFieldName = _key, ID = "09002", Code = "3302" });
            _tools.Add(new ValueRangeTool() { TableName = "NCCLJSYDZTQK", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010200" }, ID = "09003" });
            _tools.Add(new ValueCurrectTool() { TableName = "NCCLJSYDZTQK", Fields = new string[] { "XZCDM", "XZCMC", "SSXZMC" }, Split = "/", Values = ExcelManager.XZDC.Select(e => string.Format("{0}/{1}/{2}", e.XZQDM, e.XZQMC, e.XZQ.XZQMC)).ToList(), ID = "09004" });
            _tools.Add(new ValueCompareTool() { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "XZCZYDMJ" }, FieldArray2 = new string[] { "DJCZYDMJ" }, Compare = Compare.Above, ID = "09005" });
            _tools.Add(new ValueCompareTool() { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "XZCZYDMJ" }, FieldArray2 = new string[] { "QQCZYDMJ" }, Compare = Compare.Above, ID = "09006" });
            _tools.Add(new ValueCompareTool() { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "RKZS", "LCRK" }, FieldArray2 = new string[] { "HJRK", "LRRK" }, Compare = Compare.Equal, ID = "09007" });
            #endregion

            #region 存量宅基地要素基本属性结构表

            _tools.Add(new FieldStructureTool() { TableName = "CLZJD", ID = "10000" });
            _tools.Add(new ValueCountTool() { TableName = "CLZJD", ID = "10001" });
            _tools.Add(new ValueUniqueTool() { TableName = "CLZJD", CheckFieldName = _key, ID = "10002", Code = "3302" });
            _tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010300" }, ID = "10003" });
            _tools.Add(new ValueCurrectTool() { TableName = "CLZJD", Fields = new string[] { "XZCDM", "XZCMC" }, Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), Split = "/", ID = "10004" });
            _tools.Add(new ValueUniqueTool() { TableName = "CLZJD", CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "10005", Code = "3201" });
            _tools.Add(new ValueCurrectTool() { TableName = "CLZJD", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "10006" });
            _tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "10007" });
            _tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "FKDYJ", Key = _key, Values = new string[] { "1", "2", "" }, ID = "10008" });
            _tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "10009" });
            _tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "KZYY", Key = _key, Values = new string[] { "1", "2", "3", "4", "5", "" }, ID = "10010" });
            _tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "FQYY", Key = _key, Values = new string[] { "1", "2", "3", "4", "" }, ID = "10011" });
            _tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "KZYY", "FQYY", "QTYY" }, Key = _key, WhereCaluse = "LYZT='1'", Is_Nullable = true, ID = "10012" });//为空
            _tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", Is_Nullable = false, ID = "10013" });
            _tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='2'", Is_Nullable = true, ID = "10014" });
            _tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LJZT='3'", Is_Nullable = false, ID = "10015" });
            _tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LJZT='3'", Is_Nullable = true, ID = "10016" });
            _tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "QTYY" }, Key = _key, WhereCaluse = "KZYY='5'||FQYY='4'", Is_Nullable = false, ID = "10017" });
            _tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "SFWFKD", Key = _key, Values = new string[] { "是", "否" }, WhereFields = new string[] { "XZCDM", "TBBH" }, Split = "/", WhereList = DCDYTBManager.List.Where(e => e.DCDYLX == "1").Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), ID = "10018" });
            #endregion

            #region 农村存量经营性建设用地利用现状调查表基本属性结构表
            _tools.Add(new FieldStructureTool() { TableName = "JYXJSYD", ID = "11000" });
            _tools.Add(new ValueCountTool() { TableName = "JYXJSYD", ID = "11001" });
            _tools.Add(new ValueUniqueTool() { TableName = "JYXJSYD", CheckFieldName = _key, ID = "11002", Code = "3302" });
            _tools.Add(new ValueRangeTool() { TableName = "JYXJSYD", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010400" }, ID = "11003" });
            _tools.Add(new ValueCurrectTool() { TableName = "JYXJSYD", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "11004" });
            _tools.Add(new ValueUniqueTool() { TableName = "JYXJSYD", CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "11005", Code = "3201" });
            _tools.Add(new ValueCurrectTool() { TableName = "JYXJSYD", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "11006" });
            _tools.Add(new ValueRangeTool() { TableName = "JYXJSYD", CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "11007" });
            _tools.Add(new ValueRangeTool() { TableName = "JYXJSYD", CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "11008" });
            _tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "TDYT", "NSSE", "CYRS" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "11009", Is_Nullable = false });
            _tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "KZYY", "FQYY" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "11010", Is_Nullable = true });
            _tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "11011", Is_Nullable = false });
            _tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "FQYY", "TDYT", "NSSE", "CYRS" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "11012", Is_Nullable = true });
            _tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "11013", Is_Nullable = false });
            _tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "KZYY", "TDYT", "NSSE", "CYRS" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "11014", Is_Nullable = true });
            #endregion


            #region 农村存量公共管理及公共服务设施用地利用现状调查要素基本属性结构表
            _tools.Add(new FieldStructureTool() { TableName = "GGGL_GGFWSSYD", ID = "12000" });
            _tools.Add(new ValueCountTool() { TableName = "GGGL_GGFWSSYD", ID = "12001" });
            _tools.Add(new ValueUniqueTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = _key, ID = "12002", Code = "3302" });
            _tools.Add(new ValueRangeTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010500" }, ID = "12003" });
            _tools.Add(new ValueCurrectTool() { TableName = "GGGL_GGFWSSYD", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "12004" });
            _tools.Add(new ValueUniqueTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "12005", Code = "3201" });
            _tools.Add(new ValueCurrectTool() { TableName = "GGGL_GGFWSSYD", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "12006" });
            _tools.Add(new ValueRangeTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "12007" });
            _tools.Add(new ValueRangeTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "12008" });
            _tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "TDYT", "FWRS", "SYMJ" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "12009", Is_Nullable = false });
            _tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "KZYY", "FQYY" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "12010", Is_Nullable = true });
            _tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "12011", Is_Nullable = false });
            _tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "FQYY", "TDYT", "FWRS", "SYMJ" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "12012", Is_Nullable = true });
            _tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "12013", Is_Nullable = false });
            _tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "KZYY", "TDYT", "FWRS", "SYMJ" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "12014", Is_Nullable = true });
            #endregion

            #region 农村其他存量建设用地利用现状调查表基本属性结构表
            _tools.Add(new FieldStructureTool() { TableName = "QTCLJSYD", ID = "13000" });
            _tools.Add(new ValueCountTool() { TableName = "QTCLJSYD", ID = "13001" });
            _tools.Add(new ValueUniqueTool() { TableName = "QTCLJSYD", CheckFieldName = _key, ID = "13002", Code = "3302" });
            _tools.Add(new ValueRangeTool() { TableName = "QTCLJSYD", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010600" }, ID = "13003" });
            _tools.Add(new ValueCurrectTool() { TableName = "QTCLJSYD", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "13004" });
            _tools.Add(new ValueUniqueTool() { TableName = "QTCLJSYD", CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "13005", Code = "3201" });
            _tools.Add(new ValueCurrectTool() { TableName = "QTCLJSYD", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "13006" });
            _tools.Add(new ValueRangeTool() { TableName = "QTCLJSYD", CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "13007" });
            _tools.Add(new ValueRangeTool() { TableName = "QTCLJSYD", CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "13008" });
            _tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "YXSYMJ" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "13009", Is_Nullable = false });
            _tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "KZYY", "FQYY" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "13010", Is_Nullable = true });
            _tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "13011", Is_Nullable = false });
            _tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "13012", Is_Nullable = true });
            _tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "13013", Is_Nullable = false });
            _tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "13014", Is_Nullable = true });
            #endregion
        }

        public void Program(OleDbConnection connection)
        {
            var checkTools = new List<ITool>();
            foreach(var id in IDS)
            {
                var tool = _tools.FirstOrDefault(e => e.ID == id);
                if (tool != null)
                {
                    checkTools.Add(tool);
                }
            }
            Parallel.ForEach(checkTools, tool =>
            {
                tool.Check(connection);
            });
            //var dict = checkTools.GroupBy(e => e.TableName).ToDictionary(e => e.Key, e => e.ToList());
            //Parallel.ForEach(dict, entry =>
            //{
            //    foreach(var tool in entry.Value)
            //    {
            //        tool.Check(connection);
            //    }
            //});
        }
    }
}
