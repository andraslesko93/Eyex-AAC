using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EyexAAC.Model
{
    class ActivityLogEntry
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public string MessengerName { get; set; }
        public string EncodedMessengerImage { get; set; }

        [JsonIgnore]
        public ActivityLogEntryStatus Status {get;set;}

        public ActivityLogEntry() { }

        public ActivityLogEntry(Messenger messenger)
        {
            MessengerName = messenger.Name;
            EncodedMessengerImage = messenger.EncodedImage;
            Status = ActivityLogEntryStatus.unsent;
        }


    }
    enum ActivityLogEntryStatus {
        unsent=0,
        sent
    }
}
