using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;

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