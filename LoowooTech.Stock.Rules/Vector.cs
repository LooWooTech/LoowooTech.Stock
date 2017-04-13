using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class Vector:IResult
    {
        public virtual string Name { get { return "矢量数据"; } }
        public virtual void Check()
        {

        }
    }
}
