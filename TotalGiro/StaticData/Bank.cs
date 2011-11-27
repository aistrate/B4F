using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.StaticData
{
    public class Bank : IBank
    {
        protected Bank() { }

        public Bank(string name, Address address)
        {
            this.Name = name;
            this.Address = address;
        }

        public virtual int Key { get; set; }
        public virtual string Name { get; set; }
        public virtual Address Address { get; set; }
        public virtual bool UseElfProef { get; set; }

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns>Name</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
