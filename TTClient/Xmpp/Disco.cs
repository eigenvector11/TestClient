using Utilities;
using Xmpp.Stanzas;

namespace Xmpp
{
    public class Disco : IqHandler
    {
        public Disco(Session session) : base(session)
        {}

        public override string Name
        {
            get { return "Disco handler"; }
        }

        public override Packet Handle(Packet packet)
        {
            if (packet.GetAttribute("type") == "result")
            {
                Logger.Log("Features discovered");
            }
            return packet;
        }

        public void Discover()
        {
            Logger.Log("Discovering features");
            var iq = new IQ(IQType.Get) { To = Session.Account.Domain };
            iq.AddQuery("http://jabber.org/protocol/disco#info");

            Register(iq);
            Stream.Send(iq);

        }
    }
}
