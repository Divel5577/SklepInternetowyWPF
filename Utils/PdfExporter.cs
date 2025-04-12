using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SklepInternetowyWPF.Models;
using SklepInternetowyWPF.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace SklepInternetowyWPF.Utils
{
    public static class PdfExporter
    {
        public static void ExportCartToPdf(CartViewModel cart)
        {
            var doc = new PdfDocument();
            doc.Info.Title = "Koszyk";

            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12);
            double y = 40;
            gfx.DrawString("Koszyk", new XFont("Verdana", 16, XFontStyle.Bold), XBrushes.Black, 20, y);
            y += 30;

            foreach (var item in cart.CartItems)
            {
                string line = $"{item.Product.Name} x {item.Quantity} - {item.Total:C}";
                gfx.DrawString(line, font, XBrushes.Black, 20, y);
                y += 20;
            }

            y += 10;
            gfx.DrawString($"Łącznie: {cart.Total:C}", font, XBrushes.Black, 20, y);

            SavePdf(doc, "Koszyk");
        }

        public static void ExportOrderHistory(OrderViewModel orderVM)
        {
            var doc = new PdfDocument();
            doc.Info.Title = "Historia zamówień";

            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12);
            double y = 40;
            gfx.DrawString("Historia zamówień", new XFont("Verdana", 16, XFontStyle.Bold), XBrushes.Black, 20, y);
            y += 30;

            foreach (var order in orderVM.Orders)
            {
                gfx.DrawString($"Zamówienie: {order.Date:yyyy-MM-dd HH:mm}", font, XBrushes.Black, 20, y);
                y += 20;

                foreach (var item in order.Items)
                {
                    gfx.DrawString($"- {item.Product.Name} x {item.Quantity} = {item.Total:C}", font, XBrushes.Black, 30, y);
                    y += 20;
                }

                gfx.DrawString($"Łącznie: {order.Total:C}", font, XBrushes.Black, 20, y);
                y += 30;
            }

            SavePdf(doc, "HistoriaZamowien");
        }

        public static void ExportStatistics(ProductViewModel viewModel)
        {
            var top = viewModel.GetTopSellingProducts();

            var doc = new PdfDocument();
            doc.Info.Title = "Statystyki";

            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12);
            double y = 40;
            gfx.DrawString("Najczęściej kupowane produkty", new XFont("Verdana", 16, XFontStyle.Bold), XBrushes.Black, 20, y);
            y += 30;

            foreach (var item in top)
            {
                string line = $"{item.ProductName} - {item.TotalQuantitySold} szt.";
                gfx.DrawString(line, font, XBrushes.Black, 20, y);
                y += 20;
            }

            SavePdf(doc, "StatystykiSprzedazy");
        }

        private static void SavePdf(PdfDocument doc, string filenamePrefix)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, $"{filenamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            doc.Save(filePath);
            Process.Start("explorer.exe", filePath);
        }
    }
}
