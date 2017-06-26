using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class FileStandard
    {
        //private string _folder { get; set; }
        //public string Folder { get { return _folder; }set { _folder = value; } }
        private List<string> _files { get; set; }
        public List<string> Files { get { return _files; }set { _files = value; } }
        private ConcurrentBag<string> _list { get; set; }
        public ConcurrentBag<string> List { get { return _list == null ? _list = new ConcurrentBag<string>() : _list; } }
        private void Check(string filePath)
        {
            //var array = new string[] { filePath, filePath.Replace("(", "（").Replace(")", "）") };
            //foreach(var item in array)
            //{
            //    if (System.IO.File.Exists(item))
            //    {
            //        return;
            //    }
            //}
            if (!System.IO.File.Exists(filePath))
            {
                List.Add(string.Format(@"文件：{0}不存在,请去除多余的空格，并注意全角\半角符号符合规范", filePath));
            }
          
        }
        public void Check()
        {
            foreach(var file in Files)
            {
                var str = file;
                if (file.Contains("{n}"))
                {
                    foreach(var xzc in ExcelManager.XZQ)
                    {
                        str = file;
                        var fullPath = str.Replace("{n}","").Replace("{Name}", xzc.XZCMC).Replace("{Code}", xzc.XZCDM);
                        Check(fullPath);
                    }
                }
                else
                {
                    var fullPath = str.Replace("{Name}", ParameterManager.District).Replace("{Code}", ParameterManager.Code);
                    Check(fullPath);
                }
            }
        }
        


    }
}
