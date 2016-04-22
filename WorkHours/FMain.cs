using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkHours.VisualComponents;

namespace WorkHours
{
    public partial class FMain : Form
    {
        public Database Database { get; private set; }

        public FMain()
        {
            InitializeComponent();
            this.BackColor = MyGUIs.Background.Normal.Color;
            this.Database = new Database();
        }

        private void FMain_Resize(object sender, EventArgs e)
        {
            menuP.SetBounds(12, this.Height - 60 - 12, this.Width - 24, 60);
            exitB.SetBounds(menuP.Width - 200, 0, 200, menuP.Height);

            workDaysSV.SetBounds(12, menuP.Top - 60 - 12, menuP.Width, 60);
        }

        private void FMain_Load(object sender, EventArgs e)
        {
            string checkResult = Paths.CheckPaths(true);
            if (!checkResult.Equals(string.Empty))
            {
                MessageBox.Show(checkResult, "File check ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            checkResult = this.Database.LoadDatabase(Paths.DatabaseFile);
            if (!checkResult.Equals(string.Empty))
            {
                MessageBox.Show(checkResult, "Database load ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            this.workDaysSV.Database = this.Database;
        }

        private void exitB_Click(object sender, EventArgs e)
        {
            string saveResult = this.Database.SaveDatabase(Paths.DatabaseFile);
            if (!saveResult.Equals(string.Empty))
            {
                MessageBox.Show(saveResult, "File check ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            Application.Exit();
        }
    }
}
