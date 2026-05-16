using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using System.Windows.Forms;

namespace PetCareSystem.UI
{
    public static class ExcelExporter
    {
        public static void ExportToExcel(DataTable data, string reportTitle, string userName)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Report Data");

                    int titleSpan = Math.Max(data.Columns.Count, 5);

                    ws.Row(1).Height = 45;
                    ws.Row(2).Height = 20;
                    ws.Row(3).Height = 20;

                    try
                    {
                        if (File.Exists("logo.png"))
                        {
                            ws.AddPicture("logo.png").MoveTo(ws.Cell(1, 1)).WithSize(45, 45);
                        }
                    }
                    catch { }

                    ws.Cell(1, 2).Value = "PETCARE INFORMATION SYSTEM";
                    ws.Cell(1, 2).Style.Font.Bold = true;
                    ws.Cell(1, 2).Style.Font.FontSize = 18;
                    ws.Cell(1, 2).Style.Font.FontColor = XLColor.FromHtml("#4A3728");
                    ws.Cell(1, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    if (titleSpan > 1) {
                        ws.Range(1, 2, 1, titleSpan).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    ws.Cell(2, 2).Value = reportTitle;
                    ws.Cell(2, 2).Style.Font.FontSize = 14;
                    ws.Cell(2, 2).Style.Font.Italic = true;
                    ws.Cell(2, 2).Style.Font.FontColor = XLColor.FromHtml("#666666");
                    if (titleSpan > 1) {
                        ws.Range(2, 2, 2, titleSpan).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    ws.Cell(3, 2).Value = $"Generated on: {DateTime.Now:MMMM dd, yyyy HH:mm}";
                    ws.Cell(3, 2).Style.Font.FontColor = XLColor.FromHtml("#888888");
                    if (titleSpan > 1) {
                        ws.Range(3, 2, 3, titleSpan).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    var table = ws.Cell(5, 1).InsertTable(data);
                    
                    table.Theme = XLTableTheme.None;
                    table.ShowAutoFilter = false;
                    
                    var headerRow = table.HeadersRow();
                    headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#4A3728");
                    headerRow.Style.Font.FontColor = XLColor.White;
                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        var rowRange = ws.Range(6 + i, 1, 6 + i, data.Columns.Count);
                        rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        rowRange.Style.Border.BottomBorderColor = XLColor.FromHtml("#E0D6CC");
                        
                        if (i % 2 == 0)
                            rowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#FAF7F5");
                        else
                            rowRange.Style.Fill.BackgroundColor = XLColor.White;
                    }

                    ws.Columns().AdjustToContents();
                    foreach (var col in ws.Columns())
                    {
                        if (col.Width < 15) col.Width = 15;
                    }

                    int lastRow = ws.LastRowUsed().RowNumber() + 3;
                    ws.Cell(lastRow, 1).Value = "Prepared by:";
                    ws.Cell(lastRow + 2, 1).Value = userName.ToUpper();
                    ws.Cell(lastRow + 2, 1).Style.Font.Bold = true;
                    ws.Cell(lastRow + 2, 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(lastRow + 3, 1).Value = "System Administrator";

                    var wsGraph = workbook.Worksheets.Add("Analytics View");
                    wsGraph.Cell(1, 1).Value = $"{reportTitle} - Visual Analytics";
                    wsGraph.Cell(1, 1).Style.Font.Bold = true;
                    wsGraph.Cell(1, 1).Style.Font.FontSize = 16;

                    using (Bitmap bmp = GenerateChartImage(data, reportTitle))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bmp.Save(ms, ImageFormat.Png);
                            ms.Seek(0, SeekOrigin.Begin);
                            wsGraph.AddPicture(ms).MoveTo(wsGraph.Cell(3, 1)).WithSize(800, 450);
                        }
                    }

                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string targetFolder = Path.Combine(desktopPath, "Generated Reports");
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }
                    string fileName = $"{reportTitle.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    string filePath = Path.Combine(targetFolder, fileName);
                    workbook.SaveAs(filePath);

                    CustomMessageBox.Show($"Report exported successfully to:\n{filePath}", "Export Success");
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Failed to export: {ex.Message}", "Export Error");
            }
        }

        private static Bitmap GenerateChartImage(DataTable dt, string title)
        {
            Bitmap bmp = new Bitmap(800, 450);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int margin = 60;
                int chartWidth = bmp.Width - (margin * 2);
                int chartHeight = bmp.Height - (margin * 2) - 40;

                g.DrawLine(Pens.Black, margin, margin, margin, bmp.Height - margin);
                g.DrawLine(Pens.Black, margin, bmp.Height - margin, bmp.Width - margin, bmp.Height - margin);

                if (dt.Rows.Count > 0)
                {
                    var dict = new System.Collections.Generic.Dictionary<string, double>();
                    
                    if (dt.Columns.Contains("Cost")) // Treatment Cost Report
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string name = row["Pet Name"]?.ToString() ?? "Unknown";
                            if (double.TryParse(row["Cost"].ToString(), out double cost))
                            {
                                if (dict.ContainsKey(name)) dict[name] += cost;
                                else dict[name] = cost;
                            }
                        }
                    }
                    else if (dt.Columns.Contains("Status")) // Appointments or Vet Schedules
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string status = row["Status"]?.ToString() ?? "Unknown";
                            if (dict.ContainsKey(status)) dict[status]++;
                            else dict[status] = 1;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < dt.Rows.Count && i < 10; i++) dict[$"Row {i + 1}"] = i + 1;
                    }

                    if (dict.Count > 0)
                    {
                        int barWidth = (chartWidth / dict.Count) - 20;
                        if (barWidth < 10) barWidth = 10;
                        double maxVal = dict.Values.Max();
                        if (maxVal == 0) maxVal = 1;

                        int i = 0;
                        foreach (var kvp in dict)
                        {
                            int barHeight = (int)((kvp.Value / maxVal) * chartHeight);
                            if (barHeight < 2) barHeight = 2;
                            
                            Rectangle rect = new Rectangle(margin + 20 + (i * (barWidth + 20)), bmp.Height - margin - barHeight, barWidth, barHeight);
                            
                            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.FromArgb(74, 55, 40), Color.FromArgb(124, 92, 70), 90F))
                            {
                                g.FillRectangle(brush, rect);
                            }
                            
                            string label = kvp.Key;
                            if (label.Length > 10) label = label.Substring(0, 10) + "..";
                            g.DrawString(label, new Font("Segoe UI", 8), Brushes.Gray, margin + 20 + (i * (barWidth + 20)), bmp.Height - margin + 5);
                            
                            g.DrawString(kvp.Value.ToString("0.##"), new Font("Segoe UI", 8, FontStyle.Bold), Brushes.Black, margin + 20 + (i * (barWidth + 20)), bmp.Height - margin - barHeight - 15);
                            
                            i++;
                        }
                    }
                }

                g.DrawString(title, new Font("Segoe UI", 14, FontStyle.Bold), Brushes.Black, margin, 20);
            }
            return bmp;
        }
    }
}
