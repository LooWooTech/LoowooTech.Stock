using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 作用：对质检路径进行命名规范、行政区代码、行政区名称是否一致
    /// 作者：汪建龙
    /// 编写时间：2017年5月2日14:45:17
    /// </summary>
    public class FolderTool
    {
        private const string Title = "农村存量建设用地调查数据成果";
        private string _folder { get; set; }
        /// <summary>
        /// 路径名称
        /// </summary>
        public string Folder { get { return _folder; }set { _folder = value; } }

        private string _cityName { get; set; }
        /// <summary>
        /// 行政区名称
        /// </summary>
        public string CityName { get { return _cityName; } private set { _cityName = value; } }

        private string _code { get; set; }
        /// <summary>
        /// 行政区代码
        /// </summary>
        public string Code { get { return _code; }  private set { _code = value; } }

        public bool Check()
        {
            DirectoryInfo info = new DirectoryInfo(Folder);
            var folderName=info.Name.Replace(Title,"").Replace("（", ",").Replace("）", "").Replace("(", ",").Replace(")", "");
            var array = folderName.Split(',');
            if (array.Length == 2)
            {
                CityName = array[0];
                Code = array[1];
                if (!XmlManager.Exist(string.Format("/Citys/City[@Code='{0}'][@Name='{1}']", CityName, Code), XmlEnum.City))
                {
                    var str = string.Format("未查询到行政区名称：{0}；行政区代码：{1}的相关记录！", CityName, Code);
                    LogManager.Record(str);
                    QuestionManager.Add(new Models.Question { Code = "1101", Name = "质检路径命名规则", Description = str });
                    return false;
                }
                return true;
            }
            QuestionManager.Add(new Models.Question { Code = "1101", Name = "质检路径命名规则", Description = "无法解析行政区名称、行政区代码信息" });
            LogManager.Log("无法解析行政区名称、行政区代码信息");
            return false;
        }
    }
}
