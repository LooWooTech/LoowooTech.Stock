using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public interface ITool2
    {
        string Name { get; }
        string ID { get; set; }
        string ExcelName { get; set; }
        bool Check();
    }
}
