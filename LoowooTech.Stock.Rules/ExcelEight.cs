using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelEight:ExcelBase,IExcel
    {
        public ExcelEight()
        {
            ExcelName = "表8";
            Space = 3;
        }
    }
}
