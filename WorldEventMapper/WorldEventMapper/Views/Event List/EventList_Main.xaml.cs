using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WorldEventMapper.Event_Management;
using WorldEventMapper.Help;
using WorldEventMapper.Home;
using WorldEventMapper.ViewModels;

namespace WorldEventMapper.Event_List
{
    public partial class EventList_Main : Window
    {
        private MainDataViewModel ViewModel => (MainDataViewModel)DataContext;

        public EventList_Main()
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

        private void EventManagement_Click(object sender, RoutedEventArgs e)
        {
            EventManagement_Main eventManagementWindow = new EventManagement_Main();
            eventManagementWindow.Show();

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

            AdvancedSearchPopup.IsOpen = false;
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

            AdvancedSearchPopup.IsOpen = false;
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

        private static void UpdateBinding(FrameworkElement element, DependencyProperty property)
        {
            BindingExpression? binding = element.GetBindingExpression(property);
            binding?.UpdateSource();
        }
    }
}