using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace LoowooTech.Stock.Common
{
    public static class PdfHelper2
    {
        public static void SavePdf(string excelFilePath,string pdfFilePath)
        {
            Microsoft.Office.Interop.Excel.Application lobjExcelApp = null;
            Microsoft.Office.Interop.Excel.Workbooks lobjExcelWorkbooks = null;
            Microsoft.Office.Interop.Excel.Workbook lobjExcelWorkbook = null;

            string lstrTemp = string.Empty;
            object lobjMissing = System.Reflection.Missing.Value;

            try
            {
                lobjExcelApp = new Microsoft.Office.Interop.Excel.Application();
                lobjExcelApp.Visible = true;
                lobjExcelWorkbooks = lobjExcelApp.Workbooks;
                lobjExcelWorkbook = lobjExcelWorkbooks.Open(excelFilePath, true, true, lobjMissing, lobjMissing, lobjMissing, true, lobjMissing, lobjMissing, lobjMissing, lobjMissing, lobjMissing, false, lobjMissing, lobjMissing);

                lstrTemp = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".xls" + (lobjExcelWorkbook.HasVBProject ? 'm' : 'x');
                //lstrTemp = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".xls";  
                lobjExcelWorkbook.SaveAs(lstrTemp, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel4Workbook, Type.Missing, Type.Missing, Type.Missing, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing,
                    false, Type.Missing, Type.Missing, Type.Missing);
                //输出为PDF 第一个选项指定转出为PDF,还可以指定为XPS格式  
                lobjExcelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, pdfFilePath, Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard, Type.Missing, false, Type.Missing, Type.Missing, false, Type.Missing);
                lobjExcelWorkbooks.Close();
                lobjExcelApp.Quit();


            }
            catch(Exception ex)
            {

            }
            finally
            {
                if (lobjExcelWorkbook != null)
                {
                    lobjExcelWorkbook.Close(Type.Missing, Type.Missing, Type.Missing);
                    Marshal.ReleaseComObject(lobjExcelWorkbook);
                    lobjExcelWorkbook = null;

                }
                if (lobjExcelWorkbooks != null)
                {
                    lobjExcelWorkbooks.Close();
                    Marshal.ReleaseComObject(lobjExcelWorkbooks);
                    lobjExcelWorkbooks = null;
                }
                if (lobjExcelApp != null)
                {
                    lobjExcelApp.Quit();
                    Marshal.ReleaseComObject(lobjExcelApp);
                    lobjExcelApp = null;

                }
                //主动激活垃圾回收器，主要是避免超大批量转文档时，内存占用过多，而垃圾回收器并不是时刻都在运行！  
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        public static bool Convert(string excelFilePath,string pdfFilePath)
        {
            XlFixedFormatType targetType = XlFixedFormatType.xlTypePDF;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.ApplicationClass application = null;
            Workbook workbook = null;
            try
            {
                application = new Microsoft.Office.Interop.Excel.ApplicationClass();
                object target = pdfFilePath;
                object type = targetType;
                workbook = application.Workbooks.Open(excelFilePath, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
                workbook.ExportAsFixedFormat(targetType, target, XlFixedFormatQuality.xlQualityStandard, true, false, missing, missing, missing, missing);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close(true, missing, missing);
                    workbook = null;
                }

                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                //return false;
            }

        
        }
    }
}
