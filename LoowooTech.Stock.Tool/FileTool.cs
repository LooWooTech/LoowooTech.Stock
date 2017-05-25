using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 作用：在文件夹下获取指定特定文件
    /// 作者：汪建龙
    /// 编写时间：2017年5月2日17:18:51
    /// </summary>
    public class FileTool
    {
        private string _folder { get; set; }
        /// <summary>
        /// 文件夹路径
        /// </summary>
        public string Folder { get { return _folder; }set { _folder = value; } }
        private string _filter { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string Filter { get { return _filter; } set { _filter = value; } }

        private string _regexString { get; set; }
        /// <summary>
        /// 匹配符
        /// </summary>
        public string RegexString { get { return _regexString; }set { _regexString = value; } }

        public string GetFile()
        {
            var currentFile = string.Empty;
            if (!System.IO.Directory.Exists(Folder))
            {
                Console.WriteLine("不存在文件夹:" + Folder);
                return currentFile;
            }
            var files = DialogClass.GetSpecialFiles(Folder, Filter);
            if (files.Count == 0)
            {
                Console.WriteLine(string.Format("文件夹：{0}下不存在{1}的文件", Folder, Filter));
                return currentFile;
            }
            foreach(var item in files)
            {
                var info = new FileInfo(item);
                if (Regex.IsMatch(info.Name, RegexString))
                {
                    currentFile = item;
                    break;
                }
            }
            return currentFile;
        }

    }
}
