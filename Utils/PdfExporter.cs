using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SklepInternetowyWPF.Models;
using SklepInternetowyWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;

namespace SklepInternetowyWPF.Utils
{
    public static class PdfExporter
    {
        public static void ExportCartToPdf(CartViewModel cart)
        {
            var doc = new PdfDocument();
            doc.Info.Title = $"Koszyk_{cart.CurrentUsername}_{DateTime.Now:yyyyMMdd_HHmmss}";

            const double outerMargin = 20;    // zewnętrzny margines od strony  
            const double innerPadding = 20;   // dodatkowy padding wewnątrz ramki  
            const double rowHeight = 25;

            var headerFont = new XFont("Verdana", 14, XFontStyle.Bold);
            var normalFont = new XFont("Verdana", 11);
            var footerFont = new XFont("Verdana", 9, XFontStyle.Italic);
            var sectionFont = new XFont("Verdana", 12, XFontStyle.Bold);

            // jedna strona  
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            // pole rysowania ramki  
            double frameX = outerMargin;
            double frameY = outerMargin;
            double frameWidth = page.Width - 2 * outerMargin;
            double frameHeight = page.Height - 2 * outerMargin;
            gfx.DrawRectangle(XPens.Black, frameX, frameY, frameWidth, frameHeight);

            // obszar treści wewnątrz ramki  
            double contentX = frameX + innerPadding;
            double contentY = frameY + innerPadding;
            double contentWidth = frameWidth - 2 * innerPadding;

            // nagłówek  
            double y = contentY;
            gfx.DrawString($"Koszyk użytkownika: {cart.CurrentUsername}",
                           headerFont, XBrushes.Black,
                           new XPoint(contentX, y));
            // data odsunięta od prawej krawędzi ramki minus padding  
            gfx.DrawString($"Data: {DateTime.Now:yyyy-MM-dd HH:mm}",
                           normalFont, XBrushes.DarkGray,
                           new XRect(contentX, y, contentWidth, normalFont.Height),
                           XStringFormats.TopRight);

            y += 30;

            // rysujemy tabelę zaczynając od contentX  
            y = DrawTable(
                gfx,
                contentX, y,
                contentWidth, rowHeight,
                new[] { "Produkt", "Ilość", "Cena jedn.", "Wartość" },
                cart.CartItems.Select(i => (i.Product.Name, i.Quantity, i.Product.Price, i.Total))
            );

            // jeśli tabela jest pusta, wyświetlamy komunikat
            if (!cart.CartItems.Any())
            {
                gfx.DrawString("Koszyk jest pusty", normalFont, XBrushes.Gray,
                               new XPoint(contentX + contentWidth / 2, y + rowHeight / 2),
                               XStringFormats.Center);
                y += rowHeight;
            }
            else
            {
                y += rowHeight * (cart.CartItems.Count) * 0.45;
            }
            // podsumowanie  
            gfx.DrawString("Razem:", sectionFont, XBrushes.Black,
                          new XPoint(contentX + contentWidth * 0.73, y));
            gfx.DrawString($"{cart.Total:C}", sectionFont, XBrushes.Black,
                          new XPoint(contentX + contentWidth * 0.88, y));
            y += 30;

            // stopka (strona 1)  
            gfx.DrawString("Strona 1", footerFont, XBrushes.Gray,
                           new XRect(0, page.Height - outerMargin / 2, page.Width, outerMargin / 2),
                           XStringFormats.Center);

            SavePdf(doc, $"Koszyk_{cart.CurrentUsername}");
        }

