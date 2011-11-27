using System;

namespace B4F.TotalGiro.Dal
{
    public interface IDalSessionFactory
    {
        IDalSession CreateSession();
    }
}
