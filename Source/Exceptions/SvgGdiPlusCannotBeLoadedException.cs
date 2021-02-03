using System;
using System.Runtime.Serialization;

namespace Svg
{
    [Serializable]
    public sealed class SvgGdiPlusCannotBeLoadedException : Exception
    {
        const string gdiErrorMsg = "Cannot initialize gdi+ libraries. This is likely to be caused by running on a non-Windows OS without proper gdi+ replacement. Please refer to the documentation for more details.";

        public SvgGdiPlusCannotBeLoadedException() : base(gdiErrorMsg) { }
        public SvgGdiPlusCannotBeLoadedException(string message) : base(message) { }
        public SvgGdiPlusCannotBeLoadedException(Exception inner) : base(gdiErrorMsg, inner) {}
        public SvgGdiPlusCannotBeLoadedException(string message, Exception inner) : base(message, inner) { }

        private SvgGdiPlusCannotBeLoadedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
