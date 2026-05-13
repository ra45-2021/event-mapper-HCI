using System.Windows;
using WorldEventMapper.Home;
using WorldEventMapper.Events;
using WorldEventMapper.EventTags;
using WorldEventMapper.EventTypes;

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

        private void Events_Click(object sender, RoutedEventArgs e)
        {
            Events_Main eventsWindow = new Events_Main();
            eventsWindow.Show();

            this.Close();
        }

        private void EventTypes_Click(object sender, RoutedEventArgs e)
        {
            EventTypes_Main eventTypesWindow = new EventTypes_Main();
            eventTypesWindow.Show();

            this.Close();
        }

        private void EventTags_Click(object sender, RoutedEventArgs e)
        {
            EventTags_Main eventTagsWindow = new EventTags_Main();
            eventTagsWindow.Show();

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