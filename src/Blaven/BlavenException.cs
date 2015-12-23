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

        public BlavenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}