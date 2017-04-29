using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace EyexAAC.Model
{
    class MessageMedium: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private BitmapImage image;

        public int Id { get; set; }
        public byte[] ImageAsByte { get; set; }
        public string Type { get; set; }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        [NotMapped]
        public BitmapImage Image
        {
            get{ return image; }
            set
            {
                image = value;
                RaisePropertyChanged("Image");
            }
        }

        public MessageMedium Parent { get; set; }

        public List<MessageMedium> Children { get; set; }

        public MessageMedium(){}

        public MessageMedium(string name, string image)
        {
            Name = name;
            Image = LoadImage(image);
            ImageAsByte = BitmapImageToByte(Image);
            Children = new List<MessageMedium>();
            Type = "default";
        }

        public MessageMedium(string name, string image, string type)
        {
            Name = name;
            Image = LoadImage(image);
            ImageAsByte = BitmapImageToByte(Image);
            Children = new List<MessageMedium>();
            Type = type;
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
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public void AddChild(MessageMedium messageMedium)
        {
            messageMedium.Parent = this;
            Children.Add(messageMedium);
        }
    }
}
