using Utilities;

namespace Xmpp
{
    public class Session
    {
        public Stream Stream { get; private set; }
        public Account Account { get; private set; }
        public StanzaManager StanzaManager { get; private set; }
        public string MetaToken { get; set; }

        private StreamProperties _streamProperties;

        public Session(Account account)
        {
            Account = account;
            Stream = new Stream();
            StanzaManager = new StanzaManager();
            
            Stream.OnStanzaReceived += StanzaManager.HandleStanza;

            RegisterHandlers();

        }

        public void Start()
        {

            Logger.Log("Sending stream open");

            _streamProperties = new StreamProperties
            {
                Account = Account
            };

            Stream.Open(_streamProperties);
        }

        public void End()
        {
            Stream.Close();
            Stream = null;
            StanzaManager = null;
        }

        private void RegisterHandlers()
        {
            new FeatureNegotiationHandler(this);
        }

    }
}
