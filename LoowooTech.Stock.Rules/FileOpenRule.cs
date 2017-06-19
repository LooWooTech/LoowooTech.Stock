using LoowooTech.Stock.ArcGISTool;
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
                    Check(name, ParameterManager.Folder, node);
                }
            }
        }

        private void Check(string name,string path,XmlNode node)
        {
            var folders = node.SelectNodes("/Folder");
            if (folders != null && folders.Count > 0)
            {
                for(var i = 0; i < folders.Count; i++)
                {
                    var child = folders[i];
                    var str = child.Attributes["Name"].Value;
                    Check(str, System.IO.Path.Combine(path, name),child);
                }
            }
            //var files = node.SelectNodes("/File");
            if (node.Attributes["Filter"]!= null)
            {
                var filters = node.Attributes["Filter"].Value.Split('/');
                Parallel.ForEach(filters, filter =>
                {
                    Check(path, filter);
                });
            }
           
        }
        private void Check(string folder,string filter)
        {
            var files = Directory.GetFiles(folder, filter);
            Parallel.ForEach(files, file =>
            {
                if (!Open(file))
                {
                    QuestionManager.Add(new Question { Code = ID, Name = RuleName, Project = CheckProject.数据有效性, Description = string.Format("文件：{0}不能打开，请核对", file) });
                }
            });
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
                   // return true;

            }
            return true;
        }
    }
}
