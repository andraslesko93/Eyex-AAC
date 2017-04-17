using EyexAAC.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AddNew.xaml
    /// </summary>
    public partial class AddNewMessageMedium : Window
    {
        private AddNewMessageMediumViewModel addNewMessageMediumViewModel = 
            new AddNewMessageMediumViewModel();

        public AddNewMessageMedium()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Jpg files (*.jpg, *.jpeg) | *.jpg; *.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                filePath.Text = filename;
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int returnCode = addNewMessageMediumViewModel.addNewMessageMedium(name.Text, comboBox.Text, filePath.Text);
            if (returnCode > 0)
            {
                infoMessage.Content= "Successfully saved a message medium.";
            }
            else
            {
                infoMessage.Content = "An error has occured, the message medium is not saved.";
            }
        }
    }

}
