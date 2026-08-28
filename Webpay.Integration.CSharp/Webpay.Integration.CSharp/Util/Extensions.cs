using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Webpay.Integration.CSharp.Util
{
    public static class Extensions
    {
        public static string XmlEscape(this string unescaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }

        public static string XmlUnescape(this string escaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return node.InnerText;
        }

        public static string GetXml(this KeyValuePair<string, string> item)
        {
            return new XElement("item",
                new XElement("key", item.Key),
                new XElement("value", item.Value)
            ).ToString(SaveOptions.DisableFormatting);
        }
    }
}
