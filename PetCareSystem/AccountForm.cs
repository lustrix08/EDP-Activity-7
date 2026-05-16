using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using PetCareSystem.Theme;
using PetCareSystem.UI;

namespace PetCareSystem
{
    public partial class AccountForm : Form
    {
        private List<NavButton> navButtons = new List<NavButton>();
        private ModernDataGridView dgvUsers = default!;

        private ModernTextBox txtUsername = default!;
        private ModernTextBox txtFullName = default!;
        private ModernTextBox txtPassword = default!;
        private ModernTextBox txtRecovery = default!;

        public AccountForm()
        {
            ThemeManager.ApplyFormTheme(this);
            this.Text = "PetCare - Account Management";
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;

            InitializeCustomLayout();
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                using (MySqlConnection? conn = DbConnector.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT user_id, username, full_name, status FROM users";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvUsers.DataSource = dt;

                    if (dgvUsers.Columns["user_id"] != null) dgvUsers.Columns["user_id"].HeaderText = "ID";
                    if (dgvUsers.Columns["username"] != null) dgvUsers.Columns["username"].HeaderText = "Username";
                    if (dgvUsers.Columns["full_name"] != null) dgvUsers.Columns["full_name"].HeaderText = "Full Name";
                    if (dgvUsers.Columns["status"] != null) dgvUsers.Columns["status"].HeaderText = "Status";
                }
            }
            catch (Exception ex) { MessageBox.Show("Error loading data: " + ex.Message); }
        }

        private void InitializeCustomLayout()
        {

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
            SetActiveNav(btnAccounts);


            TableLayoutPanel tlpMain = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(40, 20, 40, 40), ColumnCount = 1, RowCount = 2 };
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Panel pnlHeader = new Panel { Dock = DockStyle.Fill };
            pnlHeader.Controls.Add(new Label { Text = "User Management", Font = ThemeManager.HeaderFont, ForeColor = ThemeManager.TextDark, AutoSize = true });
            tlpMain.Controls.Add(pnlHeader, 0, 0);

