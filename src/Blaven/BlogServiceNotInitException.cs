using System;

namespace Blaven {
    [Serializable]
    public class BlogServiceNotInitException : BlavenException {
        public BlogServiceNotInitException(Exception inner) : base("Error fetching data from an index. Initialize the BlogStore.", inner) { }
    }
}
