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
            var tools = new List<IExcel>
            {
                 new ExcelOne { SaveFolder=SaveFolder, Connection = ParameterManager.Connection, List = ExcelManager.XZQ, District = ParameterManager.District, Code = ParameterManager.Code, Folder = ParameterManager.CollectFolder,CheckCode=ID },
                new ExcelTwo { SaveFolder=SaveFolder,Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID },
                new ExcelThree { SaveFolder=SaveFolder, Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID},
                new ExcelFour { SaveFolder=SaveFolder,Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID},
                new ExcelFive { SaveFolder=SaveFolder,Connection=ParameterManager.Connection,List=ExcelManager.XZQ,District=ParameterManager.District,Code=ParameterManager.Code,Folder=ParameterManager.CollectFolder,CheckCode=ID}
            };
            Parallel.ForEach(tools, tool =>
            {
                tool.Write();
                QuestionManager.AddRange(tool.ParalleQuestions.ToList());
            });
        }
    }
}
