using System;
using Utilities;

namespace Xmpp
{
    abstract public class StanzaHandler
    {
        protected Session Session { get; set; }
        protected Stream Stream { get; set; }

        abstract public string Name { get; }
        abstract public bool HandlingCondition(Packet packet);
        abstract public Packet Handle(Packet packet);

        protected StanzaHandler(Session session)
        {
            Session = session;
            Stream = session.Stream;
        }

        protected void Register()
        {
            Session.StanzaManager.RegisterHandler(this);
        }



    }
}
