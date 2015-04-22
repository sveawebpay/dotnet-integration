using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Webpay.Integration.CSharp.Util.Calculation;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class GetReconciliationReport
    {
        public readonly DateTime Date;

        public GetReconciliationReport(DateTime date)
        {
            Date = date;
        }

        public static GetReconciliationReportResponse Response(XmlDocument responseXml)
        {
            return new GetReconciliationReportResponse(responseXml);
        }
    }

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

                var dateTime = TextString(xmlNode, "./time");

            ReconciliationTransactions.Add(new ReconciliationTransaction(
                    TextInt(xmlNode, "./transactionid").Value,
                    TextString(xmlNode, "./customerrefno"),
                    TextString(xmlNode, "./paymentmethod"),
                    ToAmount(TextInt(xmlNode, "./amount").Value),
                    TextString(xmlNode, "./currency"),
                    DateTime.Parse(TextString(xmlNode, "./time").Replace("CEST", "+02"))
                    
                ));
            }
        }

        private static decimal ToAmount(int amountMinor)
        {
            return MathUtil.BankersRound(((decimal)amountMinor)/100);
        }
    }

    public class ReconciliationTransaction
    {
        public readonly int TransactionId;
        public readonly string CustomerRefNo;
        public readonly string PaymentMethod;
        public readonly decimal Amount;
        public readonly string Currency;
        public readonly DateTime Time;

        public ReconciliationTransaction(int transactionId, string customerRefNo, string paymentMethod, decimal amount, string currency, DateTime time)
        {
            TransactionId = transactionId;
            CustomerRefNo = customerRefNo;
            PaymentMethod = paymentMethod;
            Amount = amount;
            Currency = currency;
            Time = time;
        }
    }
}