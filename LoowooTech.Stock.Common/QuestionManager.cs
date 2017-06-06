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
        private static  ConcurrentBag<Question> _paralleQuestions { get; set; }
        public static ConcurrentBag<Question> ParalleQuestions { get { return _paralleQuestions; } }
        public static void Init()
        {
            _paralleQuestions = new ConcurrentBag<Question>();
        }
        public static void AddRange(List<Question> questions)
        {
            Parallel.ForEach(questions, item =>
            {
                _paralleQuestions.Add(item);
            });
        }
        public static void Add(Question question)
        {
            _paralleQuestions.Add(question);
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
        public static void Save(string folder,string district,string code)
        {
            var info = string.Empty;
            if (string.IsNullOrEmpty(ModelFile)||!System.IO.File.Exists(ModelFile))
            {
                info = string.Format("质检报告格式文件为空或者格式文件不存在");
                Console.WriteLine(info);
                return;
            }

            IWorkbook workbook = ModelFile.OpenExcel();
            if (workbook == null)
            {
                info = "打开质检报告格式文件失败";
                Console.WriteLine(info);
                return;
            }
            var sheet1 = workbook.GetSheetAt(0);
            var sheet2 = workbook.GetSheetAt(1);
            SaveCollect(sheet1, _paralleQuestions);
            SaveList(sheet2, _paralleQuestions);
            using (var fs=new FileStream(System.IO.Path.Combine(folder, string.Format(_name + ".xls", district, code)), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }
        }

        private static void SaveCollect(ISheet sheet,ConcurrentBag<Question> concurrentBag)
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
                        var val = concurrentBag.Where(e => e.Code.ToLower() == key.ToLower()).LongCount();
                        cell.SetCellValue(val);
                    }
                }
            }
        }
        private static void SaveList(ISheet sheet,ConcurrentBag<Question> concurrentBag)
        {
            var list = concurrentBag.OrderBy(e => e.Code).ThenBy(e => e.TableName).ToList();
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

    }
}
