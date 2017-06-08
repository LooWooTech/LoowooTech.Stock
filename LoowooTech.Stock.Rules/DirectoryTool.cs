using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class DirectoryTool
    {
        private string _folder { get; set; }
        public string Folder { get { return _folder; }set { _folder = value; } }
        private List<string> _children { get; set; }
        /// <summary>
        /// 子目录
        /// </summary>
        public List<string> Children { get { return _children; }set { _children = value; } }
        private List<string> _messages { get; set; }
        public List<string> Messages { get { return _messages; } }
        public DirectoryTool()
        {
            _messages = new List<string>();
        }
        
        public void Check()
        {
            var info = string.Empty;
            foreach(var child in Children)
            {
                var fullPath = System.IO.Path.Combine(Folder, child);
                if (!System.IO.Directory.Exists(fullPath))
                {
                    info = string.Format("不存在文件夹：{0}", child);
                }
            }
        }
    }
}
