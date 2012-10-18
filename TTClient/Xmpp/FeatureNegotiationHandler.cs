using System;
using System.Linq;
using Utilities;

namespace Xmpp
{
    public sealed class FeatureNegotiationHandler : StanzaHandler
    {
        public event Func<Packet, Packet> OnAuthenticated;
        public event Func<Packet, Packet> OnResourceBinding;

        public FeatureNegotiationHandler(Session session)
            : base(session)
        {
            OnAuthenticated = packet => packet;
            OnResourceBinding = packet => packet;
        }


        public override string Name 
        {
            get { return "Feature Negotiation"; }
        }
        
        public override bool HandlingCondition(Packet packet)
        {
            return (packet.Name == "features" && packet.HasChild("mechanisms")) ||
                   (packet.Name == "features" && packet.HasChild("bind"));
        }

        public override Packet Handle(Packet packet)
        {
            if (packet.HasChild("mechanisms"))
            {
                var mechanisms = packet.GetChild("mechanisms").GetChildren("mechanism");

                if (mechanisms.Any(mechanism => mechanism.Value == "PLAIN-PW-TOKEN"))
                {
                    var pwAuth = new AuthenticationPlainPw(Session);
                    pwAuth.OnAuthentication += stanza => OnAuthenticated(stanza);
                    pwAuth.Authenticate();
                }
            }

            if (packet.HasChild("bind"))
            {
                var binder = new ResourceBinder(Session);
                binder.OnBinding += stanza => OnResourceBinding(stanza);
                binder.Bind(Session.Account.Resource);
            }

            return packet;
        }

        public void RegisterForNegotiation()
        {
            Register();
        }

    }

}
