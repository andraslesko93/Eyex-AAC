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
    /// Interaction logic for PinnedMessengerView.xaml
    /// </summary>
    public partial class PinnedMessengerView : UserControl
    {
        private PinnedMessengerViewModel pinnedMessengerViewModel = new PinnedMessengerViewModel();
        public PinnedMessengerView()
        {
            InitializeComponent();
        }
        private void Messenger_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            var hasGaze = stackPanel.GetHasGaze();
            if (hasGaze)
            {
                pinnedMessengerViewModel.performActionOnPinnedMessengers((int)stackPanel.Tag);
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            pinnedMessengerViewModel.performActionOnPinnedMessengers((int)stackPanel.Tag);

        }

        private void Scroll_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            var hasGaze = stackPanel.GetHasGaze();
            var currentApp = Application.Current as App;
            if (hasGaze)
            {
                if (currentApp != null) currentApp.EyeXHost.TriggerPanningBegin();
            }
            else { if (currentApp != null) currentApp.EyeXHost.TriggerPanningEnd(); }
        }
    }
}
