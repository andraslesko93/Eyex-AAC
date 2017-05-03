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
    using View;
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MessageMediumViewModel messageMediumViewModelObject;
        public MainWindow()
        {
            Console.WriteLine("--------------------");
            InitializeComponent();
        }
        private void MessageMediumViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            messageMediumViewModelObject = new MessageMediumViewModel();
            messageMediumViewModelObject.LoadMessageMediums();
            MessageMediumViewControl.DataContext = messageMediumViewModelObject;
            DataContext = messageMediumViewModelObject;
        }
        private void BasicMessageMediumViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            BasicMessageMediumViewModel messageMediumViewModelObject = new BasicMessageMediumViewModel();
            messageMediumViewModelObject.LoadBasicMessageMediums();
            BasicMessageMediumViewControl.DataContext = messageMediumViewModelObject;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
        private void Manage_Button_Click(object sender, RoutedEventArgs e)
        {
            ManageWindow manageWindow = new ManageWindow();
            manageWindow.ShowDialog();
        }

        private void Scroll_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            var hasGaze = stackPanel.GetHasGaze();
            if (hasGaze)
            {
                string type = (string)stackPanel.Tag;
                if (type == "next")
                {
                    messageMediumViewModelObject.NextPage();
                }
                else if (type == "previous")
                {
                    messageMediumViewModelObject.PreviousPage();
                }
            }
        }

    }
}
