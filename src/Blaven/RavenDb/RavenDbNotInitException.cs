using System;

namespace Blaven.RavenDb
{
    [Serializable]
    public class RavenDbNotInitException : BlavenException
    {
        public RavenDbNotInitException(Exception inner)
            : base(inner, "Error fetching data from an index. Initialize RavenDB.")
        {
        }
    }
}