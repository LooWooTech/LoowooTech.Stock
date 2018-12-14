using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class VillageExcel
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Title { get; set; }

        public List<ExcelField2> Fields { get; set; }
    }
}
