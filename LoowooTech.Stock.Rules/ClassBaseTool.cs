using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System.Collections.Generic;
using System.Linq;

namespace LoowooTech.Stock.Rules
{
    public class ClassBaseTool:ArcGISBaseTool
    {
        public List<IVTool> Tools { get; set; } = new List<IVTool>();

        public virtual void Check()
        {
            foreach(var tool in Tools)
            {
                if (tool.Check(ParameterManager2.Connection) == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        TableName = tool.TableName,
                        CheckProject = CheckProject,
                        Description = string.Format("执行质检规则【{0}】失败", tool.Name)
                    });
                }

                if (tool.Messages2.Count > 0)
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
