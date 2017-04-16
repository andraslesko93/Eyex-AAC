//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

namespace EyexAAC
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using EyeXFramework.Wpf;
    using System;
    using EyexAAC.ViewModel;
    using EyexAAC.Model;
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;
    using System.Data.Entity;
    using System.Linq;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowsViewModel mainWindowViewModel = new MainWindowsViewModel();
        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel.AddInitData();
            MessageMediums.ItemsSource = mainWindowViewModel.GetMessageMediums();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void MessegaMedium_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            var hasGaze = stackPanel.GetHasGaze();
            if (hasGaze)
            {
                MessageMedium messageMedium = mainWindowViewModel.GetMessageMediumById((int)stackPanel.Tag);
                if (messageMedium.IsItMeta)
                {
                    //TODO: Store items
                    MessageMediums.ItemsSource = mainWindowViewModel.GetMetaMessageMediumList(messageMedium);
                }
                else
                {
                    Console.WriteLine(messageMedium.Name);
                }
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
