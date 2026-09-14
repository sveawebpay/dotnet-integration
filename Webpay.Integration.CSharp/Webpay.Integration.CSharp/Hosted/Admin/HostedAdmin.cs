using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway;
using Webpay.Integration.CSharp.Util;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class HostedAdmin
    {
        public readonly IConfigurationProvider ConfigurationProvider;
        public readonly CountryCode CountryCode;
        public readonly string MerchantId;
        public readonly List<AdminRequestHeader> Headers;
        public HostedAdmin(IConfigurationProvider configurationProvider, CountryCode countryCode)
        {
            ConfigurationProvider = configurationProvider;
            MerchantId = configurationProvider.GetMerchantId(PaymentType.HOSTED, countryCode);
            CountryCode = countryCode;
            Headers = new List<AdminRequestHeader>();
        }

        private static string CreateXml(XElement rootElement)
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                rootElement
            );
            return $"{doc.Declaration}\n{doc}";
        }

        public HostedActionRequest Annul(Annul annul)
        {
            var xml = CreateXml(
                new XElement("annul",
                    new XElement("transactionid", annul.TransactionId)
                )
            );
            AddCorrelationIdHeader(annul.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/annul");
        }

        public HostedActionRequest CancelRecurSubscription(CancelRecurSubscription cancelRecurSubscription)
        {
            var xml = CreateXml(
                new XElement("cancelrecursubscription",
                    new XElement("subscriptionid", cancelRecurSubscription.SubscriptionId)
                )
            );
            AddCorrelationIdHeader(cancelRecurSubscription.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
                "/cancelrecursubscription");
        }

        public HostedActionRequest Confirm(Confirm confirm)
        {
            var xml = CreateXml(
                new XElement("confirm",
                    new XElement("transactionid", confirm.TransactionId),
                    new XElement("capturedate", confirm.CaptureDate.ToString("yyyy-MM-dd"))
                )
            );
            AddCorrelationIdHeader(confirm.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/confirm");
        }

        public HostedActionRequest ConfirmPartial(ConfirmPartial confirmPartial)
        {
            var orderrows = !string.IsNullOrEmpty(confirmPartial.GetXmlForOrderRows())
                ? XElement.Parse("<orderrows>" + confirmPartial.GetXmlForOrderRows() + "</orderrows>").Elements()
                : Enumerable.Empty<XElement>();

            var xml = CreateXml(
                new XElement("confirmPartial",
                    new XElement("captureRequestId", confirmPartial.CallerReferenceId.ToString()),
                    new XElement("transactionid", confirmPartial.TransactionId),
                    new XElement("amount", confirmPartial.Amount),
                    new XElement("orderrows", orderrows)
                )
            );
            AddCorrelationIdHeader(confirmPartial.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/confirmpartial");
        }

        public HostedActionRequest Credit(Credit credit)
        {
            var deliveriesXml = credit.GetXmlForDeliveries();
            var creditElement = new XElement("credit",
                new XElement("transactionid", credit.TransactionId),
                credit.Deliveries != null && credit.Deliveries.Count > 0
                    ? null
                    : new XElement("amounttocredit", credit.AmountToCredit),
                !string.IsNullOrEmpty(deliveriesXml)
                    ? XElement.Parse(deliveriesXml)
                    : null
            );
            var xml = CreateXml(creditElement);
            AddCorrelationIdHeader(credit.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/credit");
        }

        public HostedActionRequest GetPaymentMethods(GetPaymentMethods getPaymentMethods)
        {
            var xml = CreateXml(
                new XElement("getpaymentmethods",
                    new XElement("merchantid", getPaymentMethods.MerchantId)
                )
            );
            AddCorrelationIdHeader(getPaymentMethods.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
                "/getpaymentmethods");
        }

        public HostedActionRequest GetReconciliationReport(GetReconciliationReport getReconciliationReport)
        {
            var xml = CreateXml(
                new XElement("getreconciliationreport",
                    new XElement("date", getReconciliationReport.Date.ToString("yyyy-MM-dd"))
                )
            );
            AddCorrelationIdHeader(getReconciliationReport.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
                "/getreconciliationreport");
        }

        public HostedActionRequest LowerAmount(LowerAmount lowerAmount)
        {
            var xml = CreateXml(
                new XElement("loweramount",
                    new XElement("transactionid", lowerAmount.TransactionId),
                    new XElement("amounttolower", lowerAmount.AmountToLower)
                )
            );
            AddCorrelationIdHeader(lowerAmount.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/loweramount");
        }

        public HostedActionRequest LowerOrderRow(LowerOrderRow lowerOrderRow)
        {
            var orderrows = !string.IsNullOrEmpty(lowerOrderRow.GetXmlForOrderRows())
                ? XElement.Parse("<orderrows>" + lowerOrderRow.GetXmlForOrderRows() + "</orderrows>").Elements()
                : Enumerable.Empty<XElement>();

            var xml = CreateXml(
                new XElement("lowerorderrow",
                    new XElement("transactionid", lowerOrderRow.TransactionId),
                    new XElement("orderrows", orderrows)
                )
            );
            AddCorrelationIdHeader(lowerOrderRow.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/lowerorderrow");
        }

        public HostedActionRequest LowerOrderRowConfirm(LowerOrderRowConfirm lowerOrderRowConfrim)
        {
            var orderrows = !string.IsNullOrEmpty(lowerOrderRowConfrim.GetXmlForOrderRows())
                ? XElement.Parse("<orderrows>" + lowerOrderRowConfrim.GetXmlForOrderRows() + "</orderrows>").Elements()
                : Enumerable.Empty<XElement>();

            var xml = CreateXml(
                new XElement("lowerorderrowconfirm",
                    new XElement("transactionid", lowerOrderRowConfrim.TransactionId),
                    new XElement("capturerequestid", lowerOrderRowConfrim.CaptureRequestId),
                    new XElement("orderrows", orderrows)
                )
            );
            AddCorrelationIdHeader(lowerOrderRowConfrim.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/lowerorderrowconfirm");
        }

        public HostedActionRequest LowerAmountConfirm(LowerAmountConfirm lowerAmount)
        {
            var xml = CreateXml(
                new XElement("loweramountconfirm",
                    new XElement("transactionid", lowerAmount.TransactionId),
                    new XElement("amounttolower", lowerAmount.AmountToLower),
                    new XElement("capturedate", lowerAmount.CaptureDate.ToString("yyyy-MM-dd"))
                )
            );
            AddCorrelationIdHeader(lowerAmount.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/loweramountconfirm");
        }

        public HostedActionRequest AddOrderRow(AddOrderRow addOrderRow)
        {
            var orderrows = !string.IsNullOrEmpty(addOrderRow.GetXmlForOrderRows())
                ? XElement.Parse("<orderrows>" + addOrderRow.GetXmlForOrderRows() + "</orderrows>").Elements()
                : Enumerable.Empty<XElement>();

            var xml = CreateXml(
                new XElement("addorderrow",
                    new XElement("transactionid", addOrderRow.TransactionId),
                    new XElement("orderrows", orderrows)
                )
            );
            AddCorrelationIdHeader(addOrderRow.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/addorderrow");
        }

        public HostedActionRequest ReplaceOrderRow(ReplaceOrderRow replaceOrderRow)
        {
            var orderrows = !string.IsNullOrEmpty(replaceOrderRow.GetXmlForOrderRows())
                ? XElement.Parse("<orderrows>" + replaceOrderRow.GetXmlForOrderRows() + "</orderrows>").Elements()
                : Enumerable.Empty<XElement>();

            var xml = CreateXml(
                new XElement("replaceorderrow",
                    new XElement("transactionid", replaceOrderRow.TransactionId),
                    new XElement("orderrows", orderrows)
                )
            );
            AddCorrelationIdHeader(replaceOrderRow.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/replaceorderrow");
        }

        public HostedActionRequest UpdateOrderRow(UpdateOrderRow updateOrderRow)
        {
            var orderrows = !string.IsNullOrEmpty(updateOrderRow.GetXmlForOrderRows())
                ? XElement.Parse("<orderrows>" + updateOrderRow.GetXmlForOrderRows() + "</orderrows>").Elements()
                : Enumerable.Empty<XElement>();

            var xml = CreateXml(
                new XElement("updateorderrow",
                    new XElement("transactionid", updateOrderRow.TransactionId),
                    new XElement("orderrows", orderrows),
                    new XElement("reference", updateOrderRow.Reference)
                )
            );
            AddCorrelationIdHeader(updateOrderRow.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/updateorderrow");
        }

        public HostedActionRequest UpdateMetadata(UpdateMetadata metadata)
        {
            var metadataElements = !string.IsNullOrEmpty(metadata.GetXmlForMetadata())
                ? XElement.Parse("<metadata>" + metadata.GetXmlForMetadata() + "</metadata>").Elements()
                : Enumerable.Empty<XElement>();

            var xml = CreateXml(
                new XElement("updatepaymentmetadata",
                    new XElement("transactionid", metadata.TransactionId),
                    new XElement("metadata", metadataElements)
                )
            );
            AddCorrelationIdHeader(metadata.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/updatepaymentmetadata");
        }

        public HostedActionRequest Query(QueryByTransactionId query)
        {
            var xml = CreateXml(
                new XElement("query",
                    new XElement("transactionid", query.TransactionId)
                )
            );
            AddCorrelationIdHeader(query.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
                "/querytransactionid");
        }

        public HostedActionRequest Query(QueryByCustomerRefNo query)
        {
            var xml = CreateXml(
                new XElement("query",
                    new XElement("customerrefno", query.CustomerRefNo)
                )
            );
            AddCorrelationIdHeader(query.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
                "/querycustomerrefno");
        }

        public HostedActionRequest Recur(Recur recur)
        {
            var xml = CreateXml(
                new XElement("recur",
                    new XElement("customerrefno", recur.CustomerRefNo),
                    new XElement("subscriptionid", recur.SubscriptionId),
                    new XElement("currency", recur.Currency),
                    new XElement("amount", recur.Amount),
                    recur.Vat != 0 ? new XElement("vat", recur.Vat) : null
                )
            );
            AddCorrelationIdHeader(recur.CorrelationId);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/recur");
        }

        private void AddCorrelationIdHeader(Guid? correlationId)
        {
            Headers.Add(new AdminRequestHeader("X-Svea-CorrelationId", correlationId));
        }
    }
}