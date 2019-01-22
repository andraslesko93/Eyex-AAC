using EyexAAC.Model;
using EyexAAC.ViewModel;
using EyexAAC.ViewModel.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace EyexAAC.View
{
    /// <summary>
    /// Interaction logic for ManageWindow.xaml
    /// </summary>
    public partial class ManageWindow : Window
    {
        ManageViewModel manageViewModel;
        public ManageWindow()
        {
            manageViewModel = new ManageViewModel();
            DataContext = manageViewModel;
            InitializeComponent();
        }

        private void treeView_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = e.OriginalSource as TreeViewItem;
            if (treeViewItem != null)
            {
                Messenger messenger = treeViewItem.Header as Messenger;
                if (messenger.Type == MessengerType.root)
                {
                    EditorGrid.Visibility = Visibility.Hidden;
                }
                else
                {
                    EditorGrid.Visibility = Visibility.Visible; 
                }
                manageViewModel.SetMessengerToFocus(messenger);
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            bool result = manageViewModel.SaveFocusedMessenger();
            if (result == true)
            {
                InfoMessage.Content = "Successfully saved.";
            }
            else
            {
                InfoMessage.Content = "You must fill all attributes.";
            }
           
        }

        private void Change_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Jpg files (*.jpg, *.jpeg) | *.jpg; *.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                Image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            manageViewModel.DeleteFocusedMesageMedium();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (manageViewModel.FocusedMessenger!=null)
            {
                EditorGrid.Visibility = System.Windows.Visibility.Visible;
                manageViewModel.AddChildToFocusedMessenger();
                InfoMessage.Content = "";
            }
            else
            {
                InfoMessage.Content = "Select a parent item first.";
            }

        }

        private void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            manageViewModel.Connect(Password.Password);
        }
        private void Save_User_Button_Click(object sender, RoutedEventArgs e)
        {
            int rowCount;
            if (string.IsNullOrEmpty(RowCount.Text) || !int.TryParse(RowCount.Text, out rowCount) || rowCount > 3 || rowCount < 1) {
                AppearanceStateMessage.Content = "Sorok száma minimum 1, maximum 3.";
                return;
            }
            int columnCount;
            if (string.IsNullOrEmpty(ColumnCount.Text) || !int.TryParse(ColumnCount.Text, out columnCount) || columnCount > 10 || columnCount < 1)
            {
                AppearanceStateMessage.Content = "Oszolopok száma minimum 1, maximum 10.";
                return;
            }

            manageViewModel.SaveUserAppearanceConnectionData();
            AppearanceStateMessage.Content = "Sikeres mentés!\nA változások érvénybe lépéséhez újra kell indítani az alkalmazást.";
        }

        private void Disconnect_Button_Click(object sender, RoutedEventArgs e)
        {
            manageViewModel.Disconnect();
        }

        private void Start_Sharing_Button_Click(object sender, RoutedEventArgs e)
        {
            manageViewModel.ShareMessengers();
        }
        private void Leave_Sharing_Session_Button_Click(object sender, RoutedEventArgs e)
        {
            manageViewModel.LeaveSharingSession();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
