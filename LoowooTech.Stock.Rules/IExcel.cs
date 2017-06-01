using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    interface IExcel
    {
        string ExcelName { get; set; }
        string TableName { get; }
        void Check(OleDbConnection connection);
    }
}
