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

        
        public List<string> FileNames { get; set; }
        public string CityName { get; set; }//行政区名称
        public string Code { get; set; }//行政区代码

        private List<string> _messages { get; set; }
        private string _reportPath { get; set; }
        public string ReportPath { get { return _reportPath; }set { _reportPath = value; } }

        public FileFolder(string folder)
        {
            _folder = folder;
            _messages = new List<string>();
        }
        public bool Check()
        {
            foreach(var file in FileNames)
            {
                var fullPath = System.IO.Path.Combine(_folder, file.Replace("{Name}",CityName).Replace("{Code}",Code));
                if (!System.IO.File.Exists(fullPath))
                {
                    _messages.Add(string.Format("文件路径:{0}不存在，请核对", fullPath));
                    return false;
                }
                if (!Open(fullPath))
                {
                    _messages.Add(string.Format("文件：{0}无法打开", fullPath));
                }
            }
            Name.Write(_messages,ReportPath);
            return _messages.Count==0;
        }

        /// <summary>
        /// 作用：验证文件是否能打开  主要对.xml .mdb .xls文件进行验证
        /// 作者：汪建龙
        /// 编写时间：2017年4月7日09:26:42
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
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

            }
            return true;
        }
    }
}
