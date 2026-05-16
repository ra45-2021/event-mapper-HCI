using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using WorldEventMapper.EventTags;
using WorldEventMapper.EventTypes;
using WorldEventMapper.Help;
using WorldEventMapper.Home;
using WorldEventMapper.Models;
using WorldEventMapper.ViewModels;
using WorldEventMapper.Services;
using System.Linq;

namespace WorldEventMapper.Events
{
    public partial class Events_Main : Window
    {
        private MainDataViewModel ViewModel => (MainDataViewModel)DataContext;
        private readonly DispatcherTimer _toastTimer = new DispatcherTimer();
        private EventModel? _eventToDelete;
        private readonly JsonDataService _dataService = new JsonDataService();

        public Events_Main()
        {
            InitializeComponent();
            DataContext = new MainDataViewModel();

            _toastTimer.Interval = TimeSpan.FromSeconds(3);
            _toastTimer.Tick += ToastTimer_Tick;

            NotificationService.NotificationRequested += OnNotificationRequested;
        }

        private void OnNotificationRequested(NotificationMessage notification)
        {
            if (notification.Type == NotificationType.Success)
            {
                ShowSuccessToast(notification.Message);
            }
            else
            {
                ShowErrorToast(notification.Message);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            NotificationService.NotificationRequested -= OnNotificationRequested;
            base.OnClosed(e);
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.Content = new HomePage();

            Close();
        }

        private void EventTypes_Click(object sender, RoutedEventArgs e)
        {
            EventTypes_Main eventTypesWindow = new EventTypes_Main();
            eventTypesWindow.Show();

            Close();
        }

        private void EventTags_Click(object sender, RoutedEventArgs e)
        {
            EventTags_Main eventTagsWindow = new EventTags_Main();
            eventTagsWindow.Show();

            Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help_Main helpWindow = new Help_Main();
            helpWindow.Show();

            Close();
        }

        private void CloseToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Close();
        }

        private void NewEvent_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;

            Events_Add_Edit addEditWindow = new Events_Add_Edit();
            addEditWindow.Owner = this;
            addEditWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            bool? result = addEditWindow.ShowDialog();

            PopupOverlay.Visibility = Visibility.Collapsed;

            if (result == true)
            {
                NotificationService.ShowSuccess("Event created successfully.");

                DataContext = new MainDataViewModel();
            }
        }


        private void ViewEvent_Click(object sender, RoutedEventArgs e)
        {
            EventModel? selectedEvent = GetEventFromButton(sender);

            if (selectedEvent == null)
                return;

            MessageBox.Show($"View event: {selectedEvent.Name}", "View Event");
        }

        private void EditEvent_Click(object sender, RoutedEventArgs e)
        {
            EventModel? selectedEvent = GetEventFromButton(sender);

            if (selectedEvent == null)
                return;

            PopupOverlay.Visibility = Visibility.Visible;

            Events_Add_Edit addEditWindow = new Events_Add_Edit(selectedEvent);
            addEditWindow.Owner = this;
            addEditWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            bool? result = addEditWindow.ShowDialog();

            PopupOverlay.Visibility = Visibility.Collapsed;

            if (result == true)
            {
                NotificationService.ShowSuccess("Event updated successfully.");
            }

            DataContext = new MainDataViewModel();
        }

        private void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            EventModel? selectedEvent = GetEventFromButton(sender);

            if (selectedEvent == null)
                return;

            _eventToDelete = selectedEvent;

            DeleteEventNameTextBlock.Text = selectedEvent.Name;

            DeleteEventIdTextBlock.Text = selectedEvent.ID;
            DeleteEventCostTextBlock.Text = $"${selectedEvent.Cost:N0}";

            AppData data = _dataService.Load();

            EventTypeModel? eventType = data.EventTypes
                .FirstOrDefault(t => t.ID == selectedEvent.EventTypeId);

            DeleteEventTypeTextBlock.Text = eventType?.Name ?? "Unknown";

            DeleteEventIconImage.Source = new ImagePathConverter().Convert(
                selectedEvent.IconPath,
                typeof(ImageSource),
                null,
                System.Globalization.CultureInfo.CurrentCulture
            ) as ImageSource;

