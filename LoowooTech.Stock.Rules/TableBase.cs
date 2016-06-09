using LoowooTech.Stock.Tool;
using System.Collections.Generic;

namespace LoowooTech.Stock.Rules
{
    public class TableBase
    {
        protected string _tableName { get; set; }
        protected string _key { get; set; }
        protected List<ITool> list { get; set; }
        public TableBase()
        {
            list = new List<ITool>();
        }
    }
}
