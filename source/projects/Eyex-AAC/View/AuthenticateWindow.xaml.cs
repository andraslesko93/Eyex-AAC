using EyexAAC.ViewModel;
using System.Windows;

namespace EyexAAC.View
{
    /// <summary>
    /// Interaction logic for Authenticate.xaml
    /// </summary>
    public partial class AuthenticaWindow : Window
    {
        private SessionViewModel sessionViewModel;
        public AuthenticaWindow()
        {
            InitializeComponent();
            sessionViewModel = new SessionViewModel();
            DataContext = sessionViewModel;
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sessionViewModel.Login())
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
        }
    }
}
