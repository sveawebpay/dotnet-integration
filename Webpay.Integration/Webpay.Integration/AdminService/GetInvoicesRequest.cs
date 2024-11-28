using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class GetInvoicesRequest : WebpayAdminRequest
{
    private readonly GetInvoicesBuilder _builder;

    public GetInvoicesRequest(GetInvoicesBuilder builder)
    {
        _builder = builder;
    }

    public async Task<GetInvoicesResponse> DoRequestAsync()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(_builder.InvoiceType, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(_builder.InvoiceType, _builder.GetCountryCode())
        };

        var invoicesToRetrieve = _builder.InvoiceIds.Select(invoiceId => new GetInvoiceInformation()
        {
            InvoiceId = invoiceId,
            ClientId = _builder.GetConfig().GetClientNumber(_builder.InvoiceType, _builder.GetCountryCode())
        }).ToArray();

        var request = new AdminWS.GetInvoicesRequest()
        {
            Authentication = auth,
            InvoicesToRetrieve = invoicesToRetrieve
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);

        var response = await adminWS.GetInvoicesAsync(request);

        return response;
    }
}