            PopupOverlay.Visibility = Visibility.Visible;
            DeleteConfirmPopup.Visibility = Visibility.Visible;
        }

        private void CancelDelete_Click(object sender, RoutedEventArgs e)
        {
            _eventToDelete = null;

            DeleteConfirmPopup.Visibility = Visibility.Collapsed;
            PopupOverlay.Visibility = Visibility.Collapsed;
        }

        private void ConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_eventToDelete == null)
                return;

            AppData data = _dataService.Load();

            EventModel? eventToRemove = data.Events
                .FirstOrDefault(e => e.ID == _eventToDelete.ID);

            if (eventToRemove != null)
            {
                data.Events.Remove(eventToRemove);
                _dataService.Save(data);

                NotificationService.ShowSuccess("Event deleted successfully.");
            }

            _eventToDelete = null;

            DeleteConfirmPopup.Visibility = Visibility.Collapsed;
            PopupOverlay.Visibility = Visibility.Collapsed;

            DataContext = new MainDataViewModel();
        }

        private static EventModel? GetEventFromButton(object sender)
        {
            return (sender as Button)?.DataContext as EventModel;
        }

        private void OpenSearchPopup_Click(object sender, RoutedEventArgs e)
        {
            AdvancedSearchPopup.IsOpen = true;
        }

        private void CloseSearchPopup_Click(object sender, RoutedEventArgs e)
        {
            AdvancedSearchPopup.IsOpen = false;
        }

        private void ApplyAdvancedSearch_Click(object sender, RoutedEventArgs e)
        {
            UpdateAdvancedSearchBindings();

            ViewModel.RefreshSearch();

            ClearAdvancedSearchButton.Visibility =
                HasAdvancedSearchValue() ? Visibility.Visible : Visibility.Collapsed;

            AdvancedSearchPopup.IsOpen = false;
        }

        private void QuickFilterBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterEventsBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(14, 126, 236));
            FilterEventsBorder.Background = Brushes.White;
        }

        private void QuickFilterBox_LostFocus(object sender, RoutedEventArgs e)
        {
            FilterEventsBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(199, 210, 224));
            FilterEventsBorder.Background = Brushes.White;
        }

        private void QuickFilterBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!QuickFilterBox.IsKeyboardFocused)
            {
                FilterEventsBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(14, 126, 236));
            }
        }

        private void QuickFilterBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!QuickFilterBox.IsKeyboardFocused)
            {
                FilterEventsBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(199, 210, 224));
                FilterEventsBorder.Background = Brushes.White;
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.EventSearchText = "";
        }

        private void ClearAdvancedSearch_Click(object sender, RoutedEventArgs e)
        {
            ClearAdvancedSearchInputFields();
            UpdateAdvancedSearchBindings();

            ViewModel.ClearAdvancedSearch();
            ViewModel.RefreshSearch();

            ClearAdvancedSearchButton.Visibility = Visibility.Collapsed;

            AdvancedSearchPopup.IsOpen = false;
        }

        private void AdvancedSearchBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AdvancedSearchBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(14, 126, 236));
        }

        private void AdvancedSearchBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AdvancedSearchBorder.Background = Brushes.White;
            AdvancedSearchBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(199, 210, 224));
        }

        private void ClearSearchId_Click(object sender, RoutedEventArgs e)
        {
            SearchIdBox.Text = "";
        }

        private void ClearSearchName_Click(object sender, RoutedEventArgs e)
        {
            SearchNameBox.Text = "";
        }

        private void ClearSearchDescription_Click(object sender, RoutedEventArgs e)
        {
            SearchDescriptionBox.Text = "";
        }

        private void ClearSearchEventType_Click(object sender, RoutedEventArgs e)
        {
            SearchEventTypeBox.SelectedIndex = -1;
            SearchEventTypeBox.SelectedValue = null;
        }

        private void ClearSearchAttendance_Click(object sender, RoutedEventArgs e)
        {
            SearchAttendanceBox.SelectedIndex = -1;
            SearchAttendanceBox.SelectedItem = null;
        }

        private void ClearSearchHumanitarian_Click(object sender, RoutedEventArgs e)
        {
            SearchHumanitarianAny.IsChecked = true;
            SearchHumanitarianYes.IsChecked = false;
            SearchHumanitarianNo.IsChecked = false;
        }

        private void ClearSearchCost_Click(object sender, RoutedEventArgs e)
        {
            SearchCostBox.Text = "";
        }

        private void ClearSearchLocation_Click(object sender, RoutedEventArgs e)
        {
            SearchLocationBox.Text = "";
        }

        private void ClearSearchPastYears_Click(object sender, RoutedEventArgs e)
        {
            SearchPastYearsBox.Text = "";
        }

        private void ClearSearchUpcomingDate_Click(object sender, RoutedEventArgs e)
        {
            SearchUpcomingDateBox.SelectedDate = null;
        }

        private void ClearSearchTag_Click(object sender, RoutedEventArgs e)
        {
            SearchTagBox.Text = "";
        }

        private void UpdateAdvancedSearchBindings()
        {
            UpdateBinding(SearchIdBox, TextBox.TextProperty);
            UpdateBinding(SearchNameBox, TextBox.TextProperty);
            UpdateBinding(SearchDescriptionBox, TextBox.TextProperty);
            UpdateBinding(SearchCostBox, TextBox.TextProperty);
            UpdateBinding(SearchLocationBox, TextBox.TextProperty);
            UpdateBinding(SearchPastYearsBox, TextBox.TextProperty);
            UpdateBinding(SearchTagBox, TextBox.TextProperty);

            UpdateBinding(SearchEventTypeBox, ComboBox.SelectedValueProperty);
            UpdateBinding(SearchAttendanceBox, ComboBox.SelectedItemProperty);

            UpdateBinding(SearchUpcomingDateBox, DatePicker.SelectedDateProperty);

            UpdateBinding(SearchHumanitarianAny, RadioButton.IsCheckedProperty);
            UpdateBinding(SearchHumanitarianYes, RadioButton.IsCheckedProperty);
            UpdateBinding(SearchHumanitarianNo, RadioButton.IsCheckedProperty);
        }

        private void ClearAdvancedSearchInputFields()
        {
            SearchIdBox.Text = "";
            SearchNameBox.Text = "";
            SearchDescriptionBox.Text = "";
            SearchCostBox.Text = "";
            SearchLocationBox.Text = "";
            SearchPastYearsBox.Text = "";
            SearchTagBox.Text = "";

            SearchEventTypeBox.SelectedIndex = -1;
            SearchEventTypeBox.SelectedValue = null;

            SearchAttendanceBox.SelectedIndex = -1;
            SearchAttendanceBox.SelectedItem = null;

            SearchUpcomingDateBox.SelectedDate = null;

            SearchHumanitarianAny.IsChecked = true;
            SearchHumanitarianYes.IsChecked = false;
            SearchHumanitarianNo.IsChecked = false;
        }

        private bool HasAdvancedSearchValue()
        {
            return !string.IsNullOrWhiteSpace(SearchIdBox.Text)
                || !string.IsNullOrWhiteSpace(SearchNameBox.Text)
                || !string.IsNullOrWhiteSpace(SearchDescriptionBox.Text)
                || !string.IsNullOrWhiteSpace(SearchCostBox.Text)
                || !string.IsNullOrWhiteSpace(SearchLocationBox.Text)
                || !string.IsNullOrWhiteSpace(SearchPastYearsBox.Text)
                || !string.IsNullOrWhiteSpace(SearchTagBox.Text)
                || SearchEventTypeBox.SelectedValue != null
                || SearchAttendanceBox.SelectedItem != null
                || SearchUpcomingDateBox.SelectedDate != null
                || SearchHumanitarianYes.IsChecked == true
                || SearchHumanitarianNo.IsChecked == true;
        }

        private static void UpdateBinding(FrameworkElement element, DependencyProperty property)
        {
            BindingExpression? binding = element.GetBindingExpression(property);
            binding?.UpdateSource();
        }

        public void ShowSuccessToast(string message)
        {
            ShowToast("SUCCESS", message, true);
        }

        public void ShowErrorToast(string message)
        {
            ShowToast("ERROR", message, false);
        }

        private void ShowToast(string title, string message, bool isSuccess)
        {
            _toastTimer.Stop();

            ToastTitleTextBlock.Text = title;
            ToastMessageTextBlock.Text = message;

            if (isSuccess)
            {
                ToastBorder.Background = new SolidColorBrush(Color.FromRgb(220, 252, 231));
                ToastBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(134, 239, 172));

                ToastIconCircle.Background = new SolidColorBrush(Color.FromRgb(34, 197, 94));
                ToastIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Check;

                ToastTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(22, 101, 52));
                ToastMessageTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(20, 83, 45));
            }
            else
            {
                ToastBorder.Background = new SolidColorBrush(Color.FromRgb(254, 226, 226));
                ToastBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(252, 165, 165));

                ToastIconCircle.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                ToastIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Exclamation;

                ToastTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(153, 27, 27));
                ToastMessageTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(127, 29, 29));
            }

            Topmost = true;
            Activate();

            ToastBorder.Visibility = Visibility.Visible;

            _toastTimer.Start();

            Topmost = false;
        }

        private void ToastTimer_Tick(object? sender, EventArgs e)
        {
            _toastTimer.Stop();
            ToastBorder.Visibility = Visibility.Collapsed;
        }
    }
}