using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class FileOpenRule:IRule
    {
        public string RuleName { get; set; }
        public string ID { get; set; }
        public void Check()
        {
            var children = ParameterManager.ChildrenFolder;
            Parallel.ForEach(children, child =>
            {
                
            });
        }

        private void Check(string name)
        {
            var node = XmlManager.GetSingle(string.Format("/Folders/Folder[@Name='{0}']", name), XmlEnum.DataTree);
            var folders = node.SelectNodes("/Folder");
            var files = node.SelectNodes("/File");
        }
    }
}
