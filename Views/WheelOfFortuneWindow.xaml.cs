#define ENABLE_DAILY_LIMIT

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SklepInternetowyWPF.ViewModels;

namespace SklepInternetowyWPF.Views
{
    public partial class WheelOfFortuneWindow : Window
    {
        private readonly ProductViewModel _vm;
        private readonly UserViewModel _uvm;
        private readonly int[] _segments = { 0, 5, 10, 15, 20, 25, 0, 5 };
        private readonly int[] _weights = { 6, 5, 4, 3, 2, 1, 6, 5 };

        public WheelOfFortuneWindow(ProductViewModel vm, UserViewModel uvm)
        {
            InitializeComponent();
            _vm = vm;
            _uvm = uvm;
            DrawWheel();
        }

        private void SpinButton_Click(object sender, RoutedEventArgs e)
        {
#if ENABLE_DAILY_LIMIT
            var last = _vm.CurrentUser.LastWheelSpinDate;
            if (last.HasValue && last.Value.Date == DateTime.Today)
            {
                MessageBox.Show("Koło można zakręcić tylko raz dziennie.", "Limit");
                return;
            }
#endif
            SpinButton.IsEnabled = false;

            int index = GetWeightedIndex();
            int discount = _segments[index];

            double segCount = _segments.Length;
            double anglePer = 360.0 / segCount;
            double midAngle = index * anglePer + anglePer / 2;
            double pointerAng = 270; // góra
            double rotation = 360 * 5 + (pointerAng - midAngle);

            var anim = new DoubleAnimation
            {
                From = 0,
                To = rotation,
                Duration = TimeSpan.FromSeconds(4),
                AccelerationRatio = 0.3,
                DecelerationRatio = 0.5,
                FillBehavior = FillBehavior.HoldEnd
            };
            anim.Completed += (s, a) =>
            {
#if ENABLE_DAILY_LIMIT
                var now = DateTime.Now;
                _vm.CurrentUser.LastWheelSpinDate = now;
                _vm.CurrentUser.LastWheelDiscount = discount;
                _uvm.UpdateUserWheel(
                    _vm.CurrentUser.Username,
                    now,
                    discount
                );
#endif
                if (discount > 0)
                {
                    MessageBox.Show($"Gratulacje! Masz {discount}% rabatu.", "Rabat");
                    _vm.ApplyGlobalDiscount(discount);
                }
                else
                {
                    MessageBox.Show("Brak rabatu. Spróbuj ponownie!", "Przykro nam");
                }

                DialogResult = true;
                Close();
            };

            WheelRotate.BeginAnimation(
                RotateTransform.AngleProperty,
                anim
            );
        }

        private int GetWeightedIndex()
        {
            int total = _weights.Sum();
            int r = new Random().Next(total);
            int cum = 0;
            for (int i = 0; i < _weights.Length; i++)
            {
                cum += _weights[i];
                if (r < cum) return i;
            }
            return _weights.Length - 1;
        }

        private void DrawWheel()
        {
            int count = _segments.Length;
            double radius = 175, center = radius;
            Brush[] brushes = {
                Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Yellow,
                Brushes.Orange, Brushes.Purple, Brushes.Turquoise, Brushes.Magenta
            };

            for (int i = 0; i < count; i++)
            {
                double start = i * 360.0 / count;
                double end = (i + 1) * 360.0 / count;
                var pf = new PathFigure { StartPoint = new Point(center, center) };

                var p1 = new Point(
                    center + radius * Math.Cos(start * Math.PI / 180),
                    center + radius * Math.Sin(start * Math.PI / 180)
                );
                pf.Segments.Add(new LineSegment(p1, true));

                var p2 = new Point(
                    center + radius * Math.Cos(end * Math.PI / 180),
                    center + radius * Math.Sin(end * Math.PI / 180)
                );
                pf.Segments.Add(new ArcSegment(
                    p2, new Size(radius, radius), 0, false,
                    SweepDirection.Clockwise, true));
                pf.Segments.Add(new LineSegment(new Point(center, center), true));

                var geom = new PathGeometry();
                geom.Figures.Add(pf);

                var path = new Path
                {
                    Data = geom,
                    Fill = brushes[i],
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                WheelCanvas.Children.Add(path);

                double mid = (start + end) / 2;
                double tx = center + radius * 0.6 * Math.Cos(mid * Math.PI / 180);
                double ty = center + radius * 0.6 * Math.Sin(mid * Math.PI / 180);

                var tb = new System.Windows.Controls.TextBlock
                {
                    Text = $"{_segments[i]}%",
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold
                };
                tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                var sz = tb.DesiredSize;
                Canvas.SetLeft(tb, tx - sz.Width / 2);
                Canvas.SetTop(tb, ty - sz.Height / 2);
                WheelCanvas.Children.Add(tb);
            }

            var hub = new Ellipse
            {
                Width = 50,
                Height = 50,
                Fill = Brushes.Gray,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(hub, center - 25);
            Canvas.SetTop(hub, center - 25);
            WheelCanvas.Children.Add(hub);
        }
    }
}
