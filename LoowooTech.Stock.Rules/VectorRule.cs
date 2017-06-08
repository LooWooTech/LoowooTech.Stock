using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 矢量图层完整性
    /// </summary>
    public class VectorRule:IRule
    {
        public string RuleName { get { return "矢量图层是否完整，是否符合《浙江省农村存量建设用地调查数据库标准》的要求"; } }
        public string ID { get { return "2101"; } }
        public void Check()
        {
            var tool = new TableStructure();
            tool.Check(ParameterManager.Connection);
            QuestionManager.AddRange(tool.Erros.Select(e => new Question { Code = "2101", Name = RuleName, Project = CheckProject.图层完整性, Description = e }).ToList());
        }
    }
}
