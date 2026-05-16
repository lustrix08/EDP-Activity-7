using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using PetCareSystem.Theme;
using PetCareSystem.UI;

namespace PetCareSystem
{
    public partial class ReportsForm : Form
    {
        private List<NavButton> navButtons = new List<NavButton>();
        private TableLayoutPanel tlpMain = new TableLayoutPanel();
        private Panel pnlReportView = new Panel();
        private ModernDataGridView dgvReport = new ModernDataGridView();
        private Label lblReportTitle = new Label();
        private string currentReportName = "";

        public ReportsForm()
        {
            ThemeManager.ApplyFormTheme(this);
            this.Text = "PetCare - Report Generator";
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;


            TableLayoutPanel tlpSidebar = new TableLayoutPanel { Dock = DockStyle.Left, Width = 320, BackColor = Color.White, RowCount = 3, ColumnCount = 1 };
            tlpSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize)); tlpSidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); tlpSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpSidebar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(ThemeManager.Border), tlpSidebar.Width - 1, 0, tlpSidebar.Width - 1, tlpSidebar.Height);
            tlpSidebar.Controls.Add(new Label { Text = "AdminPanel", Font = new Font("Georgia", 25, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Margin = new Padding(25, 40, 0, 40), AutoSize = true }, 0, 0);

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
            SetActiveNav(btnReports);


            tlpMain = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(50, 30, 50, 50), ColumnCount = 1, RowCount = 2 };
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F)); tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.Controls.Add(tlpMain); tlpMain.BringToFront();

            ShowReportSelection();
        }

        private void ShowReportSelection()
        {
            tlpMain.Controls.Clear();
            
            Panel pnlHeader = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            pnlHeader.Controls.Add(new Label { Text = "Report Generator", Font = ThemeManager.HeaderFont, ForeColor = ThemeManager.TextDark, Location = new Point(0, 0), AutoSize = true });
            pnlHeader.Controls.Add(new Label { Text = "Select a transaction category to generate detailed clinic reports.", Font = ThemeManager.BodyFont, ForeColor = Color.Gray, Location = new Point(6, 50), AutoSize = true });
            tlpMain.Controls.Add(pnlHeader, 0, 0);

            FlowLayoutPanel flp = new FlowLayoutPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 20, 0, 0) };
            flp.Controls.Add(CreateReportCard("Appointment Records", "Daily/Weekly clinic visit logs and booking status.", "View Report", "Appointments"));
            flp.Controls.Add(CreateReportCard("Treatment Transactions", "Medical services rendered and revenue breakdown.", "View Report", "Treatments"));
            flp.Controls.Add(CreateReportCard("Vet Schedules", "Veterinarian appointments and schedules.", "View Report", "VetSchedules"));
            tlpMain.Controls.Add(flp, 0, 1);
        }

        private void LoadReportData(string type)
        {
            currentReportName = type;
            tlpMain.Controls.Clear();

            Panel pnlHeader = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            lblReportTitle = new Label { Text = $"{type} Report", Font = ThemeManager.HeaderFont, ForeColor = ThemeManager.TextDark, Location = new Point(0, 0), AutoSize = true };
            pnlHeader.Controls.Add(lblReportTitle);
            
            RoundedButton btnBack = new RoundedButton { Text = "← Back", Size = new Size(100, 35), Location = new Point(0, 60), BackColor = Color.Gray };
            btnBack.Click += (s, e) => ShowReportSelection();
            pnlHeader.Controls.Add(btnBack);

            RoundedButton btnExport = new RoundedButton { Text = "📊 Export to MS Excel", Size = new Size(200, 35), Location = new Point(120, 60), BackColor = ColorTranslator.FromHtml("#166534") };
            btnExport.Click += (s, e) => ExcelExporter.ExportToExcel((DataTable)dgvReport.DataSource, lblReportTitle.Text, UserAccountManager.CurrentUserFullName);
            pnlHeader.Controls.Add(btnExport);

            tlpMain.Controls.Add(pnlHeader, 0, 0);

            DataTable dt = new DataTable();
            try
            {
                using (var conn = DbConnector.GetConnection())
                {
                    if (conn != null)
                    {
                        string query = "";
                        if (type == "Appointments")
                        {
                            query = "SELECT pet_name AS 'Pet Name', owner_name AS 'Owner', vet_name AS 'Veterinarian', appointment_date AS 'Date', status AS 'Status' FROM appointment_report";
                        }
                        else if (type == "Treatments")
                        {
                            query = "SELECT pet_name AS 'Pet Name', diagnosis AS 'Diagnosis', cost AS 'Cost' FROM treatment_cost_report";
                        }
                        else if (type == "VetSchedules")
                        {
                            query = "SELECT CONCAT(first_name, ' ', last_name) AS 'Veterinarian', appointment_date AS 'Date', status AS 'Status' FROM vet_schedule_report";
                        }
                        
                        if (!string.IsNullOrEmpty(query))
                        {
                            using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                            {
                                using (var adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(cmd))
                                {
                                    adapter.Fill(dt);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: " + ex.Message);
            }

            dgvReport = new ModernDataGridView { Dock = DockStyle.Fill, Margin = new Padding(0, 20, 0, 0) };
            dgvReport.DataSource = dt;
            tlpMain.Controls.Add(dgvReport, 0, 1);
        }

        private Panel CreateReportCard(string title, string desc, string btnText, string reportType)
        {
            RoundedPanel pnl = new RoundedPanel { Size = new Size(540, 180), Margin = new Padding(0, 0, 30, 30), Radius = 24 };
            pnl.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Location = new Point(30, 30), AutoSize = true });
            pnl.Controls.Add(new Label { Text = desc, Font = new Font("Segoe UI", 11), ForeColor = Color.Gray, Location = new Point(30, 75), AutoSize = true });
            RoundedButton btn = new RoundedButton { Text = btnText, Size = new Size(200, 50), Location = new Point(30, 110), Radius = 12 };
            btn.Click += (s, e) => LoadReportData(reportType);
            pnl.Controls.Add(btn); return pnl;
        }

        private NavButton CreateNavButton(string text)
        {
            NavButton btn = new NavButton { Text = text };
            btn.Click += (s, e) => ThemeManager.NavigateTo(this, text);
            navButtons.Add(btn);
            return btn;
        }

        private void SetActiveNav(NavButton b) { foreach (var btn in navButtons) btn.IsActive = false; b.IsActive = true; }
        protected override void OnFormClosed(FormClosedEventArgs e) { Application.Exit(); }
    }
}