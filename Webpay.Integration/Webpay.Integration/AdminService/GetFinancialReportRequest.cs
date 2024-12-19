using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class GetFinancialReportRequest : WebpayAdminRequest
{
    private readonly GetFinancialReportBuilder _builder;

    public GetFinancialReportRequest(GetFinancialReportBuilder builder)
    {
        _builder = builder;
    }

    public async Task<GetFinancialReportResponse> DoRequestAsync()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(PaymentType.ADMIN_TYPE, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(PaymentType.ADMIN_TYPE, _builder.GetCountryCode())
        };

        var request = new AdminWS.GetFinancialReportRequest()
        {
            Authentication = auth,
            ClientId = _builder.GetConfig().GetClientNumber(PaymentType.ADMIN_TYPE, _builder.GetCountryCode()),
            FromDate = _builder.FromDate,
            ToDate = _builder.ToDate
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);

        var response = await adminWS.GetFinancialReportAsync(request);

        return response;
    }
}
