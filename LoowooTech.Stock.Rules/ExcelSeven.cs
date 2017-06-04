using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelSeven:ExcelBase,IExcel
    {
        public ExcelSeven()
        {
            ExcelName = "表7";
            Space = 3;
        }
    }
}
