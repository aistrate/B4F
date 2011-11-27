using System;
using System.IO;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Reports.Documents;

namespace B4F.TotalGiro.ClientApplicationLayer.Reports
{
    public static class DocViewerAdapter
    {
        public static byte[] GetDocumentContent(int documentId, out string fileName)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                fileName = "";

                try
                {
                    IDocument document = SecurityLayerAdapter.GetOwnedDocument(session, documentId);
                    
                    if (!string.IsNullOrEmpty(document.FullPath))
                    {
                        fileName = document.FileName;
                        return File.ReadAllBytes(document.FullPath);
                    }
                    else
                        throw new ApplicationException(string.Format("File name could not be retrieved for document '{0}'.", documentId));
                }
                catch (DirectoryNotFoundException)
                {
                    throw new DirectoryNotFoundException(string.Format("Physical file path not found for document \"{0}\".", fileName));
                }
                catch (FileNotFoundException)
                {
                    throw new FileNotFoundException(string.Format("Physical file not found for document \"{0}\".", fileName));
                }
                catch (IOException)
                {
                    throw new IOException(string.Format("Problem retrieving physical file for document \"{0}\".", fileName));
                }
            }
        }
    }
}
