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
        private static string[] _keyHead = { "表1", "表2", "表3", "表4", "表5", "表6" };
        public static List<StockFile> GetExcelFiles(string folder,string ext)
        {
            var list = new List<StockFile>();
            #region 读取文件夹
            DirectoryInfo di = new DirectoryInfo(folder);
            foreach(FileInfo file in di.GetFiles(ext))
            {
                var fileName = file.Name;
                var key = fileName.Substring(0, 2);
                if (_keyHead.Contains(key))
                {
                    var entry = new StockFile
                    {
                        Path = folder,
                        Ext = file.Extension,
                        FileName = file.Name,
                        FullName = file.FullName,
                        TableName = key
                    };
                    var index = fileName.IndexOf("(");
                    if (index > 0)
                    {
                        entry.XZQDM = fileName.Substring(index + 1, 6);
                        entry.XZQMC = fileName.Substring(3, index - 3);
                    }
                    list.Add(entry);

                }
            }
            #endregion

            #region  读取当前目录下的文件夹

            DirectoryInfo[] dis = di.GetDirectories();
            foreach (var childFolder in dis)
            {
                var result = GetExcelFiles(childFolder.FullName, ext);
                if (result.Count > 0)
                {
                    list.AddRange(result);
                }
            }
            #endregion

            return list;
        }
        public static List<string> GetFiles2(string folder,string[] exts)
        {
            var list = new List<string>();
            DirectoryInfo di = new DirectoryInfo(folder);
            foreach(var ext in exts)
            {
                foreach (FileInfo file in di.GetFiles(ext))
                {
                    list.Add(file.FullName);
                }
            }
           
            return list;
        }

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
