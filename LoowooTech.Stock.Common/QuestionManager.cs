using LoowooTech.Stock.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Common
{
    public static class QuestionManager
    {
        private static string _name = "{0}({1})农村存量建设用地调查数据成果质检结果";

        private static readonly object _syncRoot = new object();

        public static List<Question> Questions { get; private set; }

        static QuestionManager() {
            Questions = new List<Question>();
        }
        public static void Clear()
        {
            lock(_syncRoot)
            {
                Questions.Clear();
            }
        }

        public static void AddRange(List<Question> questions)
        {
            lock(_syncRoot)
            {
                Questions.AddRange(questions);
            }
        }
        public static void Add(Question question)
        {
            lock(_syncRoot)
            {
                Questions.Add(question);
            }
            
        } 

        private static string _modelFile { get; set; }
        public static string ModelFile
        {
            get
            {
                if (string.IsNullOrEmpty(_modelFile))
                {
                    _modelFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["Report"]);
                }
                return _modelFile;
            }
        }
        public static string Save(string folder,string district,string code)
        {
            var info = string.Empty;
            if (string.IsNullOrEmpty(ModelFile)||!System.IO.File.Exists(ModelFile))
            {
                info = string.Format("质检报告格式文件为空或者格式文件不存在");
                Console.WriteLine(info);
                return string.Empty;
            }

            IWorkbook workbook = ModelFile.OpenExcel();
            if (workbook == null)
            {
                info = "打开质检报告格式文件失败";
                Console.WriteLine(info);
                return string.Empty;
            }
            var sheet1 = workbook.GetSheetAt(0);
            var sheet2 = workbook.GetSheetAt(1);
            var sheet3 = workbook.GetSheetAt(2);
            SaveCollect(sheet1);
            SaveList(sheet2);
            SaveInfo(sheet3, LogManager.List);
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            var filePath = System.IO.Path.Combine(folder, string.Format(_name + ".xls", district, code));
            using (var fs=new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }
            return filePath;
        }

        /// <summary>
        /// 作用：生成各个检查类别数量汇总表
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="concurrentBag"></param>
        private static void SaveCollect(ISheet sheet)
        {
            IRow row = null;
            for(var i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null)
                {
                    break;
                }

                var cell = row.GetCell(5);
                if (cell != null)
                {
                    var str = cell.ToString();
                    if (!string.IsNullOrEmpty(str) && str.Contains("{") && str.Contains("}"))
                    {
                        var key = str.Replace("{", "").Replace("}", "");
                        var val = Questions.Where(e => e.Code.ToLower() == key.ToLower()).LongCount();
                        cell.SetCellValue(val);
                    }
                }
            }
        }
        /// <summary>
        /// 作用：生成具体问题明细表格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="concurrentBag"></param>
        private static void SaveList(ISheet sheet)
        {
            var list = Questions.OrderBy(e => e.Code).ThenBy(e => e.TableName).ToList();
            var i = 1;
            IRow row = null;
            var modelrow = sheet.GetRow(i);
            foreach (var item in list)
            {
                row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                var cell = ExcelClass.GetCell(row, 0, modelrow);
                cell.SetCellValue(i++);
                ExcelClass.GetCell(row, 1, modelrow).SetCellValue(item.Code);
                ExcelClass.GetCell(row, 2, modelrow).SetCellValue(item.Name);
                ExcelClass.GetCell(row, 3, modelrow).SetCellValue(item.TableName);
                ExcelClass.GetCell(row, 4, modelrow).SetCellValue(item.BSM);
                ExcelClass.GetCell(row, 5, modelrow).SetCellValue(item.Description);
                ExcelClass.GetCell(row, 6, modelrow).SetCellValue(item.Project.ToString());
            }
        }
        /// <summary>
        /// 作用：生成质检过程中存在的问题表格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="list"></param>
        private static void SaveInfo(ISheet sheet,ConcurrentBag<string> list)
        {
            var i = 1;
            IRow row = null;
            var modelrow = sheet.GetRow(1);
            foreach(var item in list)
            {
                row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                var cell = ExcelClass.GetCell(row, 0, modelrow);
                cell.SetCellValue(i++);
                ExcelClass.GetCell(row, 1, modelrow).SetCellValue(item);
            }
        }

    }
}
