using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Utilities;

namespace Xmpp.Stanzas
{
    public enum SessionStatus
    {
        Available,
        Busy,
        Away
    }

    public enum Show
    {
        Chat,
        Dnd,
        Xa,
        Away,
        NotSpecified,
        Invisible,
        Unknown
    }

    public enum PresenceType
    {
        Error,
        Probe,
        Subscribe,
        Subscribed,
        Unavailable,
        Unsubscribe,
        Unsubscribed,
        Available
    }

    public class Presence : Stanza
    {
        private const string _PRIORITY = "priority";
        private const string _SHOW = "show";
        private const string _STATUS = "status";
        private const string _NAME = "presence";

        private static readonly Dictionary<PresenceType, string> PresenceTypeMap;
        private static readonly Dictionary<Show, string> ShowMap;
        private static readonly Dictionary<string, Show> StringShowMap;

        #region Constructors
        static Presence()
        {
            PresenceTypeMap = new Dictionary<PresenceType, string>
                                {
                                    {PresenceType.Error, "error"},
                                    {PresenceType.Probe, "probe"},
                                    {PresenceType.Subscribe, "subscribe"},
                                    {PresenceType.Available, "available"},
                                    {PresenceType.Subscribed, "subscribed"},
                                    {PresenceType.Unavailable, "unavailable"},
                                    {PresenceType.Unsubscribe, "unsubscribe"},
                                    {PresenceType.Unsubscribed, "unsubscribed"}
                                };

            ShowMap = new Dictionary<Show, string>
                            {
                                {Show.Chat, "default"},
                                {Show.Away, "away"},
                                {Show.Dnd, "dnd"},
                                {Show.Invisible, "invisible"},
                                {Show.NotSpecified, "notspecified"},
                                {Show.Xa, "xa"},
                                {Show.Unknown, "unknown"}
                            };

            StringShowMap = new Dictionary<string, Show>
                                {
                                    {"default", Show.Chat},
                                    {"chat", Show.Chat},
                                    {"available", Show.Chat},
                                    {"dnd", Show.Dnd},
                                    {"xa", Show.Away},
                                    {"away", Show.Away},
                                    {"invisible", Show.Invisible}
                                };
        }

        public Presence(string name)
            : base(name)
        {
        }

        public Presence(string name, string xmlns)
            : base(name, xmlns)
        {
        }

        public Presence(PresenceType type, string from, string to)
            : base(_NAME, _XMLNS)
        {
            base.Type = GetPresenceType(type);
            From = from;
            To = to;
        }
        
        #endregion

        public int Priority
        {
            get
            {
                var priority = GetChild(_PRIORITY);
                return priority != null ? Int32.Parse(priority.Value) : 0;
            }

            set 
            {
                var priority = new Packet(_PRIORITY)
                                   {
                                       Value = (value > 0 ? value : 0).ToString(CultureInfo.InvariantCulture)
                                   };
                AddChild(priority);
            }
        }

        public Show Show
        {
            get 
            { 
                var show = GetChild(_SHOW);
                return show == null ? Show.Chat : GetShow(show.Value);
            }
            set 
            {
                var show = new Packet(_SHOW)
                               {
                                   Value = (value != Show.NotSpecified && value != Show.Unknown) ? GetShow(value) : null
                               };
                AddChild(show);
            }
        }

        public string Status
        {
            get
            {
                var status = GetChild(_STATUS);
                return status == null ? "" : status.Value;
            }

            set 
            {
                var status = new Packet(_STATUS) { Value = value };
                AddChild(status);
            }
        }
        
        public new PresenceType Type
        {
            get
            {
                var type = GetAttribute(_TYPE);
                return type != null ? GetPresenceType(type) : PresenceType.Available;
            }
            set
            {
                base.Type = PresenceTypeMap[value];
            }
        }

        private static Show GetShow(string show)
        {
            return StringShowMap[show];
        }

        private static string GetShow(Show show)
        {
            return ShowMap[show];
        }

        private static string GetPresenceType(PresenceType presenceType)
        {
            return PresenceTypeMap[presenceType];
        }

        private static PresenceType GetPresenceType(string presenceType)
        {
            return PresenceTypeMap.Keys.FirstOrDefault(key => PresenceTypeMap[key] == presenceType);
        }

    }
}
