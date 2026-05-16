using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetCareSystem.Theme
{
    public static class ThemeManager
    {
        // Colors
        public static Color Primary = ColorTranslator.FromHtml("#FDFBF7"); // Off-white
        public static Color Accent = ColorTranslator.FromHtml("#7C5C46");  // Earthy Brown
        public static Color AccentHover = ColorTranslator.FromHtml("#967259");
        public static Color TextDark = ColorTranslator.FromHtml("#4A3728");
        public static Color TextGray = Color.Gray;
        public static Color Border = ColorTranslator.FromHtml("#E8E2D9");
        public static Color Success = ColorTranslator.FromHtml("#166534");
        public static Color Surface = Color.White;
        public static Color Background = Color.White;

        // Fonts
        public static Font HeaderFont = new Font("Georgia", 28, FontStyle.Bold);
        public static Font SubHeaderFont = new Font("Georgia", 18, FontStyle.Bold);
        public static Font BodyFont = new Font("Segoe UI", 10);
        public static Font SmallBoldFont = new Font("Segoe UI", 9, FontStyle.Bold);
        public static Font NavFont = new Font("Segoe UI", 11, FontStyle.Bold);

        public static void ApplyFormTheme(Form form)
        {
            form.BackColor = Primary;
            form.Font = BodyFont;
            form.ForeColor = TextDark;
        }

        public static void NavigateTo(Form current, string target)
        {
            Form? next = target switch
            {
                "Dashboard" => new DashboardForm(),
                "Account Management" => new AccountForm(),
                "Clinic Registry" => new ClinicRegistryForm(),
                "Report Generator" => new ReportsForm(),
                "About Program" => new AboutForm(),
                _ => null
            };

            if (next != null)
            {
                next.Show();
                current.Hide();
            }
        }
    }
}
