﻿using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class TBAreaRule:IRule
    {
        public string RuleName { get { return "数据库计算面积与属性填写面积一致性"; } }
        public string ID { get { return "3401"; } }
        public bool Space { get { return false; } }
        public void Check()
        {
            var tools = new List<GainAreaTool>()
            {
                    new GainAreaTool { TableName="CLZJD",AreaFields=new string[] { "JZZDMJ", "FSYDMJ" } ,Denominator=10000},
                    new GainAreaTool { TableName="JYXJSYD",AreaFields=new string[] { "JSYDMJ" },Denominator=10000 },
                    new GainAreaTool { TableName="GGGL_GGFWSSYD",AreaFields=new string[] { "JSYDMJ" },Denominator=10000 },
                    new GainAreaTool { TableName="QTCLJSYD",AreaFields=new string[] { "JSYDMJ" },Denominator=10000 }
            };
            try
            {
                Parallel.ForEach(tools, tool =>
                {
                    tool.Gain(ParameterManager.Connection);
                });
                DCDYTBManager.Program();

            }catch(AggregateException ae)
            {
                foreach (var exp in ae.InnerExceptions)
                {
                    LogManager.LogRecord(exp.ToString());

                }
            }catch(Exception ex)
            {
                LogManager.LogRecord(ex.ToString());
            }
        }
    }
}
