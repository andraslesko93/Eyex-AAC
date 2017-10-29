using EyexAAC.ViewModel;
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

namespace EyexAAC.View
{
    /// <summary>
    /// Interaction logic for Authenticate.xaml
    /// </summary>
    public partial class AuthenticaWindow : Window
    {
        private UserViewModel userViewModel;
        public AuthenticaWindow()
        {
            InitializeComponent();
            userViewModel = new UserViewModel();
            DataContext = userViewModel;
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            if (userViewModel.Login())
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
        }
    }
}
