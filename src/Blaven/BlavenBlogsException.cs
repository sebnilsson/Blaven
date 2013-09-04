using System;
using System.Collections.Generic;

namespace Blaven
{
    public class BlavenBlogsException : BlavenException
    {
        internal BlavenBlogsException(IEnumerable<string> blogKeys)
        {
            this.BlogKeys = blogKeys;
        }

        internal BlavenBlogsException(IEnumerable<string> blogKeys, Exception inner = null, string message = null)
            : base(inner, message)
        {
            this.BlogKeys = blogKeys;
        }

        public IEnumerable<string> BlogKeys { get; private set; }
    }
}