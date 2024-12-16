using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class GetPaymentPlanReportBuilder : Builder<GetPaymentPlanReportBuilder>
{
    private DateTime _fromDate;
    private DateTime _toDate;

    public GetPaymentPlanReportBuilder(IConfigurationProvider config) : base(config)
    {
    }

    public GetPaymentPlanReportBuilder SetFromDate(DateTime fromDate)
    {
        _fromDate = fromDate;
        return this;
    }

    public GetPaymentPlanReportBuilder SetToDate(DateTime toDate)
    {
        _toDate = toDate;
        return this;
    }

    public override GetPaymentPlanReportBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public DateTime FromDate => _fromDate;

    public DateTime ToDate => _toDate;

    public AdminService.GetPaymentPlanReportRequest Build()
    {
        return new AdminService.GetPaymentPlanReportRequest(this);
    }

    public override GetPaymentPlanReportBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}
