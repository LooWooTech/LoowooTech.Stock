using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class WriteCollectTool
    {
        private List<CollectXZQ> _collectXZQ { get; set; }
        public List<CollectXZQ> CollectXZQ { get { return _collectXZQ; } set { _collectXZQ = value; } }
        private CollectTable _collectTable { get; set; }
        public CollectTable CollectTable { get { return _collectTable; } set { _collectTable = value; } }
        private List<Collect> _collects { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Collect> Collects { get { return _collects; } set { _collects = value; } }


        #region  主要针对CollectExcel
        private List<Collect2> _collect2 { get; set; }
        public List<Collect2> Collect2 { get { return _collect2; }set { _collect2 = value; } }
        #endregion

        private bool? flag { get; set; }
        /// <summary>
        /// true  Collects ; false Collect2
        /// </summary>
        public bool Flag
        {
            get
            {
                if (flag.HasValue)
                {
                    return flag.Value;
                }

                flag = Collects != null;
                return flag.Value;
            }
        }

        private List<ExcelField> _fields { get; set; }
        public List<ExcelField> Fields { get { return _fields; } set { _fields = value; } }
        private string _saveFolder { get; set; }
        /// <summary>
        /// 文件保存路径
        /// </summary>
        public string SaveFolder { get { return _saveFolder; } set { _saveFolder = value; } }
        private CollectExcelType[] _collectExcelTypes { get; set; }
        public CollectExcelType[] CollectExcelTypes { get { return _collectExcelTypes; }set { _collectExcelTypes = value; } }





        
        /// <summary>
        /// 生成保存的Excel模板文件路径
        /// </summary>
        public string ModelFilePath { get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excels", CollectTable.Model2); } }
        public string ModelFilePath2 { get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excels", CollectTable.Model3); } }
        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get { return System.IO.Path.Combine(SaveFolder, string.Format("汇总表1 {0}{1}.xls", CollectTable.Name, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"))); } }
        public string SaveFilePath2 { get { return System.IO.Path.Combine(SaveFolder, string.Format("汇总表2 {0}{1}.xls", CollectTable.Name, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"))); } }
        public void Program()
        {
            foreach(var type in CollectExcelTypes)
            {
                if (type == CollectExcelType.Excel1)
                {
                    Write1();
                }
                else
                {
                    Write2();
                }
            }

        }

        private List<FieldValue> Search(string XZCDM,string XZCMC)
        {
            if (Flag == true)
            {
                var result = Collects.FirstOrDefault(e => e.XZQDM.ToLower() == XZCDM.ToLower() && e.XZQMC.ToLower() == XZCMC.ToLower());
                if (result != null)
                {
                    return result.ValueList;
                }
            }
            else
            {
                var result = Collect2.FirstOrDefault(e => e.XZQDM.ToLower() == XZCDM.ToLower() && e.XZQMC.ToLower() == XZCMC.ToLower());
                if (result != null)
                {
                    return result.Values;
                }
            }

            return null;
        }

        private void Write2()
        {
            IWorkbook workbook = ModelFilePath2.OpenExcel();
            if (workbook != null)
            {
                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    IRow modelRow = sheet.GetRow(CollectTable.StartIndex);
                    var header = sheet.GetRow(0);
                    ExcelClass.GetCell(header, 0).SetCellValue(CollectTable.Title);
                    IRow row = null;
                    ICell cell = null;
                    var index = CollectTable.StartIndex;
                    var all = new List<FieldValue>();
                    var sumA = 0;
                    var two = 0;
                    sheet.ShiftRows(index + 1, index + 5, CollectXZQ.Count - 1);
                    foreach(var shi in CollectXZQ)
                    {
                        row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                        row.Height = modelRow.Height;
                        cell = ExcelClass.GetCell(row, 0, modelRow);
                        cell.SetCellValue(shi.XZQDM);
                        ExcelClass.GetCell(row, 1, modelRow).SetCellValue(shi.XZQMC);
                       
                        if (shi.Children != null)
                        {
                            if (Flag)
                            {
                                ExcelClass.GetCell(row, 2, modelRow).SetCellValue(shi.Children.Count);
                            }
                         
                            two += shi.Children.Count;
                            var sumlist = new List<FieldValue>();
                            var sum1 = 0;
                            foreach (var quxian in shi.Children)
                            {
                                var values = Search(quxian.XZCDM, quxian.XZCMC);
                                if (values != null)
                                {
                                    sum1 += values.Count;
                                    sumlist.AddRange(values);
                                }
                            }
                            if (Flag)
                            {
                                ExcelClass.GetCell(row, 3, modelRow).SetCellValue(sum1);
                            }
                         
                            foreach(var field in Fields)
                            {
                                cell = ExcelClass.GetCell(row, Flag? field.Index + CollectTable.CollectIndex - 1:field.Index-2, modelRow);
                                var list = sumlist.Where(e => e.Index == field.Index).ToList();
                                #region 写入小计
                                if (field.Type == ExcelType.Double)
                                {
                                    var a = .0;
                                    var sum = .0;
                                    foreach (var item in list)
                                    {
                                        if (double.TryParse(item.Value, out a))
                                        {
                                            sum += Math.Round(a, 4);
                                        }
                                    }
                                    cell.SetCellValue(sum);
                                }
                                else if (field.Type == ExcelType.Int)
                                {
                                    var b = 0;
                                    var sum = 0;
                                    foreach (var item in list)
                                    {
                                        if (int.TryParse(item.Value, out b))
                                        {
                                            sum += b;
                                        }
                                    }
                                    cell.SetCellValue(sum);
                                }
                                #endregion
                            }
                            all.AddRange(sumlist);
                            sumA += sum1;

                        }
             
                        index++;
                    }

                    #region 写总计
                    row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                    row.Height = modelRow.Height;
                    if (Flag)
                    {
                        ExcelClass.GetCell(row, 2, modelRow).SetCellValue(two);
                        ExcelClass.GetCell(row, 3, modelRow).SetCellValue(sumA);
                    }
                 
                    foreach(var field in Fields)
                    {
                        cell = ExcelClass.GetCell(row, Flag? field.Index + CollectTable.CollectIndex - 1:field.Index-2, modelRow);
                        var list = all.Where(e => e.Index == field.Index).ToList();
                        if (field.Type == ExcelType.Double)
                        {
                            var a = .0;
                            var sum = .0;
                            foreach (var item in list)
                            {
                                if (double.TryParse(item.Value, out a))
                                {
                                    sum += Math.Round(a, 4);
                                }
                            }
                            cell.SetCellValue(sum);
                        }
                        else if (field.Type == ExcelType.Int)
                        {
                            var b = 0;
                            var sum = 0;
                            foreach (var item in list)
                            {
                                if (int.TryParse(item.Value, out b))
                                {
                                    sum += b;
                                }
                            }
                            cell.SetCellValue(sum);
                        }
                    }

                    #endregion
                }
            }
            using (var fs=new FileStream(SaveFilePath2, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }
        }

        private void Write1()
        {
            IWorkbook workbook = ModelFilePath.OpenExcel();
            if (workbook != null)
            {
                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    IRow modelRow = sheet.GetRow(CollectTable.StartIndex);
                    var header = sheet.GetRow(0);
                    ExcelClass.GetCell(header, 0).SetCellValue(CollectTable.Title);
                    IRow row = null;
                    ICell cell = null;
                    var index = CollectTable.StartIndex;
                    var all = new List<FieldValue>();
                    var sumA = 0;
                    sheet.ShiftRows(index + 1, index + 5, CollectXZQ.Count + CollectXZQ.Sum(e => e.Children.Count) - 1);
                    foreach (var shi in CollectXZQ)
                    {
                        row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                        row.Height = modelRow.Height;
                        cell = ExcelClass.GetCell(row, 0, modelRow);
                        cell.SetCellValue(shi.XZQDM);//地市代码
                        ExcelClass.GetCell(row, 1, modelRow).SetCellValue(shi.XZQMC);//地市名称
                        if (shi.Children != null)
                        {
                            var start = index;
                            #region 写入每个县区数值
                            var sumList = new List<FieldValue>();
                            var sum1 = 0;
                            foreach (var quxian in shi.Children)
                            {

                                row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                                row.Height = modelRow.Height;

                                ExcelClass.GetCell(row, 0, modelRow);
                                ExcelClass.GetCell(row, 1, modelRow);

                                cell = ExcelClass.GetCell(row, 2, modelRow);
                                cell.SetCellValue(quxian.XZCDM);//县（区）（市）代码
                                ExcelClass.GetCell(row, 3, modelRow).SetCellValue(quxian.XZCMC);//县（区）（市）名称

                                var values = Search(quxian.XZCDM, quxian.XZCMC);
                                if (values != null)
                                {
                                    if (Flag)
                                    {
                                        cell = ExcelClass.GetCell(row, 4, modelRow);
                                        cell.SetCellValue(values.Count);
                                        sum1 += values.Count;
                                    }
                                  
                                    foreach (var field in Fields)
                                    {
                                        cell = ExcelClass.GetCell(row, Flag? CollectTable.CollectIndex + field.Index:field.Index, modelRow);
                                        var list = values.Where(e => e.Index == field.Index).ToList();
                                        #region 合计字段值 并写入值
                                        if (field.Type == ExcelType.Double)
                                        {
                                            var a = .0;
                                            var sum = .0;
                                            foreach (var item in list)
                                            {
                                                if (double.TryParse(item.Value, out a))
                                                {
                                                    sum += Math.Round(a, 4);
                                                }
                                            }
                                            cell.SetCellValue(sum);
                                        }
                                        else if (field.Type == ExcelType.Int)
                                        {
                                            var b = 0;
                                            var sum = 0;
                                            foreach (var item in list)
                                            {
                                                if (int.TryParse(item.Value, out b))
                                                {
                                                    sum += b;
                                                }
                                            }
                                            cell.SetCellValue(sum);
                                        }
                                        #endregion
                                    }
                                    sumList.AddRange(values);
                                }
                                index++;
                            }
                            row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                            row.Height = modelRow.Height;
                            #region 写入地市小计
                            ExcelClass.GetCell(row, 2, modelRow).SetCellValue("小计");

                            ExcelClass.GetCell(row, 0, modelRow);
                            ExcelClass.GetCell(row, 1, modelRow);

                            if (Flag)
                            {
                                cell = ExcelClass.GetCell(row, 4, modelRow);
                                cell.SetCellValue(sum1);
                            }
                          
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(index, index, 2, 3));
                            foreach (var field in Fields)
                            {
                                cell = ExcelClass.GetCell(row, Flag? field.Index + CollectTable.CollectIndex:field.Index, modelRow);
                                var list = sumList.Where(e => e.Index == field.Index).ToList();
                                #region 合计字段值 并写入值
                                if (field.Type == ExcelType.Double)
                                {
                                    var a = .0;
                                    var sum = .0;
                                    foreach (var item in list)
                                    {
                                        if (double.TryParse(item.Value, out a))
                                        {
                                            sum += Math.Round(a, 4);
                                        }
                                    }
                                    cell.SetCellValue(sum);
                                }
                                else if (field.Type == ExcelType.Int)
                                {
                                    var b = 0;
                                    var sum = 0;
                                    foreach (var item in list)
                                    {
                                        if (int.TryParse(item.Value, out b))
                                        {
                                            sum += b;
                                        }
                                    }
                                    cell.SetCellValue(sum);
                                }
                                #endregion
                            }
                            #endregion

                            all.AddRange(sumList);
                            sumA += sum1;


                            #endregion
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(start, index, 0, 0));
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(start, index, 1, 1));
                        }
                        index++;
                    }

                    #region 合计
                    row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                    row.Height = modelRow.Height;
                    if (Flag)
                    {
                        ExcelClass.GetCell(row, 4, modelRow).SetCellValue(sumA);
                    }
                  
                    foreach (var field in Fields)
                    {
                        cell = ExcelClass.GetCell(row,Flag? field.Index + CollectTable.CollectIndex:field.Index, modelRow);
                        var list = all.Where(e => e.Index == field.Index).ToList();
                        if (field.Type == ExcelType.Double)
                        {
                            var a = .0;
                            var sum = .0;
                            foreach (var item in list)
                            {
                                if (double.TryParse(item.Value, out a))
                                {
                                    sum += Math.Round(a, 4);
                                }
                            }
                            cell.SetCellValue(sum);
                        }
                        else if (field.Type == ExcelType.Int)
                        {
                            var b = 0;
                            var sum = 0;
                            foreach (var item in list)
                            {
                                if (int.TryParse(item.Value, out b))
                                {
                                    sum += b;
                                }
                            }
                            cell.SetCellValue(sum);
                        }
                    }


                    #endregion


                }
            }
            using (var fs = new FileStream(SaveFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }

        }
    }
}
