using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class ApproveInvoiceRequest : WebpayAdminRequest
{
    private readonly ApproveInvoiceBuilder _builder;

    public ApproveInvoiceRequest(ApproveInvoiceBuilder builder)
    {
        _builder = builder;
    }

    public async Task<BasicResponse> DoRequestAsync()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(PaymentType.INVOICE, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(PaymentType.INVOICE, _builder.GetCountryCode())
        };

        var request = new AdminWS.ApproveInvoiceRequest()
        {
            Authentication = auth,
            ClientId = _builder.ClientId,
            InvoiceId = _builder.InvoiceId
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);
        var response = await adminWS.ApproveInvoiceAsync(request);

        return response;
    }
}
