using System.Linq;
using Utilities;

namespace Xmpp
{
    public sealed class FeatureNegotiationHandler : StanzaHandler
    {
        public FeatureNegotiationHandler(Session session)
            : base(session)
        {

        }

        public override string Name 
        {
            get { return "Feature Negotiation"; }
        }
        
        public override bool HandlingCondition(Packet packet)
        {
            return packet.Name == "features" && packet.HasChild("mechanisms");
        }

        public override Packet Handle(Packet packet)
        {
            var child = packet.GetChild("mechanisms");
            var mechanisms = child.GetChildren("mechanism");

            if (string.IsNullOrEmpty(Session.MetaToken))
            {
                foreach (var mechanism in mechanisms.Where(mechanism => mechanism.Value == "PLAIN"))
                {
                    new AuthenticationPlain(Session, packet);
                    return packet;
                }
            }
            else
            {
                foreach (var mechanism in mechanisms.Where(mechanism => mechanism.Value == "PLAIN-PW-TOKEN"))
                {
                    new AuthenticationPlainPw(Session, packet);
                    return packet;
                }
            }
            return packet;
        }

    }

}
