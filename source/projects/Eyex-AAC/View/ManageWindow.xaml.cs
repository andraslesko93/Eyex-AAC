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
        ManageMessageMediumViewModel manageMessageMediumViewModel;
        public ManageWindow()
        {
            manageMessageMediumViewModel = new ManageMessageMediumViewModel();
            DataContext = manageMessageMediumViewModel;
            InitializeComponent();
        }
        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = e.OriginalSource as TreeViewItem;     
            if (treeViewItem != null)
            {
                MessageMedium messageMedium = treeViewItem.Header as MessageMedium;
                manageMessageMediumViewModel.SetChildren(messageMedium);
            }
        }

        private void treeView_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = e.OriginalSource as TreeViewItem;
            if (treeViewItem != null)
            {
                MessageMedium messageMedium = treeViewItem.Header as MessageMedium;
                if (messageMedium.Type == MessageMediumType.root)
                {
                    EditorGrid.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    EditorGrid.Visibility = System.Windows.Visibility.Visible; 
                }
                manageMessageMediumViewModel.SetMessageMediumToFocus(messageMedium);
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            manageMessageMediumViewModel.SaveFocusedMesageMedium();
            InfoMessage.Content = "Successfully saved.";
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
            if (manageMessageMediumViewModel.IsFocusMessageMediumSetted())
            {
                EditorGrid.Visibility = System.Windows.Visibility.Visible;
                manageMessageMediumViewModel.AddChildToFocusedMessageMedium();
                InfoMessage.Content = "";
            }
            else
            {
                InfoMessage.Content = "Select a parent item first.";
            }

        }
    }
}
