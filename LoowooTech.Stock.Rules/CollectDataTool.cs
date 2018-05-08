using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace LoowooTech.Stock.Rules
{
    public class CollectDataTool:ICollect
    {
        private CollectType _collectType { get; set; }
        /// <summary>
        /// 读取数据类型是MDB还是Excel类型
        /// </summary>
        public CollectType CollectType { get { return _collectType; } set { _collectType = value; } }
        private string _sourceFolder { get; set; }
        /// <summary>
        /// 数据读取文件夹路径
        /// </summary>
        public string SourceFolder { get { return _sourceFolder; } set { _sourceFolder = value; } }
        private string _saveFolder { get; set; }
        public string SaveFolder { get { return _saveFolder; } set { _saveFolder = value; } }
        private string[] _collectTables { get; set; }
        public string[] CollectTables { get { return _collectTables; }set { _collectTables = value; } }
        private List<CollectXZQ> _collectXZQ { get; set; }
        /// <summary>
        /// 每个市关联下属的区县列表信息
        /// </summary>
        public List<CollectXZQ> CollectXZQ { get { return _collectXZQ; } set { _collectXZQ = value; } }
        private CollectExcelType[] _collectExcelTypes { get; set; }
        public CollectExcelType[] CollectExcelTypes { get { return _collectExcelTypes; }set { _collectExcelTypes = value; } }


     


        private List<Collect> _result { get; set; } = new List<Collect>();
        public List<Collect> Result { get { return _result; } }
        private List<Collect2> _result2 { get; set; } = new List<Collect2>();
        public List<Collect2> Result2 { get { return _result2; } }

        private readonly object _syncRoot = new object();


        private List<Thread> _tList { get; set; } = new List<Thread>();


        private void InitExcel()
        {
            var files = FolderExtensions.GetExcelFiles(SourceFolder, "*.xls");
            OutputMessage("成功获取数据路径下的所有Excel文件");
            var tools = new List<CollectExcelTool>();
            foreach(var entry in Arguments.TableFieldDict)
            {
                var list = files.Where(e => e.TableName == entry.Key.Name).ToList();
                tools.Add(new CollectExcelTool { Fields = entry.Value, CollectTable = entry.Key, SaveFolder = SaveFolder, CollectXZQ = CollectXZQ, Files = list,CollectExcelTypes=CollectExcelTypes });
            }
            OutputMessage("完成创建读取数据工具，正在读取获取数据");
            foreach(var tool in tools)
            {
                tool.Program();
                OutputMessage(string.Format("完成{0}的生成", tool.CollectTable.Name));
            }
            //Parallel.ForEach(tools, tool =>
            //{
            //    tool.Program();
            //});
            OutputMessage("成功完成数据文件合并操作");

        }

        private void CollectNew()
        {
            var mdbfiles = FolderExtensions.GetFiles(SourceFolder, "*.mdb");
            var codefiles = FolderExtensions.GetFiles(SourceFolder, "*.xls");

            OutputMessage("成功获取数据路径下的所有矢量文件和单位代码表文件");

            var ranges = new Dictionary<CollectTable, List<ExcelField>>();
            foreach(var name in CollectTables)
            {
                var table = Arguments.CollectTableDict.Keys.FirstOrDefault(e => e.Name.ToLower() == name.ToLower());
                if (table != null)
                {
                    ranges.Add(table, Arguments.CollectTableDict[table]);
                }
            }

            OutputMessage("成功获取需要生成的表格类型信息");

            var tools = new List<GatherTool>();
            foreach(var shi in CollectXZQ)
            {
                if (shi.Children != null)
                {
                    foreach(var quxian in shi.Children)
                    {
                        var code = codefiles.FirstOrDefault(e => e.XZQDM.ToLower() == quxian.XZCDM.ToLower() && e.XZQMC.ToLower() == quxian.XZCMC.ToLower());
                        var mdb = mdbfiles.FirstOrDefault(e => e.XZQDM.ToLower() == quxian.XZCDM.ToLower() && e.XZQMC.ToLower() == quxian.XZCMC.ToLower());
                        if (mdb != null)
                        {
                            tools.Add(new GatherTool { Dict = ranges, MdbFile = mdb.FullName, XZQDM = quxian.XZCDM, XZQMC = quxian.XZCMC });
                        }
                     
                    }
                }
            }
            OutputMessage("成功创建每个区县获取工具");
            foreach(var tool in tools)
            {
                OutputMessage(string.Format("正在读取行政区代码【{0}】行政区名称【{1}】的矢量数据，请稍等", tool.XZQDM, tool.XZQMC));
                Console.WriteLine(tool.XZQMC);
                tool.Program();
                var output = tool.Collect2;
                _result2.AddRange(output);
                OutputMessage(string.Format("完成对行政区代码【{0}】行政区名称【{1}】的数据查询操作", tool.XZQDM, tool.XZQMC));
            }
            OutputMessage(string.Format("完成所有数据信息的读取，共获取{0}个区县数据信息", tools.Count));
            var writes = new List<WriteCollectTool>();
            foreach(var tableInfo in ranges)
            {
                var collects = Result2.Where(e => e.Table.Name == tableInfo.Key.Name).ToList();
                writes.Add(new WriteCollectTool { CollectExcelTypes = CollectExcelTypes, SaveFolder = SaveFolder, Fields = tableInfo.Value, Collect2 = collects, CollectTable = tableInfo.Key, CollectXZQ = CollectXZQ });
            }

            foreach(var tool in writes)
            {
                tool.Program();
            }
            //Parallel.ForEach(writes, tool =>
            //{
            //    tool.Program();
            //});
            OutputMessage("成功保存文件，请在保存路径中查看文件！");


        }
        private void InitMdb()
        {
            var mdbfiles = FolderExtensions.GetFiles(SourceFolder, "*.mdb");//获取文件及下所有的mdb文件列表
            var codefiles = FolderExtensions.GetFiles(SourceFolder, "*.xls");//获取文件夹下的所有的Excel文件  即单位代码表文件

            OutputMessage("成功获取数据路径下的所有矢量文件和单位代码表文件");
            #region  读取获取数据
            var info = string.Empty;
            var tools = new List<CollectTool>();
            foreach(var shi in CollectXZQ)
            {
                if (shi.Children != null)
                {
                    foreach(var quxian in shi.Children)
                    {
                        var code = codefiles.FirstOrDefault(e => e.XZQDM.ToLower() == quxian.XZCDM.ToLower() && e.XZQMC.ToLower() == quxian.XZCMC.ToLower());
                        var mdb = mdbfiles.FirstOrDefault(e => e.XZQDM.ToLower() == quxian.XZCDM.ToLower() && e.XZQMC.ToLower() == quxian.XZCMC.ToLower());
                        if (code != null && mdb != null)
                        {
                            tools.Add(new CollectTool { MdbFile = mdb.FullName, CodeFile = code.FullName, TableFieldDict = Arguments.TableFieldDict,XZQDM=quxian.XZCDM,XZQMC=quxian.XZCMC });
                        }
                        else
                        {
                            info = string.Format("缺少行政区代码【{0}】行政区名称【{1}】的相关数据文件或者单位代码表，故未进行统计操作", quxian.XZCDM, quxian.XZCMC);
                            Console.WriteLine(info);
                            //OutputMessage(info);
                        }
                    }
                }
                else
                {
                    info = string.Format("市级{0}下未获取区县列表，请核对", shi.XZQMC);
                    Console.WriteLine(info);
                    OutputMessage(info);
                }
            }
            OutputMessage("正在获取每个县区市数据信息，请稍等......");

            foreach (var tool in tools)
            {
                OutputMessage(string.Format("正在读取行政区代码【{0}】行政区名称【{1}】的矢量数据，请稍等", tool.XZQDM, tool.XZQMC));
                tool.Program();
                var output = tool.Result2;
                AddResult(output);
                OutputMessage(string.Format("完成对行政区代码【{0}】行政区名称【{1}】的数据查询操作", tool.XZQDM, tool.XZQMC));
            }
            #endregion

            #region 多线程
            //foreach (var tool in tools)
            //{
            //    var t = new Thread(tool.Program);
            //    t.IsBackground = true;
            //    t.Start();
            //    _tList.Add(t);
            //}

            //var flag = false;
            //while (flag == false)
            //{
            //    flag = true;
            //    Thread.Sleep(500);
            //    foreach(var t in _tList)
            //    {
            //        if (t.IsAlive == true)
            //        {
            //            flag = false;
            //        }

            //    }
            //}

            #endregion


            //Parallel.ForEach(tools, tool =>
            //{
            //    tool.Program();
            //    var output = tool.Result2;
            //    AddResult(output);
            //});

            OutputMessage(string.Format("成功完成获取所有数据信息，共获得{0}个县区市数据信息", tools.Count));
            var writes = new List<WriteCollectTool>();
            OutputMessage("正在保存文件，请稍等......");
            foreach(var tableInfo in Arguments.TableFieldDict)
            {
                var collects = Result.Where(e => e.Table.Name == tableInfo.Key.Name).ToList();
                writes.Add(new WriteCollectTool { CollectXZQ = CollectXZQ, CollectTable = tableInfo.Key, Collects = collects,Fields=tableInfo.Value,SaveFolder=SaveFolder,CollectExcelTypes=CollectExcelTypes });
            }
            Parallel.ForEach(writes, tool =>
            {
                tool.Program();
            });
            OutputMessage("成功保存文件，请在保存路径中查看文件");
        }

        private void AddResult(List<Collect> list)
        {
            lock (_syncRoot)
            {
                _result.AddRange(list);
            }
        }



        public void Program()
        {
            switch (CollectType)
            {
                case CollectType.MDB:
                    if (CollectTables != null && CollectTables.Length > 0)
                    {
                        CollectNew();
                    }
                    else
                    {
                        InitMdb();
                    }
                   
                    break;
                case CollectType.Excel:
                    InitExcel();
                    break;

            }



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
