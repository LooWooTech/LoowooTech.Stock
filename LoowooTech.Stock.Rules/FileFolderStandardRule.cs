using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 目录和文件规范
    /// </summary>
    public class FileFolderStandardRule : IRule
    {
        public string RuleName { get { return "目录及文件规范性"; } }
        public string ID { get { return "11"; } }
        public bool Space { get { return false; } }
        public void Check()
        {
            #region  检查目录文件夹
            var resultComplete = new ResultComplete(ParameterManager.Folder) { Children = ParameterManager.ChildrenFolder };
            resultComplete.Check();
            QuestionManager.AddRange(
                resultComplete.Messages.Select(e => 
                new Question
                {
                    Code = ID,
                    Name = RuleName,
                    Project = CheckProject.目录及文件规范性,
                    Description = e,
                    ShowType=ShowType.Folder,
                    Folder=ParameterManager.Folder
                }).ToList());
            #endregion
            #region  各个文件夹下面的文件是否丢失 是否缺失


            var tool = new FileStandard { Files = ParameterManager.ChildrenFiles };
            tool.Check();
            QuestionManager.AddRange(
                tool.List.Select(e => 
                new Question {
                    Code = "1102",
                    Name = RuleName,
                    Project = CheckProject.目录及文件规范性,
                    Description = string.Format(@"文件：{0}不存在,请去除多余的空格，并注意全角\半角符号符合规范", e),
                    ShowType=ShowType.Folder,
                    Folder=System.IO.Path.GetDirectoryName(e)
                }).ToList());
            //Parallel.ForEach(tools, tool =>
            //{
            //    tool.Check();
            //    QuestionManager.AddRange(tool.List.Select(e => new Question { Code = "1102", Name = RuleName, Project = CheckProject.目录及文件规范性, Description = e }).ToList());
            //});
            #endregion
        }
    }
}
