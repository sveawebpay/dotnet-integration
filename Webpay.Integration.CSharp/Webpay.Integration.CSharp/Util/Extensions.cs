using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
    }
}
