using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueBaseTool
    {
        public string TableName { get; set; }
        public string ID { get; set; }
        public List<string> Messages { get; set; }
        protected List<Question> _questions { get; set; }
        public ValueBaseTool()
        {
            Messages = new List<string>();
            _questions = new List<Question>();
        }
    }
}
