using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files
{
    public class ImportedFileMapper
    {
        public static bool CheckFileNotImported(IDalSession session, FileInfo FileToCheck)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("FileShortName", FileToCheck.Name));
            IList list = session.GetList(typeof(ImportedFile), expressions);
            if (list.Count == 0)
                return true;
            else
                return false;
        }

    }
}
