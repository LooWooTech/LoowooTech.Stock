using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelFive:ExcelBase,IExcel
    {
        public ExcelFive()
        {
            ExcelName = "表5";
            Space = 1;
        }

        public override void Check()
        {
            base.Check();
            if (CheckCode == "6101")
            {
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "51", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.Equal, FieldArray1 = new string[] { "建设用地总面积" }, FieldArray2 = new string[] { "建筑类型-砖混-建设用地总面积", "建筑类型-钢混-建设用地总面积", "建筑类型-砖木-建设用地总面积", "建筑类型-其他-建设用地总面积" } });
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "52", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.MoreEqual, FieldArray1 = new string[] { "建设用地总面积" }, FieldArray2 = new string[] { "有效使用总面积", "空置面积", "废弃面积" } });
                foreach(var tool in Tools)
                {
                    tool.Check();
                }
            }
        }
    }
}
