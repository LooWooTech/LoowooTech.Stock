using LoowooTech.Stock.Models;
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
        string Name { get; }
        List<Question> Questions { get; }
        List<XZC> List { get; set; }
        Dictionary<XZC,List<ExcelField>> Dict { get; }
        string Code { get; set; }
        string District { get; set; }
        string Folder { get; set; }
        void Check(OleDbConnection connection);
    }
}
