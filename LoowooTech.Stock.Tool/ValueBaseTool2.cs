using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueBaseTool2
    {
        public string ID { get; set; }
        public string TableName { get; set; }
        public string RelationName { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public List<VillageMessage> Messages2 { get; set; } = new List<VillageMessage>();
        public string Key { get; set; }

    }
}
