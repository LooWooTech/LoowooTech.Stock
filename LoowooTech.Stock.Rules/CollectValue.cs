using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class CollectValue
    {
        private string _sourceFolder { get; set; }
        /// <summary>
        /// 原始读取路径
        /// </summary>
        public string SourceFolder { get { return _sourceFolder; }set { _sourceFolder = value; } }

        public void Collect()
        {
            var tools = new List<IExcel>
            {
                new ExcelOne { CollectFolder=SourceFolder,CollectType=CollectType.MDB,CollectXZQ=ParameterManager.CollectXZQ}
            };

            foreach(var tool in tools)
            {
                tool.CollectWrite();
            }
        }
    }
}
