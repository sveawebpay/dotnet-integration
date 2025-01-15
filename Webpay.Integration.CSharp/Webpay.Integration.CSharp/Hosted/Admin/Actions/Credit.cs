using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Xml;
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
            var xml = "<deliveries>";
            Deliveries.ForEach(delivery =>
            {
                if(delivery != null)
                {
                    xml += GetXmlForDelivery(delivery);
                }
                
            });
            xml += "</deliveries>";
            return xml;
        }

        public static CreditResponse Response(XmlDocument responseXml)
        {
            return new CreditResponse(responseXml);
        }
        private string GetXmlForDelivery(Delivery delivery)
        {
            return $"<delivery>" +
                        $"<id>{delivery.Id}</id> " +
                        $"<orderrows>{GetXmlForOrderRows(delivery)}</orderrows>" +
                        $"</delivery>";
        }
        private string GetXmlForOrderRows(Delivery delivery)
        {
            var xml = "";
            if (delivery.OrderRows.Count() > 0 || delivery.NewOrderRows.Count() > 0)
            {

                foreach (var row in delivery.OrderRows)
                {
                    xml += GetXmlForOrderRow(row);

                }
                foreach (var row in delivery.NewOrderRows)
                {
                    xml += GetXmlForOrderRow(row);

                }
            }
            return xml;
        }
        private string GetXmlForOrderRow(NewCreditOrderRowBuilder orderRow)
        {
            return $"<row>" +
                         $"<name>{orderRow.Name.XmlEscape()}</name>" +
                         $"<unitprice>{orderRow.UnitPrice}</unitprice>" +
                         $"<quantity>{orderRow.Quantity.ToString(CultureInfo.InvariantCulture)}</quantity>" +
                         $"<vatpercent>{orderRow.VatPercent.ToString(CultureInfo.InvariantCulture)}</vatpercent>" +
                         $"<discountpercent>{orderRow.DiscountPercent.ToString(CultureInfo.InvariantCulture)}</discountpercent>" +
                         $"<discountamount>{orderRow.DiscountAmount}</discountamount>" +
                         $"<unit>{orderRow.Unit}</unit>" +
                         $"<articlenumber>{orderRow.ArticleNumber}</articlenumber>" +
                         $"</row>";
        }
        private string GetXmlForOrderRow(CreditOrderRowBuilder orderRow)
        {
            var quantity = orderRow.Quantity.HasValue ? orderRow.Quantity.Value.ToString(CultureInfo.InvariantCulture) : orderRow.Quantity.ToString();
            return $"<row>" +
                   $"<rowid>{orderRow.RowId}</rowid>" +
                   $"<quantity>{quantity}</quantity>" +
                   $"</row>";
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
            var ValidationErrorResponseXml = new XmlDocument();
            ValidationErrorResponseXml.LoadXml($"<?xml version='1.0' encoding='UTF-8'?>" +
                $"<response>" +
                $"<statuscode>403</statuscode>" +
                $"<errorMessage>{message}</errorMessage>" +
                $"</response>");

            return Credit.Response(ValidationErrorResponseXml);
        }
    }
}