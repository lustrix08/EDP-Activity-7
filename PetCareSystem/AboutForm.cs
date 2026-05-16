// Comment

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PetCareSystem.Theme;
using PetCareSystem.UI;

namespace PetCareSystem
{
    public partial class AboutForm : Form
    {
        private List<NavButton> navButtons = new List<NavButton>();

        public AboutForm()
        {
            ThemeManager.ApplyFormTheme(this);
            this.Text = "PetCare - About Program";
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;

            // --- SIDEBAR ---
            TableLayoutPanel tlpSidebar = new TableLayoutPanel { Dock = DockStyle.Left, Width = 320, BackColor = Color.White, RowCount = 3, ColumnCount = 1 };
            tlpSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpSidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpSidebar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(ThemeManager.Border), tlpSidebar.Width - 1, 0, tlpSidebar.Width - 1, tlpSidebar.Height);

            Label lblBrand = new Label { Text = "AdminPanel", Font = new Font("Georgia", 25, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Margin = new Padding(25, 40, 0, 40), AutoSize = true };
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
            SetActiveNav(btnAbout);

            // --- MAIN CONTENT ---
            Panel pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(80) };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            Label lblTitle = new Label
            {
                Text = "About Pet Clinic Information System",
                Font = ThemeManager.HeaderFont,
                ForeColor = ThemeManager.TextDark,
                AutoSize = true,
                Location = new Point(80, 80)
            };

            Label lblDetails = new Label
            {
                Text = "Version 1.0.5 Patch Build\n\n" +
                       "Developed by Owhie Lumbang\n" +
                       "Bicol University - Information Technology Student\n\n" +
                       "Because every patient has a heartbeat, PetCare was designed to do more than \n" +
                       "just manage data. " +
                       "Built with Visual Studio C#, this system streamlines \nthe 'paperwork' so you can get back to the " +
                       "'paw-work'—combining a \nhigh-end, responsive interface with the reliability \nyour clinic depends on.",
                Font = ThemeManager.BodyFont,
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(80, 200)
            };

            pnlMain.Controls.Add(lblTitle);
            pnlMain.Controls.Add(lblDetails);
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

        protected override void OnFormClosed(FormClosedEventArgs e) { Application.Exit(); }
    }
}