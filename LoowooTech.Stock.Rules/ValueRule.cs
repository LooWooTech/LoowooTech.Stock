using LoowooTech.Stock.ArcGISTool;
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
    /// <summary>
    /// 作用：核对数字那个字段的值是否符合标准
    /// </summary>
    public class ValueRule:ValueBaseRule, IRule
    {
        public override string RuleName
        {
            get
            {
                return "属性字段的值是否符合《浙江省农村存量建设用地调查数据库标准》规定的值域范围";
            }
        }
        public override string ID
        {
            get
            {
                return "3201";
            }
        }
        
        public override void Init()
        {
            #region 行政区（乡镇）要素基本属性结构表
            Tools.Add(new ValueCountTool() { TableName = "XZQ_XZ", ID = "05001" });       
            Tools.Add(new ValueRangeTool() { TableName = "XZQ_XZ", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600100" }, ID = "05003" });
            Tools.Add(new ValueMathTool() { TableName = "XZQ_XZ", CheckFieldName = "XZQDM", Key = _key, RegexString = "33[0-9]{7}", ID = "05004" });
            Tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZ", CheckFieldName = "XZQDM", ID = "05005", Code = "3301" });
            Tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZ", CheckFieldName = "XZQMC", ID = "05006", Code = "3301" });
            Tools.Add(new ValueCompareTool { TableName = "XZQ_XZ", Key = _key, FieldArray1 = new string[] { "BSM" }, Value = .0, Compare = Compare.Above, ID = "05007" });
            Tools.Add(new ValueNullTool { TableName = "XZQ_XZ", Key = _key, CheckFields = new string[] { "BSM", "YSDM", "XZQDM", "XZQMC" },Is_Nullable=false, ID = "05008" });

            #endregion

            #region  行政区（村级）要素基本属性结构表
            Tools.Add(new ValueCountTool() { TableName = "XZQ_XZC", ID = "06001" });
            Tools.Add(new ValueRangeTool() { TableName = "XZQ_XZC", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600100" }, ID = "06003" });
            Tools.Add(new ValueMathTool() { TableName = "XZQ_XZC", CheckFieldName = "XZCDM", Key = _key, RegexString = "33[0-9]{10}", ID = "06004" });
            Tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZC", CheckFieldName = "XZCDM", ID = "06005", Code = "3301" });
            //Tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZC", CheckFieldName = "XZCMC", ID = "06006", Code = "3301" });
            Tools.Add(new ValueCompareTool() { TableName = "XZQ_XZC", Key = _key, FieldArray1 = new string[] { "BSM" }, Value = .0, Compare = Compare.Above, ID = "06007" });
            Tools.Add(new ValueNullTool { TableName = "XZQ_XZC", Key = _key, CheckFields = new string[] { "BSM", "YSDM", "XZCDM", "XZCMC" }, Is_Nullable = false, ID = "06008" });

            #endregion

            #region 行政区界线要素基本属性结构表
            Tools.Add(new ValueCountTool() { TableName = "XZQJX", ID = "07001" });
            Tools.Add(new ValueRangeTool() { TableName = "XZQJX", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600200", "1000600220", "1000600230", "1000600240", "1000600250", "1000600260" }, ID = "07003" });
            Tools.Add(new ValueRangeTool() { TableName = "XZQJX", CheckFieldName = "JXLX", Key = _key, Values = new string[] { "630200", "640200", "650200", "660200", "670500" }, ID = "07004" });
            Tools.Add(new ValueRangeTool() { TableName = "XZQJX", CheckFieldName = "JXXZ", Key = _key, Values = new string[] { "600001", "600002", "600003", "600004", "600009" }, ID = "07005" });
            Tools.Add(new ValueCompareTool { TableName = "XZQJX", Key = _key, FieldArray1 = new string[] { "BSM" }, Value = .0, Compare = Compare.Above, ID = "07006" });
            Tools.Add(new ValueNullTool { TableName = "XZQJX", Key = _key, CheckFields = new string[] { "BSM", "YSDM", "JXLX", "JXXZ" }, Is_Nullable = false, ID = "07007" });
            #endregion


            #region 调查单元图斑要素基本属性结构表
            Tools.Add(new ValueCountTool() { TableName = "DCDYTB", ID = "08001" });
            Tools.Add(new ValueRangeTool() { TableName = "DCDYTB", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010100" }, ID = "08003" });
            Tools.Add(new ValueUniqueTool() { TableName = "DCDYTB", CheckFieldName = "TBBH", WhereFieldName = "XZCDM", ID = "08004", Code = "" });
            Tools.Add(new ValueRangeTool() { TableName = "DCDYTB", CheckFieldName = "DCDYLX", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "08005" });
            Tools.Add(new ValueCompareTool { TableName = "DCDYTB", Key = _key, FieldArray1 = new string[] { "MJ" }, Value = .0, Compare = Compare.Above, ID = "08006" });
            Tools.Add(new ValueNullTool { TableName = "DCDYTB", Key = _key, CheckFields = new string[] { "BSM", "YSDM", "XZCDM", "XZCMC","TBBH","DCDYLX","MJ" }, Is_Nullable = false, ID = "08007" });

            #endregion


            #region 农村存量建设用地总体情况基本属性结构表
            Tools.Add(new ValueCountTool() { TableName = "NCCLJSYDZTQK", ID = "09001" });
            Tools.Add(new ValueRangeTool() { TableName = "NCCLJSYDZTQK", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010200" }, ID = "09003" });
            Tools.Add(new ValueCompareTool() { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "XZCZYDMJ" }, FieldArray2 = new string[] { "DJCZYDMJ" }, Compare = Compare.Above, ID = "09005" });
            Tools.Add(new ValueCompareTool() { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "XZCZYDMJ" }, FieldArray2 = new string[] { "QQCZYDMJ" }, Compare = Compare.Above, ID = "09006" });
            Tools.Add(new ValueCompareTool() { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "RKZS", "LCRK" }, FieldArray2 = new string[] { "HJRK", "LRRK" }, Compare = Compare.Equal, ID = "09007" });
            Tools.Add(new ValueCompareTool { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "ZRCGS" }, Value = .0, Compare = Compare.Above, ID = "09008" });
            Tools.Add(new ValueCompareTool { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "CZYDTBGS" }, Value = .0, Compare = Compare.Above, ID = "09009" });
            Tools.Add(new ValueCompareTool { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "XZCZYDMJ" }, Value = .0, Compare = Compare.Above, ID = "09010" });
            Tools.Add(new ValueNullTool { TableName = "NCCLJSYDZTQK", Key = _key, CheckFields = new string[] { "BSM", "YSDM", "XZCDM", "XZCMC","SSXZMC","ZRCGS","CZYDTBGS","XZCZYDMJ","RKZS","HS","HJRK","LRRK","LCRK","JTJYXQYGS" }, Is_Nullable = false, ID = "09011" });
            //Tools.Add(new ValueCompareTool { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "DJCZYDMJ" }, Value = .0, Compare = Compare.Above, ID = "09011" });
            //Tools.Add(new ValueCompareTool { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "QQCZYDMJ" }, Value = .0, Compare = Compare.Above, ID = "09012" });
            //Tools.Add(new ValueCompareTool { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "RKZS" }, Value = .0, Compare = Compare.Above, ID = "09011" });
            //Tools.Add(new ValueCompareTool { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "HS" }, Value = .0, Compare = Compare.Above, ID = "09012" });
            //Tools.Add(new ValueCompareTool { TableName = "NCCLJSYDZTQK", Key = _key, FieldArray1 = new string[] { "HJRK" }, Value = .0, Compare = Compare.Above, ID = "09013" });
            #endregion

            #region 存量宅基地要素基本属性结构表

            Tools.Add(new ValueCountTool() { TableName = "CLZJD", ID = "10001" });
            Tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010300" }, ID = "10003" });
            Tools.Add(new ValueUniqueTool() { TableName = "CLZJD", CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "10005", Code = "3201" });
            Tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "10007" });
            Tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "FKDYJ", Key = _key, Values = new string[] { "1", "2", "" }, ID = "10008" });
            Tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3","" }, ID = "10009" });
            Tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "KZYY", Key = _key, Values = new string[] { "1", "2", "3", "4", "5", "" }, ID = "10010" });
            Tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "FQYY", Key = _key, Values = new string[] { "1", "2", "3", "4", "" }, ID = "10011" });
            Tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "KZYY", "FQYY", "QTYY" }, Key = _key, WhereCaluse = "LYZT = '1'", Is_Nullable = true, ID = "10012" });//为空
            Tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", Is_Nullable = false, ID = "10013" });
            Tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='2'", Is_Nullable = true, ID = "10014" });
            Tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", Is_Nullable = false, ID = "10015" });
            Tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='3'", Is_Nullable = true, ID = "10016" });
            Tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "QTYY" }, Key = _key, WhereCaluse = "KZYY = '5' OR FQYY = '4'", Is_Nullable = false, ID = "10017" });
            //Tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "SFWFKD", Key = _key, Values = new string[] { "是", "否" }, WhereFields = new string[] { "XZCDM", "TBBH" }, Split = "/", WhereList = DCDYTBManager.List.Where(e => e.DCDYLX == "1").Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), ID = "10018" });
            Tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "SFWFKD", Key = _key, Values = new string[] { "是", "否" }, ID = "10018" });
            Tools.Add(new ValueRangeTool() { TableName = "CLZJD", CheckFieldName = "FKDYJ", Key = _key, Values = new string[] { "1", "2", "" }, ID = "10019" });
            Tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "FKDYJ" }, Key = _key, WhereCaluse = "SFWFKD = '是'", Is_Nullable = false, ID = "10020" });
            Tools.Add(new ValueNullTool() { TableName = "CLZJD", CheckFields = new string[] { "FKDYJ" }, Key = _key, WhereCaluse = "SFWFKD = '否'", Is_Nullable = true, ID = "10021" });
            Tools.Add(new ValueCompareTool { TableName = "CLZJD", Key = _key, FieldArray1 = new string[] { "JZZDMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "10022" });
            Tools.Add(new ValueCompareTool { TableName = "CLZJD", Key = _key, FieldArray1 = new string[] { "FSYDMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "10023" });
            Tools.Add(new ValueCompareTool { TableName = "CLZJD", Key = _key, FieldArray1 = new string[] { "JZMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "10024" });
            Tools.Add(new ValueNullTool { TableName = "CLZJD", Key = _key, CheckFields = new string[] { "BSM", "YSDM", "XZCDM", "XZCMC", "DCBH", "TBBH", "JZZDMJ", "FSYDMJ", "JZMJ", "JZLX", "JTRK", "JZSJ","SFWFKD" }, Is_Nullable = false, ID = "10021" });
            Tools.Add(new ValueNullTool { TableName = "CLZJD", Key = _key, CheckFields = new string[] { "YCSRBZ", "NHLX" }, Is_Nullable = false, WhereFields = new string[] { "XZCDM", "TBBH" },  Split = "/", WhereList = DCDYTBManager.List.Where(e => e.DCDYLX != "1").Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), ID = "10022" });
            #endregion

            #region 农村存量经营性建设用地利用现状调查表基本属性结构表
            Tools.Add(new ValueCountTool() { TableName = "JYXJSYD", ID = "11001" });
            Tools.Add(new ValueRangeTool() { TableName = "JYXJSYD", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010400" }, ID = "11003" });
            Tools.Add(new ValueUniqueTool() { TableName = "JYXJSYD", CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "11005", Code = "3201" });   
            Tools.Add(new ValueRangeTool() { TableName = "JYXJSYD", CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "11007" });
            Tools.Add(new ValueRangeTool() { TableName = "JYXJSYD", CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "11008" });
            Tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "TDYT" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "11009", Is_Nullable = false });
            Tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "KZYY", "FQYY" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "11010", Is_Nullable = true });
            Tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "11011", Is_Nullable = false });
            Tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "FQYY", "NSSE", "JYRS" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "11012", Is_Nullable = true });
            Tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "11013", Is_Nullable = false });
            Tools.Add(new ValueNullTool() { TableName = "JYXJSYD", CheckFields = new string[] { "KZYY", "NSSE", "JYRS" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "11014", Is_Nullable = true });
            Tools.Add(new ValueCompareTool { TableName = "JYXJSYD", Key = _key, FieldArray1 = new string[] { "JSYDMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "11015" });
            Tools.Add(new ValueCompareTool { TableName = "JYXJSYD", Key = _key, FieldArray1 = new string[] { "JZMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "11016" });
            Tools.Add(new ValueNullTool { TableName = "JYXJSYD", Key = _key, CheckFields = new string[] { "BSM", "YSDM", "XZCDM", "XZCMC", "DCBH", "TBBH", "JSYDMJ", "JZMJ", "JZLX", "JZSJ", "LYZT" }, Is_Nullable = false, ID = "11017" });
            Tools.Add(new ValueRangeTool
            {
                TableName = "JYXJSYD",
                CheckFieldName = "TDYT",
                Key = _key,
                ID = "11018",
                Values = new string[] {
                    "",
                    "水田","水浇地","旱地",
                    "果园","茶园","其他园地",
                    "有林地","灌木林地","其他林地",
                    "天然牧草地","人工牧草地","其他草地",
                    "批发零售用地","住宿餐饮用地","商务金融用地","其他商服用地",
                    "工业用地","采矿用地","仓储用地",
                    "城镇住宅用地","农村宅基地",
                    "机关团体用地","新闻出版用地","科教用地","医卫慈善用地","文体娱乐用地","公共设施用地","公园与绿地","风景名胜设施用地",
                    "军事设施用地","使领馆用地","监教场所用地","宗教用地","殡葬用地",
                    "铁路用地","公路用地","街巷用地","农村道路","机场用地","港口码头用地","管道运输用地",
                    "河流水面","湖泊水面","水库水面","坑塘水面","沿海滩涂","内陆滩涂","沟渠","水工建筑用地","冰川及永久积雪",
                    "空闲地","设施农业用地","田坎","盐碱地","沼泽地","沙地","裸地"
                }
            });
            #endregion


            #region 农村存量公共管理及公共服务设施用地利用现状调查要素基本属性结构表
            Tools.Add(new ValueCountTool() { TableName = "GGGL_GGFWSSYD", ID = "12001" });
            Tools.Add(new ValueRangeTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010500" }, ID = "12003" });
            Tools.Add(new ValueUniqueTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "12005", Code = "3201" });
            Tools.Add(new ValueRangeTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "12007" });
            Tools.Add(new ValueRangeTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "12008" });
            Tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "TDYT", "FWRS", "SYMJ" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "12009", Is_Nullable = false });
            Tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "KZYY", "FQYY" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "12010", Is_Nullable = true });
            Tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "12011", Is_Nullable = false });
            Tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "FQYY", "FWRS", "SYMJ" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "12012", Is_Nullable = true });
            Tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "12013", Is_Nullable = false });
            Tools.Add(new ValueNullTool() { TableName = "GGGL_GGFWSSYD", CheckFields = new string[] { "KZYY", "FWRS", "SYMJ" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "12014", Is_Nullable = true });
            Tools.Add(new ValueCompareTool { TableName = "GGGL_GGFWSSYD", Key = _key, FieldArray1 = new string[] { "JSYDMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "12015" });
            Tools.Add(new ValueCompareTool { TableName = "GGGL_GGFWSSYD", Key = _key, FieldArray1 = new string[] { "JZMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "12016" });
            Tools.Add(new ValueNullTool { TableName = "GGGL_GGFWSSYD", Key = _key, CheckFields = new string[] { "BSM", "YSDM", "XZCDM", "XZCMC", "DCBH", "TBBH", "JSYDMJ", "JZMJ", "JZLX", "LYZT","JZSJ" }, Is_Nullable = false, ID = "12017" });
            Tools.Add(new ValueRangeTool
            {
                TableName = "GGGL_GGFWSSYD",
                CheckFieldName = "TDYT",
                Key = _key,
                ID = "12018",
                Values = new string[] {
                    "",
                    "水田","水浇地","旱地",
                    "果园","茶园","其他园地",
                    "有林地","灌木林地","其他林地",
                    "天然牧草地","人工牧草地","其他草地",
                    "批发零售用地","住宿餐饮用地","商务金融用地","其他商服用地",
                    "工业用地","采矿用地","仓储用地",
                    "城镇住宅用地","农村宅基地",
                    "机关团体用地","新闻出版用地","科教用地","医卫慈善用地","文体娱乐用地","公共设施用地","公园与绿地","风景名胜设施用地",
                    "军事设施用地","使领馆用地","监教场所用地","宗教用地","殡葬用地",
                    "铁路用地","公路用地","街巷用地","农村道路","机场用地","港口码头用地","管道运输用地",
                    "河流水面","湖泊水面","水库水面","坑塘水面","沿海滩涂","内陆滩涂","沟渠","水工建筑用地","冰川及永久积雪",
                    "空闲地","设施农业用地","田坎","盐碱地","沼泽地","沙地","裸地"
                }
            });
            // Tools.Add(new ValueCompareTool { TableName = "GGGL_GGFWSSYD", Key = _key, FieldArray1 = new string[] { "SYMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "12017" });
            #endregion

            #region 农村其他存量建设用地利用现状调查表基本属性结构表
            //Tools.Add(new ValueCountTool() { TableName = "QTCLJSYD", ID = "13001" });
            Tools.Add(new ValueRangeTool() { TableName = "QTCLJSYD", CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010600" }, ID = "13003" });
            Tools.Add(new ValueUniqueTool() { TableName = "QTCLJSYD", CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "13005", Code = "3201" });
            Tools.Add(new ValueRangeTool() { TableName = "QTCLJSYD", CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "13007" });
            Tools.Add(new ValueRangeTool() { TableName = "QTCLJSYD", CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "13008" });
            Tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "YXSYMJ" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "13009", Is_Nullable = false });
            Tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "KZYY", "FQYY" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "13010", Is_Nullable = true });
            Tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "13011", Is_Nullable = false });
            Tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "13012", Is_Nullable = true });
            Tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "13013", Is_Nullable = false });
            Tools.Add(new ValueNullTool() { TableName = "QTCLJSYD", CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "13014", Is_Nullable = true });
            Tools.Add(new ValueCompareTool { TableName = "QTCLJSYD", Key = _key, FieldArray1 = new string[] { "JSYDMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "13015" });
            Tools.Add(new ValueCompareTool { TableName = "QTCLJSYD", Key = _key, FieldArray1 = new string[] { "JZMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "13016" });
            Tools.Add(new ValueNullTool { TableName = "QTCLJSYD", Key = _key, CheckFields = new string[] { "BSM", "YSDM", "XZCDM", "XZCMC", "DCBH", "TBBH", "JSYDMJ", "JZMJ", "JZLX", "JZSJ","LYZT","TDYT" }, Is_Nullable = false, ID = "13017" });
            Tools.Add(new ValueRangeTool
            {
                TableName = "QTCLJSYD",
                CheckFieldName = "TDYT",
                Key = _key,
                ID = "13018",
                Values = new string[] {
                    "水田","水浇地","旱地",
                    "果园","茶园","其他园地",
                    "有林地","灌木林地","其他林地",
                    "天然牧草地","人工牧草地","其他草地",
                    "批发零售用地","住宿餐饮用地","商务金融用地","其他商服用地",
                    "工业用地","采矿用地","仓储用地",
                    "城镇住宅用地","农村宅基地",
                    "机关团体用地","新闻出版用地","科教用地","医卫慈善用地","文体娱乐用地","公共设施用地","公园与绿地","风景名胜设施用地",
                    "军事设施用地","使领馆用地","监教场所用地","宗教用地","殡葬用地",
                    "铁路用地","公路用地","街巷用地","农村道路","机场用地","港口码头用地","管道运输用地",
                    "河流水面","湖泊水面","水库水面","坑塘水面","沿海滩涂","内陆滩涂","沟渠","水工建筑用地","冰川及永久积雪",
                    "空闲地","设施农业用地","田坎","盐碱地","沼泽地","沙地","裸地"
                }
            });
            //Tools.Add(new ValueCompareTool { TableName = "QTCLJSYD", Key = _key, FieldArray1 = new string[] { "YXSYMJ" }, Value = .0, Compare = Compare.MoreEqual, ID = "13017" });
            #endregion
        }
    }
}
