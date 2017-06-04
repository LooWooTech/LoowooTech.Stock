using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelSix:ExcelBase,IExcel
    {
        public ExcelSix()
        {
            ExcelName = "表6";
            Space = 3;
        }
    }
}
