using System;
namespace B4F.TotalGiro.Communicator.Exact
{
    public interface IExactAccount
    {
        string AccountNumber { get; set; }
        string Description { get; set; }
        int Key { get; set; }
        string FullDescription { get; }
    }
}