        public static void ExportOrderHistory(OrderHistoryViewModel history)
        {
            var doc = new PdfDocument();
            doc.Info.Title = $"HistoriaZamowien_{DateTime.Now:yyyyMMdd_HHmmss}";

            const double margin = 20;
            const double rowHeight = 20;
            var headerFont = new XFont("Verdana", 14, XFontStyle.Bold);
            var normalFont = new XFont("Verdana", 11);
            var sectionFont = new XFont("Verdana", 12, XFontStyle.Bold);
            var footerFont = new XFont("Verdana", 9, XFontStyle.Italic);

            int pageIndex = 1;
            PdfPage page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            double contentWidth = page.Width - 2 * margin;
            double y = margin + 10;

            // nagłówek pierwszej strony
            string username = history.Orders.FirstOrDefault()?.Username ?? "Gość";
            gfx.DrawString($"Historia zamówień użytkownika: {username}", headerFont, XBrushes.Black, new XPoint(margin, y));
            gfx.DrawString($"Wygenerowano: {DateTime.Now:yyyy-MM-dd HH:mm}", normalFont, XBrushes.DarkGray,
                          new XRect(margin, y, contentWidth, normalFont.Height), XStringFormats.TopRight);
            y += 30;

            foreach (var order in history.Orders)
            {
                // sekcja zamówienia
                gfx.DrawString($"Zamówienie #{order.Id} — {order.Date:yyyy-MM-dd HH:mm}", sectionFont, XBrushes.Black, margin, y);
                y += 20;

                // tabela pozycji
                DrawTable(
                    gfx,
                    margin, y,
                    contentWidth, rowHeight,
                    new[] { "Produkt", "Ilość", "Cena jedn.", "Wartość" },
                    order.Items.Select(i => (i.Product.Name, i.Quantity, i.Product.Price, i.Total))
                );

                // przesunięcie y
                y += rowHeight * (order.Items.Count() + 2);

                // podsumowanie
                gfx.DrawString("Razem:", sectionFont, XBrushes.Black,
                              new XPoint(margin + contentWidth * 0.75, y));
                gfx.DrawString($"{order.Total:C}", sectionFont, XBrushes.Black,
                              new XPoint(margin + contentWidth * 0.89, y));
                y += 30;

                // jeśli zabrakło miejsca, nowa strona
                if (y > page.Height - margin - 60)
                {
                    // footer dla bieżącej strony
                    gfx.DrawString($"Strona {pageIndex}", footerFont, XBrushes.Gray,
                                  new XRect(0, page.Height - margin / 2, page.Width, margin / 2),
                                  XStringFormats.Center);

                    // nowa strona
                    pageIndex++;
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    contentWidth = page.Width - 2 * margin;
                    y = margin + 10;

                    // nagłówek kolejnej strony
                    gfx.DrawString($"Historia zamówień użytkownika: {username}", headerFont, XBrushes.Black, new XPoint(margin, y));
                    gfx.DrawString($"Wygenerowano: {DateTime.Now:yyyy-MM-dd HH:mm}", normalFont, XBrushes.DarkGray,
                                  new XRect(margin, y, contentWidth, normalFont.Height), XStringFormats.TopRight);
                    y += 30;
                }
            }

            // footer ostatniej strony
            gfx.DrawString($"Strona {pageIndex}", footerFont, XBrushes.Gray,
                          new XRect(0, page.Height - margin / 2, page.Width, margin / 2),
                          XStringFormats.Center);

            SavePdf(doc, "HistoriaZamowien");
        }

