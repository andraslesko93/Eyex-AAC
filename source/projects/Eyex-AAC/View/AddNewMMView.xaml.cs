using EyexAAC.ViewModel;
using Microsoft.Win32;
using System.Windows;


namespace EyexAAC.View
{
    /// <summary>
    /// Interaction logic for AddNew.xaml
    /// </summary>
    public partial class AddNewMMView : Window
    {
        private MessageMediumViewModel messageMediumViewModel = 
            new MessageMediumViewModel();

        public AddNewMMView()
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
            int returnCode = messageMediumViewModel.addNewMessageMedium(name.Text, comboBox.Text, filePath.Text);
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
