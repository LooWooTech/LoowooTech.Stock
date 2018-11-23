using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ValueBaseRule2:IRule
    {
        public virtual string RuleName { get; }
        public virtual string ID { get; }
        public bool Space { get { return false; } }

        public virtual CheckProject2 CheckProject { get; }

        public List<IVTool> _tools { get; set; } = new List<IVTool>();
        public string _key { get; set; } = "BSM";
        public virtual void InitTool()
        {

        }

        public void Check()
        {
            InitTool();
            foreach(var tool in _tools)
            {
                if (tool.Check(ParameterManager2.Connection)==false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        TableName = tool.TableName,
                        CheckProject = CheckProject,
                        Description = string.Format("检查【{0}】失败", tool.Name)
                    });
                }
                else
                {
                    QuestionManager2.AddRange(tool.Messages2.Select(e => new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        TableName = tool.TableName,
                        CheckProject = CheckProject,
                        Description = e.Description,
                        LocationClause = e.WhereClause
                    }).ToList());
                }
            }
        }

    }
}
