using EyexAAC.Model;
using EyexAAC.ViewModel;
using Microsoft.Win32;
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
    /// Interaction logic for ManageWindow.xaml
    /// </summary>
    public partial class ManageWindow : Window
    {
        ManageViewModel manageMessageMediumViewModel;
        public ManageWindow()
        {
            manageMessageMediumViewModel = new ManageViewModel();
            DataContext = manageMessageMediumViewModel;
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
                    EditorGrid.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    EditorGrid.Visibility = System.Windows.Visibility.Visible; 
                }
                manageMessageMediumViewModel.SetMessageMediumToFocus(messenger);
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            Boolean result = manageMessageMediumViewModel.SaveFocusedMessenger();
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
            manageMessageMediumViewModel.DeleteFocusedMesageMedium();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (manageMessageMediumViewModel.FocusedMessenger!=null)
            {
                EditorGrid.Visibility = System.Windows.Visibility.Visible;
                manageMessageMediumViewModel.AddChildToFocusedMessenger();
                InfoMessage.Content = "";
            }
            else
            {
                InfoMessage.Content = "Select a parent item first.";
            }

        }

        private void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            manageMessageMediumViewModel.Connect(Password.SecurePassword);
        }
    }
}
