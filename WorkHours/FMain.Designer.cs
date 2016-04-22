namespace WorkHours
{
    partial class FMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuP = new WorkHours.VisualComponents.MyPanel();
            this.exitB = new WorkHours.VisualComponents.MyButton();
            this.workDaysSV = new WorkHours.VisualComponents.MySelectView();
            this.menuP.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuP
            // 
            this.menuP.Controls.Add(this.exitB);
            this.menuP.DrawPanelAccent = false;
            this.menuP.Location = new System.Drawing.Point(139, 355);
            this.menuP.Name = "menuP";
            this.menuP.Size = new System.Drawing.Size(386, 132);
            this.menuP.TabIndex = 2;
            // 
            // exitB
            // 
            this.exitB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.exitB.BigBar = false;
            this.exitB.Checked = false;
            this.exitB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.exitB.DrawBar = true;
            this.exitB.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitB.Image = null;
            this.exitB.Location = new System.Drawing.Point(143, 40);
            this.exitB.Name = "exitB";
            this.exitB.Size = new System.Drawing.Size(177, 66);
            this.exitB.TabIndex = 1;
            this.exitB.Text = "EXIT";
            this.exitB.Click += new System.EventHandler(this.exitB_Click);
            // 
            // workDaysSV
            // 
            this.workDaysSV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.workDaysSV.Checked = false;
            this.workDaysSV.Database = null;
            this.workDaysSV.Location = new System.Drawing.Point(72, 173);
            this.workDaysSV.Name = "workDaysSV";
            this.workDaysSV.Size = new System.Drawing.Size(567, 216);
            this.workDaysSV.TabIndex = 1;
            this.workDaysSV.Text = "mySelectView1";
            // 
            // FMain
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.ClientSize = new System.Drawing.Size(815, 518);
            this.Controls.Add(this.menuP);
            this.Controls.Add(this.workDaysSV);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FMain_Load);
            this.Resize += new System.EventHandler(this.FMain_Resize);
            this.menuP.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private VisualComponents.MySelectView workDaysSV;
        private VisualComponents.MyPanel menuP;
        private VisualComponents.MyButton exitB;
    }
}