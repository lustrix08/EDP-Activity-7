using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using PetCareSystem.Theme;
using PetCareSystem.UI;

namespace PetCareSystem
{
    public partial class DashboardForm : Form
    {
        private List<NavButton> navButtons = new List<NavButton>();

        public DashboardForm()
        {
            ThemeManager.ApplyFormTheme(this);
            this.Text = "PetCare - Clinic Overview";
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;


            TableLayoutPanel tlpSidebar = new TableLayoutPanel { Dock = DockStyle.Left, Width = 320, BackColor = Color.White, RowCount = 3, ColumnCount = 1 };
            tlpSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpSidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpSidebar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(ThemeManager.Border), tlpSidebar.Width - 1, 0, tlpSidebar.Width - 1, tlpSidebar.Height);

            Label lblBrand = new Label { Text = "AdminPanel", Font = new Font("Georgia", 25, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Margin = new Padding(20, 30, 0, 30), AutoSize = true };
            tlpSidebar.Controls.Add(lblBrand, 0, 0);

            FlowLayoutPanel pnlNav = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(15, 0, 15, 0) };

            NavButton btnDash = CreateNavButton("Dashboard");
            NavButton btnAccounts = CreateNavButton("Account Management");
            NavButton btnRegistry = CreateNavButton("Clinic Registry");
            NavButton btnReports = CreateNavButton("Report Generator");
            NavButton btnAbout = CreateNavButton("About Program");

            pnlNav.Controls.AddRange(new Control[] { btnDash, btnAccounts, btnRegistry, btnReports, btnAbout });
            tlpSidebar.Controls.Add(pnlNav, 0, 1);

            SignOutButton btnSignOut = new SignOutButton { Margin = new Padding(20, 0, 0, 40) };
            btnSignOut.Click += (s, e) => Application.Restart();
            tlpSidebar.Controls.Add(btnSignOut, 0, 2);
            this.Controls.Add(tlpSidebar);

            SetActiveNav(btnDash);


            TableLayoutPanel tlpMain = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(35, 20, 35, 30), ColumnCount = 1, RowCount = 3 };
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.Controls.Add(tlpMain);
            tlpMain.BringToFront();


