using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class CollectExcelTool2:ICollect2
    {
        private string _resultFolder { get; set; }
        public string ResultFolder { get { return _resultFolder; } set { _resultFolder = value; } }

        private string _saveFolder { get; set; }
        public string SaveFolder { get { return _saveFolder; } set { _saveFolder = value; } }
        private string[] _excels { get; set; }
        public string[] Excels { get { return _excels; } set { _excels = value; } }


        public void Program()
        {
            var mdbfiles = FolderExtensions.GetFiles(ResultFolder, "*.mdb");
            var codefiles = FolderExtensions.GetFiles(ResultFolder, "*.xls");

            OutputMessage("成功获取成果路径下的所有矢量文件和单位代码表文件");

            var tools = new List<GatherTool2>();

            var ranges = GetTable();

            var info = string.Empty;
            foreach (var item in mdbfiles)
            {
                var code = codefiles.FirstOrDefault(e => e.XZQDM.ToLower() == item.XZQDM.ToLower() && e.XZQMC.ToLower() == item.XZQMC.ToLower());
                if (code != null)
                {
                    tools.Add(new GatherTool2 { Dict=ranges, MdbFile = item.FullName, CodeFile = code.FullName, XZQDM = code.XZQDM, XZQMC = item.XZQMC, SaveFolder = SaveFolder });
                }
                else
                {
                    info = string.Format("缺少行政区代码【{0}】行政区名称【{1}】的相关数据文件或者单位代码表，故未进行统计操作", item.XZQDM, item.XZQMC);
                    OutputMessage(info);
                }
            }

            foreach(var tool in tools)
            {
                tool.Program();
            }
            //OutputMessage("成功生成文件");
        }

        private Dictionary<CollectTable,List<ExcelField>> GetTable()
        {
            var dict = new Dictionary<CollectTable, List<ExcelField>>();
            foreach(var entry in Arguments.CollectTableDict)
            {
                if (Excels.Contains(entry.Key.Name.ToLower()))
                {
                    dict.Add(entry.Key, entry.Value);
                }
            }

            return dict;
        }

        public event ProgramCollectProgressHandler OnProgramProcess;

        private bool OutputMessage(string message)
        {
            return OutputMessage(new CollectProgressEventArgs { Message = message });
        }
        private bool OutputMessage(CollectProgressEventArgs e)
        {
            OnProgramProcess(this, e);
            return e.Cancel;
        }
    }
}
