using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class VillageTable
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsSpace { get; set; }
        public bool Topo { get; set; }

        public List<VillageField> Fields { get; set; }
    }
}
