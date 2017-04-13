using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public interface IResult
    {
        string Name { get; }
        void Check();

    }
}
