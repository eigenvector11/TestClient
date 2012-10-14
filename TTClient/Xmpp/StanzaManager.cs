using System;
using Utilities;

namespace Xmpp
{
    public class StanzaManager
    {
        private event Func<Packet, Packet> OnPacketReceived;

        public StanzaManager()
        {
            OnPacketReceived = packet => packet;
        }

        public void RegisterHandler(StanzaHandler stanzaHandler)
        {
            Logger.Log("Registering handler - " + stanzaHandler.Name);
            OnPacketReceived += packet =>
            {
                var toProcess = false;
                try
                {
                    toProcess = stanzaHandler.HandlingCondition(packet);
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception while evaluating stanza handling condition " + stanzaHandler.Name + " for packet " + packet);
                    Logger.Log(ex);
                }

                if (toProcess)
                {
                    try
                    {
                        stanzaHandler.Handle(packet);
                    }
                    catch (Exception ex1)
                    {
                        Logger.Log("Exception while processing packet handler " + stanzaHandler.Name + " for packet " + packet);
                        Logger.Log(ex1);
                    }
                }

                return packet;
            };
        }

        public Packet HandleStanza(Packet packet)
        {
            Logger.Log("Handling packet at Xmpp ...");
            OnPacketReceived(packet);
            return packet;
        }


    }
}