        private static double DrawTable(
            XGraphics gfx,
            double startX,
            double startY,
            double totalWidth,
            double rowHeight,
            string[] headers,
            System.Collections.Generic.IEnumerable<(string Name, int Qty, decimal Price, decimal LineTotal)> rows)
        {
            int cols = headers.Length;
            // procentowe szerokości kolumn: 50%, 15%, 17.5%, 17.5%
            double[] colWidths = { totalWidth * 0.5, totalWidth * 0.15, totalWidth * 0.175, totalWidth * 0.175 };
            double[] colX = new double[cols + 1];
            colX[0] = startX;
            for (int i = 0; i < cols; i++)
                colX[i + 1] = colX[i] + colWidths[i];

            double y = startY;
            var font = new XFont("Verdana", 11);

            // 1) Nagłówki
            for (int i = 0; i < cols; i++)
            {
                var rect = new XRect(colX[i] - 1, y, colWidths[i] - 2, rowHeight);
                var fmt = (i == 0) ? XStringFormats.CenterLeft : XStringFormats.CenterRight;
                gfx.DrawString(headers[i], font, XBrushes.Black, rect, fmt);
            }
            y += rowHeight;

            // linia pod nagłówkami
            gfx.DrawLine(XPens.Black, startX, y, startX + totalWidth, y);
            y += 5;

            // 2) Wiersze danych
            bool shade = false;
            foreach (var (Name, Qty, Price, LineTotal) in rows)
            {
                // naprzemienne cieniowanie
                if (shade)
                    gfx.DrawRectangle(XBrushes.LightGray, startX, y, totalWidth, rowHeight);
                shade = !shade;

                // ramka wiersza
                gfx.DrawRectangle(XPens.Gray, startX, y, totalWidth, rowHeight);

                // wartości, każda w swojej kolumnie, wyśrodkowane w pionie
                gfx.DrawString(Name, font, XBrushes.Black,
                    new XRect(colX[0] + 4, y, colWidths[0] - 8, rowHeight),
                    XStringFormats.CenterLeft);

                gfx.DrawString(Qty.ToString(), font, XBrushes.Black,
                    new XRect(colX[1], y, colWidths[1], rowHeight),
                    XStringFormats.CenterRight);

                gfx.DrawString($"{Price:C}", font, XBrushes.Black,
                    new XRect(colX[2], y, colWidths[2], rowHeight),
                    XStringFormats.CenterRight);

                gfx.DrawString($"{LineTotal:C}", font, XBrushes.Black,
                    new XRect(colX[3] - 2, y, colWidths[3] - 4, rowHeight),
                    XStringFormats.CenterRight);

                y += rowHeight;
            }

            return y + 5; // zwracamy pozycję końcową + mały odstęp
        }
        public static void ExportStatistics(ProductViewModel viewModel)
        {
            // 1) Przygotuj dokument PDF
            var doc = new PdfDocument();
            doc.Info.Title = $"StatystykiSprzedazy_{DateTime.Now:yyyyMMdd_HHmmss}";

            // 2) Dodaj stronę i XGraphics
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            const double margin = 20;
            double y = margin;

            // 3) Nagłówek
            var headerFont = new XFont("Verdana", 16, XFontStyle.Bold);
            gfx.DrawString(
                "Najczęściej kupowane produkty",
                headerFont,
                XBrushes.Black,
                new XPoint(margin, y)
            );
            y += 30;

            // 4) Stwórz wykres WinForms w pamięci
            var data = viewModel
                .GetTopSellingProducts()
                .Select(x => new { x.ProductName, x.TotalQuantitySold })
                .ToList();

            using (var chart = new Chart())
            {
                // rozmiar wykresu odpowiada szerokości PDF minus marginesy, 300px wysokości
                int chartWidth = (int)(page.Width - 2 * margin);
                int chartHeight = 300;
                chart.Size = new Size(chartWidth, chartHeight);
                chart.BackColor = System.Drawing.Color.White;

                // obszar wykresu
                var area = new ChartArea("Default");
                area.AxisX.Title = "Produkt";
                area.AxisX.Interval = 1;
                area.AxisY.Title = "Suma sprzedanych";
                chart.ChartAreas.Add(area);

                // seria słupkowa
                var series = new Series("Sprzedaż")
                {
                    ChartType = SeriesChartType.Column,
                    XValueMember = "ProductName",
                    YValueMembers = "TotalQuantitySold",
                    IsValueShownAsLabel = true
                };
                chart.Series.Add(series);

                chart.DataSource = data;
                chart.DataBind();

                // 5) Renderuj wykres do bitmapy
                using (var bmp = new Bitmap(chartWidth, chartHeight))
                {
                    chart.DrawToBitmap(bmp, new Rectangle(0, 0, chartWidth, chartHeight));

                    // 6) Wczytaj bitmapę do XImage
                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0;
                        var img = XImage.FromStream(ms);

                        // 7) Narysuj obraz na PDF
                        gfx.DrawImage(
                            img,
                            margin,
                            y,
                            page.Width - 2 * margin,
                            chartHeight
                        );
                    }
                }
            }

            // 8) Stopka z numeracją
            var footerFont = new XFont("Verdana", 9, XFontStyle.Italic);
            gfx.DrawString(
                "Strona 1",
                footerFont,
                XBrushes.Gray,
                new XRect(0, page.Height - margin / 2, page.Width, margin / 2),
                XStringFormats.Center
            );

            // 9) Zapisz i otwórz
            SavePdf(doc, "StatystykiSprzedazy");
        }


        private static void SavePdf(PdfDocument doc, string filenamePrefix)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, $"{filenamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            doc.Save(filePath);
            Process.Start("explorer.exe", $"\"{filePath}\"");
        }
    }
}
