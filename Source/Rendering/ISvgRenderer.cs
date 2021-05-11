using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    public interface ISvgRenderer : IDisposable
    {
        float DpiY { get; }
        void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit graphicsUnit);
        void DrawImageUnscaled(Image image, Point location);
        void DrawPath(Pen pen, GraphicsPath path);
        void FillPath(Brush brush, GraphicsPath path);
        ISvgBoundable GetBoundable();
        Region GetClip();
        Bitmap GetMask();
        ISvgBoundable PopBoundable();
        void RotateTransform(float fAngle, MatrixOrder order = MatrixOrder.Append);
        void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Append);
        void SetBoundable(ISvgBoundable boundable);
        void SetClip(Region region, CombineMode combineMode = CombineMode.Replace);
        void SetMask(Bitmap mask);
        void DisposeMask();
        SmoothingMode SmoothingMode { get; set; }
        Matrix Transform { get; set; }
        SizeF RenderSize { get; }
        void TranslateTransform(float dx, float dy, MatrixOrder order = MatrixOrder.Append);
        void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit graphicsUnit, float opacity);
    }
}
