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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Message = string.Empty;
        }

        public string Message { get; private set; }

        /// <summary>
        /// Handler for Behavior.HasGazeChanged events for the instruction text block.
        /// </summary>
        private void CardOnGaze(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(e.a);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                Close();
            }
            else if (e.Key == Key.C)
            {
                var model = (MainWindowModel) DataContext;
                model.CloseInstruction();
            }
        }

    }
}
