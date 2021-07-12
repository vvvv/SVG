using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using Svg.ExCSS;
using Svg.Css;
using System.Threading;
using System.Globalization;
using Svg.Exceptions;
using System.Runtime.InteropServices;

namespace Svg
{
    /// <summary>
    /// The class used to create and load SVG documents.
    /// </summary>
    public partial class SvgDocument : SvgFragment, ITypeDescriptorContext
    {
        /// <summary>
        /// Skip check whether the GDI+ can be loaded.
        /// </summary>
        /// <remarks>
        /// Set to true on systems that do not support GDI+ like UWP.
        /// </remarks>
        public static bool SkipGdiPlusCapabilityCheck { get; set; }

        /// <summary>
        /// Skip the Dtd Processing for faster loading of svgs that have a DTD specified.
        /// For Example Adobe Illustrator svgs.
        /// </summary>
        public static bool DisableDtdProcessing { get; set; }

        /// <summary>
        /// Defaults to `false` to prevent XXE.  Set to `true` to resolve external resources.
        /// </summary>
        /// <see ref="https://owasp.org/www-community/vulnerabilities/XML_External_Entity_(XXE)_Processing"/>
        public static bool ResolveExternalResources { get; set; }

        private static int? pointsPerInch;

        public static int PointsPerInch
        {
            get { return pointsPerInch ?? (int)(pointsPerInch = GetSystemDpi()); }
            set { pointsPerInch = value; }
        }

        private SvgElementIdManager _idManager;

        private Dictionary<string, IEnumerable<SvgFontFace>> _fontDefns = null;

        public override SvgOverflow Overflow
        {
            get { return GetAttribute("overflow", false, SvgOverflow.Visible); }
        }

