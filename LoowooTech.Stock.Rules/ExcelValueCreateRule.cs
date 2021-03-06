﻿using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class ExcelValueCreateRule
    {
        private string _saveFolder { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SaveFolder { get { return _saveFolder; }set { _saveFolder = value; } }
        public string ID { get; set; }
        public void Write()
        {
            var Time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            var tools = new List<IExcel>
            {
                new ExcelSix {MStartLine=6,SaveFolder=SaveFolder,Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID,TitleName=string.Format("{0}({1})农村存量建设用地情况汇总表 {2}",ParameterManager.District,ParameterManager.Code,Time) },
                new ExcelOne { MStartLine=3, SaveFolder=SaveFolder, Connection = ParameterManager.Connection, List = ExcelManager.XZQ, District = ParameterManager.District, Code = ParameterManager.Code, Folder = ParameterManager.CollectFolder,CheckCode=ID,TitleName=string.Format("{0}({1})农村存量建设用地总体情况汇总表 {2}",ParameterManager.District,ParameterManager.Code,Time) },
                new ExcelTwo { MStartLine=5, SaveFolder=SaveFolder,Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID,TitleName=string.Format("{0}({1})农村存量宅基地基本情况汇总表 {2}",ParameterManager.District,ParameterManager.Code,Time) },
                new ExcelThree { MStartLine=5, SaveFolder=SaveFolder, Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID,TitleName=string.Format("{0}({1})农村存量经营性建设用地基本情况汇总表 {2}",ParameterManager.District,ParameterManager.Code,Time)},
                new ExcelFour { MStartLine=5, SaveFolder=SaveFolder,Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID,TitleName=string.Format("{0}({1})农村存量公共管理及公共服务设施用地基本情况汇总表 {2}",ParameterManager.District,ParameterManager.Code,Time)},
                new ExcelFive { MStartLine=5, SaveFolder=SaveFolder,Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID,TitleName=string.Format("{0}({1})农村其它存量建设用地基本情况汇总表 {2}",ParameterManager.District,ParameterManager.Code,Time)}
                
            };

            Parallel.ForEach(tools, tool =>
            {
                tool.Write();
                QuestionManager.AddRange(tool.ParalleQuestions.ToList());
            });
        }
    }
}
