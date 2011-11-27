using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Communicator.FSInterface;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class DownloadFSFileAdapter
    {
        public static string GetFSFileName(int fileid)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            FSExportFile fsfile = FSExportFileMapper.GetExportFile(session, fileid);

            string fsfilename = fsfile.FullName;

            session.Close();

            return fsfilename;
        }
    }
}
