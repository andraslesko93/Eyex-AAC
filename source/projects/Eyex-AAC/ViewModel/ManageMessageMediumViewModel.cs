using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using EyexAAC.ViewModel.Utils;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.ComponentModel;

namespace EyexAAC.ViewModel
{
    class ManageMessageMediumViewModel : INotifyPropertyChanged
    {
        private MessageMedium _focusedMessageMedium;

        public event PropertyChangedEventHandler PropertyChanged;

        public static ObservableCollection<MessageMedium> MessageMediums { get; set; }

        public MessageMedium FocusedMessageMedium
        {
            get { return _focusedMessageMedium; }
            set
            {
                _focusedMessageMedium = value;
                RaisePropertyChanged("FocusedMessageMedium");
            }
        }

        public ManageMessageMediumViewModel()
        {
            MessageMediums = new ObservableCollection<MessageMedium>();
            FocusedMessageMedium = new MessageMedium();
            SetRootObjects();
        }

        public void SetChildren(MessageMedium messageMedium)
        {
            if (messageMedium.Type==MessageMediumType.root)
            {
                return;
            }

            messageMedium.Children = MessageMediumProxyUtil.GetChildren(messageMedium);
        }

        private void SetRootObjects()
        {
            MessageMedium TableRoot = new MessageMedium("Table message mediums", MessageMediumType.root);
            foreach (MessageMedium msg in MessageMediumProxyUtil.GetTableRootMessageMediums())
            {
                if (msg.Parent == null)
                {
                    TableRoot.AddChild(msg);
                }
            }
            MessageMediums.Add(TableRoot);

            MessageMedium BasicRoot = new MessageMedium("Basic message mediums", MessageMediumType.root);
            foreach (MessageMedium msg in MessageMediumProxyUtil.GetBasicMessageMediums())
            {
                //The parent of a basic message medium is always null.
                BasicRoot.AddChild(msg);
            }
            MessageMediums.Add(BasicRoot);
        }

        public void SetMessageMediumToFocus(MessageMedium messageMedium)
        {
            FocusedMessageMedium = messageMedium;
        }

        public void SaveFocusedMesageMedium()
        {
            if (FocusedMessageMedium.Name != null)
            {
                //Change in the UI.
                if (FocusedMessageMedium.Type == MessageMediumType.table)
                {
                    MessageMedium tableMessageMedium = MessageMediumViewModel.MessageMediums.SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);

                    if(tableMessageMedium != null)
                    {
                        //It is not a subtype
                        tableMessageMedium.Name = FocusedMessageMedium.Name;
                        tableMessageMedium.Image = FocusedMessageMedium.Image;
                    }
                    

                }
                else if(FocusedMessageMedium.Type == MessageMediumType.basic)
                {
                    MessageMedium basicMessageMedium = BasicMessageMediumViewModel.BasicMessageMediums.SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);
                    basicMessageMedium.Name = FocusedMessageMedium.Name;
                    basicMessageMedium.Image = FocusedMessageMedium.Image;
                }
                //Save to db.
                using (var context = new MessageMediumContext())
                {
                    var result = context.MessageMediums.SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);
                    result.Name = FocusedMessageMedium.Name;
                    result.Image = FocusedMessageMedium.Image;
                    context.SaveChanges();
                }
            }
            
        }

        public void DeleteFocusedMesageMedium()
        {
            
            //FocusedMessageMedium.Parent.Children.Remove(FocusedMessageMedium);
            //MessageMedium parent = MessageMediums.SingleOrDefault(i => i.Id == FocusedMessageMedium.Parent.Id);
           // parent.Children.Remove(FocusedMessageMedium);

            if (FocusedMessageMedium.Type == MessageMediumType.table)
            {
                MessageMediumViewModel.MessageMediums.Remove(MessageMediumViewModel.MessageMediums.SingleOrDefault(i => i.Id == FocusedMessageMedium.Id));
            }
            else if (FocusedMessageMedium.Type == MessageMediumType.basic)
            {
                BasicMessageMediumViewModel.BasicMessageMediums.Remove(BasicMessageMediumViewModel.BasicMessageMediums.SingleOrDefault(i => i.Id == FocusedMessageMedium.Id));
            }
            using (var context = new MessageMediumContext())
            {
                var result = context.MessageMediums.SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);
                DeleteChildrenFromDB(result.Children);
                context.MessageMediums.Remove(result);
                context.SaveChanges();
            }
            FocusedMessageMedium = null;
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void DeleteChildrenFromDB(List<MessageMedium> messageMediumList)
        {
            if (messageMediumList == null)
            {
                return;
            }
            foreach (MessageMedium msg in messageMediumList)
            {
                DeleteChildrenFromDB(msg.Children);
                using (var context = new MessageMediumContext())
                {
                    var result = context.MessageMediums.SingleOrDefault(c => c.Id == msg.Id);
                    context.MessageMediums.Remove(result);
                    context.SaveChanges();
                }
            }
        }

        private MessageMedium DFS(List<MessageMedium> messageMediumList, int id)
        {
            if (messageMediumList == null)
            {
                return null;
            }
            foreach (MessageMedium msg in messageMediumList)
            {
                if (msg.Id == id)
                {
                    return msg;
                }
                DFS(msg.Children, id);
            }
            return null;
        }
    }
}
