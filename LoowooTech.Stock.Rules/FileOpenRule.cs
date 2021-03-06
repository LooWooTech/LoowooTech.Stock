﻿using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 作用：核对提交的成果中的文件是否能够打开
    /// 备注：存在错误，可以定位打开文件夹
    /// </summary>
    public class FileOpenRule:IRule
    {
        public string RuleName { get { return "提交成果数据是否能够正常打开"; } }
        public string ID { get { return "1201"; } }
        public bool Space { get { return false; } }
        public void Check()
        {
            var nodes = XmlManager.GetList("/Folders/Folder", XmlEnum.DataTree);
            if (nodes != null && nodes.Count > 0)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    var name = node.Attributes["Name"].Value;
                    Check(System.IO.Path.Combine(ParameterManager.Folder,name), node);
                }
            }
        }

        private void Check(string path,XmlNode node)
        {
            var folders = node.SelectNodes("Folder");
            if (folders != null && folders.Count > 0)
            {
                for(var i = 0; i < folders.Count; i++)
                {
                    var child = folders[i];
                    var str = child.Attributes["Name"].Value;
                    Check(System.IO.Path.Combine(path, str),child);
                }
            }
            if (node.Attributes["Filter"]!= null)
            {
                var filters = node.Attributes["Filter"].Value.Split('/');
                foreach(var filter in filters)
                {
                    Check(path, filter);
                }
            }
           
        }
        private void Check(string folder,string filter)
        {
            var files = Directory.GetFiles(folder, filter);
            foreach(var file in files)
            {
                if (!Open(file))
                {
                    QuestionManager.Add(
                        new Question
                        {
                            Code = ID,
                            Name = RuleName,
                            Project = CheckProject.数据有效性,
                            Description = string.Format("文件：{0}不能打开，请核对", file),
                            ShowType = ShowType.Folder,
                            Folder = System.IO.Path.GetDirectoryName(file)
                        });
                }
            }
        }

        public bool Open(string filePath)
        {
            var ext = System.IO.Path.GetExtension(filePath);
            switch (ext)
            {
                case ".xml":
                    try
                    {
                        StreamReader reader = new StreamReader(filePath);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                case ".mdb":
                    try
                    {
                        var connection = new OleDbConnection(string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}", filePath));
                        return true;
                    }
                    catch
                    {
                        return false;
                    }

                case ".xls":

                    try
                    {
                        IWorkbook workbook = filePath.OpenExcel();
                        if (workbook == null)
                        {
                            return false;
                        }
                        ISheet sheet = workbook.GetSheetAt(0);
                        if (sheet == null)
                        {
                            return false;
                        }

                        return true;
                    }
                    catch
                    {
                        return false;
                    }

                default:
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        if (fs.Length == 0)
                        {
                            return false;
                        }
                    }
                    break;

            }
            return true;
        }
    }
}
