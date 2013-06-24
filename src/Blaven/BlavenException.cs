using System;

namespace Blaven
{
    public class BlavenException : Exception
    {
        public BlavenException()
        {
        }

        public BlavenException(string message)
            : base(message)
        {
        }

        public BlavenException(Exception innerException, string message = null)
            : base(message ?? (innerException != null ? innerException.Message : null), innerException)
        {
        }
    }
}