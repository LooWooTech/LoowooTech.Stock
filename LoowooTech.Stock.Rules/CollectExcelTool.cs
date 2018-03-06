using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class CollectExcelTool
    {
        private CollectTable _collectTable { get; set; }
        /// <summary>
        /// 表格类型信息
        /// </summary>
        public CollectTable CollectTable { get { return _collectTable; } set { _collectTable = value; } }
        private List<StockFile> _files { get; set; }
        /// <summary>
        /// 表格文件列表
        /// </summary>
        public List<StockFile> Files { get { return _files; } set { _files = value; } }
        private List<CollectXZQ> _collectXZQ { get; set; }
        /// <summary>
        /// 每个市关联下属的区县列表信息
        /// </summary>
        public List<CollectXZQ> CollectXZQ { get { return _collectXZQ; } set { _collectXZQ = value; } }
        private string _saveFolder { get; set; }
        public string SaveFolder { get { return _saveFolder; } set { _saveFolder = value; } }
        private List<ExcelField> _fields { get; set; }
        public List<ExcelField> Fields { get { return _fields; } set { _fields = value; } }
        private CollectExcelType _collectExcelType { get; set; }
        public CollectExcelType CollectExcelType { get { return _collectExcelType; } set { _collectExcelType = value; } }






        //public List<ExcelField> Fields { get { return _fields == null ? _fields = GetFields() : _fields; } }
        private List<ExcelField> GetFields()
        {
            var list = new List<ExcelField>();
            var nodeP = XmlManager.GetSingle(string.Format("/Tables/Excel[@Name='{0}']", CollectTable.Name), XmlEnum.Field);
            if (nodeP != null)
            {
                var nodes = nodeP.SelectNodes("Field");
                if (nodes != null && nodes.Count > 0)
                {
                    for (var i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        var val = new ExcelField
                        {
                            TableName = CollectTable.Name,
                            Name = node.Attributes["Name"].Value,
                            Title = node.Attributes["Title"].Value,
                            Index = int.Parse(node.Attributes["Index"].Value),
                            Type = node.Attributes["Type"].Value.ToLower() == "int" ? ExcelType.Int : ExcelType.Double,
                            Compute = node.Attributes["Compute"].Value.ToLower() == "sum" ? Compute.Sum : Compute.Count,
                        };
                        if (node.Attributes["Unit"] != null)
                        {
                            val.Unit = node.Attributes["Unit"].Value.Trim();
                        }

                        if (node.Attributes["View"] != null)
                        {
                            val.View = node.Attributes["View"].Value;
                        }
                        if (node.Attributes["WhereClause"] != null)
                        {
                            val.WhereClause = node.Attributes["WhereClause"].Value;
                        }
                        if (node.Attributes["TableName"] != null)
                        {
                            val.FieldTableName = node.Attributes["TableName"].Value;
                        }
                        if (node.Attributes["Indexs"] != null)
                        {
                            var indexs = node.Attributes["Indexs"].Value;
                            if (!string.IsNullOrEmpty(indexs))
                            {
                                var temps = indexs.Split(',');
                                var res = new int[temps.Length];
                                for (var j = 0; j < temps.Length; j++)
                                {
                                    var a = 0;
                                    res[j] = int.TryParse(temps[j], out a) ? a : 0;
                                }
                                val.Indexs = res;
                            }
                        }
                        list.Add(val);
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// 读取到的每个区县的合计字段值信息
        /// </summary>
        private List<CollectExcel> _result { get; set; } = new List<CollectExcel>();
        public List<CollectExcel> Result { get { return _result; } }
        public string ModelFilePath { get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excels", CollectTable.Model2); } }
        public string ModelFilePath2 { get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excels", CollectTable.Model3); } }
        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get { return System.IO.Path.Combine(SaveFolder, string.Format("{0}{1}.xls", CollectTable.Name, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"))); } }
        private readonly object _syncRoot = new object();

        public void Program()
        {
            Parallel.ForEach(Files, file =>
            {
                var collect = Program(file, Fields);
                if (collect != null)
                {
                    Add(collect);
                }
            });
            if (CollectExcelType == CollectExcelType.Excel1)
            {
                Write();
            }
            else
            {
                Write2();
            }
          

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
                    IRow row = null;
                    ICell cell = null;
                    var index = CollectTable.StartIndex;
                    sheet.ShiftRows(index + 1, index + 5, CollectXZQ.Count - 1);
                    var AllList = new List<FieldValue>();
                    var all = 0;
                    var two = 0;
                    foreach (var shi in CollectXZQ)
                    {
                        row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                        row.Height = modelRow.Height;
                        cell = ExcelClass.GetCell(row, 0, modelRow);
                        cell.SetCellValue(shi.XZQDM);
                        ExcelClass.GetCell(row, 1, modelRow).SetCellValue(shi.XZQMC);
                        #region
                        if (shi.Children != null)
                        {
                            cell = ExcelClass.GetCell(row, 2, modelRow);
                            cell.SetCellValue(shi.Children.Count);
                            two += shi.Children.Count;
                            var sumlist = new List<FieldValue>();
                            var sum1 = 0;
                            foreach(var quxian in shi.Children)
                            {
                                var entry = Result.FirstOrDefault(e => e.XZQDM.ToLower() == quxian.XZCDM.ToLower() && e.XZQMC.ToLower() == quxian.XZCMC.ToLower());
                                if (entry != null)
                                {
                                    sum1 += entry.Count;
                                    sumlist.AddRange(entry.Values);
                                }
                            }
                            ExcelClass.GetCell(row, 3, modelRow).SetCellValue(sum1);
                            foreach(var field in Fields)
                            {
                                cell = ExcelClass.GetCell(row, field.Index + CollectTable.CollectIndex - 1, modelRow);
                                var list = sumlist.Where(e => e.Index == field.Index).ToList();
                                #region 写入市 小计值
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
                            AllList.AddRange(sumlist);
                            all += sum1;
                        }
                        #endregion
                        index++;
                    }

                    #region 写合计
                    row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                    row.Height = modelRow.Height;
                    ExcelClass.GetCell(row, 2, modelRow).SetCellValue(two);
                    ExcelClass.GetCell(row, 3, modelRow).SetCellValue(all);
                    foreach (var field in Fields)
                    {
                        cell = ExcelClass.GetCell(row, field.Index + CollectTable.CollectIndex-1, modelRow);
                        var list = AllList.Where(e => e.Index == field.Index).ToList();
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

        private void Write()
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
                    var all = new List<FieldValue>();
                    var sumA = 0;
                    sheet.ShiftRows(index + 1, index + 5, CollectXZQ.Count + CollectXZQ.Sum(e => e.Children.Count)-1);
                    foreach (var shi in CollectXZQ)
                    {
                        row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                        row.Height = modelRow.Height;
                        cell = ExcelClass.GetCell(row, 0, modelRow);
                        cell.SetCellValue(shi.XZQDM);
                        ExcelClass.GetCell(row, 1, modelRow).SetCellValue(shi.XZQMC);
                        //sheet.ShiftRows(index + 1, index + 5, 1);
                        if (shi.Children != null)
                        {
                            var i = index;
                            //sheet.ShiftRows(index + 2, index + 6, shi.Children.Count);
                            var sumList = new List<FieldValue>();
                            var sum1 = 0;
                            #region 写入市下面的区县信息数据
                            foreach (var quxian in shi.Children)
                            {
                                row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                                row.Height = modelRow.Height;
                                cell = ExcelClass.GetCell(row, 2, modelRow);
                                cell.SetCellValue(quxian.XZCDM);
                                ExcelClass.GetCell(row, 3, modelRow).SetCellValue(quxian.XZCMC);
                                var entry = Result.FirstOrDefault(e => e.XZQDM.ToLower() == quxian.XZCDM.ToLower() && e.XZQMC.ToLower() == quxian.XZCMC.ToLower());
                                if (entry != null)
                                {
                                    cell = ExcelClass.GetCell(row, 4, modelRow);
                                    cell.SetCellValue(entry.Count);
                                    sum1 += entry.Count;
                                    foreach (var field in entry.Values)
                                    {
                                        cell = ExcelClass.GetCell(row, CollectTable.CollectIndex + field.Index, modelRow);
                                        cell.SetCellValue(field.Value);
                                    }
                                    sumList.AddRange(entry.Values);
                                }
                                index++;
                            }
                            #endregion
                            row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                            row.Height = modelRow.Height;
                            cell = ExcelClass.GetCell(row, 4, modelRow);
                            cell.SetCellValue(sum1);
                            ExcelClass.GetCell(row, 2, modelRow).SetCellValue("小计");
                            ExcelClass.GetCell(row, 3, modelRow);
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(index, index, 2, 3));
                            foreach(var field in Fields)
                            {
                                cell = ExcelClass.GetCell(row, field.Index + CollectTable.CollectIndex, modelRow);
                                var list = sumList.Where(e => e.Index == field.Index).ToList();
                                #region 写入市 小计值
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

                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, index, 0, 0));
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, index, 1, 1));
                            all.AddRange(sumList);
                            sumA += sum1;
                        }
                        index++;
                    }

                    #region 写合计
                    row = sheet.GetRow(index) ?? sheet.CreateRow(index);
                    row.Height = modelRow.Height;
                    ExcelClass.GetCell(row, 4, modelRow).SetCellValue(sumA);
                    foreach(var field in Fields)
                    {
                        cell = ExcelClass.GetCell(row, field.Index + CollectTable.CollectIndex, modelRow);
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

            using (var fs=new FileStream(SaveFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }
        }

        private void Add(CollectExcel collect)
        {
            lock (_syncRoot)
            {
                _result.Add(collect);
            }
        }

        private CollectExcel Program(StockFile stockfile,List<ExcelField> fields)
        {
            IWorkbook workbook = stockfile.FullName.OpenExcel();
            if (workbook != null)
            {
                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    int count = 0;
                    IRow row = null;
                    ICell cell = null;
                    for(var i = CollectTable.StartIndex; i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row == null)
                        {
                            break;
                        }
                        cell = row.GetCell(0);
                        if (cell.ToString().ToLower() == "合计".ToLower())
                        {
                            var result = Analyze(row, fields);
                            var collect = new CollectExcel
                            {
                                XZQDM = stockfile.XZQDM,
                                XZQMC = stockfile.XZQMC,
                                Count = count,
                                Values = result
                            };
                            return collect;

                        }else if (Regex.IsMatch(cell.ToString(), "^33[0-9]{7}$"))
                        {
                            count++;
                        }
                    }
                }
            }
            return null;
        }

        private List<FieldValue> Analyze(IRow row,List<ExcelField> fields)
        {
            var list = new List<FieldValue>();
            ICell cell = null;
            foreach(var field in fields)
            {
                cell = row.GetCell(field.Index);
                if (cell == null)
                {
                    continue;
                }

                list.Add(new FieldValue
                {
                    Index = field.Index,
                    Type = field.Type,
                    Title = field.Title,
                    Val = cell.ToString()
                });
            }
            return list;
        }
    }
}
