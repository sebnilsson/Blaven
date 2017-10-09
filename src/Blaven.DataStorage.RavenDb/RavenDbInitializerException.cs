using System;

namespace Blaven.DataStorage.RavenDb
{
    public class RavenDbInitializerException : BlavenException
    {
        public RavenDbInitializerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}