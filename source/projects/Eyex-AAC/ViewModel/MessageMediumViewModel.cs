using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using System.Speech.Synthesis;

using EyexAAC.ViewModel.Utils;

namespace EyexAAC.ViewModel
{
    class MessageMediumViewModel
    {
        public static ObservableCollection<MessageMedium> MessageMediums{ get; set; }
        public static PageManagerUtil PageManagerUtil { get; set; }
        public static RenderUtil RenderUtil { get; set; }

        private SpeechSynthesizer synthesizer;

        public MessageMediumViewModel()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;
            synthesizer.Rate = -2;
        }
        public void LoadMessageMediums()
        {
           //AddInitData();
            RenderUtil = new RenderUtil();
            PageManagerUtil = new PageManagerUtil(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, MessageMediumProxyUtil.GetTableRootMessageMediums());
            MessageMediums = new ObservableCollection<MessageMedium>();
            PageManagerUtil.LoadMessageMediumsByPageNumber(MessageMediums);
            PageManagerUtil.PreviousPageButtonStateCalculator();
            PageManagerUtil.NextPageButtonStateCalculator();
        }
        public void PerformActionOnMessageMedium(int id)
        {
            MessageMedium messageMedium = GetMessageMediumFromCollectionById(id);
            if (messageMedium.Children.Any())
            {
                MoveDownALevel(messageMedium);
            }
            else if (messageMedium.Type == MessageMediumType.goBack)
            {
                MoveUpALevel();
            }
            else
            {
                synthesizer.SpeakAsync(messageMedium.Name);
                //Console.WriteLine(messageMedium.Name);
            }
        }
        private MessageMedium GetMessageMediumFromCollectionById(int id)
        {   
            var messageMedium = MessageMediums.FirstOrDefault(c => c.Id == id);
            messageMedium.InitializeImage();
            return messageMedium;   
        }
        private MessageMedium GetMessageMediumById(int id)
        {
            using (var context = new MessageMediumContext())
            {
                var messageMedium = context.MessageMediums.Include(c => c.Children).Include(c => c.Parent).SingleOrDefault(c => c.Id == id);
                if (messageMedium.ImageAsByte != null)
                {
                    messageMedium.InitializeImage();
                }
                return messageMedium;
            }
        }
        private void AddInitData()
        {
            using (var context = new MessageMediumContext())
            {
                context.MessageMediums.Add(new MessageMedium("cartoons", "pack://application:,,,/Resources/Images/demo images/cartoons.jpg"));
                context.MessageMediums.Add(new MessageMedium("cat", "pack://application:,,,/Resources/Images/demo images/cat.jpg"));
                context.MessageMediums.Add(new MessageMedium("many", "pack://application:,,,/Resources/Images/demo images/many.jpg"));
                context.MessageMediums.Add(new MessageMedium("movie", "pack://application:,,,/Resources/Images/demo images/movie.jpg"));

                MessageMedium foods = new MessageMedium("foods", "pack://application:,,,/Resources/Images/demo images/foods/food.jpg");
                foods.AddChild(new MessageMedium("hungry", "pack://application:,,,/Resources/Images/demo images/foods/hungry.jpg"));
                foods.AddChild(new MessageMedium("orange", "pack://application:,,,/Resources/Images/demo images/foods/orange.jpg"));
                foods.AddChild(new MessageMedium("sandwich", "pack://application:,,,/Resources/Images/demo images/foods/sandwich.jpg"));
                foods.AddChild(new MessageMedium("soup", "pack://application:,,,/Resources/Images/demo images/foods/soup.jpg"));
                foods.AddChild(new MessageMedium("thirsty", "pack://application:,,,/Resources/Images/demo images/foods/thirsty.jpg"));
                context.MessageMediums.Add(foods);

                context.MessageMediums.Add(new MessageMedium("tired", "pack://application:,,,/Resources/Images/demo images/tired.jpg"));
                context.MessageMediums.Add(new MessageMedium("today", "pack://application:,,,/Resources/Images/demo images/today.jpg"));
                context.MessageMediums.Add(new MessageMedium("toothbrush", "pack://application:,,,/Resources/Images/demo images/toothbrush.jpg"));
                context.MessageMediums.Add(new MessageMedium("today", "pack://application:,,,/Resources/Images/demo images/today.jpg"));
                context.MessageMediums.Add(new MessageMedium("yesterday", "pack://application:,,,/Resources/Images/demo images/yesterday.jpg"));

                MessageMedium games = new MessageMedium("games", "pack://application:,,,/Resources/Images/demo images/game/game.jpg");
                games.AddChild(new MessageMedium("ball", "pack://application:,,,/Resources/Images/demo images/game/ball.jpg"));
                games.AddChild(new MessageMedium("balloons", "pack://application:,,,/Resources/Images/demo images/game/balloons.jpg"));
                games.AddChild(new MessageMedium("play", "pack://application:,,,/Resources/Images/demo images/game/play.jpg"));
                games.AddChild(new MessageMedium("pool", "pack://application:,,,/Resources/Images/demo images/game/pool.jpg"));
                context.MessageMediums.Add(games);

                MessageMedium school = new MessageMedium("school", "pack://application:,,,/Resources/Images/demo images/school/school.jpg");
                school.AddChild(new MessageMedium("friend", "pack://application:,,,/Resources/Images/demo images/school/friend.jpg"));
                school.AddChild(new MessageMedium("paper", "pack://application:,,,/Resources/Images/demo images/school/paper.jpg"));
                school.AddChild(new MessageMedium("pen", "pack://application:,,,/Resources/Images/demo images/school/pen.jpg"));
                school.AddChild(new MessageMedium("pencil", "pack://application:,,,/Resources/Images/demo images/school/pencil.jpg"));
                school.AddChild(new MessageMedium("school bag", "pack://application:,,,/Resources/Images/demo images/school/school bag.jpg"));
                MessageMedium study = new MessageMedium("study", "pack://application:,,,/Resources/Images/demo images/school/study/study.jpg");
                study.AddChild(new MessageMedium("computer", "pack://application:,,,/Resources/Images/demo images/school/study/computer.jpg"));
                study.AddChild(new MessageMedium("book", "pack://application:,,,/Resources/Images/demo images/school/study/book.jpg"));
                school.AddChild(study);
                context.MessageMediums.Add(school);

                context.MessageMediums.Add(new MessageMedium("hello", "pack://application:,,,/Resources/Images/demo images/basic/hello.jpg", MessageMediumType.basic));
                context.MessageMediums.Add(new MessageMedium("goodbye", "pack://application:,,,/Resources/Images/demo images/basic/goodbye.jpg", MessageMediumType.basic));
                context.MessageMediums.Add(new MessageMedium("yes", "pack://application:,,,/Resources/Images/demo images/basic/yes.jpg", MessageMediumType.basic));
                context.MessageMediums.Add(new MessageMedium("no", "pack://application:,,,/Resources/Images/demo images/basic/no.jpg", MessageMediumType.basic));
                context.MessageMediums.Add(new MessageMedium("thank you", "pack://application:,,,/Resources/Images/demo images/basic/thank you.jpg", MessageMediumType.basic));
                context.SaveChanges();
            }
        }
        public void NextPage()
        {
            PageManagerUtil.NextPage(MessageMediums);
        }
        public void PreviousPage()
        {
            PageManagerUtil.PreviousPage(MessageMediums);
        }
        private void MoveDownALevel(MessageMedium messageMedium)
        {
            MessageMediums.Clear();
            SetTurnPageUtilScope(addBackMessageMediumToList(MessageMediumProxyUtil.GetChildren(messageMedium)));
        }
        private List<MessageMedium> addBackMessageMediumToList(List<MessageMedium> list)
        {
            List<MessageMedium> messageMediumList = new List<MessageMedium>();
            MessageMedium goBack = new MessageMedium("go back", "pack://application:,,,/Resources/Images/go_back.jpg", MessageMediumType.goBack);
            messageMediumList.Add(goBack);
            list.ForEach(messageMediumList.Add);
            return messageMediumList;
        }
        private void MoveUpALevel()
        {
            MessageMedium element = MessageMediums.Last();
            //element.Parent does not contain reference to elements's grandparent, so we have to make another query.
            //The parent object will contain the reference to grandparent.
            MessageMedium parent = GetMessageMediumById(element.Parent.Id);
            MessageMediums.Clear();
            if (parent.Parent != null)
            {
                MessageMedium grandParent = GetMessageMediumById(parent.Parent.Id);
                SetTurnPageUtilScope(addBackMessageMediumToList(MessageMediumProxyUtil.GetChildren(grandParent)));
            }
            else
            {
                SetTurnPageUtilScope(MessageMediumProxyUtil.GetTableRootMessageMediums());
            }
        }      
        private void SetTurnPageUtilScope(List<MessageMedium> messageMediumList)
        {
            PageManagerUtil.NewDataScope(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, messageMediumList);
            PageManagerUtil.LoadMessageMediumsByPageNumber(MessageMediums);
            PageManagerUtil.PreviousPageButtonStateCalculator();
            PageManagerUtil.NextPageButtonStateCalculator();
        }

        /*
        public void logStatus()
        {
            PageManagerUtil.logStatus();
        }
        */

    }
}
