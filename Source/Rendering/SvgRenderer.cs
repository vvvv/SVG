using Svg.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace Svg
{
    /// <summary>
    /// Convenience wrapper around a graphics object
    /// </summary>
    public sealed class SvgRenderer : ISvgRenderer, IGraphicsProvider
    {
        private readonly Graphics _innerGraphics;
        private readonly bool _disposable;
        private readonly Image _image;
        private Bitmap _mask;

        private readonly Stack<ISvgBoundable> _boundables = new Stack<ISvgBoundable>();

        public void SetBoundable(ISvgBoundable boundable)
        {
            _boundables.Push(boundable);
        }
        public ISvgBoundable GetBoundable()
        {
            return _boundables.Count > 0 ? _boundables.Peek() : null;
        }
        public ISvgBoundable PopBoundable()
        {
            return _boundables.Pop();
        }

        public float DpiY
        {
            get { return _innerGraphics.DpiY; }
        }

        public SizeF RenderSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISvgRenderer"/> class.
        /// </summary>
        private SvgRenderer(Graphics graphics, SizeF renderSize, bool disposable = true)
        {
            _innerGraphics = graphics;
            _disposable = disposable;
            this.RenderSize = renderSize;
        }
        private SvgRenderer(Graphics graphics, Image image)
            : this(graphics, new SizeF(image.Width, image.Height))
        {
            _image = image;
        }

        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit graphicsUnit)
        {
            var bounds = destRect.Transform(this.Transform);
            DrawMasked(graphics => graphics.DrawImage(image, destRect, srcRect, graphicsUnit), bounds);
        }
        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit graphicsUnit, float opacity)
        {
            using (var attributes = new ImageAttributes())
            {
                var matrix = new ColorMatrix { Matrix33 = opacity };
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                var points = new[]
                {
                    destRect.Location,
                    new PointF(destRect.X + destRect.Width, destRect.Y),
                    new PointF(destRect.X, destRect.Y + destRect.Height)
                };
                _innerGraphics.DrawImage(image, points, srcRect, graphicsUnit, attributes);
            }
        }

        public void DrawImageUnscaled(Image image, Point location)
        {
            _innerGraphics.DrawImageUnscaled(image, location);
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            var bounds = path.GetBounds(this.Transform, pen);
            DrawMasked(graphics => graphics.DrawPath(pen, path), bounds);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            var bounds = path.GetBounds(this.Transform);
            DrawMasked(graphics => graphics.FillPath(brush, path), bounds);
        }
        public Region GetClip()
        {
            return _innerGraphics.Clip;
        }
        public void RotateTransform(float fAngle, MatrixOrder order = MatrixOrder.Append)
        {
            _innerGraphics.RotateTransform(fAngle, order);
        }
        public void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Append)
        {
            _innerGraphics.ScaleTransform(sx, sy, order);
        }
        public void SetClip(Region region, CombineMode combineMode = CombineMode.Replace)
        {
            _innerGraphics.SetClip(region, combineMode);
        }
        public void TranslateTransform(float dx, float dy, MatrixOrder order = MatrixOrder.Append)
        {
            _innerGraphics.TranslateTransform(dx, dy, order);
        }

        public SmoothingMode SmoothingMode
        {
            get { return _innerGraphics.SmoothingMode; }
            set { _innerGraphics.SmoothingMode = value; }
        }

        public Matrix Transform
        {
            get { return _innerGraphics.Transform; }
            set { _innerGraphics.Transform = value; }
        }

        public void Dispose()
        {
            if (_disposable)
                _innerGraphics.Dispose();
            if (_image != null)
                _image.Dispose();
        }

        Graphics IGraphicsProvider.GetGraphics()
        {
            return _innerGraphics;
        }

        private static Graphics CreateGraphics(Image image)
        {
            var g = Graphics.FromImage(image);
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.TextContrast = 1;
            return g;
        }

        /// <summary>
        /// Creates a new <see cref="ISvgRenderer"/> from the specified <see cref="Image"/>.
        /// </summary>
        /// <param name="image"><see cref="Image"/> from which to create the new <see cref="ISvgRenderer"/>.</param>
        public static ISvgRenderer FromImage(Image image)
        {
            var g = CreateGraphics(image);
            return new SvgRenderer(g, new Size(image.Width, image.Height));
        }

        /// <summary>
        /// Creates a new <see cref="ISvgRenderer"/> from the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="Graphics"/> to create the renderer from.</param>
        /// <param name="renderSize">The size of the rendered image.</param>
        public static ISvgRenderer FromGraphics(Graphics graphics, SizeF renderSize)
        {
            return new SvgRenderer(graphics, renderSize, false);
        }

        public static ISvgRenderer FromNull()
        {
            var img = new Bitmap(1, 1);
            var g = CreateGraphics(img);
            return new SvgRenderer(g, img);
        }

        public void SetMask(Bitmap mask)
        {
            _mask = mask;
        }

        public Bitmap GetMask()
        {
            return _mask;
        }

        public void DisposeMask()
        {
            if (_mask != null)
            {
                _mask.Dispose();
            }
        }

        private void DrawMasked(Action<Graphics> drawAction, RectangleF bounds)
        {
            if (_mask == null)
            {
                drawAction(_innerGraphics);
                return;
            }

            var fullBounds = new RectangleF(new PointF(), this.RenderSize);
            var renderedBounds = Rectangle.Round(RectangleF.Intersect(bounds, fullBounds));

            if (renderedBounds.Width == 0 || renderedBounds.Height == 0)
            {
                return;
            }

            using (var buffer = new Bitmap(renderedBounds.Width, renderedBounds.Height, PixelFormat.Format32bppArgb))
            {
                var localTransform = new Matrix();
                localTransform.Translate(-renderedBounds.X, -renderedBounds.Y);
                localTransform.Multiply(this.Transform);

                var bufferGraphics = Graphics.FromImage(buffer);

                bufferGraphics.Transform = localTransform;
                drawAction(bufferGraphics);

                ApplyAlphaMask(buffer, _mask, renderedBounds);

                var previousTransform = _innerGraphics.Transform;
                _innerGraphics.Transform = new Matrix();
                _innerGraphics.DrawImageUnscaled(buffer, new Point(renderedBounds.X, renderedBounds.Y));
                _innerGraphics.Transform = previousTransform;
            }
        }

        private void ApplyAlphaMask(Bitmap buffer, Bitmap mask, Rectangle renderedBounds)
        {
            var bufferData = buffer.LockBits(new Rectangle(0, 0, buffer.Width, buffer.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var maskData = mask.LockBits(new Rectangle(0, 0, mask.Width, mask.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var bufferBytes = new byte[buffer.Width * buffer.Height * 4];
            var maskBytes = new byte[mask.Width * mask.Height * 4];

            Marshal.Copy(bufferData.Scan0, bufferBytes, 0, bufferBytes.Length);
            Marshal.Copy(maskData.Scan0, maskBytes, 0, maskBytes.Length);

            for (int y = 0; y < renderedBounds.Height; y++)
            {
                for (int x = 0; x < renderedBounds.Width; x++)
                {
                    var bufferPixelAddress = (y * buffer.Width + x) * 4;
                    var maskPixelAddress = ((y + renderedBounds.Y) * mask.Width + (x + renderedBounds.X)) * 4;

                    var alpha = (maskBytes[maskPixelAddress] + maskBytes[maskPixelAddress + 1] + maskBytes[maskPixelAddress + 2]) / 3;
                    var newAlpha = (byte)(bufferBytes[bufferPixelAddress + 3] * alpha / 255);

                    bufferBytes[bufferPixelAddress + 3] = newAlpha;
                }
            }

            Marshal.Copy(bufferBytes, 0, bufferData.Scan0, bufferBytes.Length);

            buffer.UnlockBits(bufferData);
            mask.UnlockBits(maskData);
        }
    }
}
