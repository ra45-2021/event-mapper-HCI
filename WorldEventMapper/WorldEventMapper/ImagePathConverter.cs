using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WorldEventMapper
{
    public class ImagePathConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? path = value as string;

            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                if (path.StartsWith("pack://"))
                {
                    return CreateBitmap(path, UriKind.Absolute);
                }

                if (File.Exists(path))
                {
                    return CreateBitmap(path, UriKind.Absolute);
                }

                string projectRoot = GetProjectRoot();

                string fullPath = Path.Combine(
                    projectRoot,
                    path.Replace("/", "\\")
                );

                if (File.Exists(fullPath))
                {
                    return CreateBitmap(fullPath, UriKind.Absolute);
                }

                string packUri = $"pack://application:,,,/{path}";
                return CreateBitmap(packUri, UriKind.Absolute);
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static BitmapImage CreateBitmap(string path, UriKind kind)
        {
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, kind);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            bitmap.Freeze();

            return bitmap;
        }

        private static string GetProjectRoot()
        {
            DirectoryInfo? directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (directory != null &&
                   !File.Exists(Path.Combine(directory.FullName, "WorldEventMapper.csproj")))
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}