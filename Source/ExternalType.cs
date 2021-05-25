using System;

namespace Svg
{
    [Flags]
    public enum ExternalType
    {
        None = 0x0,
        Local = 0x1,
        Remote = 0x2,
    }

    public static class ExternalTypeExtensions
    {
        public static bool AllowsResolving(this ExternalType externalType, Uri uri)
        {
            var isLocalUri = !uri.IsAbsoluteUri || uri.IsFile;
            return externalType.HasFlag(ExternalType.Local) && isLocalUri ||
                   externalType.HasFlag(ExternalType.Remote) && !isLocalUri;
        }
    }
}
