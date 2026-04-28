using System.Windows;
using WorldEventMapper.Event_List;
using WorldEventMapper.Event_Management;
using WorldEventMapper.Home;

namespace WorldEventMapper.Help
{
    public partial class Help_Main : Window
    {
        public Help_Main()
        {
            InitializeComponent();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.Content = new HomePage();

            this.Close();
        }

        private void EventList_Click(object sender, RoutedEventArgs e)
        {
            EventList_Main eventListWindow = new EventList_Main();
            eventListWindow.Show();

            this.Close();
        }

        private void EventManagement_Click(object sender, RoutedEventArgs e)
        {
            EventManagement_Main eventManagementWindow = new EventManagement_Main();
            eventManagementWindow.Show();

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