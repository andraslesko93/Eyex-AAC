using EyexAAC.ViewModel;
using EyeXFramework.Wpf;
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
    /// Interaction logic for FamilyMessageMediumView.xaml
    /// </summary>
    public partial class FamilyMessageMediumView : UserControl
    {
        private FamilyMessageMediumViewModel messageMediumViewModel = new FamilyMessageMediumViewModel();
        public FamilyMessageMediumView()
        {
            InitializeComponent();
        }
        private void MessegaMedium_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            var hasGaze = stackPanel.GetHasGaze();
            if (hasGaze)
            {
                messageMediumViewModel.performActionOnFamilyMessageMedium((int)stackPanel.Tag);
            }
        }
    }
}
