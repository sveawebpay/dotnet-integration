using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Order.Identity;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using WebpayWS;

namespace Webpay.Integration.Util.Testing;

public static class TestingTool
{
    public const CountryCode DefaultTestCountryCode = CountryCode.SE;
    public const Currency DefaultTestCurrency = Currency.SEK;
    public const string DefaultTestClientOrderNumber = "WebpayIntegration-33";
    public const string DefaultTestCustomerReferenceNumber = "ref33";
    public const string DefaultTestIndividualNationalIdNumber = "194605092222";
    public const string DefaultTestCompanyNationalIdNumber = "194608142222"; // Enskild firma
    public const string DefaultTestPayerAlias = "1234567890"; // Enskild firma
    public const string DefaultTestVippsPayerAlias = "48060316";
    public const string DefaultTestMobilePayPayerAlias = "004526856827";

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

    public static OrderRowBuilder CreatePaymentPlanOrderRow(string articleNumber = "1")
    {
        return Item.OrderRow()
                   .SetArticleNumber(articleNumber)
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

    public static IndividualCustomer CreateIndividualCustomer(CountryCode country = CountryCode.SE) =>
        country switch
        {
            CountryCode.SE => Item.IndividualCustomer()
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
                .SetLocality("Stan"),

            CountryCode.NO => Item.IndividualCustomer()
                .SetName("Ola", "Normann")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("21222222")
                .SetIpAddress("123.123.123")
                .SetStreetAddress("Testveien", "2")
                .SetNationalIdNumber("17054512066")
                .SetZipCode("359")
                .SetLocality("Oslo"),

            CountryCode.FI => Item.IndividualCustomer()
                .SetName("Kukka-Maaria", "Kanerva Haapakoski")
                .SetEmail("test@svea.com")
                .SetIpAddress("123.123.123")
                .SetStreetAddress("Atomitie", "2 C")
                .SetNationalIdNumber("160264-999N")
                .SetZipCode("370")
                .SetLocality("Helsinki"),

            CountryCode.DK => Item.IndividualCustomer()
                .SetName("Hanne", "Jensen")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("22222222")
                .SetIpAddress("123.123.123")
                .SetStreetAddress("Testvejen", "42")
                .SetCoAddress("c/o Test A/S")
                .SetNationalIdNumber("2603692503")
                .SetZipCode("2100")
                .SetLocality("KØBENHVN Ø"),

            CountryCode.DE => Item.IndividualCustomer()
                .SetName("Theo", "Giebel")
                .SetEmail("test@svea.com")
                .SetIpAddress("123.123.123")
                .SetStreetAddress("Zörgiebelweg", "21")
                .SetCoAddress("c/o Test A/S")
                .SetNationalIdNumber("19680403")
                .SetZipCode("13591")
                .SetLocality("BERLIN"),

            CountryCode.NL => Item.IndividualCustomer()
                .SetInitials("SB")
                .SetName("Sneider", "Boasman")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("999999")
                .SetIpAddress("123.123.123.123")
                .SetStreetAddress("Gate", "42")
                .SetBirthDate("19550307")
                .SetCoAddress("138")
                .SetNationalIdNumber("19550307")
                .SetZipCode("1102 HG")
                .SetLocality("BARENDRECHT"),

            _ => throw new SveaWebPayException("Unsupported argument for method.")
        };

    public static CompanyCustomer CreateMinimalCompanyCustomer()
    {
        return Item.CompanyCustomer()
                   .SetVatNumber("2345234")
                   .SetNationalIdNumber(DefaultTestCompanyNationalIdNumber)
                   .SetCompanyName("TestCompagniet");
    }

    public static CompanyCustomer CreateCompanyCustomer(CountryCode country = CountryCode.SE) =>
        country switch
        {
            CountryCode.SE => Item.CompanyCustomer()
                .SetCompanyName("Tess, T Persson")
                .SetNationalIdNumber(DefaultTestCompanyNationalIdNumber)
                .SetEmail("test@svea.com")
                .SetPhoneNumber("0811111111")
                .SetIpAddress("123.123.123.123")
                .SetStreetAddress("Testgatan", "1")
                .SetCoAddress("c/o Eriksson, Erik")
                .SetZipCode("99999")
                .SetLocality("Stan"),

            CountryCode.NO => Item.CompanyCustomer()
                .SetCompanyName("Test firma AS")
                .SetNationalIdNumber("923313850")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("22222222")
                .SetIpAddress("123.123.123.123")
                .SetStreetAddress("Testveien", "1")
                .SetZipCode("259")
                .SetLocality("Oslo"),

            CountryCode.FI => Item.CompanyCustomer()
                .SetCompanyName("Testi Yritys Oy")
                .SetNationalIdNumber("9999999-2")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("22222222")
                .SetIpAddress("123.123.123.123")
                .SetStreetAddress("Testitie", "1")
                .SetZipCode("370")
                .SetLocality("Helsinki"),

            CountryCode.DK => Item.CompanyCustomer()
                .SetCompanyName("Test A/S")
                .SetNationalIdNumber("99999993")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("22222222")
                .SetIpAddress("123.123.123.123")
                .SetStreetAddress("Testvejen", "42")
                .SetZipCode("2100")
                .SetLocality("KØBENHVN Ø"),

            CountryCode.DE => Item.CompanyCustomer()
                .SetCompanyName("K. H. Maier gmbH")
                .SetNationalIdNumber("12345")
                .SetVatNumber("DE123456789")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("0241/12 34 56")
                .SetIpAddress("123.123.123.123")
                .SetStreetAddress("Adalbertsteinweg", "1")
                .SetZipCode("52070")
                .SetLocality("AACHEN"),

            CountryCode.NL => Item.CompanyCustomer()
                .SetCompanyName("Svea bakkerij 123")
                .SetVatNumber("NL123456789A12")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("999999")
                .SetIpAddress("123.123.123.123")
                .SetStreetAddress("broodstraat", "1")
                .SetCoAddress("236")
                .SetZipCode("1111 CD")
                .SetLocality("BARENDRECHT"),

            _ => throw new SveaWebPayException("Unsupported argument for method.")
        };

    public static async Task<CreateOrderEuResponse> CreateInvoiceOrderWithOneOrderRow()
    {
        var createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency);

        var order = await createOrderBuilder.UseInvoicePayment().DoRequestAsync();
        return order;
    }

