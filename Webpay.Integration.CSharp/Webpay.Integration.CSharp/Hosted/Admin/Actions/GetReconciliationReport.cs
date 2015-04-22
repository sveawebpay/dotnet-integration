using System;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class GetReconciliationReport
    {
        public readonly DateTime Date;

        public GetReconciliationReport(DateTime date)
        {
            Date = date;
        }
    }
}