using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class GetInvoiceReportBuilder : Builder<GetInvoiceReportBuilder>
{
    private DateTime _fromDate;
    private DateTime _toDate;

    public GetInvoiceReportBuilder(IConfigurationProvider config) : base(config)
    {
    }

    public GetInvoiceReportBuilder SetFromDate(DateTime fromDate)
    {
        _fromDate = fromDate;
        return this;
    }

    public GetInvoiceReportBuilder SetToDate(DateTime toDate)
    {
        _toDate = toDate;
        return this;
    }

    public override GetInvoiceReportBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public DateTime FromDate => _fromDate;

    public DateTime ToDate => _toDate;

    public AdminService.GetInvoiceReportRequest Build()
    {
        return new AdminService.GetInvoiceReportRequest(this);
    }

    public override GetInvoiceReportBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}
