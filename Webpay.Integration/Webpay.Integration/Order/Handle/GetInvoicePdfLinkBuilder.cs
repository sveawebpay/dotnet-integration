using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class GetInvoicePdfLinkBuilder : Builder<GetInvoicePdfLinkBuilder>
{
    private long _invoiceId;

    public GetInvoicePdfLinkBuilder(IConfigurationProvider config) : base(config)
    {
    }

    public GetInvoicePdfLinkBuilder SetInvoiceId(long invoiceId)
    {
        _invoiceId = invoiceId;
        return this;
    }

    public long InvoiceId => _invoiceId;

    public AdminService.GetInvoicePdfLinkRequest Build()
    {
        return new AdminService.GetInvoicePdfLinkRequest(this);
    }

    public override GetInvoicePdfLinkBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public override GetInvoicePdfLinkBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}
