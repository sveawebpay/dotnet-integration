using System;
using Webpay.Integration.CSharp.Order.Identity;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Test.Util
{
    public static class TestingTool
    {
        public const CountryCode DefaultTestCountryCode = CountryCode.SE;
        public const Currency DefaultTestCurrency = Currency.SEK;
        public const string DefaultTestClientOrderNumber = "33";
        public const string DefaultTestCustomerReferenceNumber = "ref33";
        public const string DefaultTestIndividualNationalIdNumber = "194605092222";
        public const string DefaultTestCompanyNationalIdNumber = "164608142222";
        public static readonly DateTime DefaultTestDate = new DateTime(2012, 12, 12);

        public static OrderRowBuilder CreateMiniOrderRow()
        {
            return Item.OrderRow()
                       .SetQuantity(1)
                       .SetAmountExVat(4)
                       .SetAmountIncVat(5);
        }

        public static OrderRowBuilder CreateExVatBasedOrderRow(string articleNumber = "1")
        {
            return Item.OrderRow()
                       .SetArticleNumber(articleNumber)
                       .SetName("Prod")
                       .SetDescription("Specification")
                       .SetAmountExVat(100.00M)
                       .SetQuantity(2)
                       .SetUnit("st")
                       .SetVatPercent(25)
                       .SetVatDiscount(0);
        }

        public static OrderRowBuilder CreatePaymentPlanOrderRow()
        {
            return Item.OrderRow()
                       .SetArticleNumber("1")
                       .SetName("Prod")
                       .SetDescription("Specification")
                       .SetAmountExVat(1000.00M)
                       .SetQuantity(2)
                       .SetUnit("st")
                       .SetVatPercent(25)
                       .SetVatDiscount(0);
        }

        public static OrderRowBuilder CreateIncVatBasedOrderRow(string articleNumber = "1")
        {
            return Item.OrderRow()
                       .SetArticleNumber(articleNumber)
                       .SetName("Prod")
                       .SetDescription("Specification")
                       .SetAmountIncVat(125)
                       .SetQuantity(2)
                       .SetUnit("st")
                       .SetVatPercent(25)
                       .SetDiscountPercent(0);
        }

        public static OrderRowBuilder CreateIncAndExVatOrderRow(string articleNumber = "1")
        {
            return Item.OrderRow()
                       .SetArticleNumber(articleNumber)
                       .SetName("Prod")
                       .SetDescription("Specification")
                       .SetAmountIncVat(125)
                       .SetAmountExVat(100)
                       .SetQuantity(2)
                       .SetUnit("st")
                       .SetDiscountPercent(0);
        }

        public static OrderRowBuilder CreateOrderRowDe()
        {
            return Item.OrderRow()
                       .SetArticleNumber("1")
                       .SetName("Prod")
                       .SetDescription("Specification")
                       .SetAmountExVat(100.00M)
                       .SetQuantity(2)
                       .SetUnit("st")
                       .SetVatPercent(19)
                       .SetVatDiscount(0);
        }

        public static OrderRowBuilder CreateOrderRowNl()
        {
            return Item.OrderRow()
                       .SetArticleNumber("1")
                       .SetName("Prod")
                       .SetDescription("Specification")
                       .SetAmountExVat(100.00M)
                       .SetQuantity(2)
                       .SetUnit("st")
                       .SetVatPercent(6)
                       .SetVatDiscount(0);
        }

        public static ShippingFeeBuilder CreateExVatBasedShippingFee()
        {
            return Item.ShippingFee()
                       .SetShippingId("33")
                       .SetName("shipping")
                       .SetDescription("Specification")
                       .SetAmountExVat(50)
                       .SetUnit("st")
                       .SetVatPercent(25)
                       .SetDiscountPercent(0);
        }

        public static ShippingFeeBuilder CreateIncVatBasedShippingFee()
        {
            return Item.ShippingFee()
                       .SetShippingId("33")
                       .SetName("shipping")
                       .SetDescription("Specification")
                       .SetAmountIncVat(62.50M)
                       .SetUnit("st")
                       .SetVatPercent(25)
                       .SetDiscountPercent(0);
        }

        public static ShippingFeeBuilder CreateIncAndExVatShippingFee()
        {
            return Item.ShippingFee()
                       .SetShippingId("33")
                       .SetName("shipping")
                       .SetDescription("Specification")
                       .SetAmountIncVat(62.50M)
                       .SetAmountExVat(50)
                       .SetUnit("st")
                       .SetDiscountPercent(0);
        }

        public static InvoiceFeeBuilder CreateExVatBasedInvoiceFee()
        {
            return Item.InvoiceFee()
                       .SetName("Svea fee")
                       .SetDescription("Fee for invoice")
                       .SetAmountExVat(50)
                       .SetUnit("st")
                       .SetVatPercent(25)
                       .SetDiscountPercent(0);
        }

        public static InvoiceFeeBuilder CreateIncVatBasedInvoiceFee()
        {
            return Item.InvoiceFee()
                       .SetName("Svea fee")
                       .SetDescription("Fee for invoice")
                       .SetAmountIncVat(62.50M)
                       .SetUnit("st")
                       .SetVatPercent(25)
                       .SetDiscountPercent(0);
        }

        public static InvoiceFeeBuilder CreateIncAndExVatInvoiceFee()
        {
            return Item.InvoiceFee()
                       .SetName("Svea fee")
                       .SetDescription("Fee for invoice")
                       .SetAmountIncVat(62.50M)
                       .SetAmountExVat(50)
                       .SetUnit("st")
                       .SetDiscountPercent(0);
        }

        public static RelativeDiscountBuilder CreateRelativeDiscount()
        {
            return Item.RelativeDiscount()
                       .SetDiscountId("1")
                       .SetName("Relative")
                       .SetDescription("RelativeDiscount")
                       .SetUnit("st")
                       .SetDiscountPercent(50);
        }

        public static IndividualCustomer CreateIndividualCustomer()
        {
            return Item.IndividualCustomer()
                       .SetInitials("SB")
                       .SetName("Tess", "Persson")
                       .SetEmail("test@svea.com")
                       .SetPhoneNumber("0811111111")
                       .SetIpAddress("123.123.123")
                       .SetStreetAddress("Testgatan", "1")
                       .SetBirthDate("19231212")
                       .SetCoAddress("c/o Eriksson, Erik")
                       .SetNationalIdNumber(DefaultTestIndividualNationalIdNumber)
                       .SetZipCode("99999")
                       .SetLocality("Stan");
        }

        public static IndividualCustomer CreateIndividualCustomerNl()
        {
            return Item.IndividualCustomer()
                       .SetInitials("SB")
                       .SetBirthDate("19231212")
                       .SetName("Svea bakkerij", "123")
                       .SetEmail("test@svea.com")
                       .SetPhoneNumber("999999")
                       .SetIpAddress("123.123.123")
                       .SetStreetAddress("Gatan", "23")
                       .SetCoAddress("c/o Eriksson")
                       .SetZipCode("9999")
                       .SetLocality("Stan");
        }

        public static CompanyCustomer CreateMiniCompanyCustomer()
        {
            return Item.CompanyCustomer()
                       .SetVatNumber("2345234")
                       .SetNationalIdNumber(DefaultTestCompanyNationalIdNumber)
                       .SetCompanyName("TestCompagniet");
        }

        public static CompanyCustomer CreateCompanyCustomer()
        {
            return Item.CompanyCustomer()
                       .SetCompanyName("Tess, T Persson")
                       .SetNationalIdNumber(DefaultTestCompanyNationalIdNumber)
                       .SetEmail("test@svea.com")
                       .SetPhoneNumber("0811111111")
                       .SetIpAddress("123.123.123.123")
                       .SetStreetAddress("Testgatan", "1")
                       .SetCoAddress("c/o Eriksson, Erik")
                       .SetZipCode("99999")
                       .SetLocality("Stan");
        }
    }
}