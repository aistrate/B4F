using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class ExactFieldCollection
    {
        private const int SIZE = 40;    // TODO: centralize this for all Exact classes
        private object[] fields = new object[SIZE + 1];

        public object this[int index]
        {
            get { checkBounds(index); return fields[index]; }
            set { checkBounds(index); fields[index] = value; }
        }

        public int Length { get { return SIZE; } }

        private void checkBounds(int index)
        {
            if (index == 0)
                throw new IndexOutOfRangeException("Cannot index by 0 in an ExactFieldCollection.");
        }
    }
}
