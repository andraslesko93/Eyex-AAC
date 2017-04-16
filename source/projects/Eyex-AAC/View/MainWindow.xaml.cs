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
    using View;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowsViewModel mainWindowViewModel = new MainWindowsViewModel();
        List<MessageMedium> MessageMediumList;
        List<MessageMedium> FamilyMessageMediumList;
        List<MessageMedium> BasicMessageMediumList;

        public MainWindow()
        {
            InitializeComponent();

            MessageMediumList = mainWindowViewModel.GetMessageMediums();
            FamilyMessageMediumList = mainWindowViewModel.GetFamilyMessageMediums();
            BasicMessageMediumList = mainWindowViewModel.GetBasicMessageMediums();

            MessageMediums.ItemsSource = MessageMediumList;
            FamilyMessageMediums.ItemsSource = FamilyMessageMediumList;
            BasicMessageMediums.ItemsSource = BasicMessageMediumList;
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
                if (messageMedium.Action == "meta")
                {
                    MessageMediums.ItemsSource = mainWindowViewModel.GetMetaMessageMediumList(messageMedium);
                }
                else if (messageMedium.Action == "goBack")
                {
                    MessageMediums.ItemsSource = MessageMediumList;
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

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Add_New_Button_Click(object sender, RoutedEventArgs e)
        {
            AddNewMessageMedium addMessageMedium = new AddNewMessageMedium(this);
            addMessageMedium.ShowDialog();
        }

        private void MessageMediums_Loaded(object sender, RoutedEventArgs e)
        {
            /*MainWindowsViewModel Model = new MainWindowsViewModel();
            Model.MessageMediums = Model.GetMessageMediums();*/
            
        }
    }
}
