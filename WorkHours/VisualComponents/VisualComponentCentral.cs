using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace WorkHours.VisualComponents
{
    /// <summary>
    /// Base control from which to derive app-specific visual controls.
    /// </summary>
    public abstract class MyAppSpecificBaseControl : Control
    {
        protected bool mouseIsOver = false;
        protected bool mouseIsClicked = false;

        public MyAppSpecificBaseControl()
            : base()
        {
            this.BackColor = MyGUIs.Background.Normal.Color;
            this.DoubleBuffered = true;
        }

        protected bool isChecked;
        public bool Checked
        {
            get { return this.isChecked; }
            set { if (this.isChecked != value) { this.isChecked = value; this.Invalidate(); } }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.mouseIsOver = true;
            base.OnMouseEnter(e);
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.mouseIsOver = false;
            base.OnMouseLeave(e);
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            this.mouseIsClicked = true;
            base.OnMouseDown(mevent);
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            this.mouseIsClicked = false;
            base.OnMouseUp(mevent);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(MyGUIs.Background.GetValue(this.mouseIsOver).Color);
        }

        public static List<TYPE> CreateControlCollection<TYPE>(ICollection<string> captions, EventHandler clickEventHandler, string namePrefix, ICollection<Tuple<string, object>> customProperties)
            where TYPE : MyAppSpecificBaseControl
        {
            List<TYPE> result = new List<TYPE>();
            int lastNumber = 0;
            foreach (string caption in captions)
            {
                TYPE item = (TYPE) typeof(TYPE).GetConstructor(new Type[] { }).Invoke(new object[] { });
                item.Name = namePrefix + (++lastNumber);
                item.Text = caption;
                item.Click += clickEventHandler;
                if (customProperties != null)
                    foreach (Tuple<string, object> customProperty in customProperties)
                        typeof(TYPE).GetProperty(customProperty.Item1).SetValue(item, customProperty.Item2);
                result.Add(item);
            }
            return result;
        }
    }

    /// <summary>
    /// A color-brush-pen resource class.
    /// </summary>
    public class ColorResource
    {
        public Color Color { get; private set; }
        public Brush Brush { get; private set; }
        public Pen Pen { get; private set; }

        public ColorResource(Color color)
        {
            this.SetColorAndUpdateResource(color);
        }

        public void SetColorAndUpdateResource(Color color)
        {
            this.Color = color;
            this.Brush = new SolidBrush(color);
            this.Pen = new Pen(color);
        }
    }

    /// <summary>
    /// The constant (or at least static) values used for the GUIs.
    /// </summary>
    public static class MyGUIs
    {
        public static Pair<ColorResource> Background;
        public static Pair<ColorResource> Text;
        public static Pair<ColorResource> Accent;
        public static Pair<ColorResource> Category;

        static MyGUIs()
        {
            MyGUIs.Reset();
        }

        /// <summary>Initializes to default values.</summary>
        public static void Reset()
        {
            MyGUIs.Background = new Pair<ColorResource>(new ColorResource(ColorTranslator.FromHtml("#101010")), new ColorResource(ColorTranslator.FromHtml("#202020")));
            MyGUIs.Text = new Pair<ColorResource>(new ColorResource(Color.WhiteSmoke), new ColorResource(Color.OrangeRed));
            MyGUIs.Accent = new Pair<ColorResource>(new ColorResource(ColorTranslator.FromHtml("#FFFFFF")), new ColorResource(Color.OrangeRed));
            MyGUIs.Category = new Pair<ColorResource>(new ColorResource(ColorTranslator.FromHtml("#A0A0A0")), new ColorResource(ColorTranslator.FromHtml("#C0C0C0")));
        }
    }
}
