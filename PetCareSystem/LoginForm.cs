using System;
using System.Drawing;
using System.Windows.Forms;
using PetCareSystem.Theme;
using PetCareSystem.UI;

namespace PetCareSystem
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            // 1. Form Setup 
            ThemeManager.ApplyFormTheme(this);
            this.Text = "Pet Clinic Information System";
            this.WindowState = FormWindowState.Maximized;

            RoundedPanel card = new RoundedPanel
            {
                Size = new Size(450, 560),
                Radius = 25,
                BackColor = Color.White
            };

            PictureBox logo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(100, 100),
                Location = new Point((card.Width - 100) / 2, 50)
            };
            try { logo.Image = Image.FromFile("logo.png"); } catch { }

            Label lblTitle = new Label { 
                Text = "PetClinic Admin", 
                Font = new Font("Georgia", 26, FontStyle.Bold), 
                ForeColor = ThemeManager.TextDark, 
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            Label lblSub = new Label { 
                Text = "Secure System Access", 
                Font = new Font("Segoe UI", 10, FontStyle.Italic), 
                ForeColor = ThemeManager.TextGray, 
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            ModernTextBox txtUser = new ModernTextBox("Admin Username") { Location = new Point(50, 240) };
            ModernTextBox txtPass = new ModernTextBox("Password", true) { Location = new Point(50, 310) };

            RoundedButton btnLogin = new RoundedButton
            {
                Text = "Login",
                Size = new Size(350, 55),
                Location = new Point(50, 390),
                Radius = 15
            };

            // Login Logic
            btnLogin.Click += (s, e) => {
                string username = txtUser.TextBox.Text;
                string password = txtPass.TextBox.Text;

                if (username == "Admin Username" || string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Please enter your username.");
                    return;
                }

                if (UserAccountManager.Authenticate(username, password))
                {
                    MessageBox.Show("Login Successful! Welcome to PetCare Admin.");
                    new DashboardForm().Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid credentials or account is inactive.");
                }
            };

            LinkLabel lnkForgot = new LinkLabel
            {
                Text = "Forgot Password?",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                LinkColor = ThemeManager.Accent,
                LinkBehavior = LinkBehavior.HoverUnderline,
                Cursor = Cursors.Hand
            };

            lnkForgot.Click += (s, e) => {
                new RecoveryForm(this).Show();
                this.Hide();
            };

            card.Controls.Add(logo); 
            card.Controls.Add(lblTitle); 
            card.Controls.Add(lblSub);
            card.Controls.Add(txtUser); 
            card.Controls.Add(txtPass);
            card.Controls.Add(btnLogin); 
            card.Controls.Add(lnkForgot);

            this.Load += (s, e) => {
                lblTitle.Left = (card.Width - lblTitle.Width) / 2; lblTitle.Top = 160;
                lblSub.Left = (card.Width - lblSub.Width) / 2; lblSub.Top = 205;
                lnkForgot.Left = (card.Width - lnkForgot.Width) / 2; lnkForgot.Top = 490;
                card.Location = new Point((this.ClientSize.Width - card.Width) / 2, (this.ClientSize.Height - card.Height) / 2);
            };

            this.Controls.Add(card);
            this.Resize += (s, e) => { card.Location = new Point((this.ClientSize.Width - card.Width) / 2, (this.ClientSize.Height - card.Height) / 2); };
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { Application.Exit(); }
    }
}