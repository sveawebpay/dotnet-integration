namespace Webpay.Integration.Util.Constant;

public class InvoiceType
{
    public static readonly InvoiceType INVOICESE = new InvoiceType("SVEAINVOICESE", CountryCode.SE);
    public static readonly InvoiceType INVOICEEUSE = new InvoiceType("SVEAINVOICEEU_SE", CountryCode.SE);
    public static readonly InvoiceType INVOICENO = new InvoiceType("SVEAINVOICEEU_NO", CountryCode.NO);
    public static readonly InvoiceType INVOICEDK = new InvoiceType("SVEAINVOICEEU_DK", CountryCode.DK);
    public static readonly InvoiceType INVOICEFI = new InvoiceType("SVEAINVOICEEU_FI", CountryCode.FI);
    public static readonly InvoiceType INVOICENL = new InvoiceType("SVEAINVOICEEU_NL", CountryCode.NL);
    public static readonly InvoiceType INVOICEDE = new InvoiceType("SVEAINVOICEEU_DE", CountryCode.DE);

    public string Value { get; private set; }
    public CountryCode CountryCode { get; private set; }

    private InvoiceType(string value, CountryCode countryCode)
    {
        Value = value;
        CountryCode = countryCode;
    }

    public static readonly List<InvoiceType> AllInvoiceValueTypes = new List<InvoiceType>
    {
        INVOICESE,
        INVOICEEUSE,
        INVOICENO,
        INVOICEDK,
        INVOICEFI,
        INVOICENL,
        INVOICEDE
    };

    public static IEnumerable<string> GetAllInvoiceValues()
    {
        return AllInvoiceValueTypes.Select(it => it.Value);
    }

    public static IEnumerable<InvoiceType> GetAllInvoiceTypes()
    {
        return AllInvoiceValueTypes.Select(it => it);
    }
}