using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class BaseResult
    {
        public virtual string Name { get; }
        protected List<string> _messages { get; set; }
        public List<string> Messages { get { return _messages; } }
        
        public BaseResult()
        {
            _messages = new List<string>();
        }

        public virtual void Check()
        {
           
        }
    }
}
