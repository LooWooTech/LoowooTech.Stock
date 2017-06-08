using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class ExcelValueLogicRule:IRule
    {
        public string RuleName { get { return "汇总表的记录条数的缺失，多余，检查汇总表记录中病毒属性值与数据库图层属性值是否完全一致"; } }
        public string ID { get { return "6101"; } }
        public void Check()
        {
            var tools = new List<IExcel>
            {
                new ExcelOne { Connection = ParameterManager.Connection, List = ExcelManager.XZC, District = ParameterManager.District, Code = ParameterManager.Code, Folder = ParameterManager.CollectFolder },
                new ExcelOne { Connection=ParameterManager.Connection,List=ExcelManager.XZC,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder },
                new ExcelThree { Connection=ParameterManager.Connection,List=ExcelManager.XZC,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder},
                new ExcelFour { Connection=ParameterManager.Connection,List=ExcelManager.XZC,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder},
                new ExcelFive { Connection=ParameterManager.Connection,List=ExcelManager.XZC,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder}
            };
            Parallel.ForEach(tools, item =>
            {
                item.Check();
                QuestionManager.AddRange(item.ParalleQuestions.ToList());
            });
        }
    }
}
