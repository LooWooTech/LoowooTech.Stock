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
        /// 质检起始文件夹路径
        /// </summary>
        private string _folder { get; set; }
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
        private string _reportPath { get; set; }
        /// <summary>
        /// 质检报告路径
        /// </summary>
        public string ReportPath { get { return _reportPath; } set { _reportPath = value; } }


        private List<string> _messages { get; set; }
        public List<string> Messages { get { return _messages; } }
        private List<IResult> _results { get; set; }
        private string[] _ids { get; set; }
        public string[] IDS { get { return _ids; }set { _ids = value; } }

        protected const string Title = "农村存量建设用地调查数据成果";
        protected const string DataBase = "1空间数据库";


        private string _cityName { get; set; }
        public string _cityCode { get; set; }
        private List<IFolder> _folderTool { get; set; }




        /// <summary>
        /// 作用：检查当前路径正确性  路径是否存在 路径中的行政区划名称和行政区划代码信息
        /// 作者：汪建龙
        /// 编写时间：2017年4月11日09:46:53
        /// </summary>
        private bool CheckFolder()
        {
            if (!System.IO.Directory.Exists(_folder))
            {
                Console.WriteLine(string.Format("质检路径：{0}不存在", _folder));
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
                if (!XmlManager.Exist(string.Format("/Citys/City[@Code='{0}'][@Name='{1}']", _cityCode, _cityName),XmlEnum.City))
                {
                    Console.WriteLine("未查询到相关行政区划以及行政区划代码");
                    _messages.Add("未查询到相关行政区划以及行政区划代码");
                    return false;
                }
                return true;
            }
            Console.WriteLine("质检路径命名要求不符");
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
            var resultComplete = new ResultComplete(_folder) { Children = XmlManager.Get("/Folders/Folder", "Name",XmlEnum.DataTree) };//数据完整性
            resultComplete.Check();//核对质检数据文件夹下面的文件夹是否存在
            _messages.AddRange(resultComplete.Messages);

            _folderTool.AddRange(resultComplete.ExistPath.Select(e => new FileFolder() {
                Folder=e,
                FileNames =XmlManager.GetChildren(string.Format("/Folders/Folder[@Name='{0}']", new DirectoryInfo(e).Name), "Name",XmlEnum.DataTree),
                //ReportPath=ReportPath,
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
                        TableHeart.Program(currentMdbFile,IDS);

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
