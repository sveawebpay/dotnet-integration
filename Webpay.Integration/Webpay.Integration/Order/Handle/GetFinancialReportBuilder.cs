using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class GetFinancialReportBuilder : Builder<GetFinancialReportBuilder>
{
    private DateTime _fromDate;
    private DateTime _toDate;

    public GetFinancialReportBuilder(IConfigurationProvider config) : base(config)
    {
    }

    public GetFinancialReportBuilder SetFromDate(DateTime fromDate)
    {
        _fromDate = fromDate;
        return this;
    }

    public GetFinancialReportBuilder SetToDate(DateTime toDate)
    {
        _toDate = toDate;
        return this;
    }

    public override GetFinancialReportBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public DateTime FromDate => _fromDate;

    public DateTime ToDate => _toDate;

    public AdminService.GetFinancialReportRequest Build()
    {
        return new AdminService.GetFinancialReportRequest(this);
    }

    public override GetFinancialReportBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}
