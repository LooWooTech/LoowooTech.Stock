using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class CoordinateRule2:IRule
    {
        public string RuleName { get { return "数学基础"; } }
        public string ID { get { return "1202"; } }
        public bool Space { get { return true; } }

        public void Check()
        {
            var tables = ParameterManager2.Tables.Where(e => e.IsSpace == true).Select(e => e.Name).ToList();
            if (ParameterManager2.WorkSpace == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject2.数学基础,
                    Folder = "",
                    Description = string.Format("无法打开矢量数据的workspace,故无法检查规则")
                });
            }
            else
            {
                var results = ArcGISManager.CheckCoordinate2(ParameterManager2.WorkSpace, tables);
                if (results.Count > 0)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject2.数学基础,
                        Description = string.Format("【{0}】不符合坐标系", string.Join("、", results.ToArray())),
                        Folder = ""
                    });
                }
            }
        }
    }
}
