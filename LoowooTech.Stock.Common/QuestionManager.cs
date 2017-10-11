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
        private static int MAXNUMBER = 65534;

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
        /// <summary>
        /// 作用：生成XLS格式质检报告文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string Save(string filePath)
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
            var sheet4 = workbook.GetSheetAt(3);
            SaveCollect(sheet1);
            SaveList(sheet2);
            SaveInfo(sheet3, LogManager.List);
            SaveSQLError(sheet4);
            var folder = System.IO.Path.GetDirectoryName(filePath);
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            using (var fs=new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }
            return filePath;
        }

        private static void SaveSQLError(ISheet sheet)
        {
            var list = Questions.Where(e => e.Project == CheckProject.数据库查询).OrderBy(e => e.Code).ThenBy(e => e.TableName).ToList();
            var i = 2;
            IRow row = null;
            var modelrow = sheet.GetRow(i);
            foreach (var item in list.Take(MAXNUMBER))
            {
                row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                var cell = ExcelClass.GetCell(row, 0, modelrow);
                cell.SetCellValue(i++);
                ExcelClass.GetCell(row, 1, modelrow).SetCellValue(item.Description);
                ExcelClass.GetCell(row, 2, modelrow).SetCellValue(item.Code);
            }
            if (list.Count > MAXNUMBER)
            {
                row = sheet.GetRow(MAXNUMBER + 1) ?? sheet.CreateRow(MAXNUMBER + 1);
                ExcelClass.GetCell(row, 0, modelrow).SetCellValue(string.Format("错误列表超过{0}，超过部分不再显示", MAXNUMBER));
            }
        }

        private const string ALLKey = "ALL";

        /// <summary>
        /// 作用：生成各个检查类别数量汇总表
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="concurrentBag"></param>
        private static void SaveCollect(ISheet sheet)
        {
            IRow row = null;
            var temp = Questions.Where(e => !string.IsNullOrEmpty(e.Code));
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
                        var val = key.ToLower()==ALLKey.ToLower()?temp.LongCount(): temp.Where(e => e.Code.ToLower() == key.ToLower()).LongCount();
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
            var list = Questions.Where(e=> e.Project != CheckProject.数据库查询).OrderBy(e => e.Code).ThenBy(e => e.TableName).ToList();
            var i = 2;
            var serial = 1;
            IRow row = null;
            var modelrow = sheet.GetRow(i);
            foreach (var item in list.Take(MAXNUMBER))
            {
                row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                i++;
                var cell = ExcelClass.GetCell(row, 0, modelrow);
                cell.SetCellValue(serial++);
                ExcelClass.GetCell(row, 1, modelrow).SetCellValue(item.Code);
                ExcelClass.GetCell(row, 2, modelrow).SetCellValue(item.Name);
                ExcelClass.GetCell(row, 3, modelrow).SetCellValue(item.TableName);
                ExcelClass.GetCell(row, 4, modelrow).SetCellValue(item.BSM);
                ExcelClass.GetCell(row, 5, modelrow).SetCellValue(item.Description);
                ExcelClass.GetCell(row, 6, modelrow).SetCellValue(item.Project.ToString());
            }
            if (list.Count > MAXNUMBER)
            {
                row = sheet.GetRow(MAXNUMBER + 1) ?? sheet.CreateRow(MAXNUMBER + 1);
                ExcelClass.GetCell(row, 0, modelrow).SetCellValue(string.Format("错误列表超过{0}，超过部分不再显示", MAXNUMBER));
            }
        }
        /// <summary>
        /// 作用：生成质检过程中存在的问题表格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="list"></param>
        private static void SaveInfo(ISheet sheet,ConcurrentBag<string> list)
        {
            var i = 2;
            IRow row = null;
            var modelrow = sheet.GetRow(1);
            foreach(var item in list.Take(MAXNUMBER))
            {
                row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                var cell = ExcelClass.GetCell(row, 0, modelrow);
                cell.SetCellValue(i++);
                ExcelClass.GetCell(row, 1, modelrow).SetCellValue(item);
            }

            if (list.Count > MAXNUMBER)
            {
                row = sheet.GetRow(MAXNUMBER + 1) ?? sheet.CreateRow(MAXNUMBER + 1);
                ExcelClass.GetCell(row, 0, modelrow).SetCellValue(string.Format("错误列表超过{0}，超过部分不再显示", MAXNUMBER));
            }
        }

    }
}
