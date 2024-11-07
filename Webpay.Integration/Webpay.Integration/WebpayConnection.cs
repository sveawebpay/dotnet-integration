using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Order.Create;
using Webpay.Integration.Order.Handle;
using WebpayWS;
using Webpay.Integration.Webservice.Getaddresses;
using Webpay.Integration.Webservice.Getpaymentplanparams;

namespace Webpay.Integration;

/// <summary>
/// Start build request object by choosing the right method.
/// </summary>
public static class WebpayConnection
{
    /// <summary>
    /// Start build order request to create an order for all payments.
    /// </summary>
    /// <param name="config"></param>
    /// <returns>CreateOrderBuilder</returns>
    public static CreateOrderBuilder CreateOrder(IConfigurationProvider config = null)
    {
        if (config == null)
        {
            throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
        }

        return new CreateOrderBuilder(config);
    }

    /// <summary>
    /// Start building request to close order.
    /// </summary>
    /// <param name="config"></param>
    /// <remarks>Deprecated</remarks>
    /// <returns>CloseOrderBuilder</returns>
    [Obsolete("Use WebpayAdmin.CancelOrder() instead")]
    public static CloseOrderBuilder CloseOrder(IConfigurationProvider config = null)
    {
        if (config == null)
        {
            throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
        }

        return new CloseOrderBuilder(config);
    }

    /// <summary>
    ///     DeliverOrderBuilder request = WebpayConnection.DeliverOrder(config)
    ///         .SetOrderId()                  // invoice or payment plan only, required
    ///         .SetCountryCode()              // required
    ///         .SetInvoiceDistributionType()  // invoice only, required
    ///         .SetNumberOfCreditDays()       // invoice only, optional
    ///         .AddOrderRow()                 // invoice only, required
    ///         .SetCreditInvoice()            // invoice only, optional -- use WebPayAdmin.CreditOrderRows instead
    ///     ;
    ///     // then select the corresponding request class and send request
    ///     response = request.DeliverInvoiceOrder().doRequest();       // returns DeliverOrderResponse
    ///     response = request.DeliverPaymentPlanOrder().doRequest();   // returns DeliverOrderResponse
    ///     response = request.DeliverCardOrder().doRequest();   // Confirms a card order and returns ConfirmResponse
    /// </summary>
    /// <param name="config"></param>
    /// <returns>DeliverOrderBuilder</returns>
    public static DeliverOrderBuilder DeliverOrder(IConfigurationProvider config = null)
    {
        if (config == null)
        {
            throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
        }

        return new DeliverOrderBuilder(config);
    }

    /// <summary>
    /// Get payment plan parameters to present to customer before creating a payment plan payment request
    /// </summary>
    /// <param name="config"></param>
    /// <returns>GetPaymentPlanParams</returns>
    public static GetPaymentPlanParams GetPaymentPlanParams(IConfigurationProvider config = null)
    {
        if (config == null)
        {
            throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
        }

        return new GetPaymentPlanParams(config);
    }

    /// <summary>
    /// Start building request for getting addresses.
    /// </summary>
    /// <param name="config"></param>
    /// <returns>GetAddresses</returns>
    public static GetAddresses GetAddresses(IConfigurationProvider config = null)
    {
        if (config == null)
        {
            throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
        }

        return new GetAddresses(config);
    }

    /// <summary>
    /// Calculate the prices per month for the payment plan campaigns
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="paymentPlanParams"></param>
    /// <returns>PaymentPlanPricePerMonth</returns>
    public static List<Dictionary<string, long>> PaymentPlanPricePerMonth(decimal amount, GetPaymentPlanParamsEuResponse paymentPlanParams)
    {
        return new PaymentPlanPricePerMonth().Calculate(amount, paymentPlanParams);
    }
}