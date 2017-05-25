using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LoowooTech.Stock.Models;

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
        private static string[] GetCellValues(IRow row,int start,int end)
        {
            var values = new string[end - start];
            for(var i = start; i < end; i++)
            {
                var cell = row.GetCell(i);
                if (cell != null)
                {
                    values[i - start] = cell.ToString().Trim();
                }
            }
            return values;
        }

        private static XZ GainXZ(IRow row)
        {
            var values = GetCellValues(row, 0, 2);
            return new XZ
            {
                XZQMC = values[0],
                XZQDM = values[1]
            };
        }
        public static List<XZ> GainXZ(string filePath)
        {
            var list = new List<XZ>();
            IWorkbook workbook = filePath.OpenExcel();
            if (workbook != null)
            {
                ISheet sheet=workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    IRow row = null;
                    for(var i = 1; i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row != null)
                        {
                            list.Add(GainXZ(row));
                        }
                    }
                }
            }
            return list;
        }
    }
}
