﻿using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Rules;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoowooTech.Stock.WorkBench
{
    public class WorkBench
    {
        protected const string report = "5质检报告";
        protected const string DataBase = "1空间数据库";
        private string _folder { get; set; }
        /// <summary>
        /// 质检路径
        /// </summary>
        public string Folder { get { return _folder; }set { _folder = value; } }
        /// <summary>
        /// 质检报告路径
        /// </summary>
        private string _reportPath { get; set; }
        public string ReportPath { get { return string.IsNullOrEmpty(_reportPath) ? _reportPath = System.IO.Path.Combine(Folder, report) : _reportPath; } }

        private List<IFolder> _folderTools { get; set; }

        public WorkBench()
        {
            _folderTools = new List<IFolder>();
        }


        public void Program()
        {
            if (!System.IO.Directory.Exists(Folder))
            {
                Console.WriteLine(string.Format("质检路径不存在：{0}，请核对！", Folder));
                return;
            }
            var folderTool = new FolderTool { Folder = Folder };//对质检路径进行命名规范检查
            if (!folderTool.Check())
            {
                return;
            }
            var resultComplete = new ResultComplete(Folder) { Children = XmlManager.Get("/Folders/Folder", "Name", XmlEnum.DataTree) };
            resultComplete.Check();//对质检路径下的文件夹、文件是否存在，是否能够打开进行检查
            _folderTools.AddRange(resultComplete.ExistPath.Select(e => new FileFolder()
            {
                Folder = e,
                FileNames = XmlManager.GetChildren(string.Format("/Folders/Folder[@Name='{0}']", new DirectoryInfo(e).Name), "Name", XmlEnum.DataTree),
                CityName = folderTool.CityName,
                Code = folderTool.Code
            }));
            foreach(var tool in _folderTools)
            {
                tool.Check();//验证每个文件夹下应存在的文件
            }
            var path = System.IO.Path.Combine(Folder, DataBase);
            //获取空间数据库文件夹下的单位代码表文件，并获取单位代码信息
            var codefileTool = new FileTool { Folder = path, Filter = "*.xls", RegexString = @"^[\u4e00-\u9fa5]+\(\d{6}\)单位代码表.xls$" };
            var currentCodeFile = codefileTool.GetFile();
            if (string.IsNullOrEmpty(currentCodeFile))
            {
                Console.WriteLine("未识别到单位代码表文件，请核对空间数据库文件下的文件");
            }
            else
            {
                ExcelManager.Init(currentCodeFile);
            }
            //获取空间数据库文件夹下的空间数据库文件，并对数据库进行检查
            var mdbfileTool = new FileTool() { Folder = path, Filter = "*.mdb", RegexString = @"^[\u4e00-\u9fa5]+\(\d{6}\)农村存量建设用地调查成功空间数据库.mdb$" };
            var currentMdbFile = mdbfileTool.GetFile();
            if (string.IsNullOrEmpty(currentMdbFile))
            {
                Console.WriteLine("未识别到数据库文件,请核对农村存量建设用地调查成功空间数据库.mdb文件");
            }
            else
            {
                TableHeart.Program(currentMdbFile);
                var gisheart = new ArcGISHeart() { MDBFilePath = currentMdbFile, FeatureClassNames = XmlManager.Get("/Tables/Table[@IsSpace='true']", "Name", XmlEnum.Field) };
                gisheart.Program();
            }

            Console.WriteLine("结束");


        }
    }
}
