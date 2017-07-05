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
        protected const string _name = "{0}({1})农村存量建设用地调查数据成果质检结果";

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
            var codeFileTool = new FileTool() { Folder = path, Filter = "*.xls", RegexString = @"^[\u4e00-\u9fa5]{3,}\(33[0-9]{4}\)单位代码表.xls$" };
            ParameterManager.CodeFilePath = codeFileTool.GetFile();
            var mdbfileTool = new FileTool() { Folder = path, Filter = "*.mdb", RegexString = @"^[\u4e00-\u9fa5]{3,}\(33[0-9]{4}\)农村存量建设用地调查成果空间数据库.mdb$" };
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
        
        public string ReportPath { get { return string.IsNullOrEmpty(_reportPath)? Path.Combine(Folder, report, string.Format(_name+".xls", ParameterManager.District, ParameterManager.Code)):_reportPath; } }

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
            if (ExcelManager.Dict.Count == 0)
            {
                QuestionManager.Add(new Question { Code = "00", TableName = "单位代码表", Description = "未获取单位代码表中的相关数据信息" });
                OutputMessage("00", "未获取单位代码表中的相关数据信息", ProgressResultTypeEnum.Fail);
            }
            else
            {
                OutputMessage("00", string.Format("成功读取行政区（乡镇）单位代码表信息:{0}条",ExcelManager.Dict.Count), ProgressResultTypeEnum.Other);
            }
            if (ExcelManager.XZQ.Count == 0)
            {
                QuestionManager.Add(new Question { Code = "00", TableName = "单位代码表", Description = "读取到的单位代码表中未填写行政区（乡镇）代码信息" });
                OutputMessage("00", "读取到的单位代码表中未填写行政区（乡镇）代码信息", ProgressResultTypeEnum.Fail);
            }
            if (ExcelManager.XZC.Count == 0)
            {
                QuestionManager.Add(new Question { Code = "00", TableName = "单位代码表", Description = "读取到的单位代码表中未填写行政区（村级）代码信息" });
                OutputMessage("00", "读取到的单位代码表中未填写行政区（村级）代码信息", ProgressResultTypeEnum.Fail);
            }
            var list = ArcGISManager.GainDCDYTB(ParameterManager.Workspace, DCDYTBManager.ClassName)??new List<DCDYTB>();
            if (list.Count == 0)
            {
                OutputMessage("00", "未获取调查单元类型相关基础信息", ProgressResultTypeEnum.Fail);
            }
            QuestionManager.AddRange(list.Where(e => e.Right == false).Select(e =>
                new Question
                {
                    Code = "3401",
                    Name="面积一致性",
                    TableName = DCDYTBManager.ClassName,
                    Description = string.Format("行政区名称：【{0}】行政区代码：【{1}】图斑编号：【{2}】的MJ:【{3}】图斑实际面积：【{4}】容差超过1平方米", e.XZCMC, e.XZCDM, e.TBBH,e.MJ,e.Area),
                    BSM=e.BSM,
                    WhereClause=string.Format("[BSM] = {0}",e.BSM)
                }).ToList());
            DCDYTBManager.List = list;
            DCDYTBManager.Init(ParameterManager.Connection);
            OutputMessage("00", "成功读取调查单元图斑信息", ProgressResultTypeEnum.Other);
            InitRules();
            ParameterManager.Folder = Folder;
            
            return true;

        }
        public void Write(string folder)
        {
            var tool = new ExcelValueCreateRule() { SaveFolder = folder };
            tool.Write();
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
               
                    if (result != ProgressResultTypeEnum.Pass)
                    {
                        QuestionManager.Add(new Question { Code = rule.ID, Name = rule.RuleName, Description = sb.ToString() });
                    }
                    if (OutputMessage(rule.ID, sb.ToString(), result) == true)
                    {
                        break;
                    }
                   
                }
            }
            _reportPath = QuestionManager.Save(ReportPath);

        }
        private bool OutputMessage(string code,string message,ProgressResultTypeEnum result)
        {
            return  OutputMessage(new ProgressEventArgs { Code = code, Message = message, Result = result });
        }
        private bool OutputMessage(ProgressEventArgs e)
        {
            OnProgramProcess(this, e);
            return e.Cancel;
        }
    }
}
