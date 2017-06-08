using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Rules;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.WorkBench
{
    public class WorkBench2
    {
        protected const string report = "5质检报告";
        protected const string DataBase = "1空间数据库";
        protected const string Collect = "3统计报告";
        protected const string Title = "农村存量建设用地调查数据成果";

        private string _folder { get; set; }
        /// <summary>
        /// 质检文件夹路径
        /// </summary>
        public string Folder { get { return _folder; }set { _folder = value; } }
        private string _codeFile { get; set; }
        /// <summary>
        /// 单位代码表文件路径
        /// </summary>
        public string CodeFile { get { return _codeFile; } }
        private string _mdbFile { get; set; }
        /// <summary>
        /// 数据库文件路径
        /// </summary>
        public string MDBFile { get { return _mdbFile; } }
        /// <summary>
        /// 作用：通过质检路径获取行政区代码以及行政区名称信息
        /// </summary>
        /// <returns></returns>
        private bool ReadXZQ()
        {
            var info = new DirectoryInfo(_folder);
            var array = info.Name.Replace(Title, "").Replace("（", ",").Replace("）", "").Replace("(", ",").Replace(")", "").Split(',');
            if (array.Length == 2)
            {
                ParameterManager.District = array[0];
                ParameterManager.Code = array[1];
                return true;
            }
            return false;
        }
        /// <summary>
        /// 作用：找寻单位代码表以及数据库文件，都找到返回true
        /// </summary>
        /// <returns></returns>
        private bool SearchFile()
        {
            var path = System.IO.Path.Combine(Folder, DataBase);
            var codeFileTool = new FileTool() { Folder = path, Filter = "*.xls", RegexString = @"^[\u4e00-\u9fa5]+\(\d{6}\)单位代码表.xls$" };
            _codeFile = codeFileTool.GetFile();
            var mdbfileTool = new FileTool() { Folder = path, Filter = "*.mdb", RegexString = @"^[\u4e00-\u9fa5]+\(\d{6}\)农村存量建设用地调查成功空间数据库.mdb$" };
            _mdbFile = mdbfileTool.GetFile();

            return !string.IsNullOrEmpty(_codeFile) && !string.IsNullOrEmpty(_mdbFile) && System.IO.File.Exists(_codeFile) && System.IO.File.Exists(_mdbFile);
        }
        private List<IRule> _rules { get; set; }
        
        private void InitRules()
        {
            _rules = new List<IRule>();
            _rules.Add(new VectorRule { ID = "2101", RuleName = "矢量图层是否完整，是否符合《浙江省农村存量建设用地调查数据库标准》的要求" });
            _rules.Add(new CoordinateRule { ID = "2201", RuleName = "平面坐标系统是否采用‘1980’西安坐标系、3度带、带带号，检查高程系统是否采用‘1985’国家高程基准，检查投影方式是否采用高斯-克吕格投影" });
            _rules.Add(new StructureRule { ID = "3101", RuleName = "检查图层名称、图层中属性字段的数量和属性字段代码、类型、长度、小数位数是否符合《浙江省农村存量建设用地调查数据库标注》要求" });
            _rules.Add(new ValueRule { ID = "3201", RuleName = "属性字段的值是否符合《浙江省农村存量建设用地调查数据库标准》规定的值域范围" });
            _rules.Add(new XZCDMRule { ID = "3301", RuleName = "行政区编码一致性检查" });
            _rules.Add(new BSMRule { ID = "3302", RuleName = "标识码唯一性检查" });
            _rules.Add(new TBAreaRule { ID = "3401", RuleName = "数据库计算面积与属性填写面积一致性" });
            _rules.Add(new TBBHRule { ID = "5101", RuleName = "图层内属性一致性" });



            _rules.Add(new DirectoryFileRule { ID = "", RuleName = "" });
        }
        private void Init()
        {
            if (!ReadXZQ())
            {
                LogManager.LogRecord("无法解析到行政区代码和行政区名称");
            }
            if (!SearchFile())
            {
                LogManager.LogRecord("未找到单位代码表文件或者数据库文件");
            }
            else
            {
                ExcelManager.Init(_codeFile);
            }
            DCDYTBManager.Init(ParameterManager.Connection);//获取DCDYTB中的信息;

        }
        public void Program()
        {
            QuestionManager.Clear();
            Init();


        }
    }
}
