using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class ApproveInvoiceBuilder : Builder<ApproveInvoiceBuilder>
{
    internal long InvoiceId { get; private set; }
    internal long ClientId { get; private set; }

    public ApproveInvoiceBuilder(IConfigurationProvider config) : base(config) { }

    public ApproveInvoiceBuilder SetInvoiceId(long invoiceId)
    {
        InvoiceId = invoiceId;
        return this;
    }

    public override ApproveInvoiceBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }

    public override ApproveInvoiceBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public ApproveInvoiceBuilder SetClientId(long clientId)
    {
        ClientId = clientId;
        return this;
    }

    public AdminService.ApproveInvoiceRequest ApproveInvoice()
    {
        return new AdminService.ApproveInvoiceRequest(this);
    }
}
