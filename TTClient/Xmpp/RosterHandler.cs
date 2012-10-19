using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Xmpp
{
    public class RosterHandler : IqHandler
    {
        public RosterHandler(Session session) : base(session)
        {

        }

        public override string Name
        {
            get { return "Roster Hander"; }
        }

        public override Packet Handle(Packet packet)
        {
            return packet;
        }
    }
}
