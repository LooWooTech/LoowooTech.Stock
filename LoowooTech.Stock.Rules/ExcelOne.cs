using LoowooTech.Stock.Common;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelOne:ExcelBase,IExcel
    {
        public ExcelOne()
        {
            ExcelName = "表1";
            Space = 1;
        }
        public override void Check()
        {
            base.Check();
            if (CheckCode == "6101")
            {
                Tools.Add(new ExcelValueCompareTool { ExcelName = ExcelName, ID = "11", Dict = ExcelDict, Type = Models.ExcelType.Int, Compare = Compare.Equal, FieldArray1 = new string[] { "人口总数", "流出人口" }, FieldArray2 = new string[] { "户籍人口", "流入人口" } });
                foreach (var tool in Tools)
                {
                    tool.Check();
                }
            }
           
        }
    }
}
