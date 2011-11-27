using System;

namespace B4F.TotalGiro.ApplicationLayer.Common
{
    public class GridviewNoSelectionException : ApplicationException
    {
        public GridviewNoSelectionException()
            : base("You did not make a selection.") { }

        public GridviewNoSelectionException(string message)
            : base(message) { }

        public GridviewNoSelectionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
