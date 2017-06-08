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
        public static string[] GetCellValues(this IRow row,int start,int end)
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

        public static NPOI.SS.UserModel.ICell GetCell(IRow row, int line, IRow modelRow = null)
        {
            NPOI.SS.UserModel.ICell cell = row.GetCell(line);
            if (cell == null)
            {
                if (modelRow != null)
                {
                    cell = row.CreateCell(line, modelRow.GetCell(line).CellType);
                    cell.CellStyle = modelRow.GetCell(line).CellStyle;
                }
                else
                {
                    cell = row.CreateCell(line);
                }
            }
            return cell;
        }

        private static XZC GainXZ(IRow row)
        {
            var values = GetCellValues(row, 0, 2);
            return new XZC
            {
                XZCMC = values[0],
                XZCDM = values[1]
            };
        }
        public static List<XZC> GainXZ(string filePath)
        {
            var list = new List<XZC>();
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
