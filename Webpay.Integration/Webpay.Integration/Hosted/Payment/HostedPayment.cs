﻿using System.Xml;
using Webpay.Integration.Exception;
using Webpay.Integration.Hosted.Admin;
using Webpay.Integration.Hosted.Helper;
using Webpay.Integration.Order.Create;
using Webpay.Integration.Order.Validator;
using Webpay.Integration.Util;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Hosted.Payment;

/// <summary>
/// Description of HostedPayment: Parent to CardPayment, DirectPayment, PayPagePayment 
/// and PaymentMethodPayment classes. Prepares an order and creates a payment form
/// to integrate on web page. Uses XmlBuilder to turn formatted order into xmlMessage format.
/// </summary>
public abstract class HostedPayment
{
    protected CreateOrderBuilder CrOrderBuilder;
    protected List<HostedOrderRowBuilder> HrowBuilder;
    protected List<string> ExcludedPaymentMethod;
    protected long Amount;
    protected long Vat;
    protected string ReturnUrl;
    protected string CancelUrl;
    protected string CallbackUrl;
    protected ExcludePayments Excluded;
    protected string LanguageCode = Util.Constant.LanguageCode.en.ToString();
    protected string IpAddress;
    protected string PayerAlias;
    protected string Message;

    public HostedPayment(CreateOrderBuilder createOrderBuilder)
    {
        CrOrderBuilder = createOrderBuilder;
        HrowBuilder = new List<HostedOrderRowBuilder>();
        Excluded = new ExcludePayments();
        ExcludedPaymentMethod = new List<string>();
        ReturnUrl = "";
        PayerAlias = createOrderBuilder == null ? null : createOrderBuilder.GetPayerAlias();
        Message = createOrderBuilder == null ? null : createOrderBuilder.GetMessage();
    }

    public CreateOrderBuilder GetCreateOrderBuilder()
    {
        return CrOrderBuilder;
    }

    public List<HostedOrderRowBuilder> GetRowBuilder()
    {
        return HrowBuilder;
    }

    public List<string> GetExcludedPaymentMethod()
    {
        return ExcludedPaymentMethod;
    }

    public long GetAmount()
    {
        return Amount;
    }

    public string GetPayerAlias()
    {
        return PayerAlias;
    }
    public string GetMessage()
    {
        return Message;
    }

    public long GetVat()
    {
        return Vat;
    }

    public string GetReturnUrl()
    {
        return ReturnUrl;
    }

    public string GetCancelUrl()
    {
        return CancelUrl;
    }

    public string GetCallbackUrl()
    {
        return CallbackUrl;
    }

    public string GetPayPageLanguageCode()
    {
        return LanguageCode;
    }

    /// <summary>
    /// ValidateOrder
    /// </summary>
    /// <returns>Error message compilation string</returns>
    public string ValidateOrder()
    {
        var errors = "";
        if (string.IsNullOrEmpty(ReturnUrl))
        {
            errors += "MISSING VALUE - Return url is required, SetReturnUrl(...).\n";
        }

        var validator = new HostedOrderValidator();

        // Check if payment method is EU country, PaymentMethod: INVOICE or PAYMENTPLAN    
        var payment = this as PaymentMethodPayment;
        if (payment != null
            && (payment.GetPaymentMethod() == PaymentMethod.INVOICE ||
                payment.GetPaymentMethod() == PaymentMethod.PAYMENTPLAN))
        {
            errors += CrOrderBuilder.GetCountryCode() switch
            {
                CountryCode.NL => new IdentityValidator().ValidateNlIdentity(CrOrderBuilder),
                CountryCode.DE => new IdentityValidator().ValidateDeIdentity(CrOrderBuilder),
                _ => string.Empty
            };
        }

        errors += validator.Validate(CrOrderBuilder);

        return errors;
    }

    /// <summary>
    /// CalculateRequestValues
    /// </summary>
    /// <exception cref="SveaWebPayValidationException"></exception>
    public abstract void CalculateRequestValues();

