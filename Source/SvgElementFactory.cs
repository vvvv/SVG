using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Svg.ExCSS;

namespace Svg
{
    /// <summary>
    /// Provides the methods required in order to parse and create <see cref="SvgElement"/> instances from XML.
    /// </summary>
#if USE_SOURCE_GENERATORS
    [ElementFactory]
#endif
    internal partial class SvgElementFactory
    {
#if !USE_SOURCE_GENERATORS
        static SvgElementFactory()
        {
            // cache ElementInfo in static Field
            var svgTypes = from t in typeof(SvgDocument).Assembly.GetExportedTypes()
                let attrs = (SvgElementAttribute[])t.GetCustomAttributes(typeof(SvgElementAttribute), true)
                where attrs.Length > 0 && !string.IsNullOrWhiteSpace(attrs[0].ElementName)
                      && t.IsSubclassOf(typeof(SvgElement))
                select new ElementInfo { ElementName = attrs[0].ElementName, ElementType = t };

            availableElements = svgTypes.ToList();

            // cache ElementInfo without Svg in static field
            availableElementsWithoutSvg = availableElements
                .Where(e => !e.ElementName.Equals("svg", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(e => e.ElementName, e => e);

            // cache ElementInfo ElementTypes in static field
            availableElementsDictionary = new Dictionary<string, List<Type>>();
            foreach (var element in availableElements)
            {
                if (!availableElementsDictionary.TryGetValue(element.ElementName, out var list))
                {
                    list = new List<Type>();
                    availableElementsDictionary[element.ElementName] = list;
                }

                list.Add(element.ElementType);
            }
        }

        private static readonly Dictionary<string, List<Type>> availableElementsDictionary;
        private static readonly Dictionary<string, ElementInfo> availableElementsWithoutSvg;
        private static readonly List<ElementInfo> availableElements;
#endif
        private readonly Parser cssParser = new Parser();

        /// <summary>
        /// Gets a list of available types that can be used when creating an <see cref="SvgElement"/>.
        /// </summary>
        public List<ElementInfo> AvailableElements => availableElements;

        /// <summary>
        /// Gets a list of available types that can be used when creating an <see cref="SvgElement"/>.
        /// </summary>
        internal Dictionary<string, List<Type>> AvailableElementsDictionary => availableElementsDictionary;

        /// <summary>
        /// Creates an <see cref="SvgDocument"/> from the current node in the specified <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> containing the node to parse into an <see cref="SvgDocument"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="reader"/> parameter cannot be <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The CreateDocument method can only be used to parse root &lt;svg&gt; elements.</exception>
        public T CreateDocument<T>(XmlReader reader) where T : SvgDocument, new()
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (reader.LocalName != "svg")
            {
                throw new InvalidOperationException("The CreateDocument method can only be used to parse root <svg> elements.");
            }

            return (T)CreateElement<T>(reader, true, null);
        }

        /// <summary>
        /// Creates an <see cref="SvgElement"/> from the current node in the specified <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> containing the node to parse into a subclass of <see cref="SvgElement"/>.</param>
        /// <param name="document">The <see cref="SvgDocument"/> that the created element belongs to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="reader"/> and <paramref name="document"/> parameters cannot be <c>null</c>.</exception>
        public SvgElement CreateElement(XmlReader reader, SvgDocument document)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return CreateElement<SvgDocument>(reader, false, document);
        }

