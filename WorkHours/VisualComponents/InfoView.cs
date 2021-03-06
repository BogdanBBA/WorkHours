﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkHours.VisualComponents
{
    public class InfoView : MyAppSpecificBaseControl
    {
        public static readonly Pair<int> BarHeight = new Pair<int>(2, 4);
        public const int InfoViewHeight = 52;

        public InfoView()
            : base()
        {
            this.description = new Tuple<Font, Brush, string>(new Font("Segoe UI", 13, FontStyle.Regular), MyGUIs.Text.Normal.Brush, "#description");
            this.text = new Tuple<Font, Brush, string>(new Font("Segoe UI", 22, FontStyle.Bold), MyGUIs.Text.Highlighted.Brush, "#text");
            this.Size = new Size(250, InfoView.InfoViewHeight);
        }

        private bool drawBar = true;
        public bool DrawBar
        {
            get { return this.drawBar; }
            set { this.drawBar = value; this.Invalidate(); }
        }

        private bool bigBar = false;
        public bool BigBar
        {
            get { return this.bigBar; }
            set { this.bigBar = value; this.Invalidate(); }
        }

        private Tuple<Font, Brush, string> description;
        private Tuple<Font, Brush, string> text;

        public string TextDescription
        {
            get { return this.description.Item3; }
            set { this.description = new Tuple<Font, Brush, string>(this.description.Item1, this.description.Item2, value); this.Invalidate(); }
        }

        public string TextText
        {
            get { return this.text.Item3; }
            set { this.text = new Tuple<Font, Brush, string>(this.text.Item1, this.text.Item2, value); this.Invalidate(); }
        }

        private HorizontalAlignment textAlign;
        public HorizontalAlignment TextAlign
        {
            get { return this.textAlign; }
            set { this.textAlign = value; this.Invalidate(); }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.Clear(MyGUIs.Background.Normal.Color);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            SizeF size = e.Graphics.MeasureString(this.description.Item3, this.description.Item1);
            PointF location = new PointF(this.textAlign == HorizontalAlignment.Left ? 0 : (this.textAlign == HorizontalAlignment.Center ? this.Width / 2 - size.Width / 2 : this.Width - size.Width), 0);
            e.Graphics.DrawString(this.description.Item3, this.description.Item1, this.description.Item2, location);

            float lastBottom = location.Y + size.Height;
            size = e.Graphics.MeasureString(this.text.Item3, this.text.Item1);
            location = new PointF(this.textAlign == HorizontalAlignment.Left ? -2 : (this.textAlign == HorizontalAlignment.Center ? this.Width / 2 - size.Width / 2 : this.Width - size.Width + 2), lastBottom - 8);
            e.Graphics.DrawString(this.text.Item3, this.text.Item1, this.text.Item2, location);

            if (this.drawBar)
                e.Graphics.FillRectangle(MyGUIs.Accent.Normal.Brush, 1, this.Height - BarHeight.GetValue(this.bigBar), this.Width - 2, BarHeight.GetValue(this.bigBar));
        }
    }
}
