using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoowooTech.Stock.Common
{
    public static class FolderExtensions
    {
        private static string[] _replaceArray = { "农村存量建设用地", "农村存量","农村存", "单位代码表.xl", "单位代码", "表1 ", "表2 ", "表3 ", "表4 ", "表5 ", "表6 ", "农村其它" };
        public static List<StockFile> GetFiles(string folder,string ext)
        {
            var list = new List<StockFile>();
            #region 读取相关文件
            DirectoryInfo di = new DirectoryInfo(folder);
            foreach(FileInfo file in di.GetFiles(ext))
            {
                var entry = new StockFile
                {
                    Path = folder,
                    Ext = file.Extension,
                    FileName = file.Name,
                    FullName = file.FullName
                };
                var fileNameString = file.Name.Substring(0,18);
                foreach(var replace in _replaceArray)
                {
                    fileNameString = fileNameString.Replace(replace, "");
                }
                var index = fileNameString.IndexOf("(");
                if (index > 0)
                {
                    entry.XZQDM = fileNameString.Substring(index+1, 6);
                    entry.XZQMC = fileNameString.Substring(0, index);
                }
               
                list.Add(entry);
            }
            #endregion


            #region  读取当前目录下的文件夹

            DirectoryInfo[] dis = di.GetDirectories();
            foreach(var childFolder in dis)
            {
                var result = GetFiles(childFolder.FullName,ext);
                if (result.Count > 0)
                {
                    list.AddRange(result);
                }
            }
            #endregion


            return list;
        }
    }
}