        private SvgElement CreateElement<T>(XmlReader reader, bool fragmentIsDocument, SvgDocument document) where T : SvgDocument, new()
        {
            SvgElement createdElement = null;
            string elementName = reader.LocalName;
            string elementNS = reader.NamespaceURI;

            //Trace.TraceInformation("Begin CreateElement: {0}", elementName);

            if (elementNS == SvgNamespaces.SvgNamespace || string.IsNullOrEmpty(elementNS))
            {
                if (elementName == "svg")
                {
                    createdElement = (fragmentIsDocument) ? new T() : new SvgFragment();
                }
                else
                {
#if !USE_SOURCE_GENERATORS
                    ElementInfo validType;
                    if (availableElementsWithoutSvg.TryGetValue(elementName, out validType))
                    {
                        createdElement = (SvgElement)Activator.CreateInstance(validType.ElementType);
                    }
#else
                    if (availableElementsWithoutSvg.TryGetValue(elementName, out var validType))
                    {
                        createdElement = validType.CreateInstance();
                    }
#endif
                    else
                    {
                        createdElement = new SvgUnknownElement(elementName);
                    }
                }

                if (createdElement != null)
                {
                    SetAttributes(createdElement, reader, document);
                }
            }
            else
            {
                // All non svg element (html, ...)
                createdElement = new NonSvgElement(elementName, elementNS);
                SetAttributes(createdElement, reader, document);
            }

            //Trace.TraceInformation("End CreateElement");

            return createdElement;
        }

        private void SetAttributes(SvgElement element, XmlReader reader, SvgDocument document)
        {
            //Trace.TraceInformation("Begin SetAttributes");

            //string[] styles = null;
            //string[] style = null;
            //int i = 0;

            while (reader.MoveToNextAttribute())
            {
                var prefix = reader.Prefix;
                var localName = reader.LocalName;
                if (reader.ReadAttributeValue())
                {
                    if (prefix.Length == 0)
                    {
                        if (localName.Equals("xmlns"))
                        {
                            element.Namespaces[string.Empty] = reader.Value;
                            continue;
                        }
                        else if (localName.Equals("version"))
                            continue;
                    }
                    else if (prefix.Equals("xmlns"))
                    {
                        element.Namespaces[localName] = reader.Value;
                        continue;
                    }
                    if (localName.Equals("style") && !(element is NonSvgElement))
                    {
                        var inlineSheet = cssParser.Parse("#a{" + reader.Value + "}");
                        foreach (var rule in inlineSheet.StyleRules)
                        {
                            foreach (var decl in rule.Declarations)
                            {
                                element.AddStyle(decl.Name, decl.Term.ToString(), SvgElement.StyleSpecificity_InlineStyle);
                            }
                        }
                    }
                    else if (prefix.Length == 0 && IsStyleAttribute(localName))
                    {
                        element.AddStyle(localName, reader.Value, SvgElement.StyleSpecificity_PresAttribute);
                    }
                    else
                    {
                        var ns = prefix.Length == 0 ? string.Empty : reader.LookupNamespace(prefix);
                        SetPropertyValue(element, ns, localName, reader.Value, document);
                    }
                }
            }

            //Trace.TraceInformation("End SetAttributes");
        }

