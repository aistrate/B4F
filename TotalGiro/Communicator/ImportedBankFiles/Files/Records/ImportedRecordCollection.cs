using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public class ImportedRecordCollection : IList<ImportedRecord>
    {

        internal ImportedRecordCollection(IList BagOfRecords, ImportedFile ParentFile)
        {
            this.BagOfRecords = BagOfRecords;
            this.ParentFile = ParentFile;

        }



        public ImportedFile ParentFile
        {
            get { return parentFile; }
            set { parentFile = value; }
        }


        public IList BagOfRecords
        {
            get { return bagOfRecords; }
            set { bagOfRecords = value; }
        }


        #region IList<ImportedRecord> Members

        public int IndexOf(ImportedRecord item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Insert(int index, ImportedRecord item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ImportedRecord this[int index]
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<ImportedRecord> Members

        public void Add(ImportedRecord item)
        {
            item.ImportedFile = ParentFile;
            this.bagOfRecords.Add(item);
            this.ParentFile.NumberOfRecords++;
        }

        public void Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(ImportedRecord item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void CopyTo(ImportedRecord[] array, int arrayIndex)
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

        public bool Remove(ImportedRecord item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<ImportedRecord> Members

        public IEnumerator<ImportedRecord> GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


        private IList bagOfRecords = new ArrayList();
        private ImportedFile parentFile;
    }
}
