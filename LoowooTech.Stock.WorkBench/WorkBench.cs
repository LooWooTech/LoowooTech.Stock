using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Rules;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoowooTech.Stock.WorkBench
{
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// 是否取消后续任务
        /// </summary>
        public bool Cancel { get; set; }
        
        /// <summary>
        /// 已完成任务的编码
        /// </summary>
        public string Code { get; set; }

        
        /// <summary>
        /// 已完成任务的详情（可以是空）
        /// </summary>
        public string Message { get; set; }
    }

    public class WorkBench
    {
        public delegate void ProgramProgressHandler(object sender, ProgressEventArgs e);

        public event ProgramProgressHandler OnProgramProcess;

        protected const string report = "5质检报告";
        protected const string DataBase = "1空间数据库";
        protected const string Collect = "3统计报告";
        private string _folder { get; set; }
        /// <summary>
        /// 质检路径
        /// </summary>
        public string Folder { get { return _folder; }set { _folder = value; } }
        /// <summary>
        /// 质检报告路径
        /// </summary>
        private string _reportPath { get; set; }
        public string ReportPath { get { return string.IsNullOrEmpty(_reportPath) ? _reportPath = System.IO.Path.Combine(Folder, report) : _reportPath; } }
        private string _district { get; set; }
        /// <summary>
        /// 行政区名称
        /// </summary>
        public string District { get { return _district; } }
        private string _code { get; set; }
        /// <summary>
        /// 当前行政区代码
        /// </summary>
        public string Code { get { return _code; } }
        private string[] _ids { get; set; }
        /// <summary>
        /// 质检规则的ID
        /// </summary>
        public string[] IDS { get { return _ids; } set { _ids = value; } }
        private List<IFolder> _folderTools { get; set; }

        public WorkBench()
        {
            _folderTools = new List<IFolder>();
        }


        public void Program()
        {
            QuestionManager.Init();//质检问题初始化

            if (!System.IO.Directory.Exists(Folder))
            {
                LogManager.Log(string.Format("质检路径不存在：{0}，请核对！", Folder));
                QuestionManager.Add(new Question() { Code = "1102", Name = "质检路径不存在",Project=CheckProject.目录及文件规范性, Description = string.Format("质检路径不存在：{0}，请核对！", Folder) });
                return;
            }
            var folderTool = new FolderTool { Folder = Folder };//对质检路径进行命名规范检查
            if (!folderTool.Check())
            {
                return;
            }
            _code = folderTool.Code;
            _district = folderTool.CityName;

            var resultComplete = new ResultComplete(Folder) { Children = XmlManager.Get("/Folders/Folder", "Name", XmlEnum.DataTree) };
            resultComplete.Check();//对质检路径下的文件夹、文件是否存在，是否能够打开进行检查
            QuestionManager.AddRange(resultComplete.Messages.Select(e => new Question { Code = "1102", Name = "成果数据丢露",Project=CheckProject.目录及文件规范性, Description = e }).ToList());

            if(OnProgramProcess != null)
            {
                var args = new ProgressEventArgs() { Code = "11", Cancel = false, Message = string.Empty };
                OnProgramProcess(this, args);
                if (args.Cancel) return;
            }

            _folderTools.AddRange(resultComplete.ExistPath.Select(e => new FileFolder()
            {
                Folder = e,
                FileNames = XmlManager.GetChildren(string.Format("/Folders/Folder[@Name='{0}']", new DirectoryInfo(e).Name), "Name", XmlEnum.DataTree),
                CityName = folderTool.CityName,
                Code = folderTool.Code
            }));

            try
            {
                Parallel.ForEach(_folderTools, tool =>
                {
                    tool.Check();
                });
            }
            catch(AggregateException ae)
            {
                foreach(var exp in ae.InnerExceptions)
                {
                    LogManager.Log(exp.ToString());
                    LogManager.Record(exp.ToString());
                }
            }


            var path = System.IO.Path.Combine(Folder, DataBase);
            //获取空间数据库文件夹下的单位代码表文件，并获取单位代码信息
            var codefileTool = new FileTool { Folder = path, Filter = "*.xls", RegexString = @"^[\u4e00-\u9fa5]+\(\d{6}\)单位代码表.xls$" };
            var currentCodeFile = codefileTool.GetFile();
            if (string.IsNullOrEmpty(currentCodeFile))
            {
                LogManager.Log("未识别到单位代码表文件，请核对空间数据库文件下的文件");
            }
            else
            {
                ExcelManager.Init(currentCodeFile);
            }
            //获取空间数据库文件夹下的空间数据库文件，并对数据库进行检查
            var mdbfileTool = new FileTool() { Folder = path, Filter = "*.mdb", RegexString = @"^[\u4e00-\u9fa5]+\(\d{6}\)农村存量建设用地调查成功空间数据库.mdb$" };
            var currentMdbFile = mdbfileTool.GetFile();
            if (string.IsNullOrEmpty(currentMdbFile))
            {
                LogManager.Log("未识别到数据库文件,请核对农村存量建设用地调查成功空间数据库.mdb文件");
                QuestionManager.Add(new Question { Code = "2101", Name = "适量数据文件",Project=CheckProject.目录及文件规范性, Description = "未识别到数据库文件,请核对农村存量建设用地调查成功空间数据库.mdb文件" });
            }
            else
            {
                TableHeart.Program(currentMdbFile,IDS);
                var gisheart = new ArcGISHeart() { MDBFilePath = currentMdbFile, FeatureClassNames = XmlManager.Get("/Tables/Table[@IsSpace='true']", "Name", XmlEnum.Field) };
                gisheart.Program();
            }
            Console.WriteLine("开始对统计表格进行质检......");
            var collectfolder = System.IO.Path.Combine(_folder, Collect);
            if (!System.IO.Directory.Exists(collectfolder))
            {
                QuestionManager.Add(new Question { Code = "1101", Name = "统计表格文件夹", Project = CheckProject.目录及文件规范性, Description = string.Format("目录：{0}不存在", collectfolder) });
            }
            else
            {
                //汇总表质检
                var excel = new ExcelHeart() { Folder = collectfolder, MDBFilePath = currentMdbFile,District=folderTool.CityName,Code=folderTool.Code };
                excel.Program();
                QuestionManager.AddRange(excel.Questions);
            }
  


        }
    }
}