        private static int GetSystemDpi()
        {
            bool isWindows;

#if NETCORE
            isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
            var platform = Environment.OSVersion.Platform;
            isWindows = platform == PlatformID.Win32NT;
#endif

            if (isWindows)
            {
                // NOTE: starting with Windows 8.1, the DPI is no longer system-wide but screen-specific
                IntPtr hDC = GetDC(IntPtr.Zero);
                const int LOGPIXELSY = 90;
                int result = GetDeviceCaps(hDC, LOGPIXELSY);
                ReleaseDC(IntPtr.Zero, hDC);
                return result;
            }
            else
            {
                // hack for macOS and Linux
                return 96;
            }
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        internal Dictionary<string, IEnumerable<SvgFontFace>> FontDefns()
        {
            if (_fontDefns == null)
            {
                _fontDefns = (from f in Descendants().OfType<SvgFontFace>()
                              group f by f.FontFamily into family
                              select family).ToDictionary(f => f.Key, f => (IEnumerable<SvgFontFace>)f);
            }
            return _fontDefns;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgDocument"/> class.
        /// </summary>
        public SvgDocument()
        {
            Ppi = PointsPerInch;
        }

        private Uri baseUri;

        public Uri BaseUri
        {
            get { return baseUri; }
            set
            {
                if (value != null && !value.IsAbsoluteUri)
                    throw new ArgumentException("BaseUri is not absolute.");
                baseUri = value;
            }
        }

        /// <summary>
        /// Gets an <see cref="SvgElementIdManager"/> for this document.
        /// </summary>
        protected internal virtual SvgElementIdManager IdManager
        {
            get
            {
                if (_idManager == null)
                {
                    _idManager = new SvgElementIdManager(this);
                }

                return _idManager;
            }
        }

        /// <summary>
        /// Overwrites the current IdManager with a custom implementation.
        /// Be careful with this: If elements have been inserted into the document before,
        /// you have to take care that the new IdManager also knows of them.
        /// </summary>
        /// <param name="manager"></param>
        public void OverwriteIdManager(SvgElementIdManager manager)
        {
            _idManager = manager;
        }

        /// <summary>
        /// Gets or sets the Pixels Per Inch of the rendered image.
        /// </summary>
        public int Ppi { get; set; }

        /// <summary>
        /// Gets or sets an external Cascading Style Sheet (CSS)
        /// </summary>
        public string ExternalCSSHref { get; set; }

        internal SvgFontManager FontManager { get; private set; }

        #region ITypeDescriptorContext Members

        IContainer ITypeDescriptorContext.Container
        {
            get { throw new NotImplementedException(); }
        }

        object ITypeDescriptorContext.Instance
        {
            get { return this; }
        }

        void ITypeDescriptorContext.OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        bool ITypeDescriptorContext.OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get { throw new NotImplementedException(); }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Retrieves the <see cref="SvgElement"/> with the specified ID.
        /// </summary>
        /// <param name="id">A <see cref="string"/> containing the ID of the element to find.</param>
        /// <returns>An <see cref="SvgElement"/> of one exists with the specified ID; otherwise false.</returns>
        public virtual SvgElement GetElementById(string id)
        {
            return IdManager.GetElementById(id);
        }

        /// <summary>
        /// Retrieves the <see cref="SvgElement"/> with the specified ID.
        /// </summary>
        /// <param name="id">A <see cref="string"/> containing the ID of the element to find.</param>
        /// <returns>An <see cref="SvgElement"/> of one exists with the specified ID; otherwise false.</returns>
        public virtual TSvgElement GetElementById<TSvgElement>(string id) where TSvgElement : SvgElement
        {
            return (this.GetElementById(id) as TSvgElement);
        }

        /// <summary>
        /// Validate whether the system has GDI+ capabilities (non Windows related).
        /// </summary>
        /// <returns>Boolean whether the system is capable of using GDI+</returns>
        public static bool SystemIsGdiPlusCapable()
        {
            try
            {
                EnsureSystemIsGdiPlusCapable();
            }
            catch (SvgGdiPlusCannotBeLoadedException)
            {
                return false;
            }
            catch (Exception)
            {
                //If somehow another type of exception is raised by the ensure function we will let it bubble up, since that might indicate other issues/problems
                throw;
            }
            return true;
        }

        /// <summary>
        /// Ensure that the running system is GDI capable, if not this will yield a
        /// SvgGdiPlusCannotBeLoadedException exception.
        /// </summary>
        public static void EnsureSystemIsGdiPlusCapable()
        {
            try
            {
                using (var matrix = new Matrix(0f, 0f, 0f, 0f, 0f, 0f)) { }
            }
            // GDI+ loading errors will result in TypeInitializationExceptions,
            // for readability we will catch and wrap the error
            catch (Exception e)
            {
                if (ExceptionCaughtIsGdiPlusRelated(e))
                {
                    // Throw only the customized exception if we are sure GDI+ is causing the problem
                    throw new SvgGdiPlusCannotBeLoadedException(e);
                }
                //If the Matrix creation is causing another type of exception we should just raise that one
                throw;
            }
        }

        /// <summary>
        /// Check if the current exception or one of its children is the targeted GDI+ exception.
        /// It can be hidden in one of the InnerExceptions, so we need to iterate over them.
        /// </summary>
        /// <param name="e">The exception to validate against the GDI+ check</param>
        private static bool ExceptionCaughtIsGdiPlusRelated(Exception e)
        {
            var currE = e;
            int cnt = 0; // Keep track of depth to prevent endless-loops
            while (currE != null && cnt < 10)
            {
                var typeException = currE as DllNotFoundException;
                if (typeException?.Message?.LastIndexOf("libgdiplus", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    return true;
                }
                currE = currE.InnerException;
                cnt++;
            }
            return false;
        }

        /// <summary>
        /// Opens the document at the specified path and loads the SVG contents.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path of the file to open.</param>
        /// <returns>An <see cref="SvgDocument"/> with the contents loaded.</returns>
        /// <exception cref="FileNotFoundException">The document at the specified <paramref name="path"/> cannot be found.</exception>
        public static SvgDocument Open(string path)
        {
            return Open<SvgDocument>(path, null);
        }

        /// <summary>
        /// Opens the document at the specified path and loads the SVG contents.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path of the file to open.</param>
        /// <returns>An <see cref="SvgDocument"/> with the contents loaded.</returns>
        /// <exception cref="FileNotFoundException">The document at the specified <paramref name="path"/> cannot be found.</exception>
        public static T Open<T>(string path) where T : SvgDocument, new()
        {
            return Open<T>(path, null);
        }

        /// <summary>
        /// Opens the document at the specified path and loads the SVG contents.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path of the file to open.</param>
        /// <param name="entities">A dictionary of custom entity definitions to be used when resolving XML entities within the document.</param>
        /// <returns>An <see cref="SvgDocument"/> with the contents loaded.</returns>
        /// <exception cref="FileNotFoundException">The document at the specified <paramref name="path"/> cannot be found.</exception>
        public static T Open<T>(string path, Dictionary<string, string> entities) where T : SvgDocument, new()
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified document cannot be found.", path);
            }

            using (var stream = File.OpenRead(path))
            {
                var doc = Open<T>(stream, entities);
                doc.BaseUri = new Uri(System.IO.Path.GetFullPath(path));
                return doc;
            }
        }

        /// <summary>
        /// Attempts to open an SVG document from the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing the SVG document to open.</param>
        public static T Open<T>(Stream stream) where T : SvgDocument, new()
        {
            return Open<T>(stream, null);
        }


        /// <summary>
        /// Attempts to create an SVG document from the specified string data.
        /// </summary>
        /// <param name="svg">The SVG data.</param>
        public static T FromSvg<T>(string svg) where T : SvgDocument, new()
        {
            if (string.IsNullOrEmpty(svg))
            {
                throw new ArgumentNullException("svg");
            }

            using (var strReader = new System.IO.StringReader(svg))
            {
                var reader = new SvgTextReader(strReader, null)
                {
                    XmlResolver = new SvgDtdResolver
                    {
                        ResolveExternalResources = ResolveExternalResources
                    },
                    WhitespaceHandling = WhitespaceHandling.Significant,
                    DtdProcessing = SvgDocument.DisableDtdProcessing ? DtdProcessing.Ignore : DtdProcessing.Parse,
                };
                return Open<T>(reader);
            }
        }

        /// <summary>
        /// Opens an SVG document from the specified <see cref="Stream"/> and adds the specified entities.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing the SVG document to open.</param>
        /// <param name="entities">Custom entity definitions.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> parameter cannot be <c>null</c>.</exception>
        public static T Open<T>(Stream stream, Dictionary<string, string> entities) where T : SvgDocument, new()
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            // Don't close the stream via a dispose: that is the client's job.
            var reader = new SvgTextReader(stream, entities)
            {
                XmlResolver = new SvgDtdResolver
                {
                    ResolveExternalResources = ResolveExternalResources
                },
                WhitespaceHandling = WhitespaceHandling.Significant,
                DtdProcessing = SvgDocument.DisableDtdProcessing ? DtdProcessing.Ignore : DtdProcessing.Parse,
            };
            return Open<T>(reader);
        }