    protected void FormatRequestValues()
    {
        var errors = ValidateOrder();
        if (errors.Length > 0)
        {
            throw new SveaWebPayValidationException(errors);
        }

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();

        HrowBuilder = formatter.FormatRows(CrOrderBuilder);
        Amount = formatter.GetTotalAmount();
        Vat = formatter.GetTotalVat();
    }

    /// <summary>
    /// GetPaymentForm
    /// </summary>
    /// <returns>PaymentForm</returns>
    public PaymentForm GetPaymentForm(string htmlFormId = "paymentForm", string htmlFormName = "paymentForm")
    {
        CalculateRequestValues();
        var xmlBuilder = new HostedXmlBuilder();
        string xml = xmlBuilder.GetXml(this);

        var form = new PaymentForm(htmlFormId, htmlFormName);
        form.SetXmlMessage(xml);

        form.SetMerchantId(CrOrderBuilder.GetConfig()
                                         .GetMerchantId(PaymentType.HOSTED, CrOrderBuilder.GetCountryCode()));
        form.SetSecretWord(CrOrderBuilder.GetConfig().GetSecretWord(PaymentType.HOSTED, CrOrderBuilder.GetCountryCode()));

        form.SetSubmitMessage(CrOrderBuilder.GetCountryCode() != CountryCode.NONE
                                  ? CrOrderBuilder.GetCountryCode()
                                  : CountryCode.SE);

        form.SetPayPageUrl(CrOrderBuilder.GetConfig().GetEndPoint(PaymentType.HOSTED));

        form.SetForm();
        form.SetHtmlFields();

        return form;
    }

    public async Task<Uri> PreparePayment(string ipAddress)
    {
        IpAddress = ipAddress;
        CalculateRequestValues();
        var xmlBuilder = new HostedXmlBuilder();
        string xml = xmlBuilder.GetXml(this);

        var secretWord = CrOrderBuilder.GetConfig()
            .GetSecretWord(PaymentType.HOSTED, CrOrderBuilder.GetCountryCode());
        var sentMerchantId = CrOrderBuilder.GetConfig()
            .GetMerchantId(PaymentType.HOSTED, CrOrderBuilder.GetCountryCode());
        var payPageUrl = CrOrderBuilder.GetConfig()
            .GetEndPoint(PaymentType.HOSTED);

        var baseUrl = payPageUrl.Replace("/payment", "");
        var headers = new List<AdminRequestHeader>
        {
            new AdminRequestHeader("X-Svea-CorrelationId", CrOrderBuilder.GetCorrelationId())
        };
        var hostedRequest = new HostedAdminRequest(xml, secretWord, sentMerchantId, baseUrl, headers);

        var targetAddress = baseUrl + "/rest/preparepayment";

        var hostedAdminResponse = await HostedAdminRequest.HostedAdminCall(targetAddress, hostedRequest);
        var message = hostedAdminResponse.Message;
        var messageDoc = new XmlDocument();
        messageDoc.LoadXml(message);
        var paymentId = messageDoc.SelectSingleNode("//id").InnerText;

        return new Uri(baseUrl + "/preparedpayment/" + paymentId);
    }

    /// <summary>
    /// GetPaymentSpecificXml
    /// </summary>
    /// <param name="xmlw"></param>
    /// <returns>XMLStreamWriter</returns>
    public abstract void WritePaymentSpecificXml(XmlWriter xmlw);

    /// <summary>
    /// WriteSimpleElement
    /// </summary>
    /// <param name="xmlw"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    protected void WriteSimpleElement(XmlWriter xmlw, string name, string value)
    {
        if (value == null)
        {
            return;
        }
        xmlw.WriteStartElement(name);
        xmlw.WriteChars(value.ToCharArray(), 0, value.ToCharArray().Length);
        xmlw.WriteEndElement();
    }

    public string GetIpAddress()
    {
        return IpAddress;
    }
}