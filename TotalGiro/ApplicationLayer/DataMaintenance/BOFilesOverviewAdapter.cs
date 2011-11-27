//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using B4F.TotalGiro.Dal;
//using B4F.TotalGiro.Communicator.TextFiles;
//using System.Collections;

//namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
//{
//    public static class BOFilesOverviewAdapter
//    {
//        public static DataSet GetAllRecordsTextFile(int id)
//        {
//            IDalSession session = NHSessionFactory.CreateSession();
//            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
//                TextFileToImportMapper.GetAllRecords(session, id),
//                    "Key, Name, FileCreationDate, NumberOfRecords, DateFileImported");
//            session.Close();
//            return ds;
//        }

//        public static DataSet GetTextFileNames()
//        {
//            IDalSession session = NHSessionFactory.CreateSession();
//            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
//                TextFileToImportMapper.GetTextFilesToImport(session), "Key, Name");
//            session.Close();

//            return ds;
//        }

//        public static DataSet GetAllRecordsforImportFile(int id)
//        {
//            IDalSession session = NHSessionFactory.CreateSession();
//            ImportedFile ImportFile = (ImportedFile)session.GetObjectInstance(typeof(ImportedFile), id);

//            string dataSetFieldnames = ImportFile.ImportFileSpecification.DataSetFieldNames;
//            IList ObjectList = TextFileToImportMapper.GetAllImportedRecords(session, ImportFile);

//            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(ObjectList, dataSetFieldnames);
//            session.Close();
            
//            return ds;
//        }
//    }
//}
