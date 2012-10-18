using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Utilities;

namespace Xmpp.Stanzas
{
    public enum IQType
    {
        Error,
        Get,
        Set,
        Result,
        Query
    }

    public class IQ : Stanza
    {
        private static readonly string _NAME = "iq";

        private static int _id = new Random().Next(100000, 1000000);

        private static Dictionary<IQType, string> _enumDescriptionMap;

        #region Constructors
        
        public IQ() 
            : base(_NAME)
        {
            SetAttribute("id", GetNextId() + "");
        }

        public IQ(IQType type)
            : base(_NAME)
        {
            SetAttribute("id", GetNextId() + "");
            Type = (GetIqType(type));
        }

        public IQ(IQType type, string xmlns) 
            : base(_NAME, xmlns)
        {
            SetAttribute("id", GetNextId() + "");
            Type = (GetIqType(type));
        }

        public IQ(IQType type, String from, String to) 
            : base(_NAME)
        {
            SetAttribute("id", GetNextId() + "");
            Type = (GetIqType(type));
            From = from;
            To = to;
        }
        
        #endregion

        public static string GetNextId()
        {
            var id = ++_id;
            return id.ToString(CultureInfo.InvariantCulture);
        }

        public IQType GetIqType()
        {
            return GetIqType(GetAttribute(_TYPE));
        }

        public static bool IsSet(Packet iq)
        {
            return iq.HasAttribute(_TYPE, "set");
        }

        public static bool IsSuccess(Packet iq)
        {
            return iq.HasAttribute(_TYPE, "result");
        }

        public void AddSession(String xmlns)
        {
            AddChild("session", xmlns);
        }

        public void AddQuery(String xmlns)
        {
            AddChild("query", xmlns);
        }

        public IQ WithQuery(string xmlns)
        {
            AddQuery(xmlns);
            return this;
        }


        private static IQType GetIqType(string type)
        {
            var map = GetDescriptionMap();
            return map.Keys.FirstOrDefault(key => map[key] == type);
        }

        private static string GetIqType(IQType type)
        {
            return GetDescriptionMap()[type];
        }

        private static Dictionary<IQType, string> GetDescriptionMap()
        {
            return _enumDescriptionMap ??
                        (_enumDescriptionMap = new Dictionary<IQType, string>
                                                                     {
                                                                         {IQType.Error, "error"},
                                                                         {IQType.Get, "get"},
                                                                         {IQType.Set, "set"},
                                                                         {IQType.Result, "result"},
                                                                         {IQType.Query, "query"}
                                                                     });
        }

    }
}
