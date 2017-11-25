using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel.Utils
{
    class ActivityLogManager
    {
        private static List<ActivityLogEntry> ActivityLog { get; set; } = new List<ActivityLogEntry>();

        public static void Log(Messenger messenger)
        {
            ActivityLog.Add(new ActivityLogEntry(messenger));
        }

        public static void SaveActivityLog()
        {
            DatabaseContext.SaveActivityLog(ActivityLog);
        }

        public static void SendActivityLog()
        {
            bool isSucces = HttpManager.Send(ActivityLog);
            if (isSucces)
            {
                DatabaseContext.SetActivityLogEntriesToSent(ActivityLog);
                ActivityLog.Clear();
            }
        }

        public static void LoadUnsentLog() {
            ActivityLog = DatabaseContext.LoadUnsentActivityLogEntries();
            //If we left some unsent log from the previous use due to the lack of internet connection.
            if (ActivityLog.Any())
            {
                SendActivityLog();
            }
        }


    }
}
