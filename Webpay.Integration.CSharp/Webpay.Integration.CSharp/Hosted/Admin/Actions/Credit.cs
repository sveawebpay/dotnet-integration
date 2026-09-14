using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Order.Row.credit;
using Webpay.Integration.CSharp.Response;
using Webpay.Integration.CSharp.Util;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Credit : BasicRequest
    {
        public readonly long AmountToCredit;
        public readonly long TransactionId;
        public readonly List<Delivery> Deliveries;

        public Credit(long transactionId, long amountToCredit, List<Delivery> deliveries, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            AmountToCredit = amountToCredit;
            Deliveries = deliveries;
        }
        public string GetXmlForDeliveries()
        {
            var deliveries = new XElement("deliveries",
                Deliveries?.Where(d => d != null).Select(GetXmlForDelivery)
            );
            return deliveries.ToString(SaveOptions.DisableFormatting);
        }

        public static CreditResponse Response(XmlDocument responseXml)
        {
            return new CreditResponse(responseXml);
        }
        private XElement GetXmlForDelivery(Delivery delivery)
        {
            return new XElement("delivery",
                new XElement("id", delivery.Id),
                new XElement("orderrows", GetXmlForOrderRows(delivery))
            );
        }
        private IEnumerable<XElement> GetXmlForOrderRows(Delivery delivery)
        {
            var rows = new List<XElement>();
            if (delivery.OrderRows != null && delivery.OrderRows.Count() > 0)
            {
                rows.AddRange(delivery.OrderRows.Select(GetXmlForOrderRow));
            }
            if (delivery.NewOrderRows != null && delivery.NewOrderRows.Count() > 0)
            {
                rows.AddRange(delivery.NewOrderRows.Select(GetXmlForOrderRow));
            }
            return rows;
        }
        private XElement GetXmlForOrderRow(NewCreditOrderRowBuilder orderRow)
        {
            return new XElement("row",
                new XElement("name", orderRow.Name),
                new XElement("unitprice", orderRow.UnitPrice),
                new XElement("quantity", orderRow.Quantity.ToString(CultureInfo.InvariantCulture)),
                new XElement("vatpercent", orderRow.VatPercent.ToString(CultureInfo.InvariantCulture)),
                new XElement("discountpercent", orderRow.DiscountPercent.ToString(CultureInfo.InvariantCulture)),
                new XElement("discountamount", orderRow.DiscountAmount),
                new XElement("unit", orderRow.Unit),
                new XElement("articlenumber", orderRow.ArticleNumber)
            );
        }
        private XElement GetXmlForOrderRow(CreditOrderRowBuilder orderRow)
        {
            var quantity = orderRow.Quantity.HasValue ? orderRow.Quantity.Value.ToString(CultureInfo.InvariantCulture) : orderRow.Quantity.ToString();
            return new XElement("row",
                new XElement("rowid", orderRow.RowId),
                new XElement("quantity", quantity)
            );
        }
        public bool ValidateCreditRequest(out CreditResponse response)
        {
            response = null;
            var deliveryResponse = ValidateDeliveries();
            if (TransactionId < 0)
            {
                response = GetValidationErrorResponse("Invalid transactionId");
                return false;
            }
            else if (!deliveryResponse.Item1)
            {
                response = deliveryResponse.Item2;
                return false;
            }

            return true;
        }
        private Tuple<bool, CreditResponse> ValidateDeliveries()
        {
            if (Deliveries.Count() == 0 && AmountToCredit <= 0)
            {
                return new Tuple<bool, CreditResponse>(false, GetValidationErrorResponse("Invalid Credit Request, CreditAmount or deliveries with order rows are required"));
            }
            else if (Deliveries.Count() > 0 && AmountToCredit > 0)
            {
                return new Tuple<bool, CreditResponse>(false, GetValidationErrorResponse("Invalid Credit Request, Credit by amount and by order rows is not allowed at the same time"));
            }
            else if (Deliveries.Count() > 0 && AmountToCredit <= 0)
            {
                foreach (var delivery in Deliveries)
                {
                    var response = ValidateDelivery(delivery);
                    if (response.Item1 == false)
                        return response;
                }
            }
            return new Tuple<bool, CreditResponse>(true, null);
        }
        private Tuple<bool, CreditResponse> ValidateDelivery(Delivery delivery)
        {
            if (delivery != null)
            {
                if (AmountToCredit <= 0 && delivery.NewOrderRows.Count() == 0 && delivery.OrderRows.Count() == 0)
                {
                    return new Tuple<bool, CreditResponse>(false, GetValidationErrorResponse("Invalid Credit Request, CreditAmount or order rows are required"));
                }
                else if (AmountToCredit > 0 && delivery.NewOrderRows.Count() != 0 && delivery.OrderRows.Count() != 0)
                {
                    return new Tuple<bool, CreditResponse>(false, GetValidationErrorResponse("Invalid Credit Request, Credit by amount and by order rows is not allowed at the same time"));
                }
                else if (delivery.NewOrderRows.Count() > 0 && delivery.NewOrderRows.Any(x =>
                        string.IsNullOrEmpty(x.Name)
                        || (x.Quantity <= 0)
                        || (x.VatPercent < 0)
                        || (x.DiscountPercent < 0)
                        || (x.DiscountAmount < 0)
                    ))
                {
                    return new Tuple<bool, CreditResponse>(false, GetValidationErrorResponse($"Invalid NewOrderRow for delivery Id {delivery.Id}"));
                }
                else if (delivery.OrderRows.Count() > 0 && delivery.OrderRows.Any(x =>
                           (x.RowId <= 0)
                        || (x.Quantity <= 0)))
                {
                    return new Tuple<bool, CreditResponse>(false, GetValidationErrorResponse($"Invalid OrderRow for delivery Id {delivery.Id}"));
                }
            }
            return new Tuple<bool, CreditResponse>(true, null); ;
        }
        private CreditResponse GetValidationErrorResponse(string message)
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("response",
                    new XElement("statuscode", 403),
                    new XElement("errorMessage", message)
                )
            );
            var ValidationErrorResponseXml = new XmlDocument();
            ValidationErrorResponseXml.LoadXml($"{doc.Declaration}\n{doc.ToString(SaveOptions.DisableFormatting)}");

            return Credit.Response(ValidationErrorResponseXml);
        }
    }
}