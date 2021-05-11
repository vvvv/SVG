using System;
using System.Runtime.Serialization;

namespace Svg
{
    [Serializable]
    public class SvgException : FormatException
    {
        public SvgException() { }
        public SvgException(string message) : base(message) { }
        public SvgException(string message, Exception inner) : base(message, inner) { }

        protected SvgException(SerializationInfo info, StreamingContext context)
            : base (info, context) { }
    }

    [Serializable]
    public class SvgIDException : FormatException
    {
        public SvgIDException() { }
        public SvgIDException(string message) : base(message) { }
        public SvgIDException(string message, Exception inner) : base(message, inner) { }

        protected SvgIDException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public sealed class SvgIDExistsException : SvgIDException
    {
        public SvgIDExistsException() { }
        public SvgIDExistsException(string message) : base(message) { }
        public SvgIDExistsException(string message, Exception inner) : base(message, inner) { }

        private SvgIDExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public sealed class SvgIDWrongFormatException : SvgIDException
    {
        public SvgIDWrongFormatException() { }
        public SvgIDWrongFormatException(string message) : base(message) { }
        public SvgIDWrongFormatException(string message, Exception inner) : base(message, inner) { }

        private SvgIDWrongFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
