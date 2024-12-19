using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class GetRegressionReportBuilder : Builder<GetRegressionReportBuilder>
{
    private DateTime _fromDate;
    private DateTime _toDate;

    public GetRegressionReportBuilder(IConfigurationProvider config) : base(config)
    {
    }

    public GetRegressionReportBuilder SetFromDate(DateTime fromDate)
    {
        _fromDate = fromDate;
        return this;
    }

    public GetRegressionReportBuilder SetToDate(DateTime toDate)
    {
        _toDate = toDate;
        return this;
    }

    public override GetRegressionReportBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public DateTime FromDate => _fromDate;

    public DateTime ToDate => _toDate;

    public AdminService.GetRegressionReportRequest Build()
    {
        return new AdminService.GetRegressionReportRequest(this);
    }

    public override GetRegressionReportBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}
