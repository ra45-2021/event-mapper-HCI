using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WorldEventMapper.EventTags;
using WorldEventMapper.EventTypes;
using WorldEventMapper.Help;
using WorldEventMapper.Home;
using WorldEventMapper.Models;
using WorldEventMapper.ViewModels;

namespace WorldEventMapper.Events
{
    public partial class Events_Main : Window
    {
        private MainDataViewModel ViewModel => (MainDataViewModel)DataContext;

        public Events_Main()
        {
            InitializeComponent();
            DataContext = new MainDataViewModel();
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
            Events_Add addWindow = new Events_Add();
            addWindow.Show();

            Close();
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

            MessageBox.Show($"Edit event: {selectedEvent.Name}", "Edit Event");
        }

        private void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            EventModel? selectedEvent = GetEventFromButton(sender);

            if (selectedEvent == null)
                return;

            MessageBox.Show($"Delete event: {selectedEvent.Name}", "Delete Event");
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
    }
}