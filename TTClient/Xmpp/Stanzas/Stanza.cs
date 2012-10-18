using System.Runtime.Serialization;
using Utilities;

namespace Xmpp.Stanzas
{
    [DataContract]
    public class Stanza : Packet
    {
        protected const string _TYPE = "type";
        protected const string _FROM = "from";
        protected const string _ID = "id";
        protected const string _TO = "to";
        protected const string _XMLNS = "jabber:client";

        
        public Stanza(string name) : base(name, _XMLNS)
        {}

        public Stanza(string name, string xmlns) : base(name, xmlns)
        {}

        [DataMember]
        public string From
        {
            get { return HasAttribute(_FROM) ? GetAttribute(_FROM) : ""; }
            set { SetAttribute(_FROM, value); }
        }

        [DataMember]
        public string To
        {
            get { return HasAttribute(_TO) ? GetAttribute(_TO) : ""; }
            set { SetAttribute(_TO, value); }
        }

        [DataMember]
        public string Id
        {
            get { return HasAttribute(_ID) ? GetAttribute(_ID) : ""; }
            set { SetAttribute(_ID, value); }
        }

        [DataMember]
        public string Type
        {
            get { return HasAttribute(_TYPE) ? GetAttribute(_TYPE) : ""; }
            set { SetAttribute(_TYPE, value); }
        }

        [DataMember]
        public string FromBare
        {
            get { return RemoveResource(From); }
        }

        [DataMember]
        public string ToBare
        {
            get { return RemoveResource(To); }
        }

        public static string RemoveResource(string jid)
        {
            return jid.IndexOf("/", System.StringComparison.Ordinal) != -1 
                ? jid.Substring(0, jid.IndexOf("/", System.StringComparison.Ordinal)) 
                : jid;
        }
    }
}
