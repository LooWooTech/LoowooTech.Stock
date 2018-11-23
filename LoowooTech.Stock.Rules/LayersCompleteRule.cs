using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class LayersCompleteRule:IRule
    {
        public string RuleName { get { return "图层完整性"; } }
        public string ID { get { return "1201"; } }
        public bool Space { get { return false; } }

        public void Check()
        {
            var existTables = MdbClass.GetTables(ParameterManager2.Connection);
            var requireTables = ParameterManager2.Tables.Select(e => e.Name).ToList();
            var messages = new List<string>();
            foreach(var table in requireTables)
            {
                if (existTables.Contains(table) == false)
                {
                    messages.Add(table);
                }
            }
            if (messages.Count > 0)
            {
                QuestionManager2.Add(new Question2 { Code = ID, Name = RuleName, CheckProject = CheckProject2.图层完整性, Description = string.Format("缺少表：{0}", string.Join("、", messages.ToArray())) });
            }
          
        }
    }
}
