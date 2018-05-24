using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{

    public interface IMerge
    {
        event ProgramCollectProgressHandler OnProgramProcess;
        void Program();
        string SourceFolder { get; set; }
        string SaveFile { get; set; }
    }
}
