using System;

namespace Webpay.Integration.CSharp.Hosted.Admin
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