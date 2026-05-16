using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PetCareSystem.Theme;
using PetCareSystem.UI;

namespace PetCareSystem
{
    public partial class CustomMessageBox : Form
    {
        public CustomMessageBox(string message, string title)
        {
            this.Size = new Size(400, 220);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.ShowInTaskbar = false;

            Label lblTitle = new Label { Text = title, Font = ThemeManager.SubHeaderFont, ForeColor = ThemeManager.TextDark, Location = new Point(30, 30), AutoSize = true };
            Label lblMsg = new Label { Text = message, Font = ThemeManager.BodyFont, ForeColor = ThemeManager.TextGray, Location = new Point(30, 80), MaximumSize = new Size(340, 0), AutoSize = true };

            int requiredHeight = lblMsg.Top + lblMsg.PreferredHeight + 90;
            this.Size = new Size(400, Math.Max(220, requiredHeight));

            RoundedButton btnOk = new RoundedButton { Text = "OK", Size = new Size(140, 45), Radius = 12 };
            btnOk.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblMsg);
            this.Controls.Add(btnOk);

            this.Load += (s, e) => {
                btnOk.Location = new Point((this.Width - btnOk.Width) / 2, this.Height - 70);
                this.Region = new Region(RoundedPanel.GetRoundedPath(this.ClientRectangle, 20));
            };

            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ThemeManager.Border, ButtonBorderStyle.Solid);
            };
        }

        public static void Show(string message, string title)
        {
            using (CustomMessageBox msgBox = new CustomMessageBox(message, title))
            {
                msgBox.ShowDialog();
            }
        }
    }
}