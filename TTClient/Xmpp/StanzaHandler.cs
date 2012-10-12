using Utilities;

namespace Xmpp
{
    abstract public class StanzaHandler
    {
        private StanzaManager _stanzaManager;

        abstract public string Name { get; protected set; }
        abstract public bool HandlingCondition(Packet packet);
        abstract public Packet Handler(Packet packet);

        protected StanzaHandler(StanzaManager stanzaManager)
        {
            _stanzaManager = stanzaManager;
            Register();
        }

        private void Register()
        {
            _stanzaManager.RegisterHandler(Name, HandlingCondition, Handler);
        }



    }
}
