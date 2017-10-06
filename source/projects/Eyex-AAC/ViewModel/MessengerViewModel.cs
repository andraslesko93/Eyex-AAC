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
    class MessengerViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Messenger> Messengers{ get; set; }
        //Need a reference property for databinding.
        public static PageManagerUtil PageManagerUtil { get; set; }
        public static RenderUtil RenderUtil { get; set; }

        private SpeechSynthesizer synthesizer;
        private static bool sentenceMode;

        public bool SentenceMode
        {
            get { return sentenceMode; }
            set
            { sentenceMode = value;
                RaisePropertyChanged("SentenceMode");
            }
        }

        private static List<String> wordList;

        public List<String> WordList
        {
            get { return wordList; }
            set
            {
                wordList = value;
                RaisePropertyChanged("WordList");
            }
        }


        public MessengerViewModel()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;
            synthesizer.Rate = -2;
            SentenceMode = false;
            WordList = new List<string>();
        }
        public void LoadMessengers()
        {
           // AddInitData();
            RenderUtil = new RenderUtil();
            Messengers = ApplicationContext.Instance.Messengers;
            PageManagerUtil = PageManagerUtil.Instance;
            PageManagerUtil.SetPageManagerUtil(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, DatabaseContext.GetTableRootMessengers(), Messengers);
        }
        public void PerformActionOnMessenger(int id)
        {
            Messenger messenger = ApplicationContext.Instance.GetMessengerFromApplicationContextById(id);
            if (messenger.Children!=null && messenger.Children.Any())
            {
                MoveDownALevel(messenger);
            }
            else
            {
                if (SentenceMode)
                {
                    WordList.Add(messenger.Name);
                }
                else
                {
                    synthesizer.SpeakAsync(messenger.Name);
                }
            }
        }
        private void AddInitData()
        {
            using (var context = new MessengerContext())
            {
                context.Messengers.Add(new Messenger("cartoons", "pack://application:,,,/Resources/Images/demo images/cartoons.jpg"));
                context.Messengers.Add(new Messenger("cat", "pack://application:,,,/Resources/Images/demo images/cat.jpg"));
                context.Messengers.Add(new Messenger("many", "pack://application:,,,/Resources/Images/demo images/many.jpg"));
                context.Messengers.Add(new Messenger("movie", "pack://application:,,,/Resources/Images/demo images/movie.jpg"));

                Messenger foods = new Messenger("foods", "pack://application:,,,/Resources/Images/demo images/foods/food.jpg");
                foods.AddChild(new Messenger("hungry", "pack://application:,,,/Resources/Images/demo images/foods/hungry.jpg"));
                foods.AddChild(new Messenger("orange", "pack://application:,,,/Resources/Images/demo images/foods/orange.jpg"));
                foods.AddChild(new Messenger("sandwich", "pack://application:,,,/Resources/Images/demo images/foods/sandwich.jpg"));
                foods.AddChild(new Messenger("soup", "pack://application:,,,/Resources/Images/demo images/foods/soup.jpg"));
                foods.AddChild(new Messenger("thirsty", "pack://application:,,,/Resources/Images/demo images/foods/thirsty.jpg"));
                context.Messengers.Add(foods);

                context.Messengers.Add(new Messenger("tired", "pack://application:,,,/Resources/Images/demo images/tired.jpg"));
                context.Messengers.Add(new Messenger("today", "pack://application:,,,/Resources/Images/demo images/today.jpg"));
                context.Messengers.Add(new Messenger("toothbrush", "pack://application:,,,/Resources/Images/demo images/toothbrush.jpg"));
                context.Messengers.Add(new Messenger("today", "pack://application:,,,/Resources/Images/demo images/today.jpg"));
                context.Messengers.Add(new Messenger("yesterday", "pack://application:,,,/Resources/Images/demo images/yesterday.jpg"));

                Messenger games = new Messenger("games", "pack://application:,,,/Resources/Images/demo images/game/game.jpg");
                games.AddChild(new Messenger("ball", "pack://application:,,,/Resources/Images/demo images/game/ball.jpg"));
                games.AddChild(new Messenger("balloons", "pack://application:,,,/Resources/Images/demo images/game/balloons.jpg"));
                games.AddChild(new Messenger("play", "pack://application:,,,/Resources/Images/demo images/game/play.jpg"));
                games.AddChild(new Messenger("pool", "pack://application:,,,/Resources/Images/demo images/game/pool.jpg"));
                context.Messengers.Add(games);

                Messenger school = new Messenger("school", "pack://application:,,,/Resources/Images/demo images/school/school.jpg");
                school.AddChild(new Messenger("friend", "pack://application:,,,/Resources/Images/demo images/school/friend.jpg"));
                school.AddChild(new Messenger("paper", "pack://application:,,,/Resources/Images/demo images/school/paper.jpg"));
                school.AddChild(new Messenger("pen", "pack://application:,,,/Resources/Images/demo images/school/pen.jpg"));
                school.AddChild(new Messenger("pencil", "pack://application:,,,/Resources/Images/demo images/school/pencil.jpg"));
                school.AddChild(new Messenger("school bag", "pack://application:,,,/Resources/Images/demo images/school/school bag.jpg"));
                Messenger study = new Messenger("study", "pack://application:,,,/Resources/Images/demo images/school/study/study.jpg");
                study.AddChild(new Messenger("computer", "pack://application:,,,/Resources/Images/demo images/school/study/computer.jpg"));
                study.AddChild(new Messenger("book", "pack://application:,,,/Resources/Images/demo images/school/study/book.jpg"));
                school.AddChild(study);
                context.Messengers.Add(school);

                context.Messengers.Add(new Messenger("hello", "pack://application:,,,/Resources/Images/demo images/basic/hello.jpg", MessengerType.pegged));
                context.Messengers.Add(new Messenger("goodbye", "pack://application:,,,/Resources/Images/demo images/basic/goodbye.jpg", MessengerType.pegged));
                context.Messengers.Add(new Messenger("yes", "pack://application:,,,/Resources/Images/demo images/basic/yes.jpg", MessengerType.pegged));
                context.Messengers.Add(new Messenger("no", "pack://application:,,,/Resources/Images/demo images/basic/no.jpg", MessengerType.pegged));
                context.Messengers.Add(new Messenger("thank you", "pack://application:,,,/Resources/Images/demo images/basic/thank you.jpg", MessengerType.pegged));
                context.SaveChanges();
            }
        }

        internal void SaySentence()
        {
            String sentence ="";
            foreach (String word in WordList) {
                sentence += word;
            }
            WordList.Clear();
            synthesizer.SpeakAsync(sentence);
        }

        public void ChangeSentenceMode()
        {
            SentenceMode = !SentenceMode;
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public void NextPage()
        {
            PageManagerUtil.NextPage();
        }
        public void PreviousPage()
        {
            PageManagerUtil.PreviousPage();
        }
        private void MoveDownALevel(Messenger messenger)
        {
           PageManagerUtil.MoveDownALevel(messenger, DatabaseContext.GetChildren(messenger));
        }

        public void MoveUpALevel()
        {
            PageManagerUtil.MoveUpALevel();
        }
    }
}
