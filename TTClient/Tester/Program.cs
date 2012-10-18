using System;
using Xmpp;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var session = new Session(new Account
                                          {
                                              Jid = "chaitanya.g.test@directi.com",
                                              Password = "qwedsa",
                                              Port = 5222,
                                              Server = "talkto.directi.com",
                                              Domain = "directi.com",
                                              Resource = "myresource"
                                          });
            session.Start();
            Console.ReadLine();
        }
    }
}
