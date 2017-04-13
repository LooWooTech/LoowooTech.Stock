using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public interface IFolder
    {
        string Name { get; }
        bool Check();
    }
}
