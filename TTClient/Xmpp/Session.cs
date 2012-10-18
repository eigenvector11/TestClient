using Utilities;

namespace Xmpp
{
    public class Session
    {
        public Stream Stream { get; private set; }
        public Account Account { get; private set; }
        public StanzaManager StanzaManager { get; private set; }
        public AuthToken Token { get; set; }

        private StreamProperties _streamProperties;

        public Session(Account account)
        {
            Account = account;
            Stream = new Stream();

            StanzaManager = new StanzaManager();
            Stream.OnStanzaReceived += StanzaManager.HandleStanza;

            SetupNegotiation();
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

        private void SetupNegotiation()
        {
            var featureNegotiation = new FeatureNegotiationHandler(this);

            featureNegotiation.OnAuthenticated += packet =>
            {
                Stream.Restart();
                return packet;
            };

            featureNegotiation.OnResourceBinding += packet =>
            {
                var disco = new Disco(this);
                disco.Discover();
                return packet;
            };

            featureNegotiation.RegisterForNegotiation();
        }


    }
}
