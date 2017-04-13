using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Rules;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoowooTech.Stock
{
    /// <summary>
    /// 质检入口
    /// 作者：汪建龙
    /// 编写时间：2017年4月11日09:25:07
    /// </summary>
    public class Heart:IDisposable
    {
        public string Name { get { return "质检总入口"; } }
        /// <summary>
        /// 目录下文件夹信息
        /// </summary>
        private string _xmlDataFilePath { get; set; }
        public string XmlDataFilePath
        {
            get
            {
                return string.IsNullOrEmpty(_xmlDataFilePath) ? _xmlDataFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["DataTree"]) : _xmlDataFilePath;
            }
        }

        private XmlTool _dataTreeTool { get; set; }
        public XmlTool DataTreeTool
        {
            get
            {
                return _dataTreeTool == null ? _dataTreeTool = new XmlTool(XmlDataFilePath) : _dataTreeTool;
            }
        }
        /// <summary>
        /// 行政区划代码信息
        /// </summary>
        private string _xmlCityFilePath { get; set; }
        public string XmlCityFilePath
        {
            get
            {
                return string.IsNullOrEmpty(_xmlCityFilePath) ? _xmlCityFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["CodeCity"]) : _xmlCityFilePath;
            }
        }
        private XmlTool _cityTool { get; set; }
        public XmlTool CityTool
        {
            get
            {
                return _cityTool == null ? _cityTool = new XmlTool(XmlCityFilePath) : _cityTool;
            }
        }
        private string _folder { get; set; }
        private List<string> _messages { get; set; }
        public List<string> Messages { get { return _messages; } }
        private List<IResult> _results { get; set; }

        protected const string Title = "农村存量建设用地调查数据成果";
        protected const string DataBase = "1空间数据库";
        protected const string Report = "5质检报告";

        private string _reportPath { get; set; }
        public string ReportPath { get { return string.IsNullOrEmpty(_reportPath) ? _reportPath = System.IO.Path.Combine(_folder, Report) : _reportPath; } }
        private string _cityName { get; set; }
        public string _cityCode { get; set; }
        private List<IFolder> _folderTool { get; set; }
        public Heart(string folder)
        {
            _folder = folder;
            Init();
        }

        public void Init()
        {
            _messages = new List<string>();
            _results = new List<IResult>();
            _folderTool = new List<IFolder>();
           
        }

        /// <summary>
        /// 作用：检查当前路径正确性  路径是否存在 路径中的行政区划名称和行政区划代码信息
        /// 作者：汪建龙
        /// 编写时间：2017年4月11日09:46:53
        /// </summary>
        private bool CheckFolder()
        {
            if (!System.IO.Directory.Exists(_folder))
            {
                _messages.Add(string.Format("质检路径：{0}不存在", _folder));
                return false;
            }
            DirectoryInfo info = new DirectoryInfo(_folder);
            var folderName=info.Name.Replace(Title, "").Replace("（", ",").Replace("）", "").Replace("(", ",").Replace(")", "");
            var arrays = folderName.Split(',');
            if (arrays.Length == 2)
            {
                _cityName = arrays[0];
                _cityCode = arrays[1];
                if (!CityTool.Exist(string.Format("/Citys/City[@Code='{0}'][@Name='{1}']", _cityCode, _cityName)))
                {
                    _messages.Add("未查询到相关行政区划以及行政区划代码");
                    return false;
                }
                return true;
            }
            _messages.Add("质检路径命名要求不符");
            return false;

        }

        public void Program()
        {
            //路径核对
            if (!CheckFolder())
            {
                Dispose();
                return;
            }
            var resultComplete = new ResultComplete(_folder) { Children = DataTreeTool.Get("/Folders/Folder", "Name") };//数据完整性
            resultComplete.Check();//核对质检数据文件夹下面的文件夹是否存在
            _messages.AddRange(resultComplete.Messages);

            _folderTool.AddRange(resultComplete.ExistPath.Select(e => new FileFolder(e) {
                FileNames = DataTreeTool.GetChildren(string.Format("/Folders/Folder[@Name='{0}']", new DirectoryInfo(e).Name), "Name"),
                ReportPath=ReportPath,
                CityName = _cityName,
                Code = _cityCode }));
            foreach(var tool in _folderTool)
            {
                tool.Check();//核对每个文件夹下面的文件是否存在  是否能够打开
            }
            var path = System.IO.Path.Combine(_folder, DataBase);
            if (System.IO.Directory.Exists(path))
            {
                var mdbfiles = DialogClass.GetSpecialFiles(path, "*.mdb");//获取空间数据的mdb文件
                if (mdbfiles.Count == 0)
                {
                    _messages.Add("空间数据库文件夹下未找到相关*.mdb文件");
                }
                else
                {
                    var currentMdbFile = string.Empty;
                    foreach (var item in mdbfiles)
                    {
                        var info = new FileInfo(item);
                        if (Regex.IsMatch(info.Name, @"^[\u4e00-\u9fa5]+\(\d{6}\)农村存量建设用地调查成功空间数据库.mdb$"))
                        {
                            currentMdbFile = item;
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(currentMdbFile))
                    {
                        _messages.Add(string.Format("未找到空间数据库文件"));
                    }
                    else
                    {
                        TableHeart.Program(currentMdbFile);

                        var gisheart = new ArcGISHeart() { MDBFilePath = currentMdbFile, FeatureClassNames = XmlClass.GetRequireTables() };
                        gisheart.Program();

                    }
                }
            }
            else
            {
                _messages.Add("不存在文件夹路径" + path+",故无法进行空间数据库核查");
            }
           
            Dispose();
          
        }
        public void Dispose()
        {
            Name.Write(_messages,ReportPath);
        }
    }
}
