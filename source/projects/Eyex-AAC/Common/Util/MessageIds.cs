using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.Common.Utility
{
    class MessageIds
    {
        public static readonly string SENDS = "üzeni";
        public static readonly string TECHNICAL_ERROR = "A technical error occured";
        public static readonly string MESSENGERS_SHARED = "Your messengers has been shared.";
        public static readonly string MESSENGER_SHARING_ASK_FOR_PERMISSION = " wants to share thier messengers with you, do want to accept them? By accepting them any unsaved changes will be discarded.";
        public static readonly string CONFIRMATION = "Confirmation";

        public static readonly string CONNECTION_FIELD_MISSING = "Please fill all of the connection fields";
        public static readonly string SHARING_SESSION_LEAVE = "You have left the sharing session.";
        public static readonly string SHARING_SESSION_SAVE_CONFIRMATION = "If you leave sharing session, all shared messengers will be discarded, do you want to save them?";

        public static readonly string CONNECTION_RESPONSE_CONNECTED = "Connected";
        public static readonly string CONNECTION_RESPONSE_REFUSED_UNACCEPTABLE_PROTOCOL_VERSION = "Connection Refused, unacceptable protocol version";
        public static readonly string CONNECTION_RESPONSE_REFUSED_IDENTIFIER_REJECTED = "Connection Refused, identifier rejected";
        public static readonly string CONNECTION_RESPONSE_REFUSED_SERVER_UNAVAILABLE = "Connection Refused, Server unavailable";
        public static readonly string CONNECTION_RESPONSE_REFUSED_BAD_USER_NAME_OR_PASSWORD = "Connection Refused, bad user name or password";
        public static readonly string CONNECTION_RESPONSE_REFUSED_NOT_AUTHORIZED = "Connection Refused, not authorized";
        public static readonly string CONNECTION_RESPONSE_REFUSED_DISCONNECTED = "Disconnected";
        public static readonly string CONNECTION_RESPONSE_UNKNOWN_ERROR= "Unknown error occured.";
    }
}
