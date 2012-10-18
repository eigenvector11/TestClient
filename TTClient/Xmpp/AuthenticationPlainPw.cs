using System;
using System.Globalization;
using System.Text;
using Utilities;

namespace Xmpp
{
    public class AuthenticationPlainPw : StanzaHandler
    {
        private Account _account;

        public event Func<Packet, Packet> OnAuthentication;

        public AuthenticationPlainPw(Session session) : base(session)
        {
            _account = Stream.Properties.Account;
            OnAuthentication = packet => packet;
        }

        override public string Name
        {
            get { return "Auth Plain PW"; }

        }

        public override bool HandlingCondition(Packet packet)
        {
            return (packet.Name == "success" && packet.HasAttribute("xmlns", "urn:ietf:params:xml:ns:xmpp-sasl")) ||
                   (packet.Name == "failure" && packet.HasAttribute("xmlns", "urn:ietf:params:xml:ns:xmpp-sasl"));

        }

        public override Packet Handle(Packet packet)
        {
            if (packet.Name == "failure")
            {
                // Logger.Log("Auth FAIL - " + packet);
            }
            if (packet.Name == "success")
            {
                var token = packet.GetChild("pw-token");
                var expiry = Int32.Parse(token.GetAttribute("expires_in_minutes"));
                var value = token.Value;
                Session.Token = new AuthToken(value, expiry);
                OnAuthentication(packet);
            }
            return packet;
        }

        public void Authenticate()
        {
            Register();
            var auth = GetAuthPacket();
            Stream.Send(auth);
        }

        private Packet GetAuthPacket()
        {
            var auth = new Packet("auth", "urn:ietf:params:xml:ns:xmpp-sasl");
            auth.SetAttribute("mechanism", "PLAIN-PW-TOKEN");

            var userId = _account.Jid;
            if (userId.Contains('@'.ToString(CultureInfo.InvariantCulture)))
                userId = userId.Substring(0, _account.Jid.IndexOf('@'));

            var token = "\0" + userId + "\0" + _account.Password + "\0" + "somerandomstring";
            Logger.Log("Token = " + token);

            auth.Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            return auth;
        }
    }
}
