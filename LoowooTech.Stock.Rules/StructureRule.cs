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
        public string RuleName { get { return "检查图层名称、图层中属性字段的数量和属性字段代码、类型、长度、小数位数是否符合《浙江省农村存量建设用地调查数据库标注》要求"; } }
        public string ID { get { return "3101"; } }
        public bool Space { get { return false; } }
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
