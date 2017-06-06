using ESRI.ArcGIS;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Rules;
using LoowooTech.Stock.WorkBench;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!RuntimeManager.Bind(ProductCode.Engine))
            {
                if (!RuntimeManager.Bind(ProductCode.Desktop))
                {
                    System.Console.WriteLine("ArcGIS runtime:unable to bind to arcgis runtime.application will be shut down.");
                    return;
                }
            }
            if (args == null||args.Length==0)
            {
                System.Console.WriteLine("未获取文件夹路径，请联系相关人员！");
                return;
            }
            var folder = args[0];
            if (!System.IO.Directory.Exists(folder))
            {
                System.Console.WriteLine("当前检查文件及路径不存在，请核对文件夹路径！");
                return;
            }
            var ids = args.Skip(1).ToArray();

            System.Console.WriteLine(string.Format("开始质检路径：{0}", folder));
            var workbench = new WorkBench.WorkBench() { Folder = folder,IDS=ids };
            workbench.Program();
            QuestionManager.Save(workbench.ReportPath, workbench.District, workbench.Code);

            System.Console.WriteLine("结束");
        }
    }
}
