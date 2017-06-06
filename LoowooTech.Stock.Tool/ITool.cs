using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public interface ITool
    {
        string TableName { get; set; }
        string Name { get; }
        string ID { get; set; }
        List<string> Messages { get; set; }
        bool Check(OleDbConnection connection);
    }
}
