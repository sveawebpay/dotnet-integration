using System.Xml;
using Webpay.Integration.Hosted.Admin.Response;

namespace Webpay.Integration.Hosted.Admin.Actions;

public class GetReconciliationReport:BasicRequest
{
    public readonly DateTime Date;

    public GetReconciliationReport(DateTime date, Guid? correlationId) : base(correlationId)
    {
        Date = date;
    }

    public static GetReconciliationReportResponse Response(XmlDocument responseXml)
    {
        return new GetReconciliationReportResponse(responseXml);
    }
}