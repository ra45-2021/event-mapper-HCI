using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WorldEventMapper.Event_Management;
using WorldEventMapper.Event_List;
using WorldEventMapper.Help;

namespace WorldEventMapper.Home
{
    public partial class HomePage : Page
    {
        private Point _dragStartPoint;

        public HomePage()
        {
            InitializeComponent();
        }

        private void HomeLogo_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void EventManagement_Click(object sender, RoutedEventArgs e)
        {
            EventManagement_Main eventManagementWindow = new EventManagement_Main();
            eventManagementWindow.Show();

            Window currentWindow = Window.GetWindow(this);
            currentWindow?.Close();
        }

        private void EventList_Click(object sender, RoutedEventArgs e)
        {
            EventList_Main eventListWindow = new EventList_Main();
            eventListWindow.Show();

            Window currentWindow = Window.GetWindow(this);
            currentWindow?.Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help_Main helpWindow = new Help_Main();
            helpWindow.Show();

            Window currentWindow = Window.GetWindow(this);
            currentWindow?.Close();
        }

        private void CloseToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Window currentWindow = Window.GetWindow(this);
            currentWindow?.Close();
        }

        private void DraggableEventsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void DraggableEventsList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            Point currentPosition = e.GetPosition(null);

            if (Math.Abs(currentPosition.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(currentPosition.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            if (DraggableEventsList.SelectedItem is ListBoxItem selectedItem)
            {
                string? eventName = selectedItem.Tag?.ToString();

                if (!string.IsNullOrWhiteSpace(eventName))
                {
                    DragDrop.DoDragDrop(selectedItem, eventName, DragDropEffects.Copy);
                }
            }
        }

        private void MapCanvas_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void MapCanvas_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.StringFormat))
                return;

            string? eventName = e.Data.GetData(DataFormats.StringFormat) as string;

            if (string.IsNullOrWhiteSpace(eventName))
                return;

            Point dropPosition = e.GetPosition(MapCanvas);

            AddMarkerToMap(eventName, dropPosition);
        }

        private void AddMarkerToMap(string eventName, Point position)
        {
            Brush markerBrush = GetMarkerBrush(eventName);
            string markerLetter = GetMarkerLetter(eventName);

            StackPanel marker = new StackPanel
            {
                Orientation = Orientation.Vertical,
                ToolTip = eventName,
                Cursor = Cursors.Hand
            };

            Border badge = new Border
            {
                Width = 34,
                Height = 34,
                Background = markerBrush,
                CornerRadius = new CornerRadius(6),
                Child = new TextBlock
                {
                    Text = markerLetter,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center
                }
            };

            Ellipse pinHead = new Ellipse
            {
                Width = 14,
                Height = 14,
                Fill = Brushes.Transparent,
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0E7EEC")),
                StrokeThickness = 3,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 2, 0, 0)
            };

            Rectangle pinLine = new Rectangle
            {
                Width = 3,
                Height = 18,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0E7EEC")),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            marker.Children.Add(badge);
            marker.Children.Add(pinHead);
            marker.Children.Add(pinLine);

            marker.MouseRightButtonDown += RemoveMarker_MouseRightButtonDown;

            double left = position.X - 17;
            double top = position.Y - 52;

            left = Math.Max(0, Math.Min(left, MapCanvas.ActualWidth - 34));
            top = Math.Max(0, Math.Min(top, MapCanvas.ActualHeight - 65));

            Canvas.SetLeft(marker, left);
            Canvas.SetTop(marker, top);

            MapCanvas.Children.Add(marker);
        }

        private void RemoveMarker_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement marker)
            {
                MapCanvas.Children.Remove(marker);
                e.Handled = true;
            }
        }

        private Brush GetMarkerBrush(string eventName)
        {
            if (eventName.Contains("Kustendorf"))
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2C94C"));

            if (eventName.Contains("Art Basel"))
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5D8AA8"));

            if (eventName.Contains("Burning Man"))
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C4AB6"));

            if (eventName.Contains("Sarajevo"))
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F29C9C"));

            if (eventName.Contains("UN Summit"))
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D00000"));

            if (eventName.Contains("Vienna"))
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF"));

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0E7EEC"));
        }

        private string GetMarkerLetter(string eventName)
        {
            if (eventName.Contains("Kustendorf"))
                return "B";

            if (eventName.Contains("Art Basel"))
                return "D";

            if (eventName.Contains("Burning Man"))
                return "E";

            if (eventName.Contains("Sarajevo"))
                return "K";

            if (eventName.Contains("UN Summit"))
                return "P";

            if (eventName.Contains("Vienna"))
                return "S";

            return "E";
        }
    }
}