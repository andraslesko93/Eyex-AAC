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
        private static MessageMedium _focusedMessageMedium;

        private static MessageMedium TableRoot;
        private static MessageMedium BasicRoot;

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
            TableRoot =  new MessageMedium("Table message mediums", MessageMediumType.root);
            BasicRoot =  new MessageMedium("Basic message mediums", MessageMediumType.root);
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
            foreach (MessageMedium msg in MessageMediumProxyUtil.GetTableRootMessageMediums())
            {
                 TableRoot.AddChild(msg);
            }
            MessageMediums.Add(TableRoot);

            foreach (MessageMedium msg in MessageMediumProxyUtil.GetBasicMessageMediums())
            {
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
                        //It is not a subtype of meta object on the communication table, so we can access to it.
                        tableMessageMedium.Name = FocusedMessageMedium.Name;
                        tableMessageMedium.Image = FocusedMessageMedium.Image;
                    }
                    else
                    {
                        if (FocusedMessageMedium.Parent.Type == MessageMediumType.root)
                        {
                            MessageMediumViewModel.MessageMediums.Add(FocusedMessageMedium);
                        }
                    }
                }
                else if(FocusedMessageMedium.Type == MessageMediumType.basic)
                {
                    MessageMedium basicMessageMedium = BasicMessageMediumViewModel.BasicMessageMediums.SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);

                    if (basicMessageMedium != null)
                    {
                        basicMessageMedium.Name = FocusedMessageMedium.Name;
                        basicMessageMedium.Image = FocusedMessageMedium.Image;
                    }
                    else
                    {
                        BasicMessageMediumViewModel.BasicMessageMediums.Add(FocusedMessageMedium);
                    }
                }
                //Save to db.
                using (var context = new MessageMediumContext())
                {
                    var result = context.MessageMediums.SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);
                    if (result != null)
                    {
                        result.Name = FocusedMessageMedium.Name;
                        result.Image = FocusedMessageMedium.Image;
                    }
                    else
                    {
                        if (FocusedMessageMedium.Parent.Type == MessageMediumType.root)
                        {
                            FocusedMessageMedium.Parent = null;
                        }
                        else
                        {
                            //get the parent.
                            var parent=context.MessageMediums.Include(c => c.Children).SingleOrDefault(c => c.Id == FocusedMessageMedium.Parent.Id);
                            parent.AddChild(FocusedMessageMedium);
                        }
                        context.MessageMediums.Add(FocusedMessageMedium);
                    }
                    context.SaveChanges();
                }
            }
            
        }

        public void DeleteFocusedMesageMedium()
        {
            if (FocusedMessageMedium == null || FocusedMessageMedium.Type==MessageMediumType.root)
            {
                return;
            }
            DeleteFromApplicationContext();
            DeleteFromDb();
            DeleteFromTreeView();
        }

        private void DeleteFromTreeView()
        {
            if (FocusedMessageMedium.Parent.Type == MessageMediumType.root)
            {
                if (FocusedMessageMedium.Parent.Name == "Table message mediums")
                {
                    TableRoot.Children = new List<MessageMedium>();
                    TableRoot.Children = MessageMediumProxyUtil.GetTableRootMessageMediums();
                    foreach (MessageMedium child in TableRoot.Children)
                    {
                        child.Parent = TableRoot;
                    }
                }
                else if (FocusedMessageMedium.Parent.Name == "Basic message mediums")
                {
                    BasicRoot.Children = new List<MessageMedium>();
                    BasicRoot.Children = MessageMediumProxyUtil.GetBasicMessageMediums();
                    foreach (MessageMedium child in BasicRoot.Children)
                    {
                        child.Parent = BasicRoot;
                    }
                }
                return;
            }
            MessageMedium parent = DfsInMessageMediums(MessageMediums, FocusedMessageMedium.Parent.Id);
            parent.Children = new List<MessageMedium>();
            SetChildren(parent);
        }

        private void DeleteFromDb()
        {
            using (var context = new MessageMediumContext())
            {
                var result = context.MessageMediums.Include(c => c.Children).SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);
                List<MessageMedium> deleteStack = new List<MessageMedium>();
                DeleteChildrenFromDB(result.Children, context, deleteStack);
                foreach (MessageMedium msg in deleteStack)
                {
                    context.MessageMediums.Remove(msg);
                }
                context.MessageMediums.Remove(result);
                context.SaveChanges();
            }
        }

        private void DeleteFromApplicationContext()
        {
            if (FocusedMessageMedium.Type == MessageMediumType.table)
            {
                MessageMediumViewModel.MessageMediums.Remove(MessageMediumViewModel.MessageMediums.SingleOrDefault(i => i.Id == FocusedMessageMedium.Id));
            }
            else if (FocusedMessageMedium.Type == MessageMediumType.basic)
            {
                BasicMessageMediumViewModel.BasicMessageMediums.Remove(BasicMessageMediumViewModel.BasicMessageMediums.SingleOrDefault(i => i.Id == FocusedMessageMedium.Id));
            }
        }
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        private void DeleteChildrenFromDB(List<MessageMedium> messageMediumList, MessageMediumContext context, List<MessageMedium> deleteStack)
        {
            if (messageMediumList == null)
            {
                return;
            }
            foreach (MessageMedium msg in messageMediumList)
            {
                var result = context.MessageMediums.Include(c => c.Children).SingleOrDefault(c => c.Id == msg.Id);
                deleteStack.Add(result);
                DeleteChildrenFromDB(msg.Children, context, deleteStack);
            }
        }
        private MessageMedium DfsInMessageMediums(ObservableCollection<MessageMedium> messageMediumList, int id)
        {
            foreach (MessageMedium msg in messageMediumList)
            {
                return DFS(msg.Children, id);
            }
            return null;
        }
        private MessageMedium DFS(List<MessageMedium> messageMediumList, int id)
        {

            if (messageMediumList == null)
            {
                return null;
            }
            foreach (MessageMedium msg in messageMediumList)
            {
                Console.WriteLine(msg.Id);
               
                if (msg.Id == id)
                {
                    return msg;
                }
                var result = DFS(msg.Children, id);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }


        public void AddChildToFocusedMessageMedium()
        {
            if (FocusedMessageMedium == null)
            {
                return;
            }
            MessageMedium parent = FocusedMessageMedium;
            FocusedMessageMedium = new MessageMedium();
            FocusedMessageMedium.Parent = parent;
            FocusedMessageMedium.Children = new List<MessageMedium>();

            if (parent.Type == MessageMediumType.root)
            {
                if (parent.Name == "Table message mediums")
                {
                    FocusedMessageMedium.Type = MessageMediumType.table;
                }
                else if (parent.Name == "Basic message mediums")
                {
                    FocusedMessageMedium.Type = MessageMediumType.basic;
                }
            }
            else
            {
                FocusedMessageMedium.Type = parent.Type;
            }
            
        }

    }
}
