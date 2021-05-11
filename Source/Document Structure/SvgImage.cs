using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Text;

namespace Svg
{
    /// <summary>
    /// Represents and SVG image
    /// </summary>
    [SvgElement("image")]
    public partial class SvgImage : SvgVisualElement
    {
        private const string MimeTypeSvg = "image/svg+xml";

        private GraphicsPath _path;
        private bool _gettingBounds;

        /// <summary>
        /// Gets an <see cref="SvgPoint"/> representing the top left point of the rectangle.
        /// </summary>
        public SvgPoint Location
        {
            get { return new SvgPoint(X, Y); }
        }

        /// <summary>
        /// Gets or sets the aspect of the viewport.
        /// </summary>
        /// <value></value>
        [SvgAttribute("preserveAspectRatio")]
        public SvgAspectRatio AspectRatio
        {
            get { return GetAttribute("preserveAspectRatio", false, new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid)); }
            set { Attributes["preserveAspectRatio"] = value; }
        }

        [SvgAttribute("x")]
        public virtual SvgUnit X
        {
            get { return GetAttribute<SvgUnit>("x", false); }
            set { Attributes["x"] = value; }
        }

        [SvgAttribute("y")]
        public virtual SvgUnit Y
        {
            get { return GetAttribute<SvgUnit>("y", false); }
            set { Attributes["y"] = value; }
        }

        [SvgAttribute("width")]
        public virtual SvgUnit Width
        {
            get { return GetAttribute<SvgUnit>("width", false); }
            set { Attributes["width"] = value; }
        }

        [SvgAttribute("height")]
        public virtual SvgUnit Height
        {
            get { return GetAttribute<SvgUnit>("height", false); }
            set { Attributes["height"] = value; }
        }

        [SvgAttribute("href", SvgAttributeAttribute.XLinkNamespace)]
        public virtual string Href
        {
            get { return GetAttribute<string>("href", false); }
            set { Attributes["href"] = value; }
        }

        /// <summary>
        /// Gets the bounds of the element.
        /// </summary>
        /// <value>The bounds.</value>
        public override RectangleF Bounds
        {
            get
            {
                if (_gettingBounds)
                {
                    // we can get here recursively in case of percent size units while calculating
                    // the size of the parent object - in this case just return empty bounds to avoid
                    // a recursion (see issue #436)
                    return new RectangleF();
                }
                _gettingBounds = true;
                var bounds = TransformedBounds(new RectangleF(Location.ToDeviceValue(null, this),
                                               new SizeF(Width.ToDeviceValue(null, UnitRenderingType.Horizontal, this),
                                                         Height.ToDeviceValue(null, UnitRenderingType.Vertical, this))));
                _gettingBounds = false;
                return bounds;
            }
        }

        /// <summary>
        /// Gets the <see cref="GraphicsPath"/> for this element.
        /// </summary>
        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            if (_path == null)
            {
                // Same size of rectangle can suffice to provide bounds of the image
                var rectangle = new RectangleF(Location.ToDeviceValue(renderer, this), SvgUnit.GetDeviceSize(Width, Height, renderer, this));

                _path = new GraphicsPath();
                _path.StartFigure();
                _path.AddRectangle(rectangle);
                _path.CloseFigure();
            }

