using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BucakliogluERP.Helpers
{
    public static class NotificationHelper
    {
        public static void ShowSuccess(string message)
        {
            ShowNotification(message, "#28A745");
        }

        public static void ShowError(string message)
        {
            ShowNotification(message, "#DC3545");
        }

        public static void ShowInfo(string message)
        {
            ShowNotification(message, "#3498DB");
        }

        public static void ShowWarning(string message)
        {
            ShowNotification(message, "#FFC107");
        }

        private static void ShowNotification(string message, string colorHex)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null) return;

            var grid = mainWindow.Content as Grid;
            if (grid == null) return;

            var notification = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20, 15, 20, 15),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 20, 0, 0),
                Child = new TextBlock
                {
                    Text = message,
                    Foreground = Brushes.White,
                    FontSize = 14,
                    FontWeight = FontWeights.SemiBold
                },
                Opacity = 0
            };

            Grid.SetColumnSpan(notification, 10);
            Grid.SetRowSpan(notification, 10);
            Panel.SetZIndex(notification, 9999);

            grid.Children.Add(notification);

            // Animasyon
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromSeconds(3)
            };

            fadeOut.Completed += (s, e) => grid.Children.Remove(notification);

            notification.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            notification.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }
    }
}