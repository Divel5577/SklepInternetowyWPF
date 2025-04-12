using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System;
using System.IO;

namespace SklepInternetowyWPF.Converters
{
    public class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            string placeholderPath = Path.Combine(imagesFolder, "placeholder.png");

            if (value is string path && !string.IsNullOrWhiteSpace(path))
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                if (File.Exists(fullPath))
                    return new BitmapImage(new Uri(fullPath));
            }

            // fallback
            return File.Exists(placeholderPath)
                ? new BitmapImage(new Uri(placeholderPath))
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}