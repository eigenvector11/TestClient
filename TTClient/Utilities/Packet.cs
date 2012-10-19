using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;


namespace Utilities
{
    [DataContract]

    // Wrapper class for System.Xml.Linq.XElement
    public class Packet
    {
        [DataMember]
        private XElement _element;


        public XElement Element
        {
            get { return _element; }

            set { _element = value; }
        }

        public String Name
        {
            get { return _element.Name.LocalName; }
        }

        public String Value
        {
            get { return _element.Value; }

            set { _element.Value = value; }

        }

        #region Constructors

        public Packet(String name) : this(name, null)
        {

        }

        public Packet(XElement element)
        {
            _element = element;
        }

        public Packet(String name, String xmlns, String namespacePrefix)
        {
            _element = new XElement(name);
        }

        public Packet(String name, String xmlns)
        {
            if (xmlns != null)
            {
                XNamespace _namespace = xmlns;
                _element = new XElement(_namespace + name, new XAttribute("xmlns", xmlns));
            }
            else
            {
                _element = new XElement(name);
            }
        }
        
        #endregion


        #region Operations on Attributes

        public bool HasAttribute(String name)
        {
            if (name == "xmlns")
            {
                return true;
            }
            return _element.Attribute(name) != null;
        }

        public bool HasAttribute(String name, String value)
        {
            if (name == "xmlns")
            {
                return _element.Name.Namespace.NamespaceName == value;
            }

            var nAttribute = _element.Attribute(name);
            return nAttribute != null && nAttribute.Value.Equals(value);
        }

        public String GetAttribute(String name)
        {
            if (name == "xmlns")
            {
                return _element.Name.Namespace.NamespaceName;
            }

            XAttribute ret;

            if ((ret = _element.Attribute(name)) != null)
            {
                return ret.Value;
            }
            return null;
        }

        public void SetAttribute(String name, string value)
        {
            if (value == null) return;

            if (name == "xmlns")
            {
                XNamespace aw = value;

                if (string.IsNullOrEmpty(_element.Name.Namespace.NamespaceName))
                    _element.Name = aw.GetName(_element.Name.LocalName);

            }
            else
                _element.SetAttributeValue(name, value);
        }

        #endregion


        #region Operations on Children
        public int ChildrenCount()
        {
            return _element.Elements().Count();
        }

        public bool HasChild(String name)
        {
            return _element.Elements().Any(e => e.Name.LocalName == name);
        }

        public Packet GetChild(String name)
        {

            var element = _element.Elements().Where(e => e.Name.LocalName == name);

            return element.Select(xElement => new Packet(xElement)).FirstOrDefault();
        }

        public bool RemoveChild(Packet child)
        {
            var elements = _element.Elements().Where(e => e.Name.LocalName == child.Name);

            if (elements.Any())
            {
                elements.Remove();
                return true;
            }
            return false;
        }

        public List<Packet> GetChildren(String name)
        {
            var element = _element.Elements().Where(e => e.Name.LocalName == name);

            return element.Select(xElement => new Packet(xElement)).ToList();

        }

        public List<Packet> GetChildren()
        {
            var element = _element.Elements();

            return element.Select(xElement => new Packet(xElement)).ToList();
        }

        public void AddChild(Packet child)
        {
            var aw = _element.Name.Namespace;
            var xmlElement = child.Element;
            if (!aw.Equals(XNamespace.None))
            {
                while (xmlElement.Name.Namespace == XNamespace.None)
                {
                    /*if (String.IsNullOrEmpty(child.getXElement().Name.Namespace.NamespaceName))
                    {   
                        child.getXElement().Name = aw.GetName(child.getXElement().Name.LocalName);
                    }*/
                    SetNameSpace(aw, xmlElement);

                }
            }
            _element.Add(child.Element);
        }

        public void AddChild(String name, String xmlns)
        {

            XNamespace nameSpace = xmlns;
            if (nameSpace != null)
            {
                var element = new XElement(nameSpace + name, new XAttribute("xmlns", xmlns));
                _element.Add(element);
            }
            else
            {
                var element = new XElement(_element.Name.Namespace + name);
                _element.Add(element);
            }
        }
        
        #endregion


        protected void add(Packet node)
        {
            _element.Add(node.Element);
        }

        public void CloneAttributesAndChildrenFrom(Packet packetToCloneFrom)
        {
            var attribute = packetToCloneFrom.Element.Attributes();
            var element = packetToCloneFrom.Element.Elements();
            foreach (var xElement in element)
            {

            }
            _element.ReplaceAttributes(attribute);
            _element.Add(element);
        }

        public static Packet ConvertPacketFromXml(XElement xmlElement)
        {

            foreach (var el in xmlElement.DescendantsAndSelf())
            {
                var xAttribute = el.Attribute("xmlns");
                if (xAttribute != null)
                {
                    XNamespace aw = xAttribute.Value;

                    if (el.Name.Namespace != aw)
                        el.Name = aw.GetName(el.Name.LocalName);
                }
                else
                {
                    if (el.Parent != null)
                    {
                        el.Name = el.Parent.Name.Namespace.GetName(el.Name.LocalName);
                    }
                }
            }


            return new Packet(xmlElement);
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        public void UpdateXmlnsIfEmpty(Packet peek)
        {
            var aw = peek.Element.Name.Namespace;

            if (string.IsNullOrEmpty(_element.Name.Namespace.NamespaceName))
                _element.Name = aw.GetName(_element.Name.LocalName);

        }

        private static bool SetNameSpace(XNamespace aw, XElement xmlElement)
        {
            if (xmlElement.Name.Namespace == XNamespace.None)
            {
                xmlElement.Name = aw.GetName(xmlElement.Name.LocalName);
                foreach (var xElement in xmlElement.Descendants())
                {
                    if (xElement.Name.Namespace == XNamespace.None)
                    {
                        xElement.Name = aw.GetName(xElement.Name.LocalName);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private void UpdateXmlns(string name, string value)
        {
            if (value != null)
                _element.SetAttributeValue(name, value);
        }


    }
}
