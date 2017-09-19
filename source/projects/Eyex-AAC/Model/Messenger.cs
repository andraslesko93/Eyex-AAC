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
using System.Collections.ObjectModel;

namespace EyexAAC.Model
{
    class Messenger: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private BitmapImage image;
        private ObservableCollection<Messenger> children;
        private Messenger parent;

        public int Id { get; set; }
        public byte[] ImageAsByte { get; set; }

        public bool HasChild { get; set; }
        public MessengerType Type { get; set; }
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
                ImageAsByte = BitmapImageToByte(image);
                RaisePropertyChanged("Image");
            }
        }

        public Messenger Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                RaisePropertyChanged("Parent");
            }
        }

        public ObservableCollection<Messenger> Children
        {
            get { return children; }
            set
            {
                children = value;
                RaisePropertyChanged("Children");
            }
        }

        public Messenger(){}

        public Messenger(string name, string image)
        {
            Name = name;
            Image = LoadImage(image);
            ImageAsByte = BitmapImageToByte(Image);
            Children = new ObservableCollection<Messenger>();
            HasChild = false;
        }

        public Messenger(string name, string image, MessengerType type)
        {
            Name = name;
            Image = LoadImage(image);
            ImageAsByte = BitmapImageToByte(Image);
            Children = new ObservableCollection<Messenger>();
            Type = type;
            HasChild = false;
        }
        public Messenger(string name, MessengerType type)
        {
            Name = name;
            Type = type;
            Children = new ObservableCollection<Messenger>();
            HasChild = false;
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

        public void AddChild(Messenger messageMedium)
        {
            messageMedium.Parent = this;
            Children.Add(messageMedium);
            HasChild = true;
            RaisePropertyChanged("Children");
            RaisePropertyChanged("HasChild");
        }

        public Messenger Copy()
        {
            Messenger msg = new Messenger();
            msg.Children = Children;
            msg.HasChild = HasChild;
            msg.Image = Image;
            msg.ImageAsByte = ImageAsByte;
            msg.Name = Name;
            msg.Parent = Parent;
            msg.Type = Type;
            


            return msg;
        }
    }

    enum MessengerType {
        table=0, //default value
        basic,
        goBack,
        root
    };
}
