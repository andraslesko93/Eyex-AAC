using EyexAAC.ViewModel;
using System.Windows;
using System;

namespace EyexAAC.View
{
    /// <summary>
    /// Interaction logic for Authenticate.xaml
    /// </summary>
    public partial class AuthenticaWindow : Window
    {
        private SessionViewModel sessionViewModel;
        private bool loggedIn = false;
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
                loggedIn = true;
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (loggedIn == false)
            {
                Application.Current.Shutdown();
            } 
        }
    }
}
