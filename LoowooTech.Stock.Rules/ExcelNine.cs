using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelNine:ExcelBase,IExcel
    {
        public ExcelNine()
        {
            ExcelName = "表9";
            Space = 3;
        }
    }
}
