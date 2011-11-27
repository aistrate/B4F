using System;

namespace B4F.TotalGiro.Dal
{
    public class DalPageInfo
    {
        public DalPageInfo(int maximumRows, int startRowIndex)
        {
            this.maximumRows = maximumRows;
            this.startRowIndex = startRowIndex;
        }
        
        public int MaximumRows
        {
            get { return this.maximumRows; }
        }

        public int StartRowIndex
        {
            get { return this.startRowIndex; }
        }

        public bool PagingEnabled
        {
            get { return (this.startRowIndex >=0 && this.maximumRows > 0) ; }
        }

        #region Privates

        private int maximumRows;
        private int startRowIndex = -1;

        #endregion

    }
}
