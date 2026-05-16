using System;
using System.Drawing;
using System.Windows.Forms;
using PetCareSystem.Theme;
using PetCareSystem.UI;

namespace PetCareSystem
{
    public partial class RecoveryForm : Form
    {
        private Form _loginForm;

        public RecoveryForm(Form loginForm)
        {
            _loginForm = loginForm;
            ThemeManager.ApplyFormTheme(this);
            this.Text = "PetCare - Reset Password";
            this.WindowState = FormWindowState.Maximized;

            RoundedPanel card = new RoundedPanel { Size = new Size(450, 580), Radius = 25 };

            Label lblTitle = new Label { Text = "Reset Password", Font = ThemeManager.SubHeaderFont, ForeColor = ThemeManager.TextDark, AutoSize = true };
            Label lblSub = new Label { Text = "Verify your identity to set a new password.", Font = new Font("Segoe UI", 10), ForeColor = ThemeManager.TextGray, AutoSize = true };

            ModernTextBox txtUser = new ModernTextBox("Username") { Location = new Point(50, 140) };
            ModernTextBox txtAnswer = new ModernTextBox("Secret Code / Answer") { Location = new Point(50, 210) };
            ModernTextBox txtNewPass = new ModernTextBox("New Password", true) { Location = new Point(50, 280) };

            Label lblResult = new Label { Text = "", Font = new Font("Segoe UI", 9, FontStyle.Bold), AutoSize = true, TextAlign = ContentAlignment.MiddleCenter };

            RoundedButton btnReset = new RoundedButton
            {
                Text = "Update Password",
                Size = new Size(350, 50),
                Location = new Point(50, 360),
                Radius = 15
            };

            btnReset.Click += (s, e) => {
                string user = txtUser.TextBox.Text;
                string secretCode = txtAnswer.TextBox.Text;
                string newPass = txtNewPass.TextBox.Text;

                if (newPass == "New Password" || string.IsNullOrWhiteSpace(newPass))
                {
                    lblResult.Text = "Please enter a valid new password.";
                    lblResult.ForeColor = Color.Red;
                }
                else
                {
                    bool isUpdated = UserAccountManager.UpdatePassword(user, secretCode, newPass);
                    if (isUpdated)
                    {
                        lblResult.Text = "Password updated successfully!";
                        lblResult.ForeColor = Color.Green;
                    }
                    else
                    {
                        lblResult.Text = "Invalid username or secret code.";
                        lblResult.ForeColor = Color.Red;
                    }
                }
                lblResult.Left = (card.Width - lblResult.Width) / 2;
            };

            LinkLabel lnkBack = new LinkLabel { Text = "< Back to Sign In", Font = new Font("Segoe UI", 10, FontStyle.Bold), LinkColor = ThemeManager.Accent, LinkBehavior = LinkBehavior.HoverUnderline, AutoSize = true, Cursor = Cursors.Hand };
            lnkBack.Click += (s, e) => { _loginForm.Show(); this.Close(); };

            card.Controls.AddRange(new Control[] { lblTitle, lblSub, txtUser, txtAnswer, txtNewPass, btnReset, lnkBack, lblResult });

            this.Load += (s, e) => {
                lblTitle.Left = (card.Width - lblTitle.Width) / 2; lblTitle.Top = 50;
                lblSub.Left = (card.Width - lblSub.Width) / 2; lblSub.Top = 95;
                lblResult.Top = 420;
                lblResult.Left = (card.Width - lblResult.Width) / 2;
                lnkBack.Left = (card.Width - lnkBack.Width) / 2; lnkBack.Top = 490;
                card.Location = new Point((this.ClientSize.Width - card.Width) / 2, (this.ClientSize.Height - card.Height) / 2);
            };

            this.Controls.Add(card);
            this.Resize += (s, e) => card.Location = new Point((this.ClientSize.Width - card.Width) / 2, (this.ClientSize.Height - card.Height) / 2);
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { if (Application.OpenForms.Count == 0) Application.Exit(); }
    }
}