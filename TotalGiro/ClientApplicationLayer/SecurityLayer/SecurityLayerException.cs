using System;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer
{
    public class SecurityLayerException : ApplicationException
    {
        public SecurityLayerException()
            : base("User not authorized to perform this action.") { }
        
        public SecurityLayerException(string message)
            : base(message) { }
        
        public SecurityLayerException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
