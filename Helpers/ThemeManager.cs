using System;
using System.Windows;
using System.Windows.Media;

namespace BucakliogluERP.Helpers
{
    public static class ThemeManager
    {
        private static bool _isDarkMode = false;

        public static void ApplyTheme(bool darkMode)
        {
            _isDarkMode = darkMode;
            var app = Application.Current;

            if (darkMode)
            {
                // Dark Mode renkleri
                app.Resources["BackgroundColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                app.Resources["ForegroundColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                app.Resources["CardBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D30"));
                app.Resources["BorderColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3E3E42"));
            }
            else
            {
                // Light Mode renkleri
                app.Resources["BackgroundColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF0F1"));
                app.Resources["ForegroundColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50"));
                app.Resources["CardBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                app.Resources["BorderColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DEE2E6"));
            }
        }

        public static bool IsDarkMode => _isDarkMode;

        public static void ToggleTheme()
        {
            ApplyTheme(!_isDarkMode);
        }
    }
}