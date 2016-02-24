using System.Collections.Generic;
using System.Linq;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class UpdateOrderRowsRequest : WebpayAdminRequest
    {
        private readonly UpdateOrderRowsBuilder _builder;

        public UpdateOrderRowsRequest(UpdateOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        public Webpay.Integration.CSharp.AdminWS.UpdateOrderRowsResponse DoRequest()
        {
            var auth = new AdminWS.Authentication()
            {
                Password = _builder.GetConfig().GetPassword(_builder.OrderType,_builder.GetCountryCode()),
                Username = _builder.GetConfig().GetUsername(_builder.OrderType,_builder.GetCountryCode())                
            };

            // TODO move this to WebpayAdminRequest class method, replace loop w/select() over NumberedOrderRows
            var updatedOrderRows = new List<AdminWS.NumberedOrderRow>();
            foreach (var nor in _builder.NumberedOrderRows)
            {
                var vatPercent = GetVatPercentFromBuilderOrderRow(nor.GetVatPercent(), nor.GetAmountIncVat(), nor.GetAmountExVat());
                //var amountIncVat = GetAmountIncVatFromBuilderOrderRow(nor.GetVatPercent(), nor.GetAmountIncVat(), nor.GetAmountExVat());
                var description = GetDescriptionFromBuilderOrderRow(nor.GetName(), nor.GetDescription());

                var updateOrderRow = new AdminWS.NumberedOrderRow()
                {
                    ArticleNumber = nor.GetArticleNumber(),
                    Description = description,
                    DiscountPercent = nor.GetDiscountPercent(),
                    NumberOfUnits = nor.GetQuantity(),
                    PriceIncludingVat = nor.GetAmountIncVat().HasValue, // true iff we have set amountIncVat
                    PricePerUnit = (decimal)(nor.GetAmountIncVat() ?? nor.GetAmountExVat()),
                    Unit = nor.GetUnit(),
                    VatPercent = vatPercent,
                    //DiscountAmount = ,                    // not yet supported
                    //DiscountAmountIncludingVat = ,        // not yet supported
                    RowNumber = nor.GetRowNumber()                        
                };
                updatedOrderRows.Add(updateOrderRow);
            }

            var request = new AdminWS.UpdateOrderRowsRequest()
            {
                Authentication = auth,
                SveaOrderId = _builder.Id,
                OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType),
                ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
                UpdatedOrderRows = updatedOrderRows.ToArray()//_builder.NumberedOrderRows.Select( x => ConvertOrderRowBuilderToAdminWSOrderRow(x) ).ToArray()
            };

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.UpdateOrderRows(request);

            return response;
        }
    }
}