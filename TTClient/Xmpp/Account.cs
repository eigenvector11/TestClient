using System.Runtime.Serialization;

namespace Xmpp
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public string Server { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public string Jid { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string Resource { get; set; }


        public Account()
        {
            Port = 5222;
        }
    }
}
