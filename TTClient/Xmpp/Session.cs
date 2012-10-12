using Utilities;

namespace Xmpp
{
    public class Session
    {
        private Stream _stream;
        private Account _account;
        private StanzaManager _stanzaManager;
        private StreamProperties _streamProperties;

        public Session(Account account)
        {
            _account = account;
            _stream = new Stream();
            _stanzaManager = new StanzaManager();
            
            _stream.OnStanzaReceived += _stanzaManager.HandleStanza;

            RegisterHandlers();

        }

        public void Start()
        {

            Logger.Log("Sending stream open");

            _streamProperties = new StreamProperties
            {
                Account = _account
            };

            _stream.Open(_streamProperties);
            _stream.Send(_streamProperties.OpeningTag());

        }

        public void End()
        {
            _stream.Close();
            _stream = null;
            _stanzaManager = null;
        }

        private void RegisterHandlers()
        {
            
        }

    }
}
