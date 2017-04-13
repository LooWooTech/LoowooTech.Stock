using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 检查数据完整性
    /// </summary>
    public class ResultComplete:BaseResult,IResult
    {
        public override string Name
        {
            get
            {
                return "成果完整性";
            }
        }

        /// <summary>
        /// 质检路径
        /// </summary>
        private string _folder { get; set; }
        /// <summary>
        /// 子文件夹
        /// </summary>
        private List<string> _children { get; set; }
        public List<string> Children { get { return _children; }set { _children = value; } }
        private List<string> _existPath { get; set; }
        //存在的文件夹路径
        public List<string> ExistPath { get { return _existPath; } }
        
        public ResultComplete(string folder)
        {
            _folder = folder;
            _existPath = new List<string>();
            
        }
        public override void Check()
        {

            if (_children.Count > 0)
            {
                foreach(var child in _children)
                {
                    var folder = System.IO.Path.Combine(_folder, child);//子文件夹全路径
                    if (!System.IO.Directory.Exists(folder))
                    {
                        _messages.Add(string.Format("缺失文件夹路径：{0}", child));
                    }
                    else
                    {
                        _existPath.Add(folder);
                        //FileTools.Add(new FileFolder(folder) { FileNames = tool.GetChildren(string.Format("/Folders/Folder@[Name='{0}']", child), Name),CityName=CityName,Code=Code });
                    }
                }
                
            }
            base.Check();
        }


    }
}
