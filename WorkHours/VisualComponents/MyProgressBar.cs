using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkHours.VisualComponents
{
    /// <summary>
    /// Custom-configured BBA progress bar.
    /// </summary>
    public class MyProgressBar : ProgressBar
    {
        public MyProgressBar()
            : base()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(MyGUIs.Background.Normal.Color);
            e.Graphics.DrawRectangle(MyGUIs.Accent.Highlighted.Pen, 0, 0, this.Width - 1, this.Height - 1);
            e.Graphics.FillRectangle(MyGUIs.Accent.Normal.Brush, 2, 2, this.Width - 4, this.Height - 4);
        }
    }
}
