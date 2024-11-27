using Webpay.Integration.Config;
using Webpay.Integration.Order.Handle;

namespace Webpay.Integration;

/// <summary>
/// Admin root. If you are looking for invoice and paymentplan administrative functionality,
/// please use the SOAP/WSDL integrations.
/// </summary>
public class WebpayAdmin
{
    /// <summary>
    ///  QueryOrderBuilder
    /// </summary>
    public static QueryOrderBuilder QueryOrder(IConfigurationProvider configurationProvider)
    {
        return new QueryOrderBuilder(configurationProvider);
    }

    /// <summary>
    /// Use WebpayAdmin.DeliverOrders to deliver one or more orders of the same type without specifying order rows.
    /// All order rows as currently held by Svea will be delivered when the request is made.
    /// 
    /// When delivered, invoice orders will return a corresponding invoice id, and payment plan orders a contract number. 
    /// These may be found in the returned DeliverOrderResult structure along with the corresponding order id.
    /// </summary>
    public static DeliverOrdersBuilder DeliverOrders(IConfigurationProvider configurationProvider)
    {
        return new DeliverOrdersBuilder(configurationProvider);
    }

    /// <summary>
    /// UpdateOrder
    /// </summary>
    public static UpdateOrderBuilder UpdateOrder(IConfigurationProvider configurationProvider)
    {
        return new UpdateOrderBuilder(configurationProvider);
    }

    /// <summary>
    ///  DeliverOrderRows
    /// </summary>
    public static DeliverOrderRowsBuilder DeliverOrderRows(IConfigurationProvider configurationProvider)
    {
        return new DeliverOrderRowsBuilder(configurationProvider);
    }

    /// <summary>
    /// CancelOrderRows
    /// </summary>
    public static CancelOrderRowsBuilder CancelOrderRows(IConfigurationProvider configurationProvider)
    {
        return new CancelOrderRowsBuilder(configurationProvider);
    }

    /// <summary>
    /// CreditOrderRows
    /// </summary>
    public static CreditOrderRowsBuilder CreditOrderRows(IConfigurationProvider configurationProvider)
    {
        return new CreditOrderRowsBuilder(configurationProvider);
    }

    /// <summary>
    /// UpdateOrderRows
    /// </summary>
    public static UpdateOrderRowsBuilder UpdateOrderRows(IConfigurationProvider configurationProvider)
    {
        return new UpdateOrderRowsBuilder(configurationProvider);
    }

    /// <summary>
    /// AddOrderRows
    /// </summary>
    public static AddOrderRowsBuilder AddOrderRows(IConfigurationProvider configurationProvider)
    {
        return new AddOrderRowsBuilder(configurationProvider);
    }        

    /// <summary>
    /// CancelOrder
    /// </summary>
    public static CancelOrderBuilder CancelOrder(IConfigurationProvider configurationProvider)
    {
        return new CancelOrderBuilder(configurationProvider);
    }

    /// <summary>
    /// CreditPayment
    /// </summary>
    public static CreditOrderBuilder CreditPayment(IConfigurationProvider configurationProvider)
    {
        return new CreditOrderBuilder(configurationProvider);
    }
}

