using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class ExcelClass
    {
        public static IWorkbook OpenExcel(this string filePath)
        {
            IWorkbook workbook = null;
            using(var fs=new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                workbook = WorkbookFactory.Create(fs);
            }
            return workbook;
        }
    }
}
