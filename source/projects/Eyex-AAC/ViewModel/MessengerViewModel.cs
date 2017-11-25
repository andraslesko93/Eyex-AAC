using EyexAAC.Model;
using System.Linq;
using System.Collections.ObjectModel;
using System.Speech.Synthesis;

using EyexAAC.ViewModel.Utils;

namespace EyexAAC.ViewModel
{
    class MessengerViewModel
    {
        public ObservableCollection<Messenger> Messengers{ get; set; }
        public static PageManagerUtil PageManagerUtil { get; set; }
        public static RenderUtil RenderUtil { get; set; }
        private SpeechSynthesizer synthesizer { get; set; }
        public SentenceModeManager SentenceModeManager { get; set; }

        public MessengerViewModel()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;
            synthesizer.Rate = -2;
            SentenceModeManager = SentenceModeManager.Instance;
        }
        public void LoadMessengers()
        {
            AddInitData();
            RenderUtil = new RenderUtil();
            Messengers = ApplicationContext.Instance.Messengers;
            PageManagerUtil = PageManagerUtil.Instance;
            PageManagerUtil.SetPageManagerUtil(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, DatabaseContext.LoadAllGeneralMessenger(), Messengers);
        }

        public void PerformActionOnMessenger(int id)
        {
            Messenger messenger = ApplicationContext.Instance.GetMessengerFromApplicationContextById(id);
            ActivityLogManager.Log(messenger);
            if (messenger.Children!=null && messenger.Children.Any())
            {
                MoveDownALevel(messenger);
            }
            else
            {
                if (SentenceModeManager.SentenceMode)
                {
                    SentenceModeManager.AddWord(messenger.Name);
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
                context.Messengers.Add(new Messenger("cartoons", "pack://application:,,,/Resources/Images/demo images/cartoons.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("cat", "pack://application:,,,/Resources/Images/demo images/cat.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("many", "pack://application:,,,/Resources/Images/demo images/many.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("movie", "pack://application:,,,/Resources/Images/demo images/movie.jpg", SessionViewModel.User.Username));

                Messenger foods = new Messenger("foods", "pack://application:,,,/Resources/Images/demo images/foods/food.jpg", SessionViewModel.User.Username);
                foods.AddChild(new Messenger("hungry", "pack://application:,,,/Resources/Images/demo images/foods/hungry.jpg", SessionViewModel.User.Username));
                foods.AddChild(new Messenger("orange", "pack://application:,,,/Resources/Images/demo images/foods/orange.jpg", SessionViewModel.User.Username));
                foods.AddChild(new Messenger("sandwich", "pack://application:,,,/Resources/Images/demo images/foods/sandwich.jpg", SessionViewModel.User.Username));
                foods.AddChild(new Messenger("soup", "pack://application:,,,/Resources/Images/demo images/foods/soup.jpg", SessionViewModel.User.Username));
                foods.AddChild(new Messenger("thirsty", "pack://application:,,,/Resources/Images/demo images/foods/thirsty.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(foods);

                context.Messengers.Add(new Messenger("tired", "pack://application:,,,/Resources/Images/demo images/tired.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("today", "pack://application:,,,/Resources/Images/demo images/today.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("toothbrush", "pack://application:,,,/Resources/Images/demo images/toothbrush.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("today", "pack://application:,,,/Resources/Images/demo images/today.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("yesterday", "pack://application:,,,/Resources/Images/demo images/yesterday.jpg", SessionViewModel.User.Username));

                Messenger games = new Messenger("games", "pack://application:,,,/Resources/Images/demo images/game/game.jpg", SessionViewModel.User.Username);
                games.AddChild(new Messenger("ball", "pack://application:,,,/Resources/Images/demo images/game/ball.jpg", SessionViewModel.User.Username));
                games.AddChild(new Messenger("balloons", "pack://application:,,,/Resources/Images/demo images/game/balloons.jpg", SessionViewModel.User.Username));
                games.AddChild(new Messenger("play", "pack://application:,,,/Resources/Images/demo images/game/play.jpg", SessionViewModel.User.Username));
                games.AddChild(new Messenger("pool", "pack://application:,,,/Resources/Images/demo images/game/pool.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(games);

                Messenger school = new Messenger("school", "pack://application:,,,/Resources/Images/demo images/school/school.jpg", SessionViewModel.User.Username);
                school.AddChild(new Messenger("friend", "pack://application:,,,/Resources/Images/demo images/school/friend.jpg", SessionViewModel.User.Username));
                school.AddChild(new Messenger("paper", "pack://application:,,,/Resources/Images/demo images/school/paper.jpg", SessionViewModel.User.Username));
                school.AddChild(new Messenger("pen", "pack://application:,,,/Resources/Images/demo images/school/pen.jpg", SessionViewModel.User.Username));
                school.AddChild(new Messenger("pencil", "pack://application:,,,/Resources/Images/demo images/school/pencil.jpg", SessionViewModel.User.Username));
                school.AddChild(new Messenger("school bag", "pack://application:,,,/Resources/Images/demo images/school/school bag.jpg", SessionViewModel.User.Username));
                Messenger study = new Messenger("study", "pack://application:,,,/Resources/Images/demo images/school/study/study.jpg", SessionViewModel.User.Username);
                study.AddChild(new Messenger("computer", "pack://application:,,,/Resources/Images/demo images/school/study/computer.jpg", SessionViewModel.User.Username));
                study.AddChild(new Messenger("book", "pack://application:,,,/Resources/Images/demo images/school/study/book.jpg", SessionViewModel.User.Username));
                school.AddChild(study);
                context.Messengers.Add(school);

                context.Messengers.Add(new Messenger("hello", "pack://application:,,,/Resources/Images/demo images/basic/hello.jpg", MessengerType.pinned, SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("goodbye", "pack://application:,,,/Resources/Images/demo images/basic/goodbye.jpg", MessengerType.pinned, SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("yes", "pack://application:,,,/Resources/Images/demo images/basic/yes.jpg", MessengerType.pinned, SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("no", "pack://application:,,,/Resources/Images/demo images/basic/no.jpg", MessengerType.pinned, SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("thank you", "pack://application:,,,/Resources/Images/demo images/basic/thank you.jpg", MessengerType.pinned, SessionViewModel.User.Username));
                context.SaveChanges();
            }
        }

        internal void SaySentence()
        {
            string sentence = SentenceModeManager.Instance.CurrentSentence.SentenceAsString;
            synthesizer.SpeakAsync(sentence);
            SentenceModeManager.PublishSentence();
            if (M2qttManager.IsSubscribed)
            {
                M2qttManager.Publish(sentence);
            }
        }

        public void ChangeSentenceMode()
        {
            SentenceModeManager.SentenceMode = !SentenceModeManager.SentenceMode;
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
           PageManagerUtil.MoveDownALevel(messenger);
        }
        public void MoveUpALevel()
        {
            PageManagerUtil.MoveUpALevel();
        }
    }
}
