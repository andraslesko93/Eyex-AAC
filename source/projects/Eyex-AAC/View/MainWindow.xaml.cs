//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

namespace EyexAAC
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using EyeXFramework.Wpf;
    using ViewModel;
    using View;
    using System.ComponentModel;
    using System.Collections.Specialized;
    using ViewModel.Utils;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MessengerViewModel messengerViewModel;

        private static readonly string UP_A_LEVEL_EVENT = "up";
        private static readonly string NEXT_PAGE_EVENT = "next";
        private static readonly string PREVIOUS_PAGE_EVENT = "previous";
        private static readonly string SENTENCE_MODE_EVENT = "sentenceMode";
        private static readonly string SAY_SENTENCE_EVENT = "saySentence";
        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)SentenceListView.Items).CollectionChanged += SentenceListView_CollectionChanged;
            ActivityLogManager.LoadUnsentLog();
        }
        private void MessengerViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            messengerViewModel = new MessengerViewModel();
            messengerViewModel.LoadMessengers();
            MessengerViewControl.DataContext = messengerViewModel;
            DataContext = messengerViewModel;
        }
        private void PinnedMessengerViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            PinnedMessengerViewModel pinnedMessengerViewModelObject = new PinnedMessengerViewModel();
            pinnedMessengerViewModelObject.LoadPinnedMessengers();
            PinnedMessengerViewControl.DataContext = pinnedMessengerViewModelObject;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ManageViewModel.M2qttManager != null)
            { 
                ManageViewModel.M2qttManager.Disconnect();
            }
            if (ManageViewModel.IsSharingSession == true) {
                string messageBoxText = "If you close the application while in sharing mode, all shared messengers will be discarded, do you want to save them?";
                MessageBoxResult result = MessageBox.Show(messageBoxText, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DatabaseContextUtility.SaveMessengers(PageManager.Instance.MessengerCache);
                }
            }
            ActivityLogManager.SendActivityLog();
            ActivityLogManager.SaveActivityLog();
            Application.Current.Shutdown();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
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
                if (type.Equals(NEXT_PAGE_EVENT))
                {
                    messengerViewModel.NextPage();
                }
                else if (type.Equals(PREVIOUS_PAGE_EVENT))
                {
                    messengerViewModel.PreviousPage();
                }

                else if (type.Equals(UP_A_LEVEL_EVENT))
                {
                    messengerViewModel.MoveUpALevel();
                }

                else if (type.Equals(SENTENCE_MODE_EVENT))
                {
                    messengerViewModel.ChangeSentenceMode();
                }
                else if (type.Equals(SAY_SENTENCE_EVENT))
                {
                    messengerViewModel.SaySentence();
                }
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            string type = (string)stackPanel.Tag;
            if (type.Equals(NEXT_PAGE_EVENT))
            {
                messengerViewModel.NextPage();
            }
            else if (type.Equals(PREVIOUS_PAGE_EVENT))
            {
                messengerViewModel.PreviousPage();
            }

            else if (type.Equals(UP_A_LEVEL_EVENT))
            {
                messengerViewModel.MoveUpALevel();
            }

            else if (type.Equals(UP_A_LEVEL_EVENT))
            {
                messengerViewModel.MoveUpALevel();
            }
            else if (type.Equals(SENTENCE_MODE_EVENT))
            {
                messengerViewModel.ChangeSentenceMode();
            }
            else if (type.Equals(SAY_SENTENCE_EVENT))
            {
                messengerViewModel.SaySentence();
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
