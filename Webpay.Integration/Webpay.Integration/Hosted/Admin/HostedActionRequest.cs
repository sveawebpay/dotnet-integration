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

    //public T DoRequest<T>()
    //{
    //    var typeToResponseMap = new Dictionary<Type, Func<HostedAdminResponse, object>>
    //    {
    //        { typeof(AnnulResponse), response => response.To(Annul.Response) },
    //        { typeof(ConfirmResponse), response => response.To(Confirm.Response) },
    //        { typeof(CreditResponse), response => response.To(Credit.Response) },
    //        { typeof(LowerAmountResponse), response => response.To(LowerAmount.Response) },
    //        { typeof(LowerAmountConfirmResponse), response => response.To(LowerAmountConfirm.Response) },
    //        { typeof(QueryResponse), response => response.To(Query.Response) },
    //        { typeof(RecurResponse), response => response.To(Recur.Response) },
    //        { typeof(HostedAdminResponse), response => response }
    //    };

    //    if (typeToResponseMap.TryGetValue(typeof(T), out var converter))
    //    {
    //        var hostedAdminResponse = HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
    //        return (T)converter(hostedAdminResponse);
    //    }

    //    throw new SveaWebPayException("Unknown request type");
    //}

    public async Task<T> DoRequest<T>()
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
            var hostedAdminResponse = await HostedAdminRequest.HostedAdminCall(GetEndPointBase(), PrepareRequest());
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