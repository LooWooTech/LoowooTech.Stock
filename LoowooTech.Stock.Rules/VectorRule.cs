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
        public string RuleName { get; set; }
        public string ID { get; set; }
        public void Check()
        {
            var tool = new TableStructure();
            tool.Check(ParameterManager.Connection);
            QuestionManager.AddRange(tool.Erros.Select(e => new Question { Code = "2101", Name = RuleName, Project = CheckProject.图层完整性, Description = e }).ToList());
        }
    }
}
