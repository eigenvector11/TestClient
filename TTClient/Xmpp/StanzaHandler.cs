using Utilities;

namespace Xmpp
{
    abstract public class StanzaHandler
    {
        protected Session Session { get; set; }

        abstract public string Name { get; }
        abstract public bool HandlingCondition(Packet packet);
        abstract public Packet Handler(Packet packet);

        protected StanzaHandler(Session session)
        {
            Session = session;
            Register();
        }

        private void Register()
        {
            Session.StanzaManager.RegisterHandler(Name, HandlingCondition, Handler);
        }



    }
}
