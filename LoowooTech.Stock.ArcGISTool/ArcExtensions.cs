using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class ArcExtensions
    {
        private static Geoprocessor _gp { get; set; }
        private static ESRI.ArcGIS.DataManagementTools.Merge _merge { get; set; }
        private static ESRI.ArcGIS.DataManagementTools.CreatePersonalGDB _createAccess { get; set; }
        private static ESRI.ArcGIS.DataManagementTools.CreateFileGDB _createGDB { get; set; }
        static ArcExtensions()
        {
            _gp = new Geoprocessor();
            _merge = new ESRI.ArcGIS.DataManagementTools.Merge();
            _createAccess = new ESRI.ArcGIS.DataManagementTools.CreatePersonalGDB();
            _createGDB = new ESRI.ArcGIS.DataManagementTools.CreateFileGDB();
        }
        public static bool Excute(IGPProcess tool)
        {
            try
            {
                _gp.Execute(tool, null);
            }
            catch (COMException comexc)
            {
                var error = string.Format("GP错误代码：{0}，详情：{1}", comexc.ErrorCode, comexc.Message);
                return false;
            }

            return true;
        }


        public static bool CreateAccess(string folder, string name)
        {
            //ESRI.ArcGIS.DataManagementTools.CreatePersonalGDB tool = new ESRI.ArcGIS.DataManagementTools.CreatePersonalGDB();
            //tool.out_folder_path = folder;
            //tool.out_name = name;
            //tool.out_version = "CURRENT";
            //return Excute(tool);
 
            _createAccess.out_folder_path = folder;
            _createAccess.out_name = name;
            _createAccess.out_version = "CURRENT";
            return Excute(_createAccess);
        }

        public static bool CreateGDB(string folder,string name)
        {
            _createGDB.out_folder_path = folder;
            _createGDB.out_name = name;
            _createGDB.out_version = "CURRENT";
            return Excute(_createGDB);
        }

        public static bool MergeFeatureClass(object inputs,IFields fields,string outputfeatureClass)
        {
            //tool.inputs = featuresClass;
            _merge.inputs = inputs;
            _merge.output = outputfeatureClass;
            //tool.field_mappings = fields;
            return Excute(_merge);
        }

        private static IFeatureClass GetFeatureClass(string mdbfile,string featureClassName)
        {
            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
            IWorkspace workspace = workspaceFactory.OpenFromFile(mdbfile, 0);
            IFeatureClass featureClass = workspace.GetFeatureClass(featureClassName);
            IWorkspaceFactoryLockControl factorylock = workspaceFactory as IWorkspaceFactoryLockControl;
            if (factorylock.SchemaLockingEnabled)
            {
                factorylock.DisableSchemaLocking();

            }
            IDataset dataset = featureClass as IDataset;
            Console.WriteLine(dataset.FullName);
            Console.WriteLine(dataset.Name);
            Console.WriteLine(dataset.BrowseName);
            Console.WriteLine(dataset.Category);
            
            Console.WriteLine(featureClass.AliasName);

            return featureClass;
        }

        public static bool Merge(string[] files,string featureClassName,string outputfeatureClass)
        {
            IArray array = new ArrayClass();
            //List<IFeatureClass> list = new List<IFeatureClass>();
            foreach(var item in files)
            {
                var entry = GetFeatureClass(item, featureClassName);
                if (entry != null)
                {
                    array.Add(entry);
                    //_merge.inputs = entry;
                    //list.Add(entry);
                }
            }
            _merge.inputs = array;
            _merge.output = outputfeatureClass;
            return Excute(_merge);
        }

        public static void Create(string mdbfilePath, List<StockTable> tables)
        {
            IWorkspace workspace = mdbfilePath.OpenAccessFileWorkSpace();
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;

            foreach (var item in tables)
            {
                if (item.IsSpace == true)
                {
                    if (item.Fields != null)
                    {
                        IFeatureClass featureClass = CreateFeatureClass(featureWorkspace, string.IsNullOrEmpty(item.Type) ? esriGeometryType.esriGeometryPolygon : esriGeometryType.esriGeometryPolyline, ParameterManager.CurrentSpatialReference, item);
                    }
                }
                else
                {
                    if (item.Fields != null)
                    {
                        ITable table = CreateTable(featureWorkspace, item);
                    }
                }

            }
        }

        public static void Create2(string mdbfilePath, List<StockTable> tables)
        {
            IWorkspace workspace = mdbfilePath.OpenGDBWorkspace();
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;

            foreach (var item in tables)
            {
                if (item.Fields != null)
                {
                    IFeatureClass featureClass = CreateFeatureClass(featureWorkspace, string.IsNullOrEmpty(item.Type) ? esriGeometryType.esriGeometryPolygon : esriGeometryType.esriGeometryPolyline, ParameterManager.CurrentSpatialReference, item);
                }

            }
        }


        public static void CreateTable(string mdbfile,List<StockTable> tables)
        {
            IWorkspace workspace = mdbfile.OpenAccessFileWorkSpace();
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;

            foreach (var item in tables)
            {
                if (item.IsSpace == false)
                {
                    if (item.Fields != null)
                    {
                        ITable table = CreateTable(featureWorkspace, item);
                    }
                }
            }
        }
        public static void CreateTable2(string mdbfile, List<StockTable> tables)
        {
            IWorkspace workspace = mdbfile.OpenAccessFileWorkSpace();
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;

            foreach (var item in tables)
            {
                if (item.Fields != null)
                {
                    ITable table = CreateTable(featureWorkspace, item);
                }
            }
        }
        public static void CreateTable3(string mdbfile, List<StockTable> tables)
        {
            IWorkspace workspace = mdbfile.OpenGDBWorkspace();
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;

            foreach (var item in tables)
            {
                if (item.Fields != null)
                {
                    ITable table = CreateTable(featureWorkspace, item);
                }
            }
        }

        public static ITable CreateTable(IFeatureWorkspace featureWorkspace,StockTable stockTable)
        {
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsedit = fields as IFieldsEdit;
            IField field = new FieldClass();
            IFieldEdit fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "OBJECTID";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "XZSDM";
            fieldedit.AliasName_2 = "行政市代码";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "XZSMC";
            fieldedit.AliasName_2 = "行政市名称";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "XZXDM";
            fieldedit.AliasName_2 = "行政县代码";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "XZXMC";
            fieldedit.AliasName_2 = "行政县名称";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsedit.AddField(field);

            foreach (var item in stockTable.Fields)
            {
                field = new FieldClass();
                fieldedit = field as IFieldEdit;
                fieldedit.Name_2 = item.Name;
                fieldedit.AliasName_2 = item.Title;
                switch (item.Type)
                {
                    case Models.FieldType.Char:
                        fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
                        break;
                    case Models.FieldType.Float:
                        fieldedit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        break;
                    case Models.FieldType.Int:
                        fieldedit.Type_2 = esriFieldType.esriFieldTypeInteger;
                        break;
                }
                fieldsedit.AddField(field);
            }

            UIDClass instanceUID = new UIDClass();
            instanceUID.Value = "esriGeoDatabase.Object";

            ITable ptable = featureWorkspace.CreateTable(stockTable.Name, fields, instanceUID, null, "");
            return ptable;
        }

        public static void Move(string filePath, List<StockTable> tables)
        {
            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
            IWorkspace workspace = workspaceFactory.OpenFromFile(filePath, 0);

            IEnumDataset dataset = workspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            IFeatureDataset featureDataset = dataset.Next() as IFeatureDataset;

            foreach(var table in tables)
            {
                
            }

            IWorkspaceFactoryLockControl factorylock = workspaceFactory as IWorkspaceFactoryLockControl;
            if (factorylock.SchemaLockingEnabled)
            {
                factorylock.DisableSchemaLocking();

            }
        }
        /// <summary>
        /// 给要素类 添加字段  XZSDM  XZSMC
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="tables"></param>
        public static void AddFields(string filePath, List<StockTable> tables)
        {
            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
            IWorkspace workspace = workspaceFactory.OpenFromFile(filePath, 0);
            foreach (var table in tables)
            {
                IFeatureClass featureClass = workspace.GetFeatureClass(table.Name);
                if (featureClass != null)
                {
                    IClass pClass = featureClass as IClass;
                    IFieldsEdit fieldsEdit = featureClass.Fields as IFieldsEdit;
                    IField field = null;
                    IFieldEdit fieldedit = null;
                    var index = featureClass.Fields.FindField("XZSDM");

                    if (index == -1)
                    {
                        field = new FieldClass();
                        fieldedit = field as IFieldEdit;
                        fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
                        fieldedit.Name_2 = "XZSDM";
                        pClass.AddField(field);
                    }
                    index = featureClass.Fields.FindField("XZSMC");
                    if (index == -1)
                    {
                        field = new FieldClass();
                        fieldedit = field as IFieldEdit;
                        fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
                        fieldedit.Name_2 = "XZSMC";
                        pClass.AddField(field);
                    }
                }
            }
             IWorkspaceFactoryLockControl factorylock = workspaceFactory as IWorkspaceFactoryLockControl;
            if (factorylock.SchemaLockingEnabled)
            {
                factorylock.DisableSchemaLocking();

            }
        }

        

        public static IFeatureClass CreateFeatureClass(IFeatureWorkspace featureWorkspace,esriGeometryType esriGeometryType,ISpatialReference spatialReference, StockTable stockTable)
        {
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsedit = fields as IFieldsEdit;
            IField field = new FieldClass();
            IFieldEdit fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "shape";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit pGeometryDefEdit = geometryDef as IGeometryDefEdit;
            pGeometryDefEdit.GeometryType_2 = esriGeometryType;

            pGeometryDefEdit.SpatialReference_2 = spatialReference;
            fieldedit.GeometryDef_2 = geometryDef;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "OBJECTID";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "XZSDM";
            fieldedit.AliasName_2 = "行政市代码";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "XZSMC";
            fieldedit.AliasName_2 = "行政市名称";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "XZXDM";
            fieldedit.AliasName_2 = "行政县代码";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "XZXMC";
            fieldedit.AliasName_2 = "行政县名称";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsedit.AddField(field);

            foreach (var item in stockTable.Fields)
            {
                field = new FieldClass();
                fieldedit = field as IFieldEdit;
                fieldedit.Name_2 = item.Name;
                fieldedit.AliasName_2 = item.Title;
                switch (item.Type)
                {
                    case Models.FieldType.Char:
                        fieldedit.Type_2 = esriFieldType.esriFieldTypeString;
                        break;
                    case Models.FieldType.Float:
                        fieldedit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        break;
                    case Models.FieldType.Int:
                        fieldedit.Type_2 = esriFieldType.esriFieldTypeInteger;
                        break;
                }
                fieldsedit.AddField(field);
            }
            IFeatureClassDescription featureClassDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription objectClassDescription = featureClassDescription as IObjectClassDescription;
            IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(stockTable.Name, fields, objectClassDescription.InstanceCLSID, objectClassDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "shape", "");
            return featureClass;
        }

        private static Dictionary<string,int> GetFieldIndex(IFeatureClass featureClass,List<Models.Field> fields)
        {
            var dict = new Dictionary<string, int>();
            foreach(var field in fields)
            {
                var index = featureClass.Fields.FindField(field.Name);
                if (index > -1)
                {
                    dict.Add(field.Name, index);
                }
            }
            return dict;
        }

        public static void Import3(string savemdbFile, StockFile stock, List<StockTable> tables, string XZSDM, string XZSMC)
        {
            IWorkspace workspaceA = savemdbFile.OpenGDBWorkspace();
            //IFeatureWorkspace featureWorkspaceA = workspaceA as IFeatureWorkspace;
            //IWorkspaceEdit workspaceEditA = featureWorkspaceA as IWorkspaceEdit;

            //workspaceEditA.StartEditing(true);
            //workspaceEditA.StartEditOperation();





            IWorkspace workspaceB = stock.FullName.OpenAccessFileWorkSpace();


            #region  插入表
            foreach (var table in tables)
            {
                if (table.IsSpace == false)
                {
                    continue;
                }

                IFeatureClass featureClassA = workspaceA.GetFeatureClass(table.Name);

                var dictA = GetFieldIndex(featureClassA, table.Fields);//字段 对应Index

                #region 2
                //IFeatureClassLoad featureClassLoadA = featureClassA as IFeatureClassLoad;
                //ISchemaLock schemaLock = featureClassA as ISchemaLock;
                //try
                //{
                //    //schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                //    featureClassLoadA.LoadOnlyMode = true;
                //    using (ComReleaser comReleaser=new ComReleaser())
                //    {
                //        IFeatureBuffer featureBufferA = featureClassA.CreateFeatureBuffer();

                //        comReleaser.ManageLifetime(featureBufferA);

                //        #region 设置地级市代码和名称
                //        int XZSDMIndex = featureClassA.Fields.FindField("XZSDM");
                //        var XZSMCIndex = featureClassA.Fields.FindField("XZSMC");
                //        featureBufferA.set_Value(XZSDMIndex, XZSDM);
                //        featureBufferA.set_Value(XZSMCIndex, XZSMC);
                //        #endregion

                //        IFeatureCursor featureCursorA = featureClassA.Insert(true);
                //        comReleaser.ManageLifetime(featureCursorA);

                //        IFeatureClass featureClassB = workspaceB.GetFeatureClass(table.Name);
                //        var dictB = GetFieldIndex(featureClassB, table.Fields);

                //        IFeatureCursor featureCursorB = featureClassB.Search(null, true);
                //        IFeature featureB = featureCursorB.NextFeature();
                //        while (featureB != null)
                //        {
                //            featureBufferA.Shape = featureB.ShapeCopy;

                //            #region 给每个字段赋值
                //            foreach (var field in table.Fields)
                //            {
                //                object val = null;
                //                if (dictB.ContainsKey(field.Name))
                //                {
                //                    val = featureB.get_Value(dictB[field.Name]);
                //                }
                //                if (val != null)
                //                {
                //                    if (dictA.ContainsKey(field.Name))
                //                    {
                //                        featureBufferA.set_Value(dictA[field.Name], val);
                //                    }
                //                }

                //            }
                //            #endregion
                //            featureCursorA.InsertFeature(featureBufferA);


                //            featureB = featureCursorB.NextFeature();
                //        }
                //        featureCursorA.Flush();
                //    }
                //}
                //catch (Exception)
                //{

                //}
                //finally
                //{
                //    featureClassLoadA.LoadOnlyMode = false;
                //    schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                //}

                #endregion

                #region 1

                IFeatureBuffer featureBufferA = featureClassA.CreateFeatureBuffer();

                #region 设置地级市代码和名称
                int XZSDMIndex = featureClassA.Fields.FindField("XZSDM");
                var XZSMCIndex = featureClassA.Fields.FindField("XZSMC");
                var XZXDMIndex = featureClassA.Fields.FindField("XZXDM");
                var XZXMCIndex = featureClassA.Fields.FindField("XZXMC");
                featureBufferA.set_Value(XZSDMIndex, XZSDM);
                featureBufferA.set_Value(XZSMCIndex, XZSMC);
                featureBufferA.set_Value(XZSDMIndex, stock.XZQDM);
                featureBufferA.set_Value(XZXMCIndex, stock.XZQMC);
                #endregion

                IFeatureCursor featureCursorA = featureClassA.Insert(true);


                IFeatureClass featureClassB = workspaceB.GetFeatureClass(table.Name);
                var dictB = GetFieldIndex(featureClassB, table.Fields);

                IFeatureCursor featureCursorB = featureClassB.Search(null, true);
                IFeature featureB = featureCursorB.NextFeature();
                var index = 0;
                while (featureB != null)
                {
                    featureBufferA.Shape = featureB.ShapeCopy;

                    #region 给每个字段赋值
                    object val = null;
                    foreach (var field in table.Fields)
                    {

                        if (dictB.ContainsKey(field.Name))
                        {
                            val = featureB.get_Value(dictB[field.Name]);
                        }
                        if (val != null)
                        {
                            if (dictA.ContainsKey(field.Name))
                            {
                                featureBufferA.set_Value(dictA[field.Name], val);
                            }
                        }

                    }
                    #endregion
                    featureCursorA.InsertFeature(featureBufferA);
                    Marshal.ReleaseComObject(featureB);
                    if (index % 1000 == 0)
                    {
                        featureCursorA.Flush();
                    }
                    featureB = featureCursorB.NextFeature();
                }
                featureCursorA.Flush();

                Marshal.ReleaseComObject(featureCursorA);
                Marshal.ReleaseComObject(featureCursorB);
                Marshal.ReleaseComObject(featureClassB);
                Marshal.ReleaseComObject(featureBufferA);
                #endregion
                Marshal.ReleaseComObject(featureClassA);

            }
            #endregion

            //workspaceEditA.StopEditOperation();
            //workspaceEditA.StopEditing(true);
            Marshal.ReleaseComObject(workspaceB);
            //Marshal.ReleaseComObject(workspaceEditA);
            //Marshal.ReleaseComObject(featureWorkspaceA);
            Marshal.ReleaseComObject(workspaceA);
        }
        public static void Import2(string savemdbFile,string sourcemdbFile,List<StockTable> tables,string XZSDM,string XZSMC)
        {
            IWorkspace workspaceA = savemdbFile.OpenAccessFileWorkSpace();
            IFeatureWorkspace featureWorkspace = workspaceA as IFeatureWorkspace;
            IFeatureWorkspaceManage featureWorkspaceManagerA = featureWorkspace as IFeatureWorkspaceManage;

            IWorkspace workspaceB = sourcemdbFile.OpenAccessFileWorkSpace();

            foreach(var table in tables)
            {
                if (table.IsSpace == false)
                {
                    continue;
                }
                var outfeatureClassName = table.Name + "_Merge";
                if(!MergeFeatureClass(string.Format("{0}\\{1};{2}\\{1}",savemdbFile,table.Name,sourcemdbFile), null, string.Format("{0}\\{1}", savemdbFile, outfeatureClassName)))
                {
                    Console.WriteLine("Merge失败");
                    continue;
                }

                IFeatureClass featureClassA = workspaceA.GetFeatureClass(table.Name);

                IDataset datasetA = featureClassA as IDataset;
                
                datasetA.Delete();


                IFeatureClass featureClassAC = workspaceA.GetFeatureClass(outfeatureClassName);
                if (featureClassAC != null)
                {
                    IDataset datasetAc = featureClassAC as IDataset;
                    try
                    {
                        if (datasetAc.CanRename())
                        {
                            datasetAc.Rename(table.Name);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("重命名失败");
                    }
                }
                Marshal.ReleaseComObject(featureClassA);
            }

            Marshal.ReleaseComObject(workspaceA);
            Marshal.ReleaseComObject(workspaceB);

        }
        public static void Import(string savemdbFile,string sourcemdbfile,List<StockTable> tables,string XZSDM,string XZSMC)
        {
            IWorkspace workspaceA = savemdbFile.OpenAccessFileWorkSpace();
            //IFeatureWorkspace featureWorkspaceA = workspaceA as IFeatureWorkspace;
            //IWorkspaceEdit workspaceEditA = featureWorkspaceA as IWorkspaceEdit;

            //workspaceEditA.StartEditing(true);
            //workspaceEditA.StartEditOperation();

            



            IWorkspace workspaceB = sourcemdbfile.OpenAccessFileWorkSpace();


            #region  插入表
            foreach(var table in tables)
            {
                if (table.IsSpace == false)
                {
                    continue;
                }

                IFeatureClass featureClassA = workspaceA.GetFeatureClass(table.Name);
                
                var dictA = GetFieldIndex(featureClassA, table.Fields);//字段 对应Index

                #region 2
                //IFeatureClassLoad featureClassLoadA = featureClassA as IFeatureClassLoad;
                //ISchemaLock schemaLock = featureClassA as ISchemaLock;
                //try
                //{
                //    //schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                //    featureClassLoadA.LoadOnlyMode = true;
                //    using (ComReleaser comReleaser=new ComReleaser())
                //    {
                //        IFeatureBuffer featureBufferA = featureClassA.CreateFeatureBuffer();

                //        comReleaser.ManageLifetime(featureBufferA);

                //        #region 设置地级市代码和名称
                //        int XZSDMIndex = featureClassA.Fields.FindField("XZSDM");
                //        var XZSMCIndex = featureClassA.Fields.FindField("XZSMC");
                //        featureBufferA.set_Value(XZSDMIndex, XZSDM);
                //        featureBufferA.set_Value(XZSMCIndex, XZSMC);
                //        #endregion

                //        IFeatureCursor featureCursorA = featureClassA.Insert(true);
                //        comReleaser.ManageLifetime(featureCursorA);

                //        IFeatureClass featureClassB = workspaceB.GetFeatureClass(table.Name);
                //        var dictB = GetFieldIndex(featureClassB, table.Fields);

                //        IFeatureCursor featureCursorB = featureClassB.Search(null, true);
                //        IFeature featureB = featureCursorB.NextFeature();
                //        while (featureB != null)
                //        {
                //            featureBufferA.Shape = featureB.ShapeCopy;

                //            #region 给每个字段赋值
                //            foreach (var field in table.Fields)
                //            {
                //                object val = null;
                //                if (dictB.ContainsKey(field.Name))
                //                {
                //                    val = featureB.get_Value(dictB[field.Name]);
                //                }
                //                if (val != null)
                //                {
                //                    if (dictA.ContainsKey(field.Name))
                //                    {
                //                        featureBufferA.set_Value(dictA[field.Name], val);
                //                    }
                //                }

                //            }
                //            #endregion
                //            featureCursorA.InsertFeature(featureBufferA);


                //            featureB = featureCursorB.NextFeature();
                //        }
                //        featureCursorA.Flush();
                //    }
                //}
                //catch (Exception)
                //{

                //}
                //finally
                //{
                //    featureClassLoadA.LoadOnlyMode = false;
                //    schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                //}

                #endregion

                #region 1

                IFeatureBuffer featureBufferA = featureClassA.CreateFeatureBuffer();

                #region 设置地级市代码和名称
                int XZSDMIndex = featureClassA.Fields.FindField("XZSDM");
                var XZSMCIndex = featureClassA.Fields.FindField("XZSMC");
                featureBufferA.set_Value(XZSDMIndex, XZSDM);
                featureBufferA.set_Value(XZSMCIndex, XZSMC);
                #endregion

                IFeatureCursor featureCursorA = featureClassA.Insert(true);


                IFeatureClass featureClassB = workspaceB.GetFeatureClass(table.Name);
                var dictB = GetFieldIndex(featureClassB, table.Fields);

                IFeatureCursor featureCursorB = featureClassB.Search(null, true);
                IFeature featureB = featureCursorB.NextFeature();
                var index = 0;
                while (featureB != null)
                {
                    featureBufferA.Shape = featureB.ShapeCopy;

                    #region 给每个字段赋值
                    object val = null;
                    foreach (var field in table.Fields)
                    {
                        
                        if (dictB.ContainsKey(field.Name))
                        {
                            val = featureB.get_Value(dictB[field.Name]);
                        }
                        if (val != null)
                        {
                            if (dictA.ContainsKey(field.Name))
                            {
                                featureBufferA.set_Value(dictA[field.Name], val);
                            }
                        }

                    }
                    #endregion
                    featureCursorA.InsertFeature(featureBufferA);
                    Marshal.ReleaseComObject(featureB);
                    if (index % 1000 == 0)
                    {
                        featureCursorA.Flush();
                    }
                    featureB = featureCursorB.NextFeature();
                }
                featureCursorA.Flush();

                Marshal.ReleaseComObject(featureCursorA);
                Marshal.ReleaseComObject(featureCursorB);
                Marshal.ReleaseComObject(featureClassB);
                Marshal.ReleaseComObject(featureBufferA);
                #endregion
                Marshal.ReleaseComObject(featureClassA);
                
            }
            #endregion

            //workspaceEditA.StopEditOperation();
            //workspaceEditA.StopEditing(true);
            Marshal.ReleaseComObject(workspaceB);
            //Marshal.ReleaseComObject(workspaceEditA);
            //Marshal.ReleaseComObject(featureWorkspaceA);
            Marshal.ReleaseComObject(workspaceA);
        }

        private static string[] Ignores = { "OBJECTID", "SHAPE", "SHAPE_LENGTH", "SHAPE_AREA", "XZSDM", "XZSMC" };

        public static void DeleteFields(string mdbfile,List<StockTable> tables)
        {
            IWorkspace workspace = mdbfile.OpenAccessFileWorkSpace();

            foreach(var table in tables)
            {
                IFeatureClass featureClass = workspace.GetFeatureClass(table.Name);
                for(var i = 0; i < featureClass.Fields.FieldCount; i++)
                {
                    IField field = featureClass.Fields.get_Field(i);
                    Console.WriteLine(table.Name+"-"+ field.Name);
                    if (Ignores.Any(e => e.ToLower() == field.Name.ToLower()) == false)
                    {
                        if (field.Type == esriFieldType.esriFieldTypeOID || field.Type == esriFieldType.esriFieldTypeGeometry || field.Type == esriFieldType.esriFieldTypeGUID)
                        {
                            continue;
                        }
                        if (table.Fields.Any(e => e.Name.ToLower() == field.Name.ToLower()) == false)
                        {
                            featureClass.DeleteField(field);
                        }
                    }
                   
                }
                Marshal.ReleaseComObject(featureClass);

            }
            Marshal.ReleaseComObject(workspace);
        }

        public static void ImportTables(string saveMdbFile,StockFile stock,List<StockTable> tables,string XZSDM,string XZSMC)
        {
            using (var connection=new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", saveMdbFile)))
            {
                connection.Open();
                foreach(var table in tables)
                {
                    if (table.IsSpace == true)
                    {
                        //var sql= string.Format("UPDATE {0} SET XZSDM = '{1}', XZSMC = '{2}' WHERE XZSDM IS NULL", table.Name, XZSDM, XZSMC);
                        //var rows= ADOSQLHelper.ExecuteNoQuery(connection, sql);
                        //if (rows <= 0)
                        //{
                        //    Console.WriteLine("更新矢量中的行政市代码和行政市名称失败");
                        //}
                        continue;
                    }
                    var sqlText = string.Format("INSERT INTO {0} SELECT {1} FROM [{2}].[{0}]", table.Name, string.Join(",", table.Fields.Select(e => e.Name).ToArray()), stock.FullName);
                    var rows1 = ADOSQLHelper.ExecuteNoQuery(connection, sqlText);
                    sqlText = string.Format("UPDATE {0} SET XZSDM = '{1}', XZSMC = '{2}',XZXDM = '{3}',XZXMC = '{4}' WHERE XZSDM IS NULL", table.Name, XZSDM, XZSMC,stock.XZQDM,stock.XZQMC);
                    var rows2 = ADOSQLHelper.ExecuteNoQuery(connection, sqlText);
                    if (rows1 == 0 || rows2 == 0 || rows1 != rows2)
                    {
                        Console.WriteLine("导入表格数据失败！");
                    }
                }

                connection.Close();
            }
        }

        public static void ImportTables2(string saveMdbFile, string sourcemdbFile, List<StockTable> tables)
        {
            using (var connection = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", saveMdbFile)))
            {
                connection.Open();
                foreach (var table in tables)
                {
                    if (table.IsSpace == true)
                    {
                        continue;
                    }
                    var sqlText = string.Format("INSERT INTO {0} SELECT {1} FROM [{2}].[{0}]", table.Name, string.Join(",", table.Fields.Select(e => e.Name).ToArray()), sourcemdbFile);
                    var rows1 = ADOSQLHelper.ExecuteNoQuery(connection, sqlText);
                    if (rows1 <= 0)
                    {
                        Console.WriteLine("插入数据失败");
                    }
                    //sqlText = string.Format("UPDATE {0} SET XZSDM = '{1}', XZSMC = '{2}' WHERE XZSDM IS NULL", table.Name, XZSDM, XZSMC);
                    //var rows2 = ADOSQLHelper.ExecuteNoQuery(connection, sqlText);
                    //if (rows1 == 0 || rows2 == 0 || rows1 != rows2)
                    //{
                    //    Console.WriteLine("导入表格数据失败！");
                    //}
                }

                connection.Close();
            }
        }
        public static void SetXZS(string filePath,List<StockTable> tables,string XZSDM,string XZSMC)
        {
            using (var connection=new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", filePath)))
            {
                connection.Open();

                foreach(var table in tables)
                {
                    var sqlText= string.Format("UPDATE {0} SET XZSDM = '{1}', XZSMC = '{2}' WHERE XZSDM IS NULL", table.Name, XZSDM, XZSMC);
                    var rows = ADOSQLHelper.ExecuteNoQuery(connection, sqlText);
                    if (rows <= 0)
                    {
                        Console.WriteLine("更新矢量中的行政市代码和行政市名称失败");
                    }
                }


                connection.Close();
            }
        }




    }
}
