namespace Xmpp
{
    public class AuthToken
    {
        public string Value { get; private set; }
        public int ExpiryMinutes { get; private set; } 

        public AuthToken(string value, int expiryMinutes)
        {
            Value = value;
            ExpiryMinutes = expiryMinutes;
        }
    }
}