            TableLayoutPanel tlpContent = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            tlpContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tlpContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));


            RoundedPanel pnlGridCard = new RoundedPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 20, 0), Padding = new Padding(20) };
            dgvUsers = new ModernDataGridView { Dock = DockStyle.Fill };
            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
            pnlGridCard.Controls.Add(dgvUsers);
            tlpContent.Controls.Add(pnlGridCard, 0, 0);


            RoundedPanel pnlFormCard = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(25) };
            Label lblDetails = new Label { Text = "Account Details", Font = ThemeManager.SubHeaderFont, ForeColor = ThemeManager.TextDark, Dock = DockStyle.Top, Height = 50 };
            pnlFormCard.Controls.Add(lblDetails);

            Label lblUsername = new Label { Text = "Username", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Location = new Point(25, 60), AutoSize = true };
            txtUsername = new ModernTextBox("Username") { Location = new Point(25, 80), Width = 300 };

            Label lblFullName = new Label { Text = "Full Name", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Location = new Point(25, 140), AutoSize = true };
            txtFullName = new ModernTextBox("Full Name") { Location = new Point(25, 160), Width = 300 };

            Label lblPassword = new Label { Text = "New Password", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Location = new Point(25, 220), AutoSize = true };
            txtPassword = new ModernTextBox("New Password", true) { Location = new Point(25, 240), Width = 300 };

            Label lblRecovery = new Label { Text = "Recovery Answer", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Location = new Point(25, 300), AutoSize = true };
            txtRecovery = new ModernTextBox("Recovery Answer") { Location = new Point(25, 320), Width = 300 };

            pnlFormCard.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblFullName, txtFullName, lblPassword, txtPassword, lblRecovery, txtRecovery });

            TableLayoutPanel tlpBtns = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 100, RowCount = 2, ColumnCount = 2, Padding = new Padding(0, 10, 0, 0) };

            tlpBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            RoundedButton btnAdd = new RoundedButton { Text = "Add New", Size = new Size(140, 40), BackColor = ColorTranslator.FromHtml("#166534"), Anchor = AnchorStyles.None };
            btnAdd.Click += BtnAdd_Click;

            RoundedButton btnUpdate = new RoundedButton { Text = "Update Info", Size = new Size(140, 40), Anchor = AnchorStyles.None };
            btnUpdate.Click += BtnUpdate_Click;

            RoundedButton btnActive = new RoundedButton { Text = "Set Active", Size = new Size(140, 40), BackColor = Color.CornflowerBlue, Anchor = AnchorStyles.None };
            btnActive.Click += (s, e) => UpdateUserStatus("Active");

            RoundedButton btnInactive = new RoundedButton { Text = "Set Inactive", Size = new Size(140, 40), BackColor = ColorTranslator.FromHtml("#991B1B"), Anchor = AnchorStyles.None };
            btnInactive.Click += (s, e) => UpdateUserStatus("Inactive");

            tlpBtns.Controls.Add(btnAdd, 0, 0); tlpBtns.Controls.Add(btnUpdate, 1, 0);
            tlpBtns.Controls.Add(btnActive, 0, 1); tlpBtns.Controls.Add(btnInactive, 1, 1);
            pnlFormCard.Controls.Add(tlpBtns);

            tlpContent.Controls.Add(pnlFormCard, 1, 0);
            tlpMain.Controls.Add(tlpContent, 0, 1);
            this.Controls.Add(tlpMain);
            tlpMain.BringToFront();
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

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.TextBox.Text) || string.IsNullOrWhiteSpace(txtPassword.TextBox.Text))
            {
                MessageBox.Show("Username and Password are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (MySqlConnection? conn = DbConnector.GetConnection())
                {
                    if (conn == null) return;
                    string query = "INSERT INTO users (username, full_name, password, recovery_answer, status) VALUES (@u, @f, @p, @r, 'Active')";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", txtUsername.TextBox.Text);
                    cmd.Parameters.AddWithValue("@f", txtFullName.TextBox.Text);
                    cmd.Parameters.AddWithValue("@p", txtPassword.TextBox.Text);
                    cmd.Parameters.AddWithValue("@r", txtRecovery.TextBox.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User added successfully!", "Success");
                    ClearInputs(); LoadUserData();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            string userId = dgvUsers.SelectedRows[0].Cells["user_id"].Value.ToString()!;
            try
            {
                using (MySqlConnection? conn = DbConnector.GetConnection())
                {
                    if (conn == null) return;
                    string query = !string.IsNullOrWhiteSpace(txtPassword.TextBox.Text) && txtPassword.TextBox.Text != "New Password"
                        ? "UPDATE users SET username=@u, full_name=@f, recovery_answer=@r, password=@p WHERE user_id=@id"
                        : "UPDATE users SET username=@u, full_name=@f, recovery_answer=@r WHERE user_id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", txtUsername.TextBox.Text);
                    cmd.Parameters.AddWithValue("@f", txtFullName.TextBox.Text);
                    cmd.Parameters.AddWithValue("@r", txtRecovery.TextBox.Text);
                    cmd.Parameters.AddWithValue("@id", userId);
                    if (query.Contains("@p")) cmd.Parameters.AddWithValue("@p", txtPassword.TextBox.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Profile updated!");
                    txtPassword.TextBox.Clear(); LoadUserData();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void UpdateUserStatus(string newStatus)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            string userId = dgvUsers.SelectedRows[0].Cells["user_id"].Value.ToString()!;
            try
            {
                using (MySqlConnection? conn = DbConnector.GetConnection())
                {
                    if (conn == null) return;
                    string query = "UPDATE users SET status=@s WHERE user_id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@s", newStatus);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                    LoadUserData();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void DgvUsers_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                txtUsername.TextBox.Text = dgvUsers.SelectedRows[0].Cells["username"].Value?.ToString() ?? "";
                txtUsername.TextBox.ForeColor = ThemeManager.TextDark;
                txtFullName.TextBox.Text = dgvUsers.SelectedRows[0].Cells["full_name"].Value?.ToString() ?? "";
                txtFullName.TextBox.ForeColor = ThemeManager.TextDark;
                txtPassword.TextBox.Clear();
            }
        }

        private void ClearInputs() { txtUsername.TextBox.Clear(); txtFullName.TextBox.Clear(); txtPassword.TextBox.Clear(); txtRecovery.TextBox.Clear(); }

        protected override void OnFormClosed(FormClosedEventArgs e) { Application.Exit(); }
    }
}