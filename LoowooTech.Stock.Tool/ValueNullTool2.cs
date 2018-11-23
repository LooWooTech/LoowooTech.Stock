using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueNullTool2:ValueBaseTool2,IVTool
    {
        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(WhereClause)?
                    string.Format("规则{0}：")
                    :string.Format("")
            }
        }
        public bool Is_Nullable { get; set; }//true  为空  false  必填
        public string WhereClause { get; set; }


        public bool Check(OleDbConnection connection)
        {


            return true;
        }
    }
}
