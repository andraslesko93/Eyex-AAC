using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace EyexAAC.Model
{
    class Messenger: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private BitmapImage image;
        private byte[] imageAsByte;
        private ObservableCollection<Messenger> children;
        private Messenger parent;
        private string encodedImage;

        public int Id { get; set; }
        [JsonIgnore]
        public byte[] ImageAsByte
        {
            get { return imageAsByte; }
            set
            {
                imageAsByte = value;
                if (Image == null && value!=null)
                {
                    Image = ByteToBitmapImage(ImageAsByte);
                }
                if (EncodedImage == null && value != null)
                {
                    EncodedImage = Convert.ToBase64String(value);
                }

            }
        }
        [NotMapped]
        public string EncodedImage {
            get { return encodedImage; }
            set
            {
                encodedImage = value;
                if (ImageAsByte == null && value != null)
                {
                    ImageAsByte=Convert.FromBase64String(value);
                }
            }
        }

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
        [JsonIgnore]
        public BitmapImage Image
        {
            get{ return image; }
            set
            {
                image = value;
                if (BitmapImageToByte(image) != ImageAsByte)
                {
                    ImageAsByte = BitmapImageToByte(image);
                }
                RaisePropertyChanged("Image");
            }
        }
        [JsonIgnore]
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
                if (value != null)
                { 
                    children = value;
                    foreach (Messenger chid in value) {
                        chid.parent = this;
                    }
                    RaisePropertyChanged("Children");
                }
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

        public void AddChild(Messenger messenger)
        {
            messenger.Parent = this;
            if (Children == null) { Children = new ObservableCollection<Messenger>(); }
            Children.Add(messenger);
            HasChild = true;
            RaisePropertyChanged("Children");
            RaisePropertyChanged("HasChild");
        }

        public void RemoveChild(Messenger messenger) {
            Children.Remove(messenger);

            if (Children!=null && !Children.Any())
            {
                HasChild = false;
                RaisePropertyChanged("HasChild");
            }
            RaisePropertyChanged("Children");
        }

        public Messenger Copy()
        {
            Messenger messenger = new Messenger();
            messenger.Children = Children;
            messenger.HasChild = HasChild;
            messenger.Image = Image;
            messenger.ImageAsByte = ImageAsByte;
            messenger.Name = Name;
            messenger.Parent = Parent;
            messenger.Type = Type;
            return messenger;
        }
    }

    enum MessengerType {
        general=0, //default value
        pinned,
        root
    };
}
