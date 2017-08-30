using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelThree:ExcelBase,IExcel
    {
        public ExcelThree()
        {
            ExcelName = "表3";
            Space = 1;
        }

        public override void Check()
        {
            base.Check();
            if (CheckCode == "6101")
            {
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "31", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.Equal, FieldArray1 = new string[] { "建设用地总面积" }, FieldArray2 = new string[] { "正常使用面积", "空置面积", "废弃面积" } });
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "32", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.Equal, FieldArray1 = new string[] { "建设用地总面积" }, FieldArray2 = new string[] { "调查单元类型-撤并型-建设用地总面积", "调查单元类型-保留型-建设用地总面积", "调查单元类型-集聚型-建设用地总面积" } });
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "33", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.Equal, FieldArray1 = new string[] { "建设用地总面积" }, FieldArray2 = new string[] { "建筑类型-砖混-建设用地总面积", "建筑类型-钢混-建设用地总面积", "建筑类型-砖木-建设用地总面积", "建筑类型-其他-建设用地总面积" } });
                foreach(var tool in Tools)
                {
                    tool.Check();
                }
            }
        }
    }
}
