using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.CRM.Contacts
{
    public interface ICompanyContactList
    {
        void Add(ICompanyContact item);
        void Clear();
        bool Contains(ICompanyContact item);
        void CopyTo(ICompanyContact[] array, int arrayIndex);
        int Count { get; }
        IEnumerator<ICompanyContact> GetEnumerator();
        int IndexOf(ICompanyContact item);
        void Insert(int index, ICompanyContact item);
        bool IsReadOnly { get; }
        bool Remove(ICompanyContact item);
        void RemoveAt(int index);
        //bool SetPrimaryContact(int Index);
        ICompanyContact this[int index] { get; set; }
    }
}
