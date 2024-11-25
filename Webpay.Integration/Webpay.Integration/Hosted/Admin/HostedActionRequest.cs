using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Hosted.Admin.Actions;
using Webpay.Integration.Hosted.Admin.Response;
using Webpay.Integration.Util;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Hosted.Admin;

public class HostedActionRequest
{
    public readonly IConfigurationProvider ConfigurationProvider;
    public readonly CountryCode CountryCode;
    public readonly string MerchantId;
    public readonly string ServicePath;
    public readonly string Xml;
    public readonly List<AdminRequestHeader> Headers;

    public HostedActionRequest(string xml, CountryCode countryCode, string merchantId,
        IConfigurationProvider configurationProvider, List<AdminRequestHeader> headers, string servicePath)
    {
        Xml = xml;
        CountryCode = countryCode;
        MerchantId = merchantId;
        ConfigurationProvider = configurationProvider;
        ServicePath = servicePath;
        Headers = headers;
    }

    // TODO: cleanup
    //public T DoRequest<T>()
    //{
    //    if (typeof(T) == typeof(AnnulResponse))
    //    {
    //        var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
    //        return (T)(object)hostedAdminResponse.To(Annul.Response);
    //    }
    //    if (typeof(T) == typeof(ConfirmResponse))
    //    {
    //        var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
    //        return (T)(object)hostedAdminResponse.To(Confirm.Response);
    //    }
    //    if (typeof(T) == typeof(CreditResponse))
    //    {
    //        var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
    //        return (T)(object)hostedAdminResponse.To(Credit.Response);
    //    }
    //    if (typeof(T) == typeof(LowerAmountResponse))
    //    {
    //        var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
    //        return (T)(object)hostedAdminResponse.To(LowerAmount.Response);
    //    }
    //    if (typeof(T) == typeof(LowerAmountConfirmResponse))
    //    {
    //        var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
    //        return (T)(object)hostedAdminResponse.To(LowerAmountConfirm.Response);
    //    }
    //    if ( typeof(T) == typeof(QueryResponse) )
    //    {
    //        var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
    //        return (T) (object) hostedAdminResponse.To(Query.Response);
    //    }
    //    if (typeof(T) == typeof(RecurResponse))
    //    {
    //        var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
    //        return (T)(object)hostedAdminResponse.To(Recur.Response);
    //    }
    //    if (typeof(T) == typeof(HostedAdminResponse))
    //    {
    //        var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
    //        return (T)(object)hostedAdminResponse;
    //    }

    //    throw new SveaWebPayException("unknown request type");
    //}

    public T DoRequest<T>()
    {
        var typeToResponseMap = new Dictionary<Type, Func<HostedAdminResponse, object>>
        {
            { typeof(AnnulResponse), response => response.To(Annul.Response) },
            { typeof(ConfirmResponse), response => response.To(Confirm.Response) },
            { typeof(CreditResponse), response => response.To(Credit.Response) },
            { typeof(LowerAmountResponse), response => response.To(LowerAmount.Response) },
            { typeof(LowerAmountConfirmResponse), response => response.To(LowerAmountConfirm.Response) },
            { typeof(QueryResponse), response => response.To(Query.Response) },
            { typeof(RecurResponse), response => response.To(Recur.Response) },
            { typeof(HostedAdminResponse), response => response }
        };

        if (typeToResponseMap.TryGetValue(typeof(T), out var converter))
        {
            var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
            return (T)converter(hostedAdminResponse);
        }

        throw new SveaWebPayException("Unknown request type");
    }

    private string GetEndPointBase()
    {
        var endPoint = ConfigurationProvider.GetEndPoint(PaymentType.HOSTED);
        var baseUrl = endPoint.Replace("/payment", "");

        var targetAddress = baseUrl + "/rest" + ServicePath;
        return targetAddress;
    }

    public HostedAdminRequest PrepareRequest()
    {
        return new HostedAdminRequest(Xml, ConfigurationProvider.GetSecretWord(PaymentType.HOSTED, CountryCode),
            MerchantId, GetEndPointBase(), Headers);
    }
}