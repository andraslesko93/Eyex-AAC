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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MessageMediumViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            MessageMediumViewModel messageMediumViewModelObject = new MessageMediumViewModel();
            messageMediumViewModelObject.LoadMessageMediums();
            MessageMediumViewControl.DataContext = messageMediumViewModelObject;
        }
        private void FamilyMessageMediumViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            FamilyMessageMediumViewModel messageMediumViewModelObject = new FamilyMessageMediumViewModel();
            messageMediumViewModelObject.LoadFamilyMessageMediums();
            FamilyMessageMediumViewControl.DataContext = messageMediumViewModelObject;
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

        }

        private void Add_New_Button_Click(object sender, RoutedEventArgs e)
        {
            AddNewMMView addMessageMedium = new AddNewMMView();
            addMessageMedium.ShowDialog();
        }
    }
}
