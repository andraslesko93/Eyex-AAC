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
        //public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public double WindowHeight { get; set; }
        public double WindowWidth { get; set; }
        public RenderManager()
        {
            WindowHeight = SystemParameters.PrimaryScreenHeight;
            WindowWidth = SystemParameters.PrimaryScreenWidth;
            MaxColumnCount = SessionViewModel.User.MaxColumnCount;
            MaxRowCount = SessionViewModel.User.MaxRowCount;
            ImageHeight = maxImageHeightCalculator();
        }

        private int maxImageHeightCalculator()
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            double screenHeightWithoutPinnedMessengers = screenHeight - 300;
            double pixelsForASingleRow = screenHeightWithoutPinnedMessengers / MaxRowCount;
            double pixelsForPicture = pixelsForASingleRow - 150;
            return (int)pixelsForPicture;
        }
    }
}