        private static T Open<T>(XmlReader reader) where T : SvgDocument, new()
        {
            if (!SkipGdiPlusCapabilityCheck)
            {
                EnsureSystemIsGdiPlusCapable(); //Validate whether the GDI+ can be loaded, this will yield an exception if not
            }
            var elementStack = new Stack<SvgElement>();
            bool elementEmpty;
            SvgElement element = null;
            SvgElement parent;
            T svgDocument = null;
            var elementFactory = new SvgElementFactory();

            var styles = new List<ISvgNode>();

            while (reader.Read())
            {
                try
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            // Does this element have a value or children
                            // (Must do this check here before we progress to another node)
                            elementEmpty = reader.IsEmptyElement;
                            // Create element
                            if (elementStack.Count > 0)
                            {
                                element = elementFactory.CreateElement(reader, svgDocument);
                            }
                            else
                            {
                                svgDocument = elementFactory.CreateDocument<T>(reader);
                                element = svgDocument;
                            }

                            // Add to the parents children
                            if (elementStack.Count > 0)
                            {
                                parent = elementStack.Peek();
                                if (parent != null && element != null)
                                {
                                    parent.Children.Add(element);
                                    parent.Nodes.Add(element);
                                }
                            }

                            // Push element into stack
                            elementStack.Push(element);

                            // Need to process if the element is empty
                            if (elementEmpty)
                            {
                                goto case XmlNodeType.EndElement;
                            }

                            break;
                        case XmlNodeType.EndElement:

                            // Pop the element out of the stack
                            element = elementStack.Pop();

                            if (element.HasNodes())
                            {
                                if (element.Nodes.OfType<SvgContentNode>().Any())
                                {
                                    element.Content = (from e in element.Nodes select e.Content).Aggregate((p, c) => p + c);
                                }
                                else
                                {
                                    element.Nodes.Clear(); // No sense wasting the space where it isn't needed
                                }
                            }

                            var unknown = element as SvgUnknownElement;
                            if (unknown != null && unknown.ElementName == "style")
                            {
                                styles.Add(unknown);
                            }
                            break;
                        case XmlNodeType.CDATA:
                        case XmlNodeType.Text:
                        case XmlNodeType.SignificantWhitespace:
                            element = elementStack.Peek();
                            element.Nodes.Add(new SvgContentNode() { Content = reader.Value });
                            break;
                        case XmlNodeType.EntityReference:
                            reader.ResolveEntity();
                            element = elementStack.Peek();
                            element.Nodes.Add(new SvgContentNode() { Content = reader.Value });
                            break;
                    }
                }
                catch (Exception exc)
                {
                    Trace.TraceError(exc.Message);
                }
            }

