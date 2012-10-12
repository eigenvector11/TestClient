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
        }

        public void Start()
        {
            _stream = new Stream();
            _stanzaManager = new StanzaManager();
            _stream.OnStanzaReceived += _stanzaManager.HandleStanza;


            Logger.Log("Sending stream open");

            _streamProperties = new StreamProperties
            {
                Account = _account
            };

            _stream.Open(_streamProperties);

            _stream.Send(_streamProperties.OpeningTag());
        }

        public void Close()
        {
            _stream.Close();
        }

    }
}
