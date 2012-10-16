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

        public string StartStream()
        {
            return "<?xml version='1.0'?>" + RestartStream();
        }

        public string RestartStream()
        {
            if (string.IsNullOrEmpty(To))
                To = Account.Domain;

            var conn = new StringBuilder();

            conn.Append("<stream:stream ");
            conn.Append("to='" + To + "'  ");
            conn.Append("version='1.0' ");
            conn.Append("xml:lang='en' ");
            conn.Append("xmlns='jabber:client' ");
            conn.Append("xmlns:stream='http://etherx.jabber.org/streams' ");
            conn.Append(">");

            return conn.ToString();
        }


    }
}
