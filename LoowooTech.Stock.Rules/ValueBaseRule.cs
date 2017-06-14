using LoowooTech.Stock.ArcGISTool;
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
        public virtual string RuleName { get; }
        public virtual string ID { get; }
        public bool Space { get { return false; } }
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
