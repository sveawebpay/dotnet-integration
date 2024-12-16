using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class GetAccountingReportBuilder : Builder<GetAccountingReportBuilder>
{
    private DateTime _fromDate;
    private DateTime _toDate;

    public GetAccountingReportBuilder(IConfigurationProvider config) : base(config)
    {
    }

    public GetAccountingReportBuilder SetFromDate(DateTime fromDate)
    {
        _fromDate = fromDate;
        return this;
    }

    public GetAccountingReportBuilder SetToDate(DateTime toDate)
    {
        _toDate = toDate;
        return this;
    }

    public override GetAccountingReportBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public DateTime FromDate => _fromDate;

    public DateTime ToDate => _toDate;

    public AdminService.GetAccountingReportRequest Build()
    {
        return new AdminService.GetAccountingReportRequest(this);
    }

    public override GetAccountingReportBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}
