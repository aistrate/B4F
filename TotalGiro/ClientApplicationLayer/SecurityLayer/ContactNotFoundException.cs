using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer
{
    public class ContactNotFoundException : SecurityLayerException
    {
        public ContactNotFoundException(int contactId)
            : base("Contact not authorized or not found.")
        {
            ContactId = contactId;
        }

        public int ContactId { get; private set; }
    }
}
