using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EyexAAC.ViewModel.Utils
{
    class RenderManager
    {
        public int MaxRowCount { get; set; }
        public int MaxColumnCount { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public double WindowHeight { get; set; }
        public double WindowWidth { get; set; }
        public RenderManager()
        {
            WindowHeight = SystemParameters.PrimaryScreenHeight;
            WindowWidth = SystemParameters.PrimaryScreenWidth;
            ImageWidth = 193;
            ImageHeight = 163;
            MaxColumnCount = maxColumnCalculator();
            MaxRowCount = maxRowCalculator();
        }

        private int maxRowCalculator()
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            //Decrease the screenHeight by the bottom row's height
            screenHeight = screenHeight - (ImageHeight + 100);
            return (int)screenHeight / (ImageHeight + 100);
        }
        private int maxColumnCalculator()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            //Decrease by the 2 scroll buttom's width
            screenWidth -= 160;
            double result = screenWidth / (ImageWidth + 80);
            return (int)result;
        }
    }
}