    public static async Task<CreateOrderEuResponse> CreateInvoiceOrderWithTwoOrderRows()
    {
        var createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency);

        var order = await createOrderBuilder.UseInvoicePayment().DoRequestAsync();
        return order;
    }

    public static async Task<CreateOrderEuResponse> CreateInvoiceOrderWithTwoOrderRowsSpecifiedIncVat()
    {
        var createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency);

        var order = await createOrderBuilder.UseInvoicePayment().DoRequestAsync();
        return order;
    }

    public static async Task<CreateOrderEuResponse> CreatePaymentPlanOrderWithOneOrderRow()
    {
        var campaigns = await WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DoRequestAsync();

        var createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency);

        var order = await createOrderBuilder.UsePaymentPlanPayment(campaigns.CampaignCodes[0].CampaignCode).DoRequestAsync();
        return order;
    }

    public static async Task<CreateOrderEuResponse> CreatePaymentPlanOrderWithTwoOrderRows()
    {
        var campaigns = await WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DoRequestAsync();

        var createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency);

        var order = await createOrderBuilder.UsePaymentPlanPayment(campaigns.CampaignCodes[0].CampaignCode).DoRequestAsync();
        return order;
    }

    public static IndividualCustomer ConfigureCustomer(CustomerAddress customerAddress, string ipAddress, string phoneNumber, string emailAddress)
    {
        return Item.IndividualCustomer()
            .SetNationalIdNumber(customerAddress.SecurityNumber)
            .SetIpAddress(ipAddress)
            .SetName(customerAddress.FirstName, customerAddress.LastName)
            .SetPhoneNumber(phoneNumber)
            .SetEmail(emailAddress)
            .SetStreetAddress(customerAddress.AddressLine1, customerAddress.AddressLine2)
            .SetLocality(customerAddress.Postarea)
            .SetZipCode(customerAddress.Zipcode);
    }

    public static CompanyCustomer ConfigureCompanyCustomer(CustomerAddress customerAddress, string ipAddress, string phoneNumber, string emailAddress)
    {
        return Item.CompanyCustomer()
            .SetNationalIdNumber(customerAddress.SecurityNumber)
            .SetIpAddress(ipAddress)
            .SetCompanyName(customerAddress.LegalName)
            .SetPhoneNumber(phoneNumber)
            .SetEmail(emailAddress)
            .SetStreetAddress(customerAddress.AddressLine1, customerAddress.AddressLine2)
            .SetLocality(customerAddress.Postarea)
            .SetZipCode(customerAddress.Zipcode);
    }

    public static OrderRowBuilder ToOrderRowBuilder(this AdminWS.OrderRow row, bool isCompany, bool isNewRow=false)
    {
        var orderRowBuilder = Item.OrderRow()
                                  .SetArticleNumber(row.ArticleNumber)
                                  .SetDescription(row.Description)
                                  .SetQuantity((int)row.NumberOfUnits)
                                  .SetUnit(row.Unit ?? "pcs")
                                  .SetVatPercent((int)row.VatPercent)
                                  .SetVatDiscount(0)
                                  .SetDiscountPercent((int)row.DiscountPercent);

        if (isCompany)
        {
            orderRowBuilder.SetAmountExVat(row.PricePerUnit);
        }
        else
        {
            if (isNewRow)
            {
                orderRowBuilder.SetAmountIncVat(row.PricePerUnit * (1 + (row.VatPercent / 100)));
            }
            else
            {
                orderRowBuilder.SetAmountIncVat(row.PricePerUnit);
            }
        }

        return orderRowBuilder;
    }

    public static NumberedOrderRowBuilder ToNumberedOrderRowBuilder(this AdminWS.NumberedOrderRow row, bool isCompany)
    {
        var numberedOrderRowBuilder = new NumberedOrderRowBuilder()
                                      .SetArticleNumber(row.ArticleNumber)
                                      .SetRowNumber((int)row.RowNumber)
                                      .SetVatPercent(row.VatPercent)
                                      .SetQuantity(row.NumberOfUnits)
                                      //.SetName(row.Description)
                                      .SetDescription(row.Description)
                                      .SetDiscountPercent((int)row.DiscountPercent);

        if (isCompany)
        {
            numberedOrderRowBuilder.SetAmountExVat(row.PricePerUnit);
        }
        else
        {
            numberedOrderRowBuilder.SetAmountIncVat(row.PricePerUnit);
        }

        return numberedOrderRowBuilder;
    }
}
