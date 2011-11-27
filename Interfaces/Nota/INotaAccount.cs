using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Notas
{
    public interface INotaAccount
    {
        int Key { get; }
        IAccountTypeInternal Account { get; }
    }
}
