using Utilities;
using Xmpp.Stanzas;

namespace Xmpp
{
    public class ResourceBinder : IqHandler
    {
        public ResourceBinder(Session session) : base(session)
        {}


        public override string Name
        {
            get { return "Resource Binder"; }
        }

        public override Packet Handle(Packet packet)
        {
            if (packet.HasChild("bind"))
            {
                var jid = packet.GetChild("bind").GetChild("jid").Value;
                Logger.Log("Full jid = " + jid);
            }
            return packet;
        }

        public void Bind(string resource)
        {
            var packet = new Packet("resource") { Value = resource };

            var binder = new Packet("bind", "urn:ietf:params:xml:ns:xmpp-bind");
            binder.AddChild(packet);

            var iq = new IQ(IQType.Set);
            iq.AddChild(binder);

            Register(iq);
            Stream.Send(iq);
        }
    }
}
