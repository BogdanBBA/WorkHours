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

        protected Point downPoint = Point.Empty;

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

        public void RegisterControlsToMoveForm(params Control[] controls)
        {
            foreach (Control control in controls)
            {
                control.MouseDown += this.ForMoving_MouseDown;
                control.MouseMove += this.ForMoving_MouseMove;
                control.MouseUp += this.ForMoving_MouseUp;
            }
        }

        private void ForMoving_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            this.downPoint = new Point(e.X, e.Y);
        }

        private void ForMoving_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.downPoint == Point.Empty)
                return;
            this.Location = new Point(this.Left + e.X - this.downPoint.X, this.Top + e.Y - this.downPoint.Y);
        }

        private void ForMoving_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            this.downPoint = Point.Empty;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(MyGUIs.Background.Normal.Color);
            if (this.drawFormAccent)
                e.Graphics.DrawRectangle(formAccentPen, 3, 3, this.Width - 6, this.Height - 6);
        }
    }
}
