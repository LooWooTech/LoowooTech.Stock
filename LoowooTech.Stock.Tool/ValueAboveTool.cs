using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueAboveTool:ITool
    {
        public string TableName { get; set; }
        public string RelationName { get; set; }
        public string Name { get; }
        public string ID { get; set; }
        public List<string> Messages { get; set; }
        public bool Check(OleDbConnection connection)
        {

            return false;
        }
    }
}
