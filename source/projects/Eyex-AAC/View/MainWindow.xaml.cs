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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.MessageMediums.ItemsSource = new MessageMedium[]
            {
            new MessageMedium{Name="no", ImageData=LoadImage("no.jpg")},
            new MessageMedium{Name="yes", ImageData=LoadImage("yes.jpg")},
            new MessageMedium{Name="nachos", ImageData=LoadImage("nachos.jpg")},
            new MessageMedium{Name="newspaper", ImageData=LoadImage("newspaper.jpg")},
          /*  new MessageMedium{Name="Card 5", ImageData=LoadImage("cat.jpg")},
            new MessageMedium{Name="Card 6", ImageData=LoadImage("cat.jpg")}*/
            };  
        }
        private void MessegaMedium_OnHasGazeChanged(object sender, RoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            var hasGaze = stackPanel.GetHasGaze();
            if (hasGaze)
            {
                Console.WriteLine(stackPanel.Tag);
            }
        }

        private BitmapImage LoadImage(string filename)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/" + filename));
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
