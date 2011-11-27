using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Notas
{
    public class NotaAccount : INotaAccount
    {
        public virtual int Key { get; set; }

        public virtual IAccountTypeInternal Account { get; private set; }
    }
}
