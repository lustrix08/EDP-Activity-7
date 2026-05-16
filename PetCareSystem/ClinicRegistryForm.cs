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
    public partial class ClinicRegistryForm : Form
    {
        private List<NavButton> navButtons = new List<NavButton>();
        private List<Button> tabButtons = new List<Button>();
        private Label lblTabTitle = new Label();
        private RoundedButton btnAddRecord = new RoundedButton();
        private ModernDataGridView dgvRegistry = new ModernDataGridView();
        private string currentTab = "Customers";

        public ClinicRegistryForm()
        {
            ThemeManager.ApplyFormTheme(this);
            this.Text = "PetCare - Clinic Registry";
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;


            TableLayoutPanel tlpSidebar = new TableLayoutPanel { Dock = DockStyle.Left, Width = 320, BackColor = Color.White, RowCount = 3, ColumnCount = 1 };
            tlpSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize)); tlpSidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); tlpSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpSidebar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(ThemeManager.Border), tlpSidebar.Width - 1, 0, tlpSidebar.Width - 1, tlpSidebar.Height);
            tlpSidebar.Controls.Add(new Label { Text = "AdminPanel", Font = new Font("Georgia", 25, FontStyle.Bold), ForeColor = ThemeManager.TextDark, Margin = new Padding(20, 30, 0, 30), AutoSize = true }, 0, 0);

            FlowLayoutPanel pnlNav = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(15, 0, 15, 0) };
            pnlNav.Controls.AddRange(new Control[] { CreateNavButton("Dashboard"), CreateNavButton("Account Management"), CreateNavButton("Clinic Registry"), CreateNavButton("Report Generator"), CreateNavButton("About Program") });
            tlpSidebar.Controls.Add(pnlNav, 0, 1);
            
            SignOutButton btnSignOut = new SignOutButton { Margin = new Padding(20, 0, 0, 40) };
            btnSignOut.Click += (s, e) => Application.Restart();
            tlpSidebar.Controls.Add(btnSignOut, 0, 2);
            this.Controls.Add(tlpSidebar);


            TableLayoutPanel tlpMain = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(40, 20, 40, 40), ColumnCount = 1, RowCount = 2 };
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F)); tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.Controls.Add(tlpMain); tlpMain.BringToFront();

            Panel pnlHeader = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            pnlHeader.Controls.Add(new Label { Text = "Clinic Registry", Font = ThemeManager.HeaderFont, ForeColor = ThemeManager.TextDark, Location = new Point(0, 0), AutoSize = true });
            pnlHeader.Controls.Add(new Label { Text = "Core data registry and records management.", Font = ThemeManager.BodyFont, ForeColor = Color.Gray, Location = new Point(5, 55), AutoSize = true });
            tlpMain.Controls.Add(pnlHeader, 0, 0);

            RoundedPanel tlpCard = new RoundedPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 20, 0, 0), Padding = new Padding(1) };
            TableLayoutPanel tlpCardContent = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
            tlpCardContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); tlpCardContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F)); tlpCardContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpCard.Controls.Add(tlpCardContent);
            tlpMain.Controls.Add(tlpCard, 0, 1);

            TableLayoutPanel tlpTabs = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, Margin = new Padding(0) };
            tlpTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F)); tlpTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F)); tlpTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpTabs.Paint += (s, e) => e.Graphics.DrawLine(new Pen(ThemeManager.Border), 0, tlpTabs.Height - 1, tlpTabs.Width, tlpTabs.Height - 1);
            tlpTabs.Controls.Add(CreateTabButton("Customers", "👥"), 0, 0); tlpTabs.Controls.Add(CreateTabButton("Patients (Pets)", "🐾"), 1, 0);
            tlpTabs.Controls.Add(CreateTabButton("Medical Staff", "🩺"), 2, 0); tlpTabs.Controls.Add(CreateTabButton("Appointments", "📅"), 3, 0);
            tlpCardContent.Controls.Add(tlpTabs, 0, 0);

            TableLayoutPanel tlpContentHeader = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, Padding = new Padding(30, 20, 30, 0) };
            tlpContentHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F)); 
            tlpContentHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tlpContentHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); 
            tlpContentHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            lblTabTitle = new Label { Text = "Customer Directory", Font = ThemeManager.SubHeaderFont, ForeColor = ThemeManager.TextDark, Anchor = AnchorStyles.Left, AutoSize = true };
            
            btnAddRecord = new RoundedButton { Text = "+ Add", Size = new Size(130, 40), Radius = 10, Anchor = AnchorStyles.Right, BackColor = ColorTranslator.FromHtml("#166534") };
            btnAddRecord.Click += (s, e) => OpenDynamicModal(false);

            RoundedButton btnEditRecord = new RoundedButton { Text = "Edit", Size = new Size(130, 40), Radius = 10, Anchor = AnchorStyles.Right, BackColor = Color.CornflowerBlue };
            btnEditRecord.Click += (s, e) => OpenDynamicModal(true);

            RoundedButton btnDeleteRecord = new RoundedButton { Text = "Delete", Size = new Size(130, 40), Radius = 10, Anchor = AnchorStyles.Right, BackColor = ColorTranslator.FromHtml("#991B1B") };
            btnDeleteRecord.Click += BtnDeleteRecord_Click;

            tlpContentHeader.Controls.Add(lblTabTitle, 0, 0); 
            tlpContentHeader.Controls.Add(btnAddRecord, 1, 0);
            tlpContentHeader.Controls.Add(btnEditRecord, 2, 0);
            tlpContentHeader.Controls.Add(btnDeleteRecord, 3, 0);

            tlpCardContent.Controls.Add(tlpContentHeader, 0, 1);

            dgvRegistry = new ModernDataGridView { Dock = DockStyle.Fill, Margin = new Padding(30, 10, 30, 30) };
            tlpCardContent.Controls.Add(dgvRegistry, 0, 2);
            LoadTabData("Customers");
        }

        private void LoadTabData(string tabName)
        {
            currentTab = tabName;
            DataTable dt = new DataTable();
            string query = "";

            if (tabName == "Customers") { 
                lblTabTitle.Text = "Customer Directory"; 
                query = "SELECT owner_id AS ID, first_name AS 'First Name', last_name AS 'Last Name', contact_number AS 'Contact', address AS 'Address' FROM owners";
            } else if (tabName == "Patients (Pets)") { 
                lblTabTitle.Text = "Patient Records"; 
                query = "SELECT pet_id AS ID, pet_name AS 'Pet Name', species AS 'Species', breed AS 'Breed', owner_id AS 'Owner ID' FROM pets";
            } else if (tabName == "Medical Staff") { 
                lblTabTitle.Text = "Veterinary Staff"; 
                query = "SELECT vet_id AS ID, first_name AS 'First Name', last_name AS 'Last Name', specialization AS 'Specialty', contact_number AS 'Contact' FROM vets";
            } else { 
                lblTabTitle.Text = "Appointments"; 
                query = "SELECT appointment_id AS ID, appointment_date AS 'Date', pet_id AS 'Pet ID', vet_id AS 'Vet ID', status AS 'Status' FROM appointments";
            }

            try
            {
                using (var conn = DbConnector.GetConnection())
                {
                    if (conn != null)
                    {
                        using (var cmd = new MySqlCommand(query, conn))
                        {
                            using (var adapter = new MySqlDataAdapter(cmd))
                            {
                                adapter.Fill(dt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }

            dgvRegistry.DataSource = dt;
        }

        private Button CreateTabButton(string text, string icon)
        {
            Button btn = new Button { Text = $"{icon}  {text}", Dock = DockStyle.Fill, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.Gray, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0; 
            btn.Paint += (s, e) => { if (currentTab == text) { btn.ForeColor = ThemeManager.TextDark; e.Graphics.FillRectangle(new SolidBrush(ThemeManager.Accent), 0, btn.Height - 4, btn.Width, 4); } else btn.ForeColor = Color.Gray; };
            btn.Click += (s, e) => { LoadTabData(text); foreach (var b in tabButtons) b.Invalidate(); }; 
            tabButtons.Add(btn); return btn;
        }

        private NavButton CreateNavButton(string text)
        {
            NavButton btn = new NavButton { Text = text };
            if (text == "Clinic Registry") btn.IsActive = true;
            btn.Click += (s, e) => ThemeManager.NavigateTo(this, text);
            navButtons.Add(btn); return btn;
        }

        private void OpenDynamicModal(bool isEdit)
        {
            string[] fields;
            List<string> initialValues = new List<string>();
            string recordId = "";

            if (currentTab == "Customers") fields = new[] { "First Name", "Last Name", "Contact Number", "Home Address" };
            else if (currentTab == "Patients (Pets)") fields = new[] { "Pet Name", "Species", "Breed", "Owner ID" };
            else if (currentTab == "Medical Staff") fields = new[] { "First Name", "Last Name", "Specialty", "Contact Number" };
            else fields = new[] { "Date (YYYY-MM-DD)", "Pet ID", "Vet ID", "Status" };

            if (isEdit)
            {
                if (dgvRegistry.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a record to edit.");
                    return;
                }
                var row = dgvRegistry.SelectedRows[0];
                recordId = row.Cells["ID"].Value.ToString() ?? "";
                

                for (int i = 0; i < fields.Length; i++)
                {
                    initialValues.Add(row.Cells[i + 1].Value?.ToString() ?? "");
                }
            }

            var modal = new DynamicEntryModal($"{(isEdit ? "Edit" : "Add")} {currentTab}", fields, initialValues, values => {
                SaveRecord(isEdit, recordId, values);
            });
            modal.ShowDialog();
        }

        private void SaveRecord(bool isEdit, string recordId, List<string> values)
        {
            try
            {
                using (var conn = DbConnector.GetConnection())
                {
                    if (conn == null) return;
                    string query = "";

                    if (currentTab == "Customers")
                    {
                        if (isEdit) query = "UPDATE owners SET first_name=@v0, last_name=@v1, contact_number=@v2, address=@v3 WHERE owner_id=@id";
                        else query = "INSERT INTO owners (first_name, last_name, contact_number, address) VALUES (@v0, @v1, @v2, @v3)";
                    }
                    else if (currentTab == "Patients (Pets)")
                    {
                        if (isEdit) query = "UPDATE pets SET pet_name=@v0, species=@v1, breed=@v2, owner_id=@v3 WHERE pet_id=@id";
                        else query = "INSERT INTO pets (pet_name, species, breed, owner_id) VALUES (@v0, @v1, @v2, @v3)";
                    }
                    else if (currentTab == "Medical Staff")
                    {
                        if (isEdit) query = "UPDATE vets SET first_name=@v0, last_name=@v1, specialization=@v2, contact_number=@v3 WHERE vet_id=@id";
                        else query = "INSERT INTO vets (first_name, last_name, specialization, contact_number) VALUES (@v0, @v1, @v2, @v3)";
                    }
                    else
                    {
                        if (isEdit) query = "UPDATE appointments SET appointment_date=@v0, pet_id=@v1, vet_id=@v2, status=@v3 WHERE appointment_id=@id";
                        else query = "INSERT INTO appointments (appointment_date, pet_id, vet_id, status) VALUES (@v0, @v1, @v2, @v3)";
                    }

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        for (int i = 0; i < values.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@v{i}", values[i]);
                        }
                        if (isEdit) cmd.Parameters.AddWithValue("@id", recordId);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"Record successfully {(isEdit ? "updated" : "added")}.");
                LoadTabData(currentTab);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving record: " + ex.Message);
            }
        }

        private void BtnDeleteRecord_Click(object? sender, EventArgs e)
        {
            if (dgvRegistry.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string recordId = dgvRegistry.SelectedRows[0].Cells["ID"].Value.ToString() ?? "";
                try
                {
                    using (var conn = DbConnector.GetConnection())
                    {
                        if (conn == null) return;
                        string query = "";

                        if (currentTab == "Customers") query = "DELETE FROM owners WHERE owner_id=@id";
                        else if (currentTab == "Patients (Pets)") query = "DELETE FROM pets WHERE pet_id=@id";
                        else if (currentTab == "Medical Staff") query = "DELETE FROM vets WHERE vet_id=@id";
                        else query = "DELETE FROM appointments WHERE appointment_id=@id";

                        using (var cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", recordId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Record deleted successfully.");
                    LoadTabData(currentTab);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting record: " + ex.Message);
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { Application.Exit(); }
    }

    public class DynamicEntryModal : Form
    {
        private List<ModernTextBox> inputBoxes = new List<ModernTextBox>();

        public DynamicEntryModal(string title, string[] fields, List<string> initialValues, Action<List<string>> onSave)
        {
            ThemeManager.ApplyFormTheme(this);
            this.Size = new Size(450, 200 + (fields.Length * 90)); 
            this.FormBorderStyle = FormBorderStyle.None; 
            this.StartPosition = FormStartPosition.CenterParent; 
            this.BackColor = Color.White;
            
            Label lblTitle = new Label { Text = title, Font = ThemeManager.SubHeaderFont, ForeColor = ThemeManager.TextDark, Location = new Point(30, 30), AutoSize = true };
            this.Controls.Add(lblTitle);
            
            int currentY = 90;
            for (int i = 0; i < fields.Length; i++)
            {
                string field = fields[i];
                string val = (initialValues != null && initialValues.Count > i) ? initialValues[i] : "";

                Panel pnl = new Panel { Size = new Size(390, 80), Location = new Point(30, currentY) };
                pnl.Controls.Add(new Label { Text = field.ToUpper(), Font = ThemeManager.SmallBoldFont, ForeColor = Color.Gray, Location = new Point(0, 0), AutoSize = true });
                
                ModernTextBox mtb = new ModernTextBox(field) { Size = new Size(380, 45), Location = new Point(0, 25) };
                mtb.TextBox.Text = val;
                if (!string.IsNullOrEmpty(val)) mtb.TextBox.ForeColor = ThemeManager.TextDark;
                
                inputBoxes.Add(mtb);
                pnl.Controls.Add(mtb);
                this.Controls.Add(pnl); currentY += 90;
            }
            
            RoundedButton btnCancel = new RoundedButton { Text = "Cancel", Size = new Size(180, 50), Location = new Point(30, currentY + 20), BackColor = ColorTranslator.FromHtml("#FDFBF7"), ForeColor = ThemeManager.TextDark, Radius = 12 };
            btnCancel.Click += (s, e) => this.Close();
            
            RoundedButton btnSave = new RoundedButton { Text = "Save Record", Size = new Size(180, 50), Location = new Point(230, currentY + 20), Radius = 12 };
            btnSave.Click += (s, e) => { 
                List<string> results = new List<string>();
                for (int i = 0; i < inputBoxes.Count; i++) {
                    string text = inputBoxes[i].TextBox.Text;
                    results.Add(text == fields[i] ? "" : text);
                }
                onSave(results);
                this.Close(); 
            };
            
            this.Controls.Add(btnCancel); this.Controls.Add(btnSave);
            this.Paint += (s, e) => { 
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ThemeManager.Border, ButtonBorderStyle.Solid); 
                this.Region = new Region(RoundedPanel.GetRoundedPath(this.ClientRectangle, 20)); 
            };
        }
    }
}