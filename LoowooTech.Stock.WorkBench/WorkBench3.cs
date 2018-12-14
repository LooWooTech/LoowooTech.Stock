using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.WorkBench
{
    public class WorkBench3:IWorkBench2
    {
        public event ProgramProgressHandler OnProgramProcess;

        private string _folder { get; set; }
        public string Folder { get { return _folder; } set { _folder = value; } }
        private string _reportPath { get; set; }
        public string ReportPath { get { return _reportPath; } set { _reportPath = value; } }
        private string _reportPdfPath { get; set; }
        public string ReportPDFPath { get { return _reportPdfPath; } set { _reportPdfPath = value; } }

        private List<IRule> _rules { get; set; } = new List<IRule>();
        private void InitRules()
        {
            _rules.Add(new FileFolderStandardRule2());//2018-11-21目录级文件规范性
            _rules.Add(new MetadataRule());//   元数据
            _rules.Add(new LayersCompleteRule());//2018-11-21图层完整性
            _rules.Add(new CoordinateRule2());//2018-11-22数学基础
            _rules.Add(new StructureRule2());//2018-11-22结构符合性
            _rules.Add(new ValueManageRule());//2018-11-24值符合性
            _rules.Add(new AttributeRule());//2018-11-26属性正确性
            _rules.Add(new AreaRule());//2018-11-30//面积一致性
            _rules.Add(new TopologyRule2());//2018-11-29 拓扑关系
            _rules.Add(new SplinterRule2());//2018-11-27 碎片多边形

            _rules.Add(new JQXZRule());//2018-11-30  基期与现状数据对比

            _rules.Add(new JQZHQHRule());//2018-12-11 基数转换前后地类转换关系正确性

            _rules.Add(new AClassRule());//2018-12-3  A类转换逻辑一致性
            _rules.Add(new BClassRule());//2018-12-3 B类转换逻辑一致性
            _rules.Add(new CClassRule());//2018-12-3 C 类转换逻辑一致性
            _rules.Add(new EClassRule());//

            _rules.Add(new FClassRule());//2018-12-5
            _rules.Add(new JClassRule());//2018-12-5

            _rules.Add(new GHYTJQDLTBRule());//2018-12-5
            _rules.Add(new GHYTTDGHDLRule());//2018-12-6
            _rules.Add(new GHYTJSYDGZQRule());//2018-12-6
            _rules.Add(new GHYTRule());//2018-12-6

            _rules.Add(new YJJBNTRule());//2018-12-7
            _rules.Add(new SFQYJJBNTRule());//2018-12-7
            _rules.Add(new JSYDGZQ040Rule());//2018-12-7
            _rules.Add(new JSYDGZQ020Rule());//2018-12-7

        }

        private List<int> _IDs { get; set; }
        public List<int> IDs { get { return _IDs; } set { _IDs = value; } }
        public List<Question2> Results { get { return QuestionManager2.Questions; } }

        public void Program()
        {
            QuestionManager2.Questions.Clear();
            if (Init() == false)
            {
                OutputMessage("00", "初始化失败，即将退出质检", ProgressResultTypeEnum.Fail);
                return;
            }

            foreach(var id in IDs)
            {
                var rule = _rules.FirstOrDefault(e => e.ID == id.ToString());
                if (rule != null)
                {
                    var sb = new StringBuilder(rule.RuleName);
                    var result = ProgressResultTypeEnum.Pass;
                    try
                    {
                        rule.Check();
                    }catch(AggregateException ae)
                    {
                        foreach (var exp in ae.InnerExceptions)
                        {
                            sb.Append(exp.Message + "\r\n");
                        }
                        result = ProgressResultTypeEnum.Fail;
                    }catch(Exception ex)
                    {
                        sb.Append(ex.ToString());
                        result = ProgressResultTypeEnum.Fail;
                    }
                    if (result != ProgressResultTypeEnum.Pass)
                    {
                        QuestionManager2.Add(new Question2 { Code = rule.ID, Name = rule.RuleName, Description = sb.ToString() });
                    }

                    if (OutputMessage(rule.ID, sb.ToString(), result) == true)
                    {
                        break;
                    }
                }
            }

        }

        public void Write(string folder)
        {

        }



        private bool Init()
        {
            OutputMessage("00", "正在初始化", ProgressResultTypeEnum.Other);
            if (ParameterManager2.AnalyzeXZQ() == false)
            {
                OutputMessage("00", "行政区划代码表中未读取相关信息", ProgressResultTypeEnum.Fail);
                return false;
            }
            if (ParameterManager2.AnalyzeDistrict() == false)
            {
                OutputMessage("00", "无法获取当前成果中的县级名称或者乡镇名称", ProgressResultTypeEnum.Fail);
                return false;
            }
            if (System.IO.File.Exists(ParameterManager2.MDBFilePath) == false)
            {
                OutputMessage("00", "无法获取矢量mdb数据文件", ProgressResultTypeEnum.Fail);
                return false;
            }
            InitRules();
            return true;
        }

        private bool OutputMessage(string code, string message, ProgressResultTypeEnum result)
        {
            QuestionManager2.Add(new Question2 { Code = code, Description = message });
            return OutputMessage(new ProgressEventArgs { Code = code, Message = message, Result = result });
        }
        private bool OutputMessage(ProgressEventArgs e)
        {
            OnProgramProcess(this, e);
            return e.Cancel;
        }
    }
}
