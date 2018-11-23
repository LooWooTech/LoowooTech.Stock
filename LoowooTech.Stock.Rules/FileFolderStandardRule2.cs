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
    public class FileFolderStandardRule2:IRule
    {
        public string RuleName { get { return "目录及文件规范性"; } }
        public string ID { get { return "1101"; } }
        public bool Space { get { return false; } }
        public void Check()
        {
            #region 检查质检目录下的所有文件夹是否存在
            var resultComplete = new ResultComplete(ParameterManager2.Folder) { Children = ParameterManager2.Folders };
            resultComplete.Check();

            QuestionManager2.AddRange(resultComplete.Messages.Select(e =>
            new Question2
            {
                Code = "110101",
                Name = RuleName,
                CheckProject = CheckProject2.目录及文件规范性,
                Description = e,
                Folder = ParameterManager2.Folder
            }).ToList());
            #endregion

            var tool = new FileStandard { Files = ParameterManager2.Files };
            tool.Check2();
            QuestionManager2.AddRange(tool.List.Select(e => new Question2
            {
                Code = "110102",
                Name = RuleName,
                CheckProject = CheckProject2.目录及文件规范性,
                Description = e,
                Folder = ParameterManager2.Folder
            }
             ).ToList());
        }
    }
}
