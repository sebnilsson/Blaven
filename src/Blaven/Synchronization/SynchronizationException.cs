using System;

namespace Blaven.Synchronization
{
    public class SynchronizationException : BlavenException
    {
        public SynchronizationException(string message)
            : base(message)
        {
        }

        public SynchronizationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}