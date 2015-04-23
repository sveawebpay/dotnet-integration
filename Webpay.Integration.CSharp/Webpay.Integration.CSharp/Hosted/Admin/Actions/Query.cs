using System;
using System.Collections.Generic;
using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public abstract class Query
    {
        public static QueryResponse Response(XmlDocument responseXml)
        {
            return new QueryResponse(responseXml);
        }
    }

}