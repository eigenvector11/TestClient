using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Xmpp.Stanzas;

namespace Xmpp
{
    public class IqHandler : StanzaHandler
    {
        private IQ _iq;

        public IqHandler(Session session) : base(session)
        {}

        public override string Name
        {
            get { return "IQ handler"; }
        }

        public override bool HandlingCondition(Packet packet)
        {
            return packet.Name == "iq" && packet.GetAttribute("id") == _iq.Id;
        }

        public override Packet Handle(Packet packet)
        {
            return packet;
        }

        protected void Register(IQ iq)
        {
            _iq = iq;
            Register();
        }
    }
}