        private static bool IsStyleAttribute(string name)
        {
            switch (name)
            {
                case "alignment-baseline":
                case "baseline-shift":
                case "clip":
                case "clip-path":
                case "clip-rule":
                case "color":
                case "color-interpolation":
                case "color-interpolation-filters":
                case "color-profile":
                case "color-rendering":
                case "cursor":
                case "direction":
                case "display":
                case "dominant-baseline":
                case "enable-background":
                case "fill":
                case "fill-opacity":
                case "fill-rule":
                case "filter":
                case "flood-color":
                case "flood-opacity":
                case "font":
                case "font-family":
                case "font-size":
                case "font-size-adjust":
                case "font-stretch":
                case "font-style":
                case "font-variant":
                case "font-weight":
                case "glyph-orientation-horizontal":
                case "glyph-orientation-vertical":
                case "image-rendering":
                case "kerning":
                case "letter-spacing":
                case "lighting-color":
                case "marker":
                case "marker-end":
                case "marker-mid":
                case "marker-start":
                case "mask":
                case "opacity":
                case "overflow":
                case "pointer-events":
                case "shape-rendering":
                case "stop-color":
                case "stop-opacity":
                case "stroke":
                case "stroke-dasharray":
                case "stroke-dashoffset":
                case "stroke-linecap":
                case "stroke-linejoin":
                case "stroke-miterlimit":
                case "stroke-opacity":
                case "stroke-width":
                case "text-anchor":
                case "text-decoration":
                case "text-rendering":
                case "text-transform":
                case "unicode-bidi":
                case "visibility":
                case "word-spacing":
                case "writing-mode":
                    return true;
            }
            return false;
        }
#if !USE_SOURCE_GENERATORS
        private static readonly Dictionary<Type, Dictionary<string, PropertyDescriptorCollection>> _propertyDescriptors = new Dictionary<Type, Dictionary<string, PropertyDescriptorCollection>>();
        private static readonly object syncLock = new object();
#endif
        internal static bool SetPropertyValue(SvgElement element, string ns, string attributeName, string attributeValue, SvgDocument document, bool isStyle = false)
        {
#if !USE_SOURCE_GENERATORS
            var elementType = element.GetType();
            PropertyDescriptorCollection properties;
            lock (syncLock)
            {
                if (_propertyDescriptors.Keys.Contains(elementType))
                {
                    if (_propertyDescriptors[elementType].Keys.Contains(attributeName))
                    {
                        properties = _propertyDescriptors[elementType][attributeName];
                    }
                    else
                    {
                        properties = TypeDescriptor.GetProperties(elementType, new[] { new SvgAttributeAttribute(attributeName) });
                        _propertyDescriptors[elementType].Add(attributeName, properties);
                    }
                }
                else
                {
                    properties = TypeDescriptor.GetProperties(elementType, new[] { new SvgAttributeAttribute(attributeName) });
                    _propertyDescriptors.Add(elementType, new Dictionary<string, PropertyDescriptorCollection>());

                    _propertyDescriptors[elementType].Add(attributeName, properties);
                }
            }

            if (properties.Count > 0)
            {
                PropertyDescriptor descriptor = properties[0];

                try
                {
                    if (attributeName == "opacity" && attributeValue == "undefined")
                    {
                        attributeValue = "1";
                    }
                    descriptor.SetValue(element, descriptor.Converter.ConvertFrom(document, CultureInfo.InvariantCulture, attributeValue));
                }
                catch
                {
                    Trace.TraceWarning(string.Format("Attribute '{0}' cannot be set - type '{1}' cannot convert from string '{2}'.", attributeName, descriptor.PropertyType.FullName, attributeValue));
                }
            }
            else
#else
            if (attributeName == "opacity" && attributeValue == "undefined")
            {
                attributeValue = "1";
            }
            var setValueResult = element.SetValue(attributeName, document, CultureInfo.InvariantCulture, attributeValue);
            if (setValueResult)
            {
                return true;
            }
#endif
            {
                if (isStyle)
                    // custom styles shall remain as style
                    return false;
                // attribute is not a svg attribute, store it in custom attributes
                element.CustomAttributes[ns.Length == 0 ? attributeName : $"{ns}:{attributeName}"] = attributeValue;
            }
            return true;
        }

        /// <summary>
        /// Contains information about a type inheriting from <see cref="SvgElement"/>.
        /// </summary>
        [DebuggerDisplay("{ElementName}, {ElementType}")]
        internal sealed class ElementInfo
        {
            /// <summary>
            /// Gets the SVG name of the <see cref="SvgElement"/>.
            /// </summary>
            public string ElementName { get; set; }
            /// <summary>
            /// Gets the <see cref="Type"/> of the <see cref="SvgElement"/> subclass.
            /// </summary>
            public Type ElementType { get; set; }
#if USE_SOURCE_GENERATORS
            /// <summary>
            /// Creates a new instance based on <see cref="ElementType"/> type.
            /// </summary>
            public Func<SvgElement> CreateInstance { get; set; }
#endif
            /// <summary>
            /// Initializes a new instance of the <see cref="ElementInfo"/> struct.
            /// </summary>
            /// <param name="elementName">Name of the element.</param>
            /// <param name="elementType">Type of the element.</param>
            public ElementInfo(string elementName, Type elementType)
            {
                this.ElementName = elementName;
                this.ElementType = elementType;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ElementInfo"/> class.
            /// </summary>
            public ElementInfo()
            {
            }
        }
    }
}
