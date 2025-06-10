using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Forms.Integration;
using WfCharting = System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using SklepInternetowyWPF.ViewModels;
using SklepInternetowyWPF.Utils;

namespace SklepInternetowyWPF.Views
{
    public partial class StatisticsWindow : Window
    {
        // prywatne pole dla wykresu WinForms
        private WfCharting.Chart _chart;

        private readonly ProductViewModel _vm;

        public StatisticsWindow(ProductViewModel viewModel)
        {
            InitializeComponent();
            _vm = viewModel;
            DataContext = _vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 1) Fade-in, jeśli jest Storyboard w Resources
            if (Resources["FadeInStoryboard"] is Storyboard sb)
                sb.Begin(this);

            // 2) Stworzenie wykresu WinForms
            _chart = new WfCharting.Chart
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            };

            // 2a) Obszar
            var area = new WfCharting.ChartArea("Default");
            area.AxisX.Title = "Produkt";
            area.AxisX.Interval = 1;
            area.AxisY.Title = "Suma sprzedanych";
            _chart.ChartAreas.Add(area);

            // 2b) Seria słupkowa
            var series = new WfCharting.Series("Suma")
            {
                ChartType = WfCharting.SeriesChartType.Column,
                XValueMember = "ProductName",
                YValueMembers = "TotalQuantitySold",
                IsValueShownAsLabel = true
            };
            _chart.Series.Add(series);

            // 3) Dane
            var data = _vm.GetTopSellingProducts()
                          .Select(x => new { x.ProductName, x.TotalQuantitySold })
                          .ToList();
            _chart.DataSource = data;
            _chart.DataBind();

            // 4) Osadzenie
            ChartHost.Child = _chart;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Controls.PrintDialog();
            if (dlg.ShowDialog() == true)
            {
                // Renderujemy wykres do bitmapy
                int w = (int)ChartHost.ActualWidth;
                int h = (int)ChartHost.ActualHeight;
                if (w == 0) w = 600;
                if (h == 0) h = 300;

                using (var bmp = new System.Drawing.Bitmap(w, h))
                {
                    _chart.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, w, h));
                    // Konwersja do WPF BitmapSource
                    var hbitmap = bmp.GetHbitmap();
                    try
                    {
                        var bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            hbitmap, IntPtr.Zero, Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                        var img = new System.Windows.Controls.Image { Source = bmpSource };
                        dlg.PrintVisual(img, "Statystyki sprzedaży");
                    }
                    finally
                    {
                        NativeMethods.DeleteObject(hbitmap);
                    }
                }
            }
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            PdfExporter.ExportStatistics(_vm);
        }

        // Import DeleteObject z GDI, aby usunąć hbitmap
        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
        }
    }
}
