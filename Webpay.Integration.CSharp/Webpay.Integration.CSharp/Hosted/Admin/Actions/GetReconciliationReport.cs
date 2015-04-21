using System;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class GetReconciliationReport
    {
        public DateTime Date { get; private set; }

        public GetReconciliationReport(DateTime date)
        {
            Date = date;
        }
    }
}