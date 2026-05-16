using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PetCareSystem.Theme;

namespace PetCareSystem.UI
{
    public class RoundedPanel : Panel
    {
        public int Radius { get; set; } = 20;
        public Color BorderColor { get; set; } = ThemeManager.Border;
        public int BorderSize { get; set; } = 1;

        public RoundedPanel()
        {
            this.BackColor = ThemeManager.Surface;
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = GetRoundedPath(this.ClientRectangle, Radius))
            {
                this.Region = new Region(path);
                if (BorderSize > 0)
                {
                    using (Pen pen = new Pen(BorderColor, BorderSize))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
        }

        public static GraphicsPath GetRoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class RoundedButton : Button
    {
        public int Radius { get; set; } = 15;
        public Color HoverColor { get; set; } = ThemeManager.AccentHover;
        private Color originalColor;

        public RoundedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = ThemeManager.Accent;
            this.ForeColor = Color.White;
            this.Font = ThemeManager.NavFont;
            this.Cursor = Cursors.Hand;
            this.originalColor = this.BackColor;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.BackColor = HoverColor;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.BackColor = originalColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (GraphicsPath path = RoundedPanel.GetRoundedPath(this.ClientRectangle, Radius))
            {
                this.Region = new Region(path);
            }
        }
    }

    public class ModernTextBox : Panel
    {
        public TextBox TextBox { get; private set; }
        public string Placeholder { get; set; }
        public bool IsPassword { get; set; }

        public ModernTextBox(string placeholder, bool isPassword = false)
        {
            this.Placeholder = placeholder;
            this.IsPassword = isPassword;
            this.Size = new Size(350, 45);
            this.BackColor = ThemeManager.Background;
            this.Padding = new Padding(15, 10, 15, 10);

            TextBox = new TextBox
            {
                Text = placeholder,
                ForeColor = Color.Gray,
                Font = ThemeManager.BodyFont,
                BorderStyle = BorderStyle.None,
                BackColor = ThemeManager.Background,
                Dock = DockStyle.Fill,
                PasswordChar = '\0'
            };

            TextBox.Enter += (s, e) => {
                if (TextBox.Text == Placeholder)
                {
                    TextBox.Text = "";
                    TextBox.ForeColor = ThemeManager.TextDark;
                    if (IsPassword) TextBox.PasswordChar = '•';
                }
            };

            TextBox.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(TextBox.Text))
                {
                    TextBox.Text = Placeholder;
                    TextBox.ForeColor = Color.Gray;
                    if (IsPassword) TextBox.PasswordChar = '\0';
                }
            };

            this.Controls.Add(TextBox);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = RoundedPanel.GetRoundedPath(this.ClientRectangle, 12))
            {
                this.Region = new Region(path);
                using (Pen pen = new Pen(ThemeManager.Border, 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }
    }
    public class NavButton : Button
    {
        private bool isActive = false;
        public bool IsActive
        {
            get => isActive;
            set { isActive = value; UpdateStyle(); }
        }

        public NavButton()
        {
            this.Size = new Size(285, 60);
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Font = ThemeManager.NavFont;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.Cursor = Cursors.Hand;
            this.Margin = new Padding(0, 0, 0, 10);
            UpdateStyle();
        }

        private void UpdateStyle()
        {
            if (isActive)
            {
                this.BackColor = ThemeManager.Accent;
                this.ForeColor = Color.White;
            }
            else
            {
                this.BackColor = Color.White;
                this.ForeColor = ThemeManager.TextDark;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (GraphicsPath path = RoundedPanel.GetRoundedPath(this.ClientRectangle, 12))
            {
                this.Region = new Region(path);
            }
        }
    }

    public class StatCard : RoundedPanel
    {
        public StatCard(string title, string value, string icon)
        {
            this.Size = new Size(180, 110);
            this.Margin = new Padding(0, 0, 15, 15);
            this.Dock = DockStyle.Fill;
            this.Radius = 15;

            Label lblIcon = new Label { 
                Text = icon, 
                Font = new Font("Segoe UI Emoji", 18), 
                ForeColor = ThemeManager.Accent, 
                Location = new Point(15, 12), 
                AutoSize = true 
            };
            
            Label lblTitle = new Label { 
                Text = title, 
                Font = ThemeManager.SmallBoldFont, 
                ForeColor = ThemeManager.TextGray, 
                Location = new Point(15, 50), 
                AutoSize = true 
            };
            
            Label lblVal = new Label { 
                Text = value, 
                Font = new Font("Segoe UI", 18, FontStyle.Bold), 
                ForeColor = ThemeManager.TextDark, 
                Location = new Point(12, 68), 
                AutoSize = true 
            };

            this.Controls.Add(lblIcon);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblVal);
        }
    }

    public class ModernDataGridView : DataGridView
    {
        public ModernDataGridView()
        {
            this.BackgroundColor = Color.White;
            this.BorderStyle = BorderStyle.None;
            this.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.EnableHeadersVisualStyles = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.MultiSelect = false;
            this.AllowUserToAddRows = false;
            this.AllowUserToResizeRows = false;
            this.RowHeadersVisible = false;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.RowTemplate.Height = 50;

            // Header Style
            this.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = ThemeManager.TextGray,
                Font = ThemeManager.SmallBoldFont,
                Padding = new Padding(10, 0, 0, 0),
                SelectionBackColor = Color.White
            };

            // Row Style
            this.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = ThemeManager.TextDark,
                Font = ThemeManager.BodyFont,
                SelectionBackColor = ColorTranslator.FromHtml("#F9FAFB"),
                SelectionForeColor = ThemeManager.Accent,
                Padding = new Padding(10, 0, 0, 0)
            };

            this.GridColor = ThemeManager.Border;
        }
    }

    public class SignOutButton : Button
    {
        public SignOutButton()
        {
            this.Text = "🚪 Sign Out";
            this.Size = new Size(200, 45);
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.Gray;
            this.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            this.Cursor = Cursors.Hand;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.Padding = new Padding(20, 0, 0, 0);
            this.MouseEnter += (s, e) => { this.ForeColor = ThemeManager.Accent; this.BackColor = ColorTranslator.FromHtml("#FDFBF7"); };
            this.MouseLeave += (s, e) => { this.ForeColor = Color.Gray; this.BackColor = Color.Transparent; };
        }
    }
}