            return _path;
        }

        /// <summary>
        /// Renders the <see cref="SvgElement"/> and contents to the specified <see cref="Graphics"/> object.
        /// </summary>
        protected override void Render(ISvgRenderer renderer)
        {
            if (!(Visible && Displayable && Width.Value > 0f && Height.Value > 0f && Href != null))
                return;

            var img = GetImage(Href);
            var bmp = img as Image;
            var svg = img as SvgFragment;
            if (bmp == null && svg == null)
                return;
            try
            {
                if (PushTransforms(renderer))
                {
                    RectangleF srcRect;
                    if (bmp != null)
                        srcRect = new RectangleF(0f, 0f, bmp.Width, bmp.Height);
                    else
                        srcRect = new RectangleF(new PointF(0f, 0f), svg.GetDimensions());

                    var destClip = new RectangleF(Location.ToDeviceValue(renderer, this),
                                                  new SizeF(Width.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this),
                                                            Height.ToDeviceValue(renderer, UnitRenderingType.Vertical, this)));
                    var destRect = destClip;
                    renderer.SetClip(new Region(destClip), CombineMode.Intersect);
                    SetClip(renderer);
                    SetMask(renderer);

                    var aspectRatio = AspectRatio;
                    if (aspectRatio.Align != SvgPreserveAspectRatio.none)
                    {
                        var fScaleX = destClip.Width / srcRect.Width;
                        var fScaleY = destClip.Height / srcRect.Height;
                        var xOffset = 0f;
                        var yOffset = 0f;

                        if (aspectRatio.Slice)
                        {
                            fScaleX = Math.Max(fScaleX, fScaleY);
                            fScaleY = Math.Max(fScaleX, fScaleY);
                        }
                        else
                        {
                            fScaleX = Math.Min(fScaleX, fScaleY);
                            fScaleY = Math.Min(fScaleX, fScaleY);
                        }

                        switch (aspectRatio.Align)
                        {
                            case SvgPreserveAspectRatio.xMinYMin:
                                break;
                            case SvgPreserveAspectRatio.xMidYMin:
                                xOffset = (destClip.Width - srcRect.Width * fScaleX) / 2;
                                break;
                            case SvgPreserveAspectRatio.xMaxYMin:
                                xOffset = (destClip.Width - srcRect.Width * fScaleX);
                                break;
                            case SvgPreserveAspectRatio.xMinYMid:
                                yOffset = (destClip.Height - srcRect.Height * fScaleY) / 2;
                                break;
                            case SvgPreserveAspectRatio.xMidYMid:
                                xOffset = (destClip.Width - srcRect.Width * fScaleX) / 2;
                                yOffset = (destClip.Height - srcRect.Height * fScaleY) / 2;
                                break;
                            case SvgPreserveAspectRatio.xMaxYMid:
                                xOffset = (destClip.Width - srcRect.Width * fScaleX);
                                yOffset = (destClip.Height - srcRect.Height * fScaleY) / 2;
                                break;
                            case SvgPreserveAspectRatio.xMinYMax:
                                yOffset = (destClip.Height - srcRect.Height * fScaleY);
                                break;
                            case SvgPreserveAspectRatio.xMidYMax:
                                xOffset = (destClip.Width - srcRect.Width * fScaleX) / 2;
                                yOffset = (destClip.Height - srcRect.Height * fScaleY);
                                break;
                            case SvgPreserveAspectRatio.xMaxYMax:
                                xOffset = (destClip.Width - srcRect.Width * fScaleX);
                                yOffset = (destClip.Height - srcRect.Height * fScaleY);
                                break;
                        }

                        destRect = new RectangleF(destClip.X + xOffset, destClip.Y + yOffset,
                                                  srcRect.Width * fScaleX, srcRect.Height * fScaleY);
                    }

                    if (bmp != null)
                    {
                        var opacity = FixOpacityValue(Opacity);
                        if (opacity == 1f)
                            renderer.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
                        else
                            renderer.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel, opacity);
                    }
                    else
                    {
                        renderer.TranslateTransform(destRect.X, destRect.Y, MatrixOrder.Prepend);
                        renderer.ScaleTransform(destRect.Width / srcRect.Width, destRect.Height / srcRect.Height, MatrixOrder.Prepend);
                        try
                        {
                            renderer.SetBoundable(new GenericBoundable(srcRect));
                            svg.RenderElement(renderer);
                        }
                        finally
                        {
                            renderer.PopBoundable();
                        }
                    }

                    ResetMask(renderer);
                    ResetClip(renderer);
                }
            }
            finally
            {
                PopTransforms(renderer);

                if (bmp != null)
                    bmp.Dispose();
            }
            // TODO: cache images... will need a shared context for this
        }

        public object GetImage()
        {
            return GetImage(Href);
        }

        public object GetImage(string uriString)
        {
            // Uri MaxLength is 65519 (https://msdn.microsoft.com/en-us/library/z6c2z492.aspx)
            // if using data URI scheme, very long URI may happen.
            var safeUriString = uriString.Length > 65519 ? uriString.Substring(0, 65519) : uriString;

            try
            {
                var uri = new Uri(safeUriString, UriKind.RelativeOrAbsolute);

                // handle data/uri embedded images (http://en.wikipedia.org/wiki/Data_URI_scheme)
                if (uri.IsAbsoluteUri && uri.Scheme == "data")
                    return GetImageFromDataUri(uriString);

                if (!uri.IsAbsoluteUri)
                    uri = new Uri(OwnerDocument.BaseUri, uri);

                // should work with http: and file: protocol urls
                var httpRequest = WebRequest.Create(uri);

                using (var webResponse = httpRequest.GetResponse())
                {
                    using (var stream = webResponse.GetResponseStream())
                    {
                        if (stream.CanSeek)
                            stream.Position = 0;

                        if (webResponse.ContentType.StartsWith(MimeTypeSvg, StringComparison.InvariantCultureIgnoreCase) ||
                            uri.LocalPath.EndsWith(".svg", StringComparison.InvariantCultureIgnoreCase))
                            return LoadSvg(stream, uri);
                        else
                            return Image.FromStream(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error loading image: '{0}', error: {1} ", uriString, ex.Message);
                return null;
            }
        }

        private object GetImageFromDataUri(string uriString)
        {
            var headerStartIndex = 5;
            var headerEndIndex = uriString.IndexOf(",", headerStartIndex);
            if (headerEndIndex < 0 || headerEndIndex + 1 >= uriString.Length)
                throw new Exception("Invalid data URI");

            var mimeType = "text/plain";
            var charset = "US-ASCII";
            var base64 = false;

            var headers = new List<string>(uriString.Substring(headerStartIndex, headerEndIndex - headerStartIndex).Split(';'));
            if (headers[0].Contains("/"))
            {
                mimeType = headers[0].Trim();
                headers.RemoveAt(0);
                charset = string.Empty;
            }

            if (headers.Count > 0 && headers[headers.Count - 1].Trim().Equals("base64", StringComparison.InvariantCultureIgnoreCase))
            {
                base64 = true;
                headers.RemoveAt(headers.Count - 1);
            }

            foreach (var param in headers)
            {
                var p = param.Split('=');
                if (p.Length < 2)
                    continue;

                var attribute = p[0].Trim();
                if (attribute.Equals("charset", StringComparison.InvariantCultureIgnoreCase))
                    charset = p[1].Trim();
            }

            var data = uriString.Substring(headerEndIndex + 1);
            if (mimeType.Equals(MimeTypeSvg, StringComparison.InvariantCultureIgnoreCase))
            {
                if (base64)
                {
                    var encoding = string.IsNullOrEmpty(charset) ? Encoding.UTF8 : Encoding.GetEncoding(charset);
                    data = encoding.GetString(Convert.FromBase64String(data));
                }
                using (var stream = new MemoryStream(Encoding.Default.GetBytes(data)))
                {
                    return LoadSvg(stream, OwnerDocument.BaseUri);
                }
            }
            // support nonstandard "img" spelling of mimetype
            else if (mimeType.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase) || mimeType.StartsWith("img/", StringComparison.InvariantCultureIgnoreCase))
            {
                var dataBytes = base64 ? Convert.FromBase64String(data) : Encoding.Default.GetBytes(data);
                using (var stream = new MemoryStream(dataBytes))
                {
                    return Image.FromStream(stream);
                }
            }
            else
                return null;
        }

        private SvgDocument LoadSvg(Stream stream, Uri baseUri)
        {
            var document = SvgDocument.Open<SvgDocument>(stream);
            document.BaseUri = baseUri;
            return document;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgImage>();
        }
    }
}
