using System;
using System.Runtime.Serialization;

namespace Svg.Exceptions
{
    [Serializable]
    public sealed class SvgMemoryException : Exception
    {
        public SvgMemoryException() { }
        public SvgMemoryException(string message) : base(message) { }
        public SvgMemoryException(string message, Exception inner) : base(message, inner) { }

        private SvgMemoryException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
