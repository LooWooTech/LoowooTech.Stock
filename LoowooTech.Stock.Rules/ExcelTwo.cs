using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelTwo:ExcelBase,IExcel
    {
        public ExcelTwo()
        {
            ExcelName = "表2";
            Space = 1;
        }

        public override void Check()
        {
            base.Check();
            if (CheckCode == "6101")
            {
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "21", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.Equal, FieldArray1 = new string[] { "建筑总面积" }, FieldArray2 = new string[] { "调查单元类型-撤并型-建筑总面积", "调查单元类型-保留型-建筑总面积", "调查单元类型-集聚型-建筑总面积" } });
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "22", Dict = ExcelDict, Type = Models.ExcelType.Double, Compare = Compare.Equal, FieldArray1 = new string[] { "建筑总面积" }, FieldArray2 = new string[] { "建筑类型-砖混-建筑总面积", "建筑类型-钢混-建筑总面积", "建筑类型-砖木-建筑总面积", "建筑类型-其他-建筑总面积" } });
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "23", Dict = ExcelDict, Type = Models.ExcelType.Int, Compare = Compare.Equal, FieldArray1 = new string[] { "家庭人口总数" }, FieldArray2 = new string[] { "调查单元类型-撤并型-人口数", "调查单元类型-保留型-人口数", "调查单元类型-集聚型-人口数" } });
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "24", Dict = ExcelDict, Type = Models.ExcelType.Int, Compare = Compare.Equal, FieldArray1 = new string[] { "家庭人口总数" }, FieldArray2 = new string[] { "建筑类型-砖混-人口数", "建筑类型-钢混-人口数", "建筑类型-砖木-人口数", "建筑类型-其他-人口数" } });
                foreach (var tool in Tools)
                {
                    tool.Check();
                }

            }

        }
    }
}