            RoundedPanel pnlBanner = new RoundedPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 15), Radius = 20, BackColor = ColorTranslator.FromHtml("#4A3728") };
            pnlBanner.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(pnlBanner.ClientRectangle, ColorTranslator.FromHtml("#4A3728"), ColorTranslator.FromHtml("#7C5C46"), 45F)) {
                    e.Graphics.FillPath(brush, RoundedPanel.GetRoundedPath(pnlBanner.ClientRectangle, 20));
                }
            };
            Label lblWelcome = new Label { Text = "Welcome back, Admin!", Font = new Font("Georgia", 24, FontStyle.Bold), ForeColor = Color.White, Location = new Point(30, 30), AutoSize = true, BackColor = Color.Transparent };
            Label lblSubBanner = new Label { Text = "Your clinic is running smoothly. 3 appointments scheduled for today.", Font = ThemeManager.BodyFont, ForeColor = Color.WhiteSmoke, Location = new Point(35, 75), AutoSize = true, BackColor = Color.Transparent };
            pnlBanner.Controls.Add(lblWelcome); pnlBanner.Controls.Add(lblSubBanner);
            tlpMain.Controls.Add(pnlBanner, 0, 0);


            TableLayoutPanel tlpStats = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, Margin = new Padding(0, 0, 0, 15) };
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            tlpStats.Controls.Add(new StatCard("CUSTOMERS", GetCount("customers"), "👥"), 0, 0);
            tlpStats.Controls.Add(new StatCard("PATIENT RECORDS", GetCount("pets"), "🐾"), 1, 0);
            tlpStats.Controls.Add(new StatCard("STAFF VETS", GetCount("users"), "🩺"), 2, 0);
            tlpStats.Controls.Add(new StatCard("APPOINTMENTS", GetCount("appointments"), "📅"), 3, 0);
            tlpMain.Controls.Add(tlpStats, 0, 1);


            TableLayoutPanel tlpData = new TableLayoutPanel { Dock = DockStyle.Fill, Margin = new Padding(0), ColumnCount = 2, RowCount = 1 };
            tlpData.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlpData.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));


            RoundedPanel pnlChart = new RoundedPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 20, 0) };
            pnlChart.Controls.Add(new Label { Text = "Revenue Overview (Monthly)", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Location = new Point(30, 25), AutoSize = true });
            

            Panel chartArea = new Panel { Location = new Point(30, 80), Size = new Size(450, 250), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            chartArea.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Pen axisPen = new Pen(ThemeManager.Border, 2);
                Pen linePen = new Pen(ThemeManager.Accent, 3);
                

                for(int i=0; i<5; i++) {
                    int y = chartArea.Height - (i * 50);
                    e.Graphics.DrawLine(new Pen(Color.WhiteSmoke, 1), 0, y, chartArea.Width, y);
                }


                Point[] points = { new Point(0, 200), new Point(80, 150), new Point(160, 180), new Point(240, 100), new Point(320, 120), new Point(400, 50) };
                e.Graphics.DrawLines(linePen, points);
                foreach(var p in points) e.Graphics.FillEllipse(new SolidBrush(ThemeManager.Accent), p.X - 4, p.Y - 4, 8, 8);
            };
            pnlChart.Controls.Add(chartArea);
            tlpData.Controls.Add(pnlChart, 0, 0);

            RoundedPanel pnlRecentAppts = new RoundedPanel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            pnlRecentAppts.Controls.Add(new Label { Text = "Recent Appointments", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Location = new Point(30, 25), AutoSize = true });
            
            ModernDataGridView dgvAppts = new ModernDataGridView { Location = new Point(30, 70), Size = new Size(480, 250), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("Time"), new DataColumn("Patient"), new DataColumn("Doctor") });
            dt.Rows.Add("10:00 AM", "Bella (Dog)", "Dr. Smith");
            dt.Rows.Add("11:30 AM", "Luna (Cat)", "Dr. Reyes");
            dt.Rows.Add("02:00 PM", "Cooper (Dog)", "Dr. Garcia");
            dgvAppts.DataSource = dt;
            pnlRecentAppts.Controls.Add(dgvAppts);
            tlpData.Controls.Add(pnlRecentAppts, 1, 0);

            tlpMain.Controls.Add(tlpData, 0, 2);
        }

        private string GetCount(string tableName)
        {
            try
            {
                using (MySqlConnection? conn = DbConnector.GetConnection())
                {
                    if (conn == null) return "0";
                    string query = $"SELECT COUNT(*) FROM {tableName}";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object? result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result).ToString("N0") : "0";
                }
            }
            catch { return "0"; }
        }

        private NavButton CreateNavButton(string text)
        {
            NavButton btn = new NavButton { Text = text };
            btn.Click += (s, e) => ThemeManager.NavigateTo(this, text);
            navButtons.Add(btn);
            return btn;
        }

        private void SetActiveNav(NavButton activeBtn)
        {
            foreach (var btn in navButtons) btn.IsActive = false;
            activeBtn.IsActive = true;
        }

        private void DrawWeeklyBarChart(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            Graphics g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            int m = 50, w = pnl.Width - (m * 2), y = pnl.Height - m;
            int[] d = { 40, 70, 55, 90, 80, 110, 35 };
            int bw = 25, bg = (w - (7 * bw)) / 6;

            for (int i = 0; i < 7; i++)
            {
                int rx = m + i * (bw + bg);
                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(rx, y - d[i], bw, d[i]), ThemeManager.Accent, ThemeManager.AccentHover, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, rx, y - d[i], bw, d[i]);
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { Application.Exit(); }
    }


    public class AppointmentModal : Form { public AppointmentModal(string t) { this.Text = t; } }
    public class DeleteModal : Form { public DeleteModal() { this.Text = "Delete Confirmation"; } }
}