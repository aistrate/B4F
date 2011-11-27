using System;
namespace B4F.TotalGiro.Communicator.ExternalInterfaces
{
    public interface IExternalInterface
    {
        string Description { get; set; }
        int Key { get; set; }
        string Name { get; set; }
    }
}
