using Webpay.Integration.Config;
using Webpay.Integration.Order.Row.LowerAmount;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class LowerOrderRowBuilder : Builder<LowerOrderRowBuilder>
{
    internal long Id;
    internal List<OrderRow> OrderRows;

    public LowerOrderRowBuilder(IConfigurationProvider config) : base(config)
    {
        OrderRows = new List<OrderRow>();
    }

    public LowerOrderRowBuilder SetTransactionId(long id)
    {
        Id = id;
        return this;
    }

    public override LowerOrderRowBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public LowerOrderRowBuilder AddOrderRows(IList<OrderRow> orderRows)
    {
        OrderRows.AddRange(orderRows);
        return this;
    }

    public AdminService.LowerOrderRowRequest LowerOrderRows()
    {
        return new AdminService.LowerOrderRowRequest(this);
    }

    public override LowerOrderRowBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}
