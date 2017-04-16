using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

namespace EyexAAC.Model
{
    class MessageMedium
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] ImageAsByte { get; set; }
        [NotMapped]
        public BitmapImage Image { get; set; }
        public bool IsSubMessage { get; set; } //main or sub
        public string Type { get; set; }
        public string Action { get; set; }
        public MessageMedium(){}
        public MessageMedium(string name, string image)
        {
            this.Name = name;
            this.Image = LoadImage(image);
            this.ImageAsByte = BitmapImageToByte(Image);
            this.IsSubMessage = false;
            this.Type = "default";
        }
        public void InitializeImage()
        {
            if (ImageAsByte != null)
            {
                Image = ByteToBitmapImage(ImageAsByte);
            }
        }

        public byte[] BitmapImageToByte(BitmapImage image)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(memStream);
            return memStream.ToArray();
        }
        public BitmapImage ByteToBitmapImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
        private BitmapImage LoadImage(string filename)
        {
            return new BitmapImage(new Uri(filename));
        }
    }
}
