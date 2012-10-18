using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Text;
using System.Net.Sockets;
using System.Xml;
using Utilities;

namespace Xmpp
{
    /// <summary>
    /// Represents an xmpp stream. Provides an event for handling stanzas and methods for sending stanzas
    /// </summary>
    public class Stream
    {
        private Utilities.Socket _socket;
        private NetworkStream _stream;
        private XmlReader _reader;
        private bool _processStream;
        private Stack<Packet> _xmlBuildStack;
        private PacketQueue _stanzaQueue;
        private Thread _xmlProcessingThread;
        private Thread _stanzaProcessingThread;

        

        public StreamProperties Properties { get; private set; }

        /// <summary>
        /// Fired when an XMPP stanza is received
        /// </summary>
        public event Func<Packet, Packet> OnStanzaReceived;

        [DataMember]
        public bool IsConnected { get; private set; }

        public void Open(StreamProperties streamProperties)
        {
            Properties = streamProperties;
            var hostname = Properties.Account.Server;
            var port = Properties.Account.Port;
            _socket = new Utilities.Socket(hostname, port);
            _stream = _socket.Connect();

            StartStreamProcessing();

            Send(Properties.StartStream());
        }

        public void Restart()
        {
            Send(Properties.RestartStream());
        }

        public void Close()
        {
            Send(Properties.CloseStream());

            StopStreamProcessing();

            _socket.Disconnect();
        }

        public void Send(string str)
        {
            if (_stream == null) return;

            if (!_stream.CanWrite) return;

            Logger.Log("SEND:\n" + str);

            var bytes = Encoding.UTF8.GetBytes(str);
            _stream.Write(bytes, 0, bytes.Length);
        }

        public void Send(Packet packet)
        {
            Send(packet.ToString());
        }

        private void StartStreamProcessing()
        {
            _processStream = true;

            _xmlBuildStack = new Stack<Packet>();
            _stanzaQueue = new PacketQueue();

            _xmlProcessingThread = new Thread(ProcessXml) { Name = "Xml Processing Thread " + DateTime.Now };
            _xmlProcessingThread.Start();

            _stanzaProcessingThread = new Thread(ProcessStanzas) { Name = "Stanza Processing Thread " + DateTime.Now };
            _stanzaProcessingThread.Start();

        }

        private void StopStreamProcessing()
        {
            _processStream = false;

            CloseXmlReader();
            StopThread(_xmlProcessingThread);
            _stream.Close();

            StopThread(_stanzaProcessingThread);

        }

        private static void StopThread(Thread thread)
        {
            if (thread == null) return;
            if (!thread.IsAlive) return;
            thread.Join();
        }

        private void InitializeXmlReader()
        {
            if (_stream == null) return;

            var nt = new NameTable();
            var nsmgr = new XmlNamespaceManager(nt);
            nsmgr.AddNamespace("stream", "stream");

            var context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

            var settings = new XmlReaderSettings
                               {
                                   ConformanceLevel = ConformanceLevel.Fragment,
                                   CheckCharacters = false,
                                   IgnoreWhitespace = true,
                                   IgnoreProcessingInstructions = true,
                               };

            _reader = XmlReader.Create(_stream, settings, context);
        }

        private void CloseXmlReader()
        {
            if (_reader == null) return;
            _reader.Close();
        }

        private void ProcessXml()
        {
            InitializeXmlReader();

            try
            {
                while (_reader.Read())
                {
                    switch (_reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                Packet packet;
                                if (_reader.IsEmptyElement)
                                {
                                    packet = GetXElement();
                                    PushPacketOnStackOrProcessPacket(packet);
                                }
                                else if (_reader.Name.Equals("stream:stream"))
                                {
                                    Logger.Log("Received stream open");
                                    _reader.MoveToElement();
                                }
                                else
                                {
                                    packet = GetXElement();
                                    _xmlBuildStack.Push(packet);
                                }
                            }
                            break;

                        case XmlNodeType.Text:
                            {
                                if (_xmlBuildStack.Peek().Name.Contains("error") && _xmlBuildStack.Count <= 1)
                                {
                                    Logger.Log("Received stream error: " + _reader.Name);
                                }
                                if (_xmlBuildStack.Count > 0)
                                {
                                    _xmlBuildStack.Peek().Value = _reader.Value;
                                }
                            }
                            break;


                        case XmlNodeType.Whitespace:
                            {
                            }
                            break;

                        case XmlNodeType.EndElement:
                            {
                                if (_reader.Name.Contains("stream:stream"))
                                {
                                    Logger.Log("Received stream close");
                                    Close();
                                    break;
                                }

                                var node = _xmlBuildStack.Peek();
                                _xmlBuildStack.Pop();
                                PushPacketOnStackOrProcessPacket(node);
                            }
                            break;
                    }

                }

            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        private Packet GetXElement()
        {
            var packet = new Packet(_reader.LocalName);
            if (!_reader.HasAttributes) return packet;

            while (_reader.MoveToNextAttribute())
            {
                packet.SetAttribute(_reader.LocalName, _reader.Value);
            }

            if (_xmlBuildStack.Count > 0)
            {
                packet.UpdateXmlnsIfEmpty(_xmlBuildStack.Peek());
            }

            _reader.MoveToElement();
            return packet;
        }

        private void PushPacketOnStackOrProcessPacket(Packet packet)
        {
            if (_xmlBuildStack.Count > 0)
            {
                _xmlBuildStack.Peek().AddChild(packet);
            }
            else
            {
                if (packet != null)
                {
                    Logger.Log("RECD:\n" + packet);
                    HandlePacket(packet);
                }
            }
        }

        private void HandlePacket(Packet packet)
        {
            if (packet == null) return;

            // Logger.Log("Handling packet ...");

            if (packet.Name.Equals("empty"))
            {
                Logger.Log("Packet is empty");
            }

            if (packet.Name.Equals("error"))
            {
                Logger.Log(packet.HasChild("host-unknown")
                               ? "error - host unknown"
                               : "error");
            }

            if (!packet.HasAttribute("to"))
            {
                // Logger.Log("Packet does not have To attribute");
            }
            else
            {
                if (!packet.GetAttribute("to").ToLower().Trim().StartsWith(Properties.Account.Jid.ToLower().Trim()))
                {
                    // Logger.Log("Packet To attribute does not start with Jid " + Properties.Account.Jid);
                }
            }

            _stanzaQueue.Enqueue(packet);
        }

        private void ProcessStanzas()
        {
            while (_processStream)
            {
                var packet = _stanzaQueue.Dequeue();
                OnStanzaReceived(packet);
            }
        }

    }
}
