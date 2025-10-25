using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BucakliogluERP.Helpers
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    public static class ToastNotification
    {
        public static void Show(string message, ToastType type = ToastType.Info, int durationSeconds = 3)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow?.Content is not Grid grid) return;

            // Renk belirleme
            var backgroundColor = type switch
            {
                ToastType.Success => "#28A745",
                ToastType.Error => "#DC3545",
                ToastType.Warning => "#FFC107",
                ToastType.Info => "#3498DB",
                _ => "#6C757D"
            };

            // İkon belirleme
            var icon = type switch
            {
                ToastType.Success => "✓",
                ToastType.Error => "✕",
                ToastType.Warning => "⚠",
                ToastType.Info => "ℹ",
                _ => ""
            };

            // Toast container
            var toast = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundColor)),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20, 15, 20, 15),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 20, 0, 0),
                MaxWidth = 500,
                Opacity = 0
            };

            // İçerik
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            var iconText = new TextBlock
            {
                Text = icon,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var messageText = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };

            stackPanel.Children.Add(iconText);
            stackPanel.Children.Add(messageText);
            toast.Child = stackPanel;

            // Grid'e ekle
            Grid.SetColumnSpan(toast, 999);
            Grid.SetRowSpan(toast, 999);
            Panel.SetZIndex(toast, 99999);
            grid.Children.Add(toast);

            // Giriş animasyonu
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            var slideIn = new ThicknessAnimation(
                new Thickness(0, -50, 0, 0),
                new Thickness(0, 20, 0, 0),
                TimeSpan.FromMilliseconds(300)
            );

            // Çıkış animasyonu
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromSeconds(durationSeconds)
            };

            fadeOut.Completed += (s, e) => grid.Children.Remove(toast);

            // Animasyonları başlat
            toast.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            toast.BeginAnimation(FrameworkElement.MarginProperty, slideIn);
            toast.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        // Kısa yol metodları
        public static void Success(string message) => Show(message, ToastType.Success);
        public static void Error(string message) => Show(message, ToastType.Error);
        public static void Warning(string message) => Show(message, ToastType.Warning);
        public static void Info(string message) => Show(message, ToastType.Info);
    }
}