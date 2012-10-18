using Utilities;
using Xmpp.Stanzas;

namespace Xmpp
{
    public abstract class IqHandler : StanzaHandler
    {
        private IQ _iq;

        protected IqHandler(Session session) : base(session)
        {}

        public override string Name
        {
            get { return "IQ handler"; }
        }

        public override bool HandlingCondition(Packet packet)
        {
            return packet.Name == "iq" && packet.GetAttribute("id") == _iq.Id;
        }

        protected void Register(IQ iq)
        {
            _iq = iq;
            Register();
        }
    }
}
