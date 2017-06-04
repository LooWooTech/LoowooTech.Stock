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
    }
}
