using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        private List<ExcelField> _fields { get; set; }
        public List<ExcelField> Fields { get { return _fields; } set { _fields = value; } }
        private string _saveFolder { get; set; }
        /// <summary>
        /// 文件保存路径
        /// </summary>
        public string SaveFolder { get { return _saveFolder; } set { _saveFolder = value; } }





        
        /// <summary>
        /// 生成保存的Excel模板文件路径
        /// </summary>
        public string ModelFilePath { get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excels", CollectTable.Model2); } }
        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get { return System.IO.Path.Combine(SaveFolder, string.Format("{0}{1}.xls", CollectTable.Name, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"))); } }
        public void Program()
        {
            IWorkbook workbook = ModelFilePath.OpenExcel();
            if (workbook != null)
            {
                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    IRow modelRow = sheet.GetRow(CollectTable.StartIndex);
                    IRow row = null;
                    ICell cell = null;
                    var index = CollectTable.StartIndex;
                    foreach(var shi in CollectXZQ)
                    {
                        row = sheet.GetRow(index)??sheet.GetRow(index);
                        cell = ExcelClass.GetCell(row, 0, modelRow);
                        cell.SetCellValue(shi.XZQDM);//地市代码
                        ExcelClass.GetCell(row, 1, modelRow).SetCellValue(shi.XZQMC);//地市名称
                        if (shi.Children != null)
                        {
                            #region 写入每个县区数值
                            var sumList = new List<FieldValue>();
                            var sum1 = 0;
                            foreach(var quxian in shi.Children)
                            {
                                row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                                cell = ExcelClass.GetCell(row, 2, modelRow);
                                cell.SetCellValue(quxian.XZCDM);//县（区）（市）代码
                                ExcelClass.GetCell(row, 3, modelRow).SetCellValue(quxian.XZCMC);//县（区）（市）名称
                                var result = Collects.FirstOrDefault(e => e.XZQDM.ToLower() == quxian.XZCDM.ToLower() && e.XZQMC.ToLower() == quxian.XZCMC.ToLower());
                                if (result != null)
                                {
                                    cell = ExcelClass.GetCell(row, 4, modelRow);
                                    cell.SetCellValue(result.Values.Count);
                                    sum1 += result.Values.Count;
                                    foreach(var field in Fields)
                                    {
                                        cell = ExcelClass.GetCell(row, CollectTable.CollectIndex+field.Index, modelRow);
                                        var list = result.ValueList.Where(e => e.Index == field.Index).ToList();
                                        #region 合计字段值 并写入值
                                        if (field.Type == ExcelType.Double)
                                        {
                                            var a = .0;
                                            var sum = .0;
                                            foreach(var item in list)
                                            {
                                                if(double.TryParse(item.Value,out a))
                                                {
                                                    sum += Math.Round(a,4);
                                                }
                                            }
                                            cell.SetCellValue(sum);
                                        }
                                        else if (field.Type == ExcelType.Int)
                                        {
                                            var b = 0;
                                            var sum = 0;
                                            foreach(var item in list)
                                            {
                                                if(int.TryParse(item.Value,out b))
                                                {
                                                    sum += b;
                                                }
                                            }
                                            cell.SetCellValue(sum);
                                        }
                                        #endregion
                                    }
                                    sumList.AddRange(result.ValueList);
                                }
                                index++;
                            }
                            row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                            #region 写入地市小计
                            cell = ExcelClass.GetCell(row, 4, modelRow);
                            cell.SetCellValue(sum1);
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(index, index, 2, 3));
                            foreach(var field in Fields)
                            {
                                cell = ExcelClass.GetCell(row, field.Index + CollectTable.CollectIndex, modelRow);
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
                         
                            #endregion
                        }
                        index++;
                    }
                }
            }
            using (var fs=new FileStream(SaveFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }


        }
    }
}
