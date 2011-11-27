using System;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Reports.Documents
{
    /// <summary>
    /// Represents a PDF document stored in a file
    /// </summary>
    public abstract class Document : IDocument
    {
        public Document() { }

        public Document(string fileName, string filePath, bool sentByPost)
        {
            FileName = fileName;
            FilePath = filePath;
            EmailNotificationHandled = false;
            SentByPost = sentByPost;
        }

        public virtual int Key { get; set; }
        public virtual DateTime CreationDate { get; private set; }
        public virtual string FileName { get; set; }
        public virtual bool EmailNotificationHandled { get; set; }
        public virtual bool SentByPost { get; set; }
        
        public virtual string FilePath
        {
            get { return filePath; }
            set { filePath = value.Replace('/', '\\').TrimEnd().TrimEnd('\\').Replace(@"\\", @"\"); }
        }

        public string FullPath
        {
            get
            {
                if (FilePath == null || FilePath.Trim() == string.Empty ||
                    FileName == null || FileName.Trim() == string.Empty)
                    return string.Empty;
                else
                    return FilePath + @"\" + FileName;
            }
        }

        public abstract ICustomerAccount Account { get; }

        private string filePath;
    }
}
