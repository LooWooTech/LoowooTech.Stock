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
    }
}
