using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace B4F.Web.WebControls
{
    /// <summary>
    /// Helper class for MultipleSelectionGridView; holds the selected Ids
    /// </summary>
    [Serializable]
    public class SelectionBuffer
    {
        private Hashtable selectedIds = new Hashtable();

        public void SetSelected(int id, bool value)
        {
            if (value)
                selectedIds[id] = null;
            else if (selectedIds.Contains(id))
                selectedIds.Remove(id);
        }

        public bool GetSelected(int id)
        {
            return selectedIds.Contains(id);
        }

        public int[] GetSelectedIds()
        {
            ArrayList idList = new ArrayList(selectedIds.Keys);
            idList.Sort();

            int[] result = new int[idList.Count];
            idList.CopyTo(result, 0);
            
            return result;
        }

        public void CopyFrom(IEnumerable coll)
        {
            selectedIds.Clear();
            foreach (int id in coll)
                selectedIds[id] = null;
        }

        // returns true if the selected set has changed
        public bool IntersectWith(IEnumerable coll)
        {
            bool hasChanged = false;

            if (selectedIds.Count > 0)
            {
                Hashtable other = new Hashtable();
                foreach (int id in coll)
                    other[id] = null;

                ArrayList idList = new ArrayList(selectedIds.Keys);
                foreach (int id in idList)
                    if (!other.Contains(id))
                    {
                        selectedIds.Remove(id);
                        hasChanged = true;
                    }
            }

            return hasChanged;
        }

        public void Clear()
        {
            selectedIds.Clear();
        }
    }
}
