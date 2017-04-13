using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class VectorBase:Vector
    {
        public override string Name
        {
            get
            {
                return base.Name + "-基本检查";
            }
        }

        private string _mdbFilePath { get; set; }
        public VectorBase(string mdbFilePath)
        {
            _mdbFilePath = mdbFilePath;
        }

        public override void Check()
        {
            
        }
    }
}
