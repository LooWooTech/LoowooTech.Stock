using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelFour:ExcelBase,IExcel
    {
        public ExcelFour()
        {
            ExcelName = "表4";
            Space = 1;
        }

        public override void Check()
        {
            base.Check();
            if (CheckCode == "6101")
            {
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "41", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.Equal, FieldArray1 = new string[] { "建设用地总面积" }, FieldArray2 = new string[] { "正常使用面积", "空置面积", "废弃面积" } });
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "42", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.Equal, FieldArray1 = new string[] { "建设用地总面积" }, FieldArray2 = new string[] { "调查单元类型-撤并型-建设用地总面积", "调查单元类型-保留型-建设用地总面积", "调查单元类型-集聚型-建设用地总面积" } });
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "43", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.Equal, FieldArray1 = new string[] { "建设用地总面积" }, FieldArray2 = new string[] { "建筑类型-砖混-建设用地总面积", "建筑类型-钢混-建设用地总面积", "建筑类型-砖木-建设用地总面积", "建筑类型-其他-建设用地总面积" } });
                foreach(var tool in Tools)
                {
                    tool.Check();
                }

            }
        }
    }
}
