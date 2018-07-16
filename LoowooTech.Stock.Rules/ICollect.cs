using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public delegate void ProgramCollectProgressHandler(object sender, CollectProgressEventArgs e);
    public interface ICollect
    {
        event ProgramCollectProgressHandler OnProgramProcess;
        CollectType CollectType { get; set; }
        CollectExcelType[] CollectExcelTypes { get; set; }
        string SourceFolder { get; set; }
        string SaveFolder { get; set; }
        string[] CollectTables { get; set; }
        List<CollectXZQ> CollectXZQ { get; set; }
        void Program();
    }


    public interface ICollect2
    {
        event ProgramCollectProgressHandler OnProgramProcess;
        string ResultFolder { get; set; }
        string SaveFolder { get; set; }
        string[] Excels { get; set; }
        void Program();
    }

    public class CollectProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool Cancel { get; set; }

    }
}
