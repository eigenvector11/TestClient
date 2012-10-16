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

            foreach (var mechanism in mechanisms.Where(mechanism => mechanism.Value == "PLAIN-PW-TOKEN"))
            {
                var pwAuth = new AuthenticationPlainPw(Session);
                pwAuth.Authenticate();
                return packet;
            }
            return packet;
        }

    }

}
