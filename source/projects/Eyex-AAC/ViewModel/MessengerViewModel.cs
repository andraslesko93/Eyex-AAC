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
        public static PageManager PageManagerUtil { get; set; }
        public static RenderManager RenderUtil { get; set; }
        private SpeechSynthesizer synthesizer { get; set; }
        public SentenceModeManager SentenceModeManager { get; set; }

        public MessengerViewModel()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;
            synthesizer.SelectVoice("Microsoft Szabolcs");
            SentenceModeManager = SentenceModeManager.Instance;
        }
        public void LoadMessengers()
        {
           // AddInitData();
            RenderUtil = new RenderManager();
            Messengers = ApplicationContext.Instance.Messengers;
            PageManagerUtil = PageManager.Instance;
            PageManagerUtil.SetPageManagerUtil(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, DatabaseContextUtility.LoadAllGeneralMessenger(), Messengers);
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
                context.Messengers.Add(new Messenger("rajzfilm", "pack://application:,,,/Resources/Images/demo images/cartoons.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("macska", "pack://application:,,,/Resources/Images/demo images/cat.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("sok", "pack://application:,,,/Resources/Images/demo images/many.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("film", "pack://application:,,,/Resources/Images/demo images/movie.jpg", SessionViewModel.User.Username));

                Messenger foods = new Messenger("ételek", "pack://application:,,,/Resources/Images/demo images/foods/food.jpg", SessionViewModel.User.Username);
                foods.AddChild(new Messenger("éhes", "pack://application:,,,/Resources/Images/demo images/foods/hungry.jpg", SessionViewModel.User.Username));
                foods.AddChild(new Messenger("narancs", "pack://application:,,,/Resources/Images/demo images/foods/orange.jpg", SessionViewModel.User.Username));
                foods.AddChild(new Messenger("szendvics", "pack://application:,,,/Resources/Images/demo images/foods/sandwich.jpg", SessionViewModel.User.Username));
                foods.AddChild(new Messenger("leves", "pack://application:,,,/Resources/Images/demo images/foods/soup.jpg", SessionViewModel.User.Username));
                foods.AddChild(new Messenger("szomjas", "pack://application:,,,/Resources/Images/demo images/foods/thirsty.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(foods);

                context.Messengers.Add(new Messenger("fáradt", "pack://application:,,,/Resources/Images/demo images/tired.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("ma", "pack://application:,,,/Resources/Images/demo images/today.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("fogkefe", "pack://application:,,,/Resources/Images/demo images/toothbrush.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("tegnap", "pack://application:,,,/Resources/Images/demo images/yesterday.jpg", SessionViewModel.User.Username));

                Messenger games = new Messenger("játékok", "pack://application:,,,/Resources/Images/demo images/game/game.jpg", SessionViewModel.User.Username);
                games.AddChild(new Messenger("labda", "pack://application:,,,/Resources/Images/demo images/game/ball.jpg", SessionViewModel.User.Username));
                games.AddChild(new Messenger("lufi", "pack://application:,,,/Resources/Images/demo images/game/balloons.jpg", SessionViewModel.User.Username));
                games.AddChild(new Messenger("játszani", "pack://application:,,,/Resources/Images/demo images/game/play.jpg", SessionViewModel.User.Username));
                games.AddChild(new Messenger("medence", "pack://application:,,,/Resources/Images/demo images/game/pool.jpg", SessionViewModel.User.Username));
                context.Messengers.Add(games);

                Messenger school = new Messenger("iskola", "pack://application:,,,/Resources/Images/demo images/school/school.jpg", SessionViewModel.User.Username);
                school.AddChild(new Messenger("barát", "pack://application:,,,/Resources/Images/demo images/school/friend.jpg", SessionViewModel.User.Username));
                school.AddChild(new Messenger("papír", "pack://application:,,,/Resources/Images/demo images/school/paper.jpg", SessionViewModel.User.Username));
                school.AddChild(new Messenger("toll", "pack://application:,,,/Resources/Images/demo images/school/pen.jpg", SessionViewModel.User.Username));
                school.AddChild(new Messenger("ceruza", "pack://application:,,,/Resources/Images/demo images/school/pencil.jpg", SessionViewModel.User.Username));
                school.AddChild(new Messenger("iskola táska", "pack://application:,,,/Resources/Images/demo images/school/school bag.jpg", SessionViewModel.User.Username));
                Messenger study = new Messenger("tanulás", "pack://application:,,,/Resources/Images/demo images/school/study/study.jpg", SessionViewModel.User.Username);
                study.AddChild(new Messenger("számítógép", "pack://application:,,,/Resources/Images/demo images/school/study/computer.jpg", SessionViewModel.User.Username));
                study.AddChild(new Messenger("könyv", "pack://application:,,,/Resources/Images/demo images/school/study/book.jpg", SessionViewModel.User.Username));
                school.AddChild(study);
                context.Messengers.Add(school);

                context.Messengers.Add(new Messenger("szia!", "pack://application:,,,/Resources/Images/demo images/basic/hello.jpg", MessengerType.pinned, SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("viszlát!", "pack://application:,,,/Resources/Images/demo images/basic/goodbye.jpg", MessengerType.pinned, SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("igen", "pack://application:,,,/Resources/Images/demo images/basic/yes.jpg", MessengerType.pinned, SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("nem", "pack://application:,,,/Resources/Images/demo images/basic/no.jpg", MessengerType.pinned, SessionViewModel.User.Username));
                context.Messengers.Add(new Messenger("köszönöm!", "pack://application:,,,/Resources/Images/demo images/basic/thank you.jpg", MessengerType.pinned, SessionViewModel.User.Username));
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
