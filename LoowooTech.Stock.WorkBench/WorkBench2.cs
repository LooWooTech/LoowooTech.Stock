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

namespace LoowooTech.Stock.WorkBench
{
    public class WorkBench2:IWorkBench
    {
        protected const string report = "5质检结果";
        protected const string DataBase = "1空间数据库";
        protected const string Collect = "3统计报告";
        protected const string Title = "农村存量建设用地调查数据成果";

        private string _folder { get; set; }
        /// <summary>
        /// 质检文件夹路径
        /// </summary>
        public string Folder { get { return _folder; }set { _folder = value; } }
        /// <summary>
        /// 作用：通过质检路径获取行政区代码以及行政区名称信息
        /// </summary>
        /// <returns></returns>
        private bool ReadXZQ()
        {
            var info = new DirectoryInfo(_folder);
            var array = info.Name.Replace(Title, "").Replace("（", ",").Replace("）", "").Replace("(", ",").Replace(")", "").Split(',');
            if (array.Length == 2)
            {
                ParameterManager.District = array[0];
                ParameterManager.Code = array[1];
                return true;
            }
            return false;
        }
        /// <summary>
        /// 作用：找寻单位代码表以及数据库文件，都找到返回true
        /// </summary>
        /// <returns></returns>
        private bool SearchFile()
        {
            var path = System.IO.Path.Combine(Folder, DataBase);
            var codeFileTool = new FileTool() { Folder = path, Filter = "*.xls", RegexString = @"^[\u4e00-\u9fa5]{3,}\(330[0-9]{3}\)单位代码表.xls$" };
            ParameterManager.CodeFilePath = codeFileTool.GetFile();
            var mdbfileTool = new FileTool() { Folder = path, Filter = "*.mdb", RegexString = @"^[\u4e00-\u9fa5]{3,}\(330[0-9]{3}\)农村存量建设用地调查成果空间数据库.mdb$" };
            ParameterManager.MDBFilePath = mdbfileTool.GetFile();

            return !string.IsNullOrEmpty(ParameterManager.CodeFilePath) 
                && !string.IsNullOrEmpty(ParameterManager.MDBFilePath) 
                && System.IO.File.Exists(ParameterManager.CodeFilePath) 
                && System.IO.File.Exists(ParameterManager.MDBFilePath);
        }
        private List<IRule> _rules { get; set; }
        private List<int> _ruleIds { get; set; }
        public List<int> RulsIds { get { return _ruleIds; }set { _ruleIds = value; } }
        public List<Question> Results { get { return QuestionManager.Questions; } }
        public string DistrictName { get { return ParameterManager.District; } }
        public string DistrictCode { get { return ParameterManager.Code; } }
        
        private string _reportPath { get; set; }
        
        public string ReportPath { get { return _reportPath; } }

        public event ProgramProgressHandler OnProgramProcess;

        private void InitRules()
        {
            _rules = new List<IRule>();

            _rules.Add(new FileFolderStandardRule());
            _rules.Add(new FileOpenRule());
            _rules.Add(new VectorRule ());
            _rules.Add(new CoordinateRule ());
            _rules.Add(new StructureRule ());
            _rules.Add(new ValueRule());
            _rules.Add(new XZCDMRule());
            _rules.Add(new BSMRule ());
            _rules.Add(new TBAreaRule ());
            _rules.Add(new TopologyRule ());
            _rules.Add(new SplinterRule());
            _rules.Add(new ContinuousRule());
            _rules.Add(new TBBHRule());
            _rules.Add(new ExcelValueLogicRule());
            _rules.Add(new ExcelValueCollectRule());
        }
        private bool Init()
        {
            OutputMessage("00","正在初始化检查机制", ProgressResultTypeEnum.Other);
            if (!ReadXZQ())//分析读取行政区
            {
                OutputMessage("00","未分析读取到行政区名称和行政区代码",  ProgressResultTypeEnum.Fail );
                return false;
            }
            if (!SearchFile())//查找单位代码表和数据库文件
            {
                OutputMessage("00", "未找到单位代码表或者数据库文件", ProgressResultTypeEnum.Fail);   
                return false;
            }
            ParameterManager.Init(Folder);
            OutputMessage("00", "参数管理器初始化完毕", ProgressResultTypeEnum.Other);
            ExcelManager.Init(ParameterManager.CodeFilePath);//初始化单位代码信息列表
            OutputMessage("00", "成功读取单位代码表信息", ProgressResultTypeEnum.Other);
            DCDYTBManager.Init(ParameterManager.Connection);//获取DCDYTB中的信息;
            OutputMessage("00", "成功读取调查单元图斑信息", ProgressResultTypeEnum.Other);
            InitRules();
            ParameterManager.Folder = Folder;
            return true;

        }
        public void Program()
        {
            QuestionManager.Clear();
            LogManager.Init();
            if (!Init())
            {
                OutputMessage("00", "初始化失败，程序终止", ProgressResultTypeEnum.Fail);
                return;
            }
            OutputMessage("00", "成功初始化", ProgressResultTypeEnum.Pass);
            foreach(var id in _ruleIds)
            {
                var rule = _rules.FirstOrDefault(e => e.ID == id.ToString());
                if (rule != null)
                {
                    var sb = new StringBuilder(rule.RuleName);
                    var result = ProgressResultTypeEnum.Pass;
                    try
                    {
                        rule.Check();
                    }
                    catch(AggregateException ae)
                    {
                        foreach(var exp in ae.InnerExceptions)
                        {
                            sb.Append(exp.Message+"\r\n");
                        }
                        result = ProgressResultTypeEnum.Fail;

                    }
                    catch(Exception ex)
                    {
                        result = ProgressResultTypeEnum.Fail;
                        sb.Append(ex.ToString());

                    }
                    OutputMessage(rule.ID, sb.ToString(), result);
                    if (result != ProgressResultTypeEnum.Pass)
                    {
                        QuestionManager.Add(new Question { Code = rule.ID, Name = rule.RuleName, Description = sb.ToString() });
                    }
                }
            }
            _reportPath = QuestionManager.Save(System.IO.Path.Combine(Folder, report), ParameterManager.District, ParameterManager.Code);

        }
        private void OutputMessage(string code,string message,ProgressResultTypeEnum result)
        {
            OutputMessage(new ProgressEventArgs { Code = code, Message = message, Result = result });
        }
        private void OutputMessage(ProgressEventArgs e)
        {
            OnProgramProcess(this, e);
        }
    }
}
