using System;
using System.Collections.Generic;
using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class GetReconciliationReportResponse : SpecificHostedAdminResponseBase
    {
        public readonly IList<ReconciliationTransaction> ReconciliationTransactions;

        public GetReconciliationReportResponse(XmlDocument response)
            : base(response)
        {
            ReconciliationTransactions = new List<ReconciliationTransaction>();

            var enumerator = response.SelectNodes("/response/reconciliation/reconciliationtransaction").GetEnumerator();
            while (enumerator.MoveNext())
            {
                var xmlNode = (XmlNode) enumerator.Current;

                ReconciliationTransactions.Add(new ReconciliationTransaction(
                    TextInt(xmlNode, "./transactionid").Value,
                    TextString(xmlNode, "./customerrefno"),
                    TextString(xmlNode, "./paymentmethod"),
                    MinorCurrencyToDecimalAmount(TextInt(xmlNode, "./amount").Value),
                    TextString(xmlNode, "./currency"),
                    DateTime.Parse(TextString(xmlNode, "./time").Replace("CEST", "+02"))
                    ));
            }
        }
    }
}