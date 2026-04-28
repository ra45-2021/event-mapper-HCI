using System.Windows;
using WorldEventMapper.Event_Management;
using WorldEventMapper.Home;
using WorldEventMapper.ViewModels;
using WorldEventMapper.Help;

namespace WorldEventMapper.Event_List
{
    public partial class EventList_Main : Window
    {
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

            this.Close();
        }

        private void EventManagement_Click(object sender, RoutedEventArgs e)
        {
            EventManagement_Main eventManagementWindow = new EventManagement_Main();
            eventManagementWindow.Show();

            this.Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help_Main helpWindow = new Help_Main();
            helpWindow.Show();

            this.Close();
        }

        private void CloseToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();
        }
    }
}