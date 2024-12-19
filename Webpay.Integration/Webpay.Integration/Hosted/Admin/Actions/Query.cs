using System.Xml;
using Webpay.Integration.Hosted.Admin.Response;

namespace Webpay.Integration.Hosted.Admin.Actions;

public abstract class Query
{
    public static QueryResponse Response(XmlDocument responseXml)
    {
        return new QueryResponse(responseXml);
    }
}