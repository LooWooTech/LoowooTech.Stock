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
    public class WorkBench3:IWorkBench
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
            _rules.Add(new FileFolderStandardRule2());//2018-11-21
            _rules.Add(new MetadataRule());
            _rules.Add(new LayersCompleteRule());//2018-11-21
            _rules.Add(new CoordinateRule2());//2018-11-22
            _rules.Add(new StructureRule2());//2018-11-22


        }

        public void Program()
        {
            ParameterManager2.Init(Folder);
            if (Init() == false)
            {
                OutputMessage("00", "初始化失败，即将退出质检", ProgressResultTypeEnum.Fail);
                return;
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

            return true;
        }

        private bool OutputMessage(string code, string message, ProgressResultTypeEnum result)
        {
            return OutputMessage(new ProgressEventArgs { Code = code, Message = message, Result = result });
        }
        private bool OutputMessage(ProgressEventArgs e)
        {
            OnProgramProcess(this, e);
            return e.Cancel;
        }
    }
}
