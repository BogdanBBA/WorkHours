using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkHours.VisualComponents
{
    public partial class MyForm : Form
    {
        protected static readonly Pen formAccentPen = new Pen(MyGUIs.Accent.Normal.Color, 2);

        public MyForm()
            : base()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ControlBox = false;
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.ForeColor = MyGUIs.Text.Normal.Color;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            this.drawFormAccent = true;
        }

        private bool drawFormAccent;
        public bool DrawFormAccent
        {
            get { return this.drawFormAccent; }
            set { this.drawFormAccent = value; this.Invalidate(); }
        }

        public virtual void RefreshInformation(object item)
        { }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(MyGUIs.Background.Normal.Color);
            if (this.drawFormAccent)
                e.Graphics.DrawRectangle(formAccentPen, 3, 3, this.Width - 6, this.Height - 6);
        }
    }
}
