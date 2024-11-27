using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class CreditPaymentPlanOrderRowsRequest : WebpayAdminRequest
{
    private readonly CreditOrderRowsBuilder _builder;

    public CreditPaymentPlanOrderRowsRequest(CreditOrderRowsBuilder builder)
    {
        _builder = builder;
    }

    public async Task<CancelPaymentPlanRowsResponse> DoRequestAsync()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())
        };

        var cancellationRows = new List<CancellationRow>();
        foreach (var rowIndex in _builder.RowIndexesToCredit)
        {
            var cancellationRow = new CancellationRow()
            {
                AmountInclVat = 0M,
                VatPercent = 0M,
                Description = null,
                RowNumber = (int)rowIndex
            };
            cancellationRows.Add(cancellationRow);
        }
        foreach (var ncr in _builder.NewCreditOrderRows)
        {
            var vatPercent = GetVatPercentFromBuilderOrderRow(ncr.GetVatPercent(), ncr.GetAmountIncVat(), ncr.GetAmountExVat());
            var amountIncVat = GetAmountIncVatFromBuilderOrderRow(ncr.GetVatPercent(), ncr.GetAmountIncVat(), ncr.GetAmountExVat());
            var description = GetDescriptionFromBuilderOrderRow(ncr.GetName(), ncr.GetDescription());

            var cancellationRow = new CancellationRow()
            {
                AmountInclVat = amountIncVat,
                VatPercent = vatPercent,
                Description = description,
                RowNumber = null
            };
            cancellationRows.Add(cancellationRow);
        }

        var request = new CancelPaymentPlanRowsRequest()
        {
            Authentication = auth,
            ContractNumber = _builder.Id,
            ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
            CancellationRows = cancellationRows.ToArray()
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);
        var response = await adminWS.CancelPaymentPlanRowsAsync(request);

        return response;
    }
}
