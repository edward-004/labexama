using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using labexam.ut;

namespace labexam.ctrl
{
    internal abstract class Shape : Control
    {
        Color _col = Color.White;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color Color
        {
            get => _col;
            set
            {
                _col = value;
                Invalidate();
            }
        }

        protected Shape()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }
    }

    internal class Circle : Shape
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using SolidBrush brush = new(Color);
            e.Graphics.FillEllipse(brush, ClientRectangle);
        }
    }

    internal class Quad : Shape
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using SolidBrush brush = new(Color);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }

    internal class Triangle : Shape
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using SolidBrush brush = new(Color);

            Point[] VertexPoints =
            [
                new(ClientSize.Height / 2, 0),
                new(0, ClientSize.Height),
                new(ClientSize.Width, ClientSize.Height),
            ];

            e.Graphics.FillPolygon(brush, VertexPoints);
        }
    }
}
