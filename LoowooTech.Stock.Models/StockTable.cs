using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class StockTable
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsSpace { get; set; }
        public string Type { get; set; }
        public List<Field> Fields { get; set; }
    }
}
