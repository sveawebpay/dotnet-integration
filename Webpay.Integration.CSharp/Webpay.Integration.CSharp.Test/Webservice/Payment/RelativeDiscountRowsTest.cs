using System;
using NUnit.Framework;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Config;

namespace Webpay.Integration.CSharp.Test.Webservice.Payment
{
    [TestFixture]
    public class RelativeDiscountRowsTest
    {
        [Test]
        public void TestAmountExVatWithRelativeDiscount()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(Item.OrderRow()
                    .SetQuantity(1)
                    .SetAmountExVat(100.00M)
                    .SetVatPercent(25.00M)
                    )
                .AddDiscount(Item.RelativeDiscount()
                    .SetDiscountPercent(50))
                .AddCustomerDetails(Item.IndividualCustomer()
                    .SetNationalIdNumber("4605092222"))
                .SetCountryCode(CountryCode.SE)
                .SetCustomerReference("dotnet-integration")
                .SetClientOrderNumber("RelativeDiscountExVat")
                .SetOrderDate(new DateTime(2012, 12, 12))
                .UseInvoicePayment()
                .DoRequest();

            Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(62.5M));
        }
        [Test]
        public void TestAmountIncVatWithRelativeDiscount()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(Item.OrderRow()
                    .SetQuantity(1)
                    .SetAmountIncVat(100.00M)
                    .SetVatPercent(25.00M)
                    )
                .AddDiscount(Item.RelativeDiscount()
                    .SetDiscountPercent(50))
                .AddCustomerDetails(Item.IndividualCustomer()
                    .SetNationalIdNumber("4605092222"))
                .SetCountryCode(CountryCode.SE)
                .SetCustomerReference("dotnet-integration")
                .SetClientOrderNumber("RelativeDiscountIncVat")
                .SetOrderDate(new DateTime(2012, 12, 12))
                .UseInvoicePayment()
                .DoRequest();

            Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(50.0M));
        }
    }
}
