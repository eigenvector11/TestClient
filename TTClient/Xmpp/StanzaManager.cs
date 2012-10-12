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

        public void RegisterHandler(string name, Func<Packet, bool> processCondition, Func<Packet, Packet> packetAction)
        {

            OnPacketReceived += packet =>
            {
                var toProcess = false;
                try
                {
                    toProcess = processCondition(packet);
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception while evaluating process-condition " + name + " for packet " + packet);
                    Logger.Log(ex);
                }

                if (toProcess)
                {
                    try
                    {
                        packetAction(packet);
                    }
                    catch (Exception ex1)
                    {
                        Logger.Log("Exception while processing packet handler " + name + " for packet " + packet);
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
