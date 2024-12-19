using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class GetInvoicePdfLinkRequest : WebpayAdminRequest
{
    private readonly GetInvoicePdfLinkBuilder _builder;

    public GetInvoicePdfLinkRequest(GetInvoicePdfLinkBuilder builder)
    {
        _builder = builder;
    }

    public async Task<PdfLinkResponse> DoRequestAsync()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(PaymentType.ADMIN_TYPE, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(PaymentType.ADMIN_TYPE, _builder.GetCountryCode())
        };

        var request = new AdminWS.GetInvoicePdfLinkRequest()
        {
            Authentication = auth,
            ClientId = _builder.GetConfig().GetClientNumber(PaymentType.ADMIN_TYPE, _builder.GetCountryCode()),
            InvoiceId = _builder.InvoiceId
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);

        var response = await adminWS.GetInvoicePdfLinkAsync(request);

        return response;
    }
}
