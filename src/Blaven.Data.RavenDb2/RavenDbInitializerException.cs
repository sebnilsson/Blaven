using System;

namespace Blaven.Data.RavenDb2
{
    public class RavenDbInitializerException : BlavenException
    {
        public RavenDbInitializerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}