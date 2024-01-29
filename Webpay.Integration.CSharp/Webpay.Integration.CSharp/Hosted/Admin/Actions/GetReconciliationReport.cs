using System;
using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class GetReconciliationReport:BasicRequest
    {
        public readonly DateTime Date;

        public GetReconciliationReport(DateTime date, string correlationId) : base(correlationId)
        {
            Date = date;
        }

        public static GetReconciliationReportResponse Response(XmlDocument responseXml)
        {
            return new GetReconciliationReportResponse(responseXml);
        }
    }
}