﻿using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class ValueBaseRule:IRule
    {
        protected const string _key = "BSM";
        private List<ITool> _tools { get; set; }
        public List<ITool> Tools { get { return _tools == null ? _tools = new List<ITool>() : _tools; } }
        public string RuleName { get; set; }
        public string ID { get; set; }
        public virtual void Init()
        {

        }
        public void Check()
        {
            Init();
            Parallel.ForEach(Tools, tool =>
            {
                tool.Check(ParameterManager.Connection);
            });
        }
    }
}
