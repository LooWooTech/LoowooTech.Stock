using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 作用：核对每张表的字段，属性，长度，小数位数
    /// </summary>
    public class StructureRule:IRule
    {
        public string RuleName { get; set; }
        public string ID { get; set; }
        public void Check()
        {
            Parallel.ForEach(ParameterManager.TableNames, table =>
            {
                var tool = new FieldStructureTool { TableName = table, ID = "3101" };
                tool.Check(ParameterManager.Connection);
            });
        }
    }
}
