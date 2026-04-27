using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WorldEventMapper.Event_Management
{
    public partial class EventManagement_Main : Window
    {
        public EventManagement_Main()
        {
            InitializeComponent();
        }

        private void CloseToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
        }

        private void EventList_Click(object sender, RoutedEventArgs e)
        {
MainWindow mainWindow = new MainWindow();   
        }
    }
}