            if (styles.Any())
            {
                var cssTotal = styles.Select((s) => s.Content).Aggregate((p, c) => p + Environment.NewLine + c);
                var cssParser = new Parser();
                var sheet = cssParser.Parse(cssTotal ?? string.Empty);

                foreach (var rule in sheet.StyleRules)
                {
                    try
                    {
                        var rootNode = new NonSvgElement();
                        rootNode.Children.Add(svgDocument);

                        var elemsToStyle = rootNode.QuerySelectorAll(rule.Selector.ToString(), elementFactory);
                        foreach (var elem in elemsToStyle)
                        {
                            foreach (var decl in rule.Declarations)
                            {
                                elem.AddStyle(decl.Name, decl.Term.ToString(), rule.Selector.GetSpecificity());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning(ex.Message);
                    }
                }
            }

            svgDocument?.FlushStyles(true);
            return svgDocument;
        }

        /// <summary>
        /// Opens an SVG document from the specified <see cref="XmlDocument"/>.
        /// </summary>
        /// <param name="document">The <see cref="XmlDocument"/> containing the SVG document XML.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="document"/> parameter cannot be <c>null</c>.</exception>
        public static SvgDocument Open(XmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var reader = new SvgNodeReader(document.DocumentElement, null);
            return Open<SvgDocument>(reader);
        }

        public static Bitmap OpenAsBitmap(string path)
        {
            return null;
        }

        public static Bitmap OpenAsBitmap(XmlDocument document)
        {
            return null;
        }

        private void Draw(ISvgRenderer renderer, ISvgBoundable boundable)
        {
            using (FontManager = new SvgFontManager())
            {
                renderer.SetBoundable(boundable);
                Render(renderer);
                FontManager = null;
            }
        }

        /// <summary>
        /// Renders the <see cref="SvgDocument"/> to the specified <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to render the document with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="renderer"/> parameter cannot be <c>null</c>.</exception>
        public void Draw(ISvgRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            this.Draw(renderer, this);
        }

        /// <summary>
        /// Renders the <see cref="SvgDocument"/> to the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="Graphics"/> to be rendered to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="graphics"/> parameter cannot be <c>null</c>.</exception>
        public void Draw(Graphics graphics)
        {
            this.Draw(graphics, null);
        }

        /// <summary>
        /// Renders the <see cref="SvgDocument"/> to the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="Graphics"/> to be rendered to.</param>
        /// <param name="size">The <see cref="SizeF"/> to render the document. If <c>null</c> document is rendered at the default document size.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="graphics"/> parameter cannot be <c>null</c>.</exception>
        public void Draw(Graphics graphics, SizeF? size)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            using (var renderer = SvgRenderer.FromGraphics(graphics))
            {
                var boundable = size.HasValue ? (ISvgBoundable)new GenericBoundable(0, 0, size.Value.Width, size.Value.Height) : this;
                this.Draw(renderer, boundable);
            }
        }

        /// <summary>
        /// Renders the <see cref="SvgDocument"/> and returns the image as a <see cref="Bitmap"/>.
        /// </summary>
        /// <returns>A <see cref="Bitmap"/> containing the rendered document.</returns>
        public virtual Bitmap Draw()
        {
            //Trace.TraceInformation("Begin Render");

            var size = Size.Round(GetDimensions());
            if (size.Width <= 0 || size.Height <= 0)
                return null;

            Bitmap bitmap = null;
            try
            {
                try
                {
                    bitmap = new Bitmap(size.Width, size.Height);
                }
                catch (ArgumentException e)
                {
                    // When processing too many files at one the system can run out of memory
                    throw new SvgMemoryException("Cannot process SVG file, cannot allocate the required memory", e);
                }

                //bitmap.SetResolution(300, 300);

                this.Draw(bitmap);
            }
            catch
            {
                bitmap?.Dispose();
                throw;
            }

            //Trace.TraceInformation("End Render");
            return bitmap;
        }

        /// <summary>
        /// Renders the <see cref="SvgDocument"/> into a given Bitmap <see cref="Bitmap"/>.
        /// </summary>
        public virtual void Draw(Bitmap bitmap)
        {
            //Trace.TraceInformation("Begin Render");

            using (var renderer = SvgRenderer.FromImage(bitmap))
            {
                var boundable = new GenericBoundable(0, 0, bitmap.Width, bitmap.Height);
                this.Draw(renderer, boundable);
            }

            //Trace.TraceInformation("End Render");
        }

        /// <summary>
        /// Renders the <see cref="SvgDocument"/> in given size and returns the image as a <see cref="Bitmap"/>.
        /// If one of rasterWidth and rasterHeight is zero, the image is scaled preserving aspect ratio,
        /// otherwise the aspect ratio is ignored.
        /// </summary>
        /// <returns>A <see cref="Bitmap"/> containing the rendered document.</returns>
        public virtual Bitmap Draw(int rasterWidth, int rasterHeight)
        {
            var svgSize = GetDimensions();
            var imageSize = svgSize;
            this.RasterizeDimensions(ref imageSize, rasterWidth, rasterHeight);

            var bitmapSize = Size.Round(imageSize);
            if (bitmapSize.Width <= 0 || bitmapSize.Height <= 0)
                return null;

            Bitmap bitmap = null;
            try
            {
                try
                {
                    bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);
                }
                catch (ArgumentException e)
                {
                    // When processing too many files at one the system can run out of memory
                    throw new SvgMemoryException("Cannot process SVG file, cannot allocate the required memory", e);
                }

                using (var renderer = SvgRenderer.FromImage(bitmap))
                {
                    renderer.ScaleTransform(imageSize.Width / svgSize.Width, imageSize.Height / svgSize.Height);
                    var boundable = new GenericBoundable(0, 0, svgSize.Width, svgSize.Height);
                    this.Draw(renderer, boundable);
                }
            }
            catch
            {
                bitmap?.Dispose();
                throw;
            }

            return bitmap;
        }

        /// <summary>
        /// If both or one of raster height and width is not given (0), calculate that missing value from original SVG size
        /// while keeping original SVG size ratio
        /// </summary>
        /// <param name="size"></param>
        /// <param name="rasterWidth"></param>
        /// <param name="rasterHeight"></param>
        public virtual void RasterizeDimensions(ref SizeF size, int rasterWidth, int rasterHeight)
        {
            if (size.Width == 0)
                return;

            // Ratio of height/width of the original SVG size, to be used for scaling transformation
            float ratio = size.Height / size.Width;

            size.Width = rasterWidth > 0 ? rasterWidth : size.Width;
            size.Height = rasterHeight > 0 ? rasterHeight : size.Height;

            if (rasterHeight == 0 && rasterWidth > 0)
            {
                size.Height = (int)(rasterWidth * ratio);
            }
            else if (rasterHeight > 0 && rasterWidth == 0)
            {
                size.Width = (int)(rasterHeight / ratio);
            }
        }

        public override void Write(XmlWriter writer)
        {
            //Save previous culture and switch to invariant for writing
            var previousCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                base.Write(writer);
            }
            finally
            {
                // Make sure to set back the old culture even an error occurred.
                //Switch culture back
                Thread.CurrentThread.CurrentCulture = previousCulture;
            }
        }

        public void Write(Stream stream, bool useBom = true)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = useBom ? Encoding.UTF8 : new System.Text.UTF8Encoding(false),
                Indent = true
            };

            using var xmlWriter = XmlWriter.Create(stream, settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteDocType("svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", null);

            if (!String.IsNullOrEmpty(this.ExternalCSSHref))
                xmlWriter.WriteProcessingInstruction("xml-stylesheet", String.Format("type=\"text/css\" href=\"{0}\"", this.ExternalCSSHref));

            this.Write(xmlWriter);

            xmlWriter.Flush();
        }

        public void Write(string path, bool useBom = true)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                this.Write(fs, useBom);
            }
        }

        protected override void WriteAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("version", "1.1");
            foreach (var ns in SvgAttributeAttribute.Namespaces)
            {
                writer.WriteAttributeString("xmlns", ns.Key, null, ns.Value);
            }

            base.WriteAttributes(writer);
        }
    }
}
