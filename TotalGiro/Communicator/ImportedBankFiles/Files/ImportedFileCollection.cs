using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files
{
    public class ImportedFileCollection : IList<ImportedFile>
    {

        internal ImportedFileCollection(IList BagOfRecords, FileToImport Parent)
        {
            this.bagOfRecords = BagOfRecords;
            this.Parent = Parent;

        }



        public FileToImport Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        //public IList BagOfRecords
        //{
        //    get { return bagOfRecords; }
        //    set { bagOfRecords = value; }
        //}



        #region IList<ImportedFiles> Members

        public int IndexOf(ImportedFile item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Insert(int index, ImportedFile item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ImportedFile this[int index]
        {
            get
            {
                return (ImportedFile)(this.bagOfRecords[index]);
            }
            set
            {
                this.bagOfRecords[index] = (ImportedFile)value;
            }
        }

        #endregion

        #region ICollection<ImportedFiles> Members

        public void Add(ImportedFile item)
        {
            this.bagOfRecords.Add(item);

        }

        public void Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(ImportedFile item)
        {
            bool ret = false;

            foreach (ImportedFile colItem in this)
            {
                if (colItem.Equals(item))
                {
                    return ret = true;
                }
            }
            return ret;
        }

        public void CopyTo(ImportedFile[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { return this.bagOfRecords.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool Remove(ImportedFile item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<ImportedFile> Members

        public IEnumerator<ImportedFile> GetEnumerator()
        {
            for (int i = 0; i < this.bagOfRecords.Count; i++)
                yield return (ImportedFile)this.bagOfRecords[i];
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Private Variables

        private FileToImport parent;
        private IList bagOfRecords = new ArrayList();

        #endregion

    }
}
