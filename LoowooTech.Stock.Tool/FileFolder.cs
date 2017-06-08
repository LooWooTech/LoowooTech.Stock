using LoowooTech.Stock.Common;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 作用：验证文件里下面应该存在哪些文件并且可以打开
    /// 作者：汪建龙
    /// 编写时间：2017年4月7日09:30:06
    /// </summary>
    public class FileFolder:IFolder
    {
        public string Name
        {
            get
            {
                return string.Format("{0}目录检查",new DirectoryInfo(_folder).Name);
            }
        }
        private string _folder { get; set; }
        /// <summary>
        /// 文件夹路径
        /// </summary>
        public string Folder { get { return _folder; }set { _folder = value; } }

        /// <summary>
        /// 当前文件夹下要求存在的文件名
        /// </summary>
        public List<string> FileNames { get; set; }
        public string CityName { get; set; }//行政区名称
        public string Code { get; set; }//行政区代码

        private List<string> _messages { get; set; }

        public FileFolder()
        {
            _messages = new List<string>();
        }
        public bool Check()
        {
            foreach(var file in FileNames)
            {
                var fullPath = System.IO.Path.Combine(_folder, file.Replace("{Name}",CityName).Replace("{Code}",Code));
                var str = string.Empty;
                if (!System.IO.File.Exists(fullPath))
                {
                    str = string.Format("文件路径：{0}不存在，请核对", fullPath);
                    LogManager.Log(str);
                    _messages.Add(str);
                    QuestionManager.Add(new Models.Question { Code = "1102", Name = "文件",Project=Models.CheckProject.目录及文件规范性, Description = str });
                    return false;
                }
            }
            return _messages.Count==0;
        }
    }
}
