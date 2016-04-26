using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkHours.VisualComponents
{
    public class MySelectView : MyAppSpecificBaseControl
    {
        private Database database = null;
        public Database Database
        {
            get { return this.database; }
            set { this.database = value; this.OnResize(null); }
        }

        private float cellWidth;
        private int startX, endX;

        private List<WorkDay> selectedDays;

        public MySelectView()
            : base()
        {
            this.selectedDays = new List<WorkDay>();
            this.Size = new Size(400, 150);
        }

        protected override void OnResize(EventArgs e)
        {
            this.cellWidth = this.database != null ? (float) this.Width / this.database.WorkDays.Count : 0;
            base.OnResize(e);
        }

        private RectangleF GetBoundsOfCell(int cellIndex)
        {
            return new RectangleF(cellIndex * cellWidth + cellWidth / 4, 0, cellWidth / 2, this.Height);
        }

        private int GetCellIndexOfXCoordinate(int x)
        {
            return (int) (x / this.cellWidth);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            this.selectedDays.Clear();
            this.Invalidate();
            this.startX = mevent.X;
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.mouseIsClicked && this.database != null)
            {
                this.endX = e.X;
                int indexA = this.GetCellIndexOfXCoordinate(this.startX), indexB = this.GetCellIndexOfXCoordinate(this.endX);
                if (indexA > indexB)
                {
                    int aux = indexA;
                    indexA = indexB;
                    indexB = aux;
                }
                this.selectedDays.Clear();
                for (int index = indexA; index <= indexB; index++)
                    this.selectedDays.Add(this.database.WorkDays[index]);
            }
            base.OnMouseMove(e);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.mouseIsClicked)
                e.Graphics.FillRectangle(Brushes.Pink, this.startX, 0, this.endX - this.startX, this.Height);

            if (this.database == null)
                e.Graphics.DrawString("database is null", SystemFonts.MessageBoxFont, Brushes.WhiteSmoke, Point.Empty);
            else
                for (int index = 0; index < this.database.WorkDays.Count; index++)
                {
                    RectangleF cell = this.GetBoundsOfCell(index);
                    e.Graphics.FillRectangle(
                        this.selectedDays.Contains(this.database.WorkDays[index]) ? Brushes.Fuchsia : Brushes.WhiteSmoke,
                        cell.Left, cell.Top, cell.Width, cell.Height);
                }

            e.Graphics.DrawRectangle(this.mouseIsOver ? MyGUIs.Accent[this.mouseIsClicked].Pen : MyGUIs.Background.Normal.Pen, 0, 0, this.Width - 1, this.Height - 1);
        }
    }
}
