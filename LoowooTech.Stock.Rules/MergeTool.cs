using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class MergeTool:IMerge
    {
        private string _sourceFolder { get; set; }
        public string SourceFolder { get { return _sourceFolder; } set { _sourceFolder = value; } }
        private string _saveFile { get; set; }
        public string SaveFile { get { return _saveFile; }set { _saveFile = value; } }
        public void Program()
        {
            if (System.IO.File.Exists(SaveFile))
            {
                System.IO.File.Delete(SaveFile);
            }
            #region  获取mdb文件列表
            var mdbfiles = FolderExtensions.GetFiles(SourceFolder, "*.mdb");
            if (mdbfiles.Count == 0)
            {
                OutputMessage("未获取矢量数据库文件，请核对！");
                return;
            }
            #endregion

            var stockTables = ParameterManager.StockTables;

            var modelMdbFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TEMPS", "MODEL.mdb");

            




            if (!System.IO.File.Exists(modelMdbFile))
            {
                var directory = System.IO.Path.GetDirectoryName(modelMdbFile);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
                #region 创建mdb文件
                if (!ArcExtensions.CreateAccess(System.IO.Path.GetDirectoryName(modelMdbFile), System.IO.Path.GetFileNameWithoutExtension(modelMdbFile)))
                {
                    OutputMessage("创建MDB文件失败！");
                    return;
                }
                #endregion

                #region 创建表和字段

                ArcExtensions.Create(modelMdbFile, stockTables);
                OutputMessage("成功创建要素类和表");
                #endregion
            }








            #region  单个区县导入数据


            #region  方法一

            //var index = 0;
            //var size = 1;

            //var currentMdbFilePath = string.Empty;
            //var files = new List<string>();
            //foreach (var DD in ParameterManager.CollectXZQ)
            //{

            //    if (index % size == 0)
            //    {
            //        if (System.IO.File.Exists(currentMdbFilePath))
            //        {
            //            ArcExtensions.DeleteFields(currentMdbFilePath, stockTables);
            //        }
            //        currentMdbFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SaveFile), System.IO.Path.GetFileNameWithoutExtension(SaveFile) + string.Format("-{0}", DD.XZQDM) + System.IO.Path.GetExtension(SaveFile));
            //        System.IO.File.Copy(modelMdbFile, currentMdbFilePath);
            //        files.Add(currentMdbFilePath);
            //    }
            //    index++;

            //    if (DD.Children != null)
            //    {
            //        foreach (var item in DD.Children)
            //        {
            //            var stockFile = mdbfiles.FirstOrDefault(e => e.XZQDM.ToLower() == item.XZCDM.ToLower());
            //            if (stockFile == null)
            //            {
            //                OutputMessage(string.Format("未识别到行政区代码【{0}】行政区名称【{1}】相关矢量文件", item.XZCDM, item.XZCMC));
            //            }
            //            else
            //            {
            //                OutputMessage(string.Format("正在导入行政区代码【{0}】行政区名称【{1}】的矢量数据", item.XZCDM, item.XZCMC));
            //                try
            //                {
            //                    ArcExtensions.Import2(currentMdbFilePath, stockFile.FullName, stockTables, DD.XZQDM, DD.XZQMC);
            //                }
            //                catch (Exception ex)
            //                {
            //                    OutputMessage("导入矢量数据，发生错误:" + ex.Message);
            //                }
            //                OutputMessage(string.Format("完成导入行政区代码【{0}】行政区名称【{1}】的矢量信息", item.XZCDM, item.XZCMC));

            //                OutputMessage(string.Format("正在导入行政区代码【{0}】行政区名称【{1}】的属性表格数据", item.XZCDM, item.XZCMC));
            //                try
            //                {
            //                    ArcExtensions.ImportTables(currentMdbFilePath, stockFile.FullName, stockTables, DD.XZQDM, DD.XZQMC);
            //                }
            //                catch (Exception ex)
            //                {
            //                    OutputMessage("导入属性数据，发生错误:" + ex.Message);
            //                }
            //                OutputMessage(string.Format("完成导入行政区代码【{0}】行政区名称【{1}】的属性表格数据", item.XZCDM, item.XZCMC));
            //            }
            //        }
            //    }



            //}

            //if (System.IO.File.Exists(currentMdbFilePath))
            //{
            //    ArcExtensions.DeleteFields(currentMdbFilePath, stockTables);
            //}

            //#region  将每个地级市数据合并
            //var size2 = 6;

            //for (var i = 0; i < files.Count / size2; i++)
            //{
            //    var temp = files.Skip(i * size2).Take(size2).ToArray();
            //    var saveFile1 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SaveFile), System.IO.Path.GetFileNameWithoutExtension(SaveFile) + "-" + i + System.IO.Path.GetExtension(SaveFile));
            //    MergeFile(temp, saveFile1, stockTables);
            //}

            //#endregion
            #endregion


            #region   方法二

            //var mergeSize = 6;

            //var results = new List<string>();
            //foreach(var dd in ParameterManager.CollectXZQ)
            //{

            //    if (dd.Children != null)
            //    {
            //        var childrenFiles = new List<string>();
            //        //childrenFiles.Add(modelMdbFile);
            //        foreach(var item in dd.Children)
            //        {
            //            var stock = mdbfiles.FirstOrDefault(e => e.XZQDM.ToLower() == item.XZCDM.ToLower());
            //            if (stock != null)
            //            {
            //                childrenFiles.Add(stock.FullName);
            //            }
            //        }

            //        for(var i = 0; i <= childrenFiles.Count / mergeSize; i++)
            //        {
            //            var temp = childrenFiles.Skip(i * mergeSize).Take(mergeSize).ToList();
            //            temp.Add(modelMdbFile);
            //            var tempFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SaveFile), string.Format("{0}_{1}_{2}.mdb", dd.XZQDM, i,DateTime.Now.Ticks));
            //            MergeFile(temp.ToArray(), tempFile, stockTables);
            //            ArcExtensions.SetXZS(tempFile, stockTables, dd.XZQDM, dd.XZQMC);
            //            results.Add(tempFile);
            //        }
            //    }

            //}


            #endregion

            #region  方法三

            var model2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TEMPS", "MM.mdb");
            if (!System.IO.File.Exists(model2))
            {
                var dir = System.IO.Path.GetDirectoryName(model2);
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                if (!ArcExtensions.CreateAccess(System.IO.Path.GetDirectoryName(model2), System.IO.Path.GetFileNameWithoutExtension(model2)))
                {
                    OutputMessage(string.Format("创建模板文件2失败"));
                    return;
                }

                ArcExtensions.CreateTable2(model2, stockTables.Where(e=>e.IsSpace==false).ToList());

            }

           



            var mergeSize = 11;
            var currentmodel = string.Empty;
            var index = 0;
            #region  合成属性表
            foreach(var dd in ParameterManager.CollectXZQ)
            {
                if (index % mergeSize == 0)
                {
                    currentmodel = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SaveFile), string.Format("{0}_{1}_属性表.mdb", System.IO.Path.GetFileNameWithoutExtension(SaveFile), index / mergeSize));
                    System.IO.File.Copy(model2, currentmodel);
                }
                index++;
                if (dd.Children != null)
                {
                    foreach(var item in dd.Children)
                    {
                        var stock = mdbfiles.FirstOrDefault(e => e.XZQDM.ToLower() == item.XZCDM.ToLower());
                        if (stock == null)
                        {
                            OutputMessage(string.Format("未识别到行政区代码【{0}】行政区名称【{1}】相关矢量文件", item.XZCDM, item.XZCMC));
                        }
                        else
                        {
                            OutputMessage(string.Format("正在导入行政区代码【{0}】行政区名称【{1}】的属性表格数据", item.XZCDM, item.XZCMC));
                            try
                            {
                                ArcExtensions.ImportTables(currentmodel, stock.FullName, stockTables, dd.XZQDM, dd.XZQMC);
                            }
                            catch (Exception ex)
                            {
                                OutputMessage("导入属性数据，发生错误:" + ex.Message);
                            }
                            OutputMessage(string.Format("完成导入行政区代码【{0}】行政区名称【{1}】的属性表格数据", item.XZCDM, item.XZCMC));
                        }
                    }
                }
            }
            OutputMessage("完成属性表数据的合并");
            #endregion


            OutputMessage("开始对矢量数据的合并");
            index = 0;
            mergeSize = 11;
            var shiTable = stockTables.Where(e => e.IsSpace == true).ToList();
            //var model3 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TEMPS", "MMM.gdb");
            //if (!System.IO.File.Exists(model3))
            //{
            //    if (!ArcExtensions.CreateGDB(System.IO.Path.GetDirectoryName(model3), System.IO.Path.GetFileNameWithoutExtension(model3)))
            //    {
            //        OutputMessage(string.Format("创建模板文件2失败"));
            //        return;
            //    }

            //    ArcExtensions.Create2(model3, shiTable);
            //}
            foreach (var dd in ParameterManager.CollectXZQ)
            {
                if (index % mergeSize == 0)
                {
                    currentmodel = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SaveFile), string.Format("{0}_{1}_矢量表.gdb", System.IO.Path.GetFileNameWithoutExtension(SaveFile), index / mergeSize));
                    if (!ArcExtensions.CreateGDB(System.IO.Path.GetDirectoryName(currentmodel), System.IO.Path.GetFileNameWithoutExtension(currentmodel)))
                    {
                        OutputMessage(string.Format("创建模板文件2失败"));
                        return;
                    }

                    ArcExtensions.Create2(currentmodel, shiTable);
                    //System.IO.File.Copy(model3, currentmodel);
                }
                index++;
                if (dd.Children != null)
                {
                    foreach(var item in dd.Children)
                    {
                        var stock = mdbfiles.FirstOrDefault(e => e.XZQDM.ToLower() == item.XZCDM.ToLower());
                        if (stock != null)
                        {
                            OutputMessage(string.Format("开始导入行政区代码【{0}】行政区名称【{1}】数据", item.XZCDM, item.XZCMC));
                            ArcExtensions.Import3(currentmodel, stock.FullName, shiTable, dd.XZQDM, dd.XZQMC);
                            OutputMessage(string.Format("完成导入行政区代码【{0}】行政区名称【{1}】数据", item.XZCDM, item.XZCMC));
                            //ArcExtensions.DeleteFields(stock.FullName, shiTable);//删除不必要的字段
                            //OutputMessage(string.Format("成功删除行政区代码【{0}】行政区名称【{1}】数据中多余字段", item.XZCDM, item.XZCMC));
                            //ArcExtensions.AddFields(stock.FullName, shiTable);//添加字段
                            //OutputMessage(string.Format("成功完成行政区代码【{0}】行政区名称【{1}】数据中图层添加字段", item.XZCDM, item.XZCMC));
                            //ArcExtensions.SetXZS(stock.FullName, shiTable, dd.XZQDM, dd.XZQMC);//给赋值
                            //OutputMessage(string.Format("成功赋值行政区代码【{0}】行政区名称【{1}】数据中地级市信息", item.XZCDM, item.XZCMC));

                        }
                    }
                }
            }

            #endregion
            #endregion

            OutputMessage(string.Format("完成所有区县数据合并"));




        


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files">源文件列表</param>
        /// <param name="savefile">保存文件（不存在）</param>
        /// <param name="tables">表</param>
        private void MergeFile(string[] files,string savefile,List<StockTable> tables)
        {
            var model2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TEMPS", "MM.mdb");
            if (!System.IO.File.Exists(model2))
            {
                var dir = System.IO.Path.GetDirectoryName(model2);
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                if (!ArcExtensions.CreateAccess(System.IO.Path.GetDirectoryName(model2), System.IO.Path.GetFileNameWithoutExtension(model2)))
                {
                    OutputMessage(string.Format("创建模板文件2失败"));
                    return;
                }

                ArcExtensions.CreateTable(model2, tables);

            }

            System.IO.File.Copy(model2, savefile);

            //if (!ArcExtensions.CreateAccess(System.IO.Path.GetDirectoryName(savefile), System.IO.Path.GetFileNameWithoutExtension(savefile)))
            //{
            //    OutputMessage(string.Format("创建mdb文件{0}失败", System.IO.Path.GetFileNameWithoutExtension(savefile)));
            //    return;
            //}
            foreach (var item in tables)
            {
                if (item.IsSpace == true)
                {
                    if(!ArcExtensions.Merge(files,item.Name, string.Format("{0}\\{1}", savefile, item.Name)))
                    {
                        OutputMessage(string.Format("合并图层{0}失败", item.Name));
                    }
                    //if (!ArcExtensions.MergeFeatureClass(string.Join(";", files.Select(e => string.Format("{0}\\{1}", e, item.Name)).ToArray()), null, string.Format("{0}\\{1}", savefile, item.Name)))
                    //{
                    //    OutputMessage(string.Format("合并图层{0}失败", item.Name));
                    //}
                }
            }

            foreach(var file in files)
            {
                if (System.IO.Path.GetFileNameWithoutExtension(file).ToLower() != "MODEL".ToLower())
                {
                    ArcExtensions.ImportTables2(savefile, file, tables);
                }
              
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
