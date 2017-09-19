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
    class ManageViewModel : INotifyPropertyChanged
    {
        private static Messenger _focusedMessageMedium;

        private static Messenger TableRoot;
        private static Messenger BasicRoot;
        private bool addInProggress = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public static ObservableCollection<Messenger> MessageMediums { get; set; }

        public Messenger FocusedMessageMedium
        {
            get { return _focusedMessageMedium; }
            set
            {
                _focusedMessageMedium = value;
                RaisePropertyChanged("FocusedMessageMedium");
            }
        }

        public ManageViewModel()
        {
            TableRoot =  new Messenger("Table message mediums", MessengerType.root);
            BasicRoot =  new Messenger("Basic message mediums", MessengerType.root);
            MessageMediums = new ObservableCollection<Messenger>();
            SetRootObjects();
        }

        public void SetChildren(Messenger messageMedium)
        {
            if (messageMedium.Type==MessengerType.root)
            {
                return;
            }

            messageMedium.Children = DatabaseContext.GetChildren(messageMedium);
        }

        private void SetRootObjects()
        {       
            foreach (Messenger msg in DatabaseContext.GetTableRootMessengers())
            {
                 TableRoot.AddChild(msg);
            }
            MessageMediums.Add(TableRoot);

            foreach (Messenger msg in DatabaseContext.GetBasicMessengers())
            {
                BasicRoot.AddChild(msg);
            }
            MessageMediums.Add(BasicRoot);
        }

        public void SetMessageMediumToFocus(Messenger messageMedium)
        {
            FocusedMessageMedium = messageMedium;
            addInProggress = false;
        }
        public void SaveFocusedMesageMedium()
        {
            if (FocusedMessageMedium.Name != null)
            {
                SaveToDB();
                SaveToApplicationContext();  
            }

            addInProggress = false;
        }

        private void SaveToApplicationContext()
        {
            if (FocusedMessageMedium.Type == MessengerType.table)
            {
                HandleTableMessageMediums();
            }
            else if (FocusedMessageMedium.Type == MessengerType.basic)
            {
                HandleBasicMessageMediums();
            }
        }

        private void HandleBasicMessageMediums()
        {
            Messenger basicMessageMedium = BasicMessageMediumViewModel.BasicMessageMediums.SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);

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

        private void HandleTableMessageMediums()
        {
            Messenger tableMessageMedium = ApplicationContext.Instance.Messengers.SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);
            //Does it exists in the current context?
            if (tableMessageMedium != null)
            {
                //It is not a subtype of meta object on the communication table, so we can access to it.
                tableMessageMedium.Name = FocusedMessageMedium.Name;
                tableMessageMedium.Image = FocusedMessageMedium.Image;
            }
            else
            {
                Messenger currentElement = ApplicationContext.Instance.Messengers.Last();

                if (FocusedMessageMedium.Parent.Type == MessengerType.root && currentElement.Parent == null)
                {
                    ApplicationContext.Instance.Messengers.Add(FocusedMessageMedium);
                    MessengerViewModel.PageManagerUtil.AddToMessengerCache(FocusedMessageMedium);
                }
                else if(FocusedMessageMedium.Parent.Id==currentElement.Parent.Id)
                {
                    ApplicationContext.Instance.Messengers.Add(FocusedMessageMedium);
                    MessengerViewModel.PageManagerUtil.AddToMessengerCache(FocusedMessageMedium);
                }
            }
            //Add to parent.
            Messenger tableMessageMediumParent = ApplicationContext.Instance.Messengers.SingleOrDefault(c => c.Id == FocusedMessageMedium.Parent.Id);
            if (tableMessageMediumParent != null)
            {
                tableMessageMediumParent.AddChild(FocusedMessageMedium);
            }
        }

        private void SaveToDB()
        {
            using (var context = new MessengerContext())
            {
                var result = context.Messengers.SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);
                if (result != null)
                {
                    result.Name = FocusedMessageMedium.Name;
                    result.Image = FocusedMessageMedium.Image;
                    context.SaveChanges();
                }
                else
                {
                    var messageMediumToSave = FocusedMessageMedium.Copy();
                    if (messageMediumToSave.Parent.Type == MessengerType.root)
                    {
                        messageMediumToSave.Parent = null;
                    }
                    else
                    {
                        //get the parent.
                        var parent = context.Messengers.Include(c => c.Children).SingleOrDefault(c => c.Id == messageMediumToSave.Parent.Id);
                        parent.AddChild(messageMediumToSave);
                    }
                    context.Messengers.Add(messageMediumToSave);
                    context.SaveChanges();
                    FocusedMessageMedium.Id = messageMediumToSave.Id;
                }
                
            }
        }

        public void DeleteFocusedMesageMedium()
        {
            if (!IsFocusMessageMediumSetted() || FocusedMessageMedium.Type==MessengerType.root)
            {
                return;
            }
            DeleteFromApplicationContext();
            DeleteFromDb();
            DeleteFromTreeView();
            //RefreshTreeView();
            MessengerViewModel.PageManagerUtil.NextPageButtonStateCalculator();
        }

        private void DeleteFromTreeView()
        {
            Messenger messenger = DFS(MessageMediums, FocusedMessageMedium.Id);
            messenger.Parent.RemoveChild(messenger);
           // messenger = null;
            //MessageMediums[0].Children.Remove(MessageMediums[0].Children.SingleOrDefault(i => i.Id == FocusedMessageMedium.Id));

            //   Messenger messenger = DfsInMessengerList(MessageMediums, FocusedMessageMedium.Parent.Id);
            //TODO: Itt kene csinalni egyet ami a treeviewbol kikeresi es kiszedi.
            //Gyakorlatilag egy dfs itt is
        }

        /*private void RefreshTreeView()
        {
            if (FocusedMessageMedium.Parent.Type == MessengerType.root)
            {
                if (FocusedMessageMedium.Parent.Name == "Table message mediums")
                {
                    TableRoot.Children = new List<Messenger>();
                    TableRoot.Children = DatabaseContext.GetTableRootMessengers();
                    foreach (Messenger child in TableRoot.Children)
                    {
                        child.Parent = TableRoot;
                    }
                }
                else if (FocusedMessageMedium.Parent.Name == "Basic message mediums")
                {
                    BasicRoot.Children = new List<Messenger>();
                    BasicRoot.Children = DatabaseContext.GetBasicMessengers();
                    foreach (Messenger child in BasicRoot.Children)
                    {
                        child.Parent = BasicRoot;
                    }
                }
                return;
            }
            Messenger parent = DfsInMessageMediums(MessageMediums, FocusedMessageMedium.Parent.Id);
            parent.Children = new List<Messenger>();
            SetChildren(parent);
        }*/

        private void DeleteFromDb()
        {
            using (var context = new MessengerContext())
            {
                var result = context.Messengers.Include(c => c.Children).SingleOrDefault(c => c.Id == FocusedMessageMedium.Id);
                List<Messenger> deleteStack = new List<Messenger>();
                DeleteChildrenFromDB(result.Children, context, deleteStack);
                foreach (Messenger msg in deleteStack)
                {
                    context.Messengers.Remove(msg);
                }
                context.Messengers.Remove(result);
                context.SaveChanges();
            }
        }

        private void DeleteFromApplicationContext()
        {
            if (FocusedMessageMedium.Type == MessengerType.table)
            {
                ApplicationContext.Instance.Messengers.Remove(ApplicationContext.Instance.Messengers.SingleOrDefault(i => i.Id == FocusedMessageMedium.Id));
                MessengerViewModel.PageManagerUtil.MessengerCache.Remove(MessengerViewModel.PageManagerUtil.MessengerCache.SingleOrDefault(i => i.Id == FocusedMessageMedium.Id));
            }
            else if (FocusedMessageMedium.Type == MessengerType.basic)
            {
                BasicMessageMediumViewModel.BasicMessageMediums.Remove(BasicMessageMediumViewModel.BasicMessageMediums.SingleOrDefault(i => i.Id == FocusedMessageMedium.Id));
            }
        }
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        private void DeleteChildrenFromDB(ObservableCollection<Messenger> messageMediumList, MessengerContext context, List<Messenger> deleteStack)
        {
            if (messageMediumList == null)
            {
                return;
            }
            foreach (Messenger msg in messageMediumList)
            {
                var result = context.Messengers.Include(c => c.Children).SingleOrDefault(c => c.Id == msg.Id);
                deleteStack.Add(result);
                DeleteChildrenFromDB(msg.Children, context, deleteStack);
            }
        }
      /*  private Messenger DfsInMessengerList(ObservableCollection<Messenger> messageMediumList, int id)
        {
            //TODO vszeg ez igy rossz
            foreach (Messenger msg in messageMediumList)
            {
                return DFS(msg.Children, id);
            }
            return null;
        }*/
        private Messenger DFS(ObservableCollection<Messenger> messageMediumList, int id)
        {
            if (messageMediumList == null)
            {
                return null;
            }
            foreach (Messenger msg in messageMediumList)
            {  
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

        public bool IsFocusMessageMediumSetted()
        {
            if (FocusedMessageMedium == null)
            {
                return false;
            }
            return true;
        }

        public void AddChildToFocusedMessageMedium()
        {
            if (addInProggress == true)
            {
                return;
            }

            addInProggress = true;
            Messenger parent = FocusedMessageMedium;
            FocusedMessageMedium = new Messenger();
            FocusedMessageMedium.Parent = parent;
            FocusedMessageMedium.Children = new ObservableCollection<Messenger>();

            if (parent.Type == MessengerType.root)
            {
                if (parent.Name == "Table message mediums")
                {
                    FocusedMessageMedium.Type = MessengerType.table;
                }
                else if (parent.Name == "Basic message mediums")
                {
                    FocusedMessageMedium.Type = MessengerType.basic;
                }
            }
            else
            {
                FocusedMessageMedium.Type = parent.Type;
            }
            
        }

    }
}
