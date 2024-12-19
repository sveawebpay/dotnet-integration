using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class GetInvoicesBuilder : Builder<GetInvoicesBuilder>
{
    internal List<long> InvoiceIds { get; private set; }
    internal PaymentType InvoiceType { get; private set; }

    public GetInvoicesBuilder(IConfigurationProvider config) : base(config)
    {
        InvoiceIds = new List<long>();
    }

    public GetInvoicesBuilder SetInvoiceId(long invoiceId)
    {
        InvoiceIds.Add(invoiceId);
        return this;
    }

    public GetInvoicesBuilder SetInvoiceIds(IList<long> invoiceIds)
    {
        InvoiceIds.AddRange(invoiceIds);
        return this;
    }

    public override GetInvoicesBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public GetInvoicesBuilder SetInvoiceType(PaymentType invoiceType)
    {
        InvoiceType = invoiceType;
        return this;
    }

    public AdminService.GetInvoicesRequest Build()
    {
        return new AdminService.GetInvoicesRequest(this);
    }

    public override GetInvoicesBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}
