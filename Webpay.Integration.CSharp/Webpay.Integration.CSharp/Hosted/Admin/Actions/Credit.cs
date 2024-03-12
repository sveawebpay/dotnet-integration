using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Order.Row.credit;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Credit : BasicRequest
    {
        public readonly long AmountToCredit;
        public readonly long TransactionId;
        public readonly List<CreditOrderRowBuilder> OrderRows;

        public Credit(long transactionId, long amountToCredit, List<CreditOrderRowBuilder> orderRows, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            AmountToCredit = amountToCredit;
            OrderRows = orderRows;
        }
        public string GetXmlForOrderRows()
        {

            var xml = "<orderrows>";
            foreach (var row in OrderRows)
            {
                xml += GetXmlForOrderRow(row);

            }
            xml += "</orderrows>";
            return xml;
        }
        public static CreditResponse Response(XmlDocument responseXml)
        {
            return new CreditResponse(responseXml);
        }

        private string GetXmlForOrderRow(CreditOrderRowBuilder orderRow)
        {
            //Existing order row
            if(String.IsNullOrEmpty(orderRow.Name))
            {
                return $"<row>" +
                       $"<rowId>{orderRow.RowId}</rowId>" +
                       $"<quantity>{orderRow.Quantity}</quantity> " +
                       $"</row>";
            }
             // new order row
            return $"<row>" +
                         $"<rowId>{orderRow.RowId}</rowId>" +
                         $"<name>{orderRow.Name}</name> " +
                         $"<unitprice>{orderRow.UnitPrice}</unitprice> " +
                         $"<quantity>{orderRow.Quantity}</quantity> " +
                         $"<vatpercent>{orderRow.VatPercent}</vatpercent> " +
                         $"<discountpercent>{orderRow.DiscountPercent}</discountpercent> " +
                         $"<discountamount>{orderRow.DiscountAmount}</discountamount> " +
                         $"<unit>{orderRow.Unit}</unit> " +
                         $"<articlenumber>{orderRow.ArticleNumber}</articlenumber> " +
                         $"</row>";
        }
        public bool ValidateCreditRequest(out CreditResponse response)
        {
            response = null;
            if ( TransactionId < 0)
            {
                response =GetValidationErrorResponse("Invalid transactionId");
                return false;
            }
            else if (AmountToCredit < 0)
            {
                response = GetValidationErrorResponse("Invalid AmountToCredit");
                return false;
            }
            else if (OrderRows.Any(x=> !string.IsNullOrEmpty(x.Name)
                    && (x.RowId<=0)
                    && (x.UnitPrice == null || x.UnitPrice <= 0)
                    && (x.Quantity <= 0)
                    && (x.VatPercent == null || x.VatPercent < 0)
                    && (x.DiscountPercent == null || x.DiscountPercent < 0)
                    && (x.DiscountAmount == null || x.DiscountAmount < 0)
                    && string.IsNullOrEmpty(x.Unit)
                    && string.IsNullOrEmpty(x.ArticleNumber)
                )
             )
            {
                response = GetValidationErrorResponse("Invalid New OrderRow");
                return false;
            }
            else if (OrderRows.Any(x => string.IsNullOrEmpty(x.Name) 
                    && (x.RowId <= 0)
                    && (x.Quantity <= 0))
                )
            {
                response = GetValidationErrorResponse("Invalid OrderRow");
                return false;
            }
           return true;
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