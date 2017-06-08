using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 检查成果数据是否满足《浙江省农村存量建设用地调查数据成果规范》对目录和文件命名要求
    /// </summary>
    public class DirectoryFileRule:IRule
    {
        private string _folder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Folder { get { return _folder; } set { _folder = value; } }
        public string RuleName { get; set; }
        public string ID { get; set; }

        public void Check()
        {
            var resultComplete = new ResultComplete(Folder) { Children = XmlManager.Get("/Folders/Folder", "Name", XmlEnum.DataTree) };
            resultComplete.Check();
            QuestionManager.AddRange(resultComplete.Messages.Select(e => new Question { Code = "1102", Name = "成果数据丢露", Project = CheckProject.目录及文件规范性, Description = e }).ToList());

        }
    }
}
