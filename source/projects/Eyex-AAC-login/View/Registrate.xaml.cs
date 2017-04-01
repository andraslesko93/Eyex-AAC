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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EyexAAC.View
{
    /// <summary>
    /// Interaction logic for Registrate.xaml
    /// </summary>
    public partial class Registrate : Page
    {
        public Registrate()
        {
            InitializeComponent();
        }

        private void Registrate_Button_Click (object sender, RoutedEventArgs e)
        {
            if (ViewModel.Authentication.Registrate(username.Text, password.Password, confirmPassword.Password))
            {
                Uri uri = new Uri("View/Login.xaml", UriKind.Relative);
                this.NavigationService.Navigate(uri);
            }

        }
    }
}
