using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    interface ITable
    {
        string _tableName { get; set; }
        string Name { get; }
        Dictionary<string,List<string>> Results { get; set; }
        List<string> Errors { get; set; }
        void Check(OleDbConnection connection);
    }
}
