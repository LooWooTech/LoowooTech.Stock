using LoowooTech.Stock.Models;
using System;
using System.Collections.Concurrent;
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
        ConcurrentBag<Question> ParalleQuestions { get; }
        List<XZC> List { get; set; }
        Dictionary<string,List<FieldValue>> Dict { get; }
        string Code { get; set; }
        string District { get; set; }
        string Folder { get; set; }
        void Check();
    }
}
