using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blaven {
    [Serializable]
    public class BlavenException : Exception {
        public BlavenException() { }
        public BlavenException(string message) : base(message) { }
        public BlavenException(string message, Exception inner) : base(message, inner) { }
        protected BlavenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
