using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelValueCollectRule:IRule
    {
        public string RuleName { get { return "表格数据中，各级汇总面积和数据库汇总面积的一致性"; } }
        public string ID { get { return "6201"; } }
        public bool Space { get { return false; } }
        public void Check()
        {
            var tools = new List<IExcel>
            {
                new ExcelOne { Connection = ParameterManager.Connection, List = ExcelManager.XZQ, District = ParameterManager.District, Code = ParameterManager.Code, Folder = ParameterManager.CollectFolder,CheckCode=ID },
                new ExcelOne { Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID },
                new ExcelThree { Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID},
                new ExcelFour { Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID},
                new ExcelFive { Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID}
            };
            foreach(var item in tools)
            {
                item.Check();
                QuestionManager.AddRange(item.ParalleQuestions.ToList());
            }
            //System.Threading.Tasks.Parallel.ForEach(tools, item =>
            //{
            //    item.Check();
            //    QuestionManager.AddRange(item.ParalleQuestions.ToList());
            //});
        }
    }
}
