using System;
using System.Collections.Generic;
using System.Threading;

namespace Utilities
{
    public class PacketQueue
    {
        private readonly List<Packet> _packetQ;
        private readonly object _lockQ;

        public bool Stopped { get; private set; }

        public PacketQueue()
        {
            _packetQ = new List<Packet>();
            _lockQ = new object();
            Stopped = false;
        }

        public int Count
        {
            get
            {
                return _packetQ == null ? 0 : _packetQ.Count;
            }
        }

        public Packet Process(Action<Packet> onPacketProcess)
        {
            var packet = Dequeue();
            onPacketProcess(packet);
            return packet;
        }

        public void Enqueue(Packet packet)
        {
            lock (_lockQ)
            {
                if (_packetQ == null) return;
                if (Stopped) return;

                _packetQ.Add(packet);
                // Logger.Log("Enqueuing stanza, count = " + _packetQ.Count);
                Monitor.Pulse(_lockQ);
            }
        }

        public Packet Dequeue()
        {
            lock (_lockQ)
            {
                if (_packetQ == null) return null;
                if (Stopped) return null;

                if (_packetQ.Count == 0)
                {
                    Monitor.Wait(_lockQ);
                }

                // Logger.Log("Dequeuing stanza, count = " + _packetQ.Count);
                var packet = _packetQ[0];
                _packetQ.RemoveAt(0);
                return packet;
            }
        }

        public Packet Peek()
        {
            lock (_lockQ)
            {
                if (_packetQ == null) return null;

                if (_packetQ.Count == 0)
                {
                    Monitor.Wait(_lockQ);
                }

                return _packetQ[0];
            }
        }

        public void Pulse(Packet packetForStop)
        {
            lock (_lockQ)
            {
                // Stopped = true;
                _packetQ.Clear();
                _packetQ.Add(packetForStop);
                Monitor.Pulse(_lockQ);
            }
        }
        
        public void Clear()
        {
            lock (_lockQ)
            {
                if (_packetQ != null)
                    _packetQ.Clear();
            }
        }
    }
}
