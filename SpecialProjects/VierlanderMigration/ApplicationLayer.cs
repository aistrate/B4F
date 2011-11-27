using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Communicator.TextFiles;

namespace VierlanderMigration
{
    public class ApplicationLayer
    {
        public ApplicationLayer ()
	    {

	    }


        public static bool ImportAllFiles()
        {

            IDalSession session = NHSessionFactory.CreateSession();
            IList files = TextFileToImportMapper.GetTextFilesToImport(session);

            foreach (TextFileToImport tfi in files)
            {
                IList importedFiles = tfi.ImportAllFiles();
                TextFileToImportMapper.Update(session, importedFiles);
            }

            session.Close();

            return true;
        }
    }
}
