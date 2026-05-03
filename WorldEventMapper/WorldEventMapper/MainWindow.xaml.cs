using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorldEventMapper.Help;
using WorldEventMapper.Home;

namespace WorldEventMapper
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoToHomePage_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new HomePage();
        }

        private void GoToHelpPage_Click(object sender, RoutedEventArgs e)
        {
            Help_Main helpWindow = new Help_Main();
            helpWindow.Show();

            this.Close();
        }
    }
}