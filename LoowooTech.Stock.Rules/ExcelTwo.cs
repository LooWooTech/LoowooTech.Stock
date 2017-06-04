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
    }
}
