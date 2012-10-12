using System.Text;
using System.Runtime.Serialization;

namespace Xmpp
{
    [DataContract]
    public class StreamProperties
    {
        [DataMember]
        public Account Account { get; set; }

        [DataMember]
        public string ServiceType { get; set; }

        [DataMember]
        public string DeviceId { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public string Route { get; set; }

        public string OpeningTag()
        {
            if (string.IsNullOrEmpty(To))
                To = Account.Domain;

            var openingTag = new StringBuilder();

            openingTag.Append("<?xml version='1.0'?>");
            openingTag.Append("<stream:stream ");
            openingTag.Append("to='" + To + "'  ");
            openingTag.Append("version='1.0' ");
            openingTag.Append("xml:lang='en' ");
            openingTag.Append("xmlns='jabber:client' ");
            openingTag.Append("xmlns:stream='http://etherx.jabber.org/streams' ");
            openingTag.Append(">");

            return openingTag.ToString();
        }

    }
}
