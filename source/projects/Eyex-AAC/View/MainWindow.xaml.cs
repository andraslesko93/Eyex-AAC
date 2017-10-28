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
    using System.Windows.Media;
    using System.Collections.Specialized;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MessengerViewModel messageMediumViewModelObject;

        private static readonly string UP_A_LEVEL_EVENT = "up";
        private static readonly string NEXT_PAGE_EVENT = "next";
        private static readonly string PREVIOUS_PAGE_EVENT = "previous";
        private static readonly string SENTENCE_MODE_EVENT = "sentenceMode";
        private static readonly string SAY_SENTENCE_EVENT = "saySentence";
        private static bool closing = false;
        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)SentenceListView.Items).CollectionChanged += SentenceListView_CollectionChanged;
        }
        private void MessageMediumViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            messageMediumViewModelObject = new MessengerViewModel();
            messageMediumViewModelObject.LoadMessengers();
            MessageMediumViewControl.DataContext = messageMediumViewModelObject;
            DataContext = messageMediumViewModelObject;
        }
        private void BasicMessageMediumViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            BasicMessageMediumViewModel messageMediumViewModelObject = new BasicMessageMediumViewModel();
            messageMediumViewModelObject.LoadBasicMessageMediums();
            BasicMessageMediumViewControl.DataContext = messageMediumViewModelObject;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (closing == false)
            {
                messageMediumViewModelObject.M2qttManager.Disconnect();
                closing = true;
                Application.Current.Shutdown();
            }
        }

       /* protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }*/
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
                if (type.Equals(NEXT_PAGE_EVENT))
                {
                    messageMediumViewModelObject.NextPage();
                }
                else if (type.Equals(PREVIOUS_PAGE_EVENT))
                {
                    messageMediumViewModelObject.PreviousPage();
                }

                else if (type.Equals(UP_A_LEVEL_EVENT))
                {
                    messageMediumViewModelObject.MoveUpALevel();
                }

                else if (type.Equals(SENTENCE_MODE_EVENT))
                {
                    messageMediumViewModelObject.ChangeSentenceMode();
                }
                else if (type.Equals(SAY_SENTENCE_EVENT))
                {
                    messageMediumViewModelObject.SaySentence();
                }
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            string type = (string)stackPanel.Tag;
            if (type.Equals(NEXT_PAGE_EVENT))
            {
                messageMediumViewModelObject.NextPage();
            }
            else if (type.Equals(PREVIOUS_PAGE_EVENT))
            {
                messageMediumViewModelObject.PreviousPage();
            }

            else if (type.Equals(UP_A_LEVEL_EVENT))
            {
                messageMediumViewModelObject.MoveUpALevel();
            }

            else if (type.Equals(UP_A_LEVEL_EVENT))
            {
                messageMediumViewModelObject.MoveUpALevel();
            }
            else if (type.Equals(SENTENCE_MODE_EVENT))
            {
                messageMediumViewModelObject.ChangeSentenceMode();
            }
            else if (type.Equals(SAY_SENTENCE_EVENT))
            {
                messageMediumViewModelObject.SaySentence();
            }
        }

        private void SentenceListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (SentenceListView.Items.Count > 0)
            {
                SentenceListView.ScrollIntoView(SentenceListView.Items[SentenceListView.Items.Count - 1]);
            }
        }

        private void SentenceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SentenceTextBox.CaretIndex = SentenceTextBox.Text.Length;
        }
    }
}
