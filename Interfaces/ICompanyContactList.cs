using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.CRM.Contacts
{
    public interface ICompanyContactList
    {
        void Add(ICompanyContactPerson item);
        void Clear();
        bool Contains(ICompanyContactPerson item);
        void CopyTo(ICompanyContactPerson[] array, int arrayIndex);
        int Count { get; }
        IEnumerator<ICompanyContactPerson> GetEnumerator();
        int IndexOf(ICompanyContactPerson item);
        void Insert(int index, ICompanyContactPerson item);
        bool IsReadOnly { get; }
        bool Remove(ICompanyContactPerson item);
        void RemoveAt(int index);
        //bool SetPrimaryContact(int Index);
        ICompanyContactPerson this[int index] { get; set; }
    }
}
