using Xmpp.Stanzas;

namespace Xmpp
{
    public class RosterItem
    {
        private string _imageUrl;
        private Account _account;

        public string Id { get; set; }

        public string AccountKey { get; set; }

        public string Group { get; set;  }

        public ServiceType ServiceType { get; set; }

        public string Jid { get; set; }

        public string Name { get; set; }

        public string StatusMessage { get; set; }

        public string Subscription { get; set; }

        public Show Show { get; set; }

        public string CountryCode { get; set; }

        public string MobileNumber { get; set; }

        public bool IsVerified { get; set; }

        public string ImageUrl
        {
            get 
            {
                if (ServiceType != ServiceType.Facebook) return _imageUrl;
                var jidId = Jid.Substring(Jid.IndexOf("-", System.StringComparison.Ordinal) + 1, Jid.IndexOf("@", System.StringComparison.Ordinal) - 1);
                return "http://graph.facebook.com/" + jidId + "/picture";
            }
            set { _imageUrl = value; }
        }

        public bool IsOnline
        {
            get
            {
                return Show == Show.Away || Show == Show.Chat || Show == Show.Dnd || Show == Show.Xa || CanSendSms;
            }
        }

        public bool CanSendSms { get { return true; } }

        public RosterItem()
        {}

        public RosterItem(Account account, string jid, string name, string group, string subscription)
        {
            Initialize(account, jid, name, group, subscription);
        }

        public void Initialize(Account account, string jid, string name, string group, string subscription)
        {
            _account = account;
            Jid = jid;
            Name = name;
            Group = group;
            Subscription = subscription;
            MarkOffline();
        }

        public void UpdatePresence(Presence presence)
        {
            if (presence.Type == PresenceType.Unavailable)
            {
                MarkOffline();
            }
            else
            {
                Show = presence.Show;
                StatusMessage = presence.Status;
            }
        }

        public void MarkOffline()
        {
            StatusMessage = "";
            Show = Show.Unknown;
        }

        

    }
}
