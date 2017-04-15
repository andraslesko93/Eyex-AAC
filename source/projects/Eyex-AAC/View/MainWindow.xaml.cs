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
        private MessageMediumContext _messageMediumContext = new MessageMediumContext();
        public MainWindow()
        {
            InitializeComponent();
           /* using (var ctx = new MessageMediumContext())
            {
                MessageMedium asd = new MessageMedium("no", LoadImage("no.jpg"));
                ctx.MessageMediums.Add(asd);
                ctx.SaveChanges();
            }*/
            List<MessageMedium> MessageMediumsToDisplay;
            using (var ctx = new MessageMediumContext())
            {
                var msgs = ctx.MessageMediums.Where(b => b.Name == "no").ToList();
                MessageMediumsToDisplay = msgs;

                foreach (MessageMedium messageMedium in MessageMediumsToDisplay)
                {
                    if (messageMedium.ImageAsByte != null)
                    {
                        messageMedium.initializeImage();
                    }
                }
            }
            MessageMediums.ItemsSource = MessageMediumsToDisplay;


            /* this.FamilyMessageMediums.ItemsSource = new MessageMedium[]
             {
             new MessageMedium{Name="no", ImageData=LoadImage("no.jpg")},
             new MessageMedium{Name="yes", ImageData=LoadImage("yes.jpg")},
             new MessageMedium{Name="yes", ImageData=LoadImage("yes.jpg")},
             new MessageMedium{Name="Card 6", ImageData=LoadImage("newspaper.jpg")}
             };
             this.BasicMessageMediums.ItemsSource = new MessageMedium[]
             {
             new MessageMedium{Name="no", ImageData=LoadImage("no.jpg")},
             new MessageMedium{Name="yes", ImageData=LoadImage("yes.jpg")},
             new MessageMedium{Name="nachos", ImageData=LoadImage("nachos.jpg")},
             new MessageMedium{Name="newspaper", ImageData=LoadImage("newspaper.jpg")},
             };
 */
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
