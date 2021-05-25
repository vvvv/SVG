using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Svg
{
    internal class SvgDtdResolver : XmlUrlResolver
    {
        /// <summary>
        /// Defaults to <see cref="ExternalType.None"/> to prevent XXE.  Set to <see cref="ExternalType.Local"/> and/or <see cref="ExternalType"/> to resolve external resources.
        /// </summary>
        /// <see ref="https://owasp.org/www-community/vulnerabilities/XML_External_Entity_(XXE)_Processing"/>
        public ExternalType ResolveExternalXmlEntities { get; set; }

        /// <summary>
        /// Maps a URI to an object containing the actual resource.
        /// </summary>
        /// <param name="absoluteUri">The URI returned from <see cref="M:System.Xml.XmlResolver.ResolveUri(System.Uri,System.String)"/></param>
        /// <param name="role">The current implementation does not use this parameter when resolving URIs. This is provided for future extensibility purposes. For example, this can be mapped to the xlink:role and used as an implementation specific argument in other scenarios.</param>
        /// <param name="ofObjectToReturn">The type of object to return. The current implementation only returns System.IO.Stream objects.</param>
        /// <returns>
        /// A System.IO.Stream object or null if a type other than stream is specified.
        /// </returns>
        /// <exception cref="T:System.Xml.XmlException">
        ///     <paramref name="ofObjectToReturn"/> is neither null nor a Stream type. </exception>
        /// <exception cref="T:System.UriFormatException">The specified URI is not an absolute URI. </exception>
        /// <exception cref="T:System.NullReferenceException">
        ///     <paramref name="absoluteUri"/> is null. </exception>
        /// <exception cref="T:System.Exception">There is a runtime error (for example, an interrupted server connection). </exception>
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (IsSvgDtdEntity(absoluteUri))
            {
                return Assembly.GetExecutingAssembly().GetManifestResourceStream("Svg.Resources.svg11.dtd");
            }

            if (ResolveExternalXmlEntities.AllowsResolving(absoluteUri))
            {
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }

            Trace.TraceWarning("Trying to resolve entity from '{0}', but resolving external entities of that type is disabled.", absoluteUri);
            return new MemoryStream();
        }

        private static bool IsSvgDtdEntity(Uri absoluteUri)
        {
            return _svgDtdRegex.IsMatch(absoluteUri.ToString());
        }

        /// <summary>
        /// Matches any reference to svg00.dtd or DTD SVG 0.0 (case-insensitive)
        /// </summary>
        /// <see ref="https://regexper.com/#%28%3F%3ASVG%5B0-9%5D%2B%5C.DTD%29%7C%28%3F%3ADTD%20SVG%20%5B0-9%5C.%5D%2B%29"/>
        private static readonly Regex _svgDtdRegex
            = new Regex(@"(?:SVG[0-9]+\.DTD)|(?:DTD SVG [0-9\.]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
