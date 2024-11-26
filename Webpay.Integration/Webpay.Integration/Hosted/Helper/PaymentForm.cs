using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Security;

namespace Webpay.Integration.Hosted.Helper;

public class PaymentForm
{
    private string _xmlMessageBase64;
    private string _xmlMessage;
    private string _merchantid;
    private string _secretWord;
    private string _completeHtmlFormWithSubmitButton;
    private string _macSha512;
    private Dictionary<string, string> _formHtmlFields;
    private string _submitText;
    private string _noScriptMessage;
    private string _htmlFormMethod;
    private string _url;

    private readonly string _htmlFormId;
    private readonly string _htmlFormName;

    public PaymentForm(string htmlFormId, string htmlFormName)
    {
        _htmlFormId = htmlFormId;
        _htmlFormName = htmlFormName;
        _formHtmlFields = new Dictionary<string, string>();
        _htmlFormMethod = "post";
        SetSubmitMessage(CountryCode.NL);
    }

    public void SetSubmitMessage(CountryCode countryCode)
    {
        (string submitText, string noScriptMessage) = countryCode switch
        {
            CountryCode.SE => ("Betala", "Javascript är inaktiverat i er webbläsare, ni får dirigera om till paypage manuellt"),
            _ => ("Submit", "Javascript is inactivated in your browser, you will manually have to redirect to the paypage")
        };

        SetSubmitText(submitText);
        _noScriptMessage = noScriptMessage;
    }

    public string GetXmlMessageBase64()
    {
        return _xmlMessageBase64;
    }

    public PaymentForm SetMessageBase64(string messageBase64)
    {
        _xmlMessageBase64 = messageBase64;
        return this;
    }

    public string GetMerchantId()
    {
        return _merchantid;
    }

    public PaymentForm SetMerchantId(string merchantid)
    {
        _merchantid = merchantid;
        return this;
    }

    public string GetSecretWord()
    {
        return _secretWord;
    }

    public PaymentForm SetSecretWord(string secretWord)
    {
        _secretWord = secretWord;
        return this;
    }

    public string GetCompleteForm()
    {
        return _completeHtmlFormWithSubmitButton;
    }

    public PaymentForm SetMacSha512(string macSha512)
    {
        _macSha512 = macSha512;
        return this;
    }

    public string GetMacSha512()
    {
        return _macSha512;
    }

    public Dictionary<string, string> GetFormHtmlFields()
    {
        return _formHtmlFields;
    }

    public PaymentForm SetForm()
    {
        _macSha512 = HashUtil.CreateHash(_xmlMessageBase64 + _secretWord);

        _completeHtmlFormWithSubmitButton = "<form name=\"" + _htmlFormName + "\" id=\"" + _htmlFormId + "\" method=\"post\" action=\""
                                            + _url
                                            + "\">"
                                            + "<input type=\"hidden\" name=\"merchantid\" value=\"" + _merchantid +
                                            "\" />"
                                            + "<input type=\"hidden\" name=\"message\" value=\"" + _xmlMessageBase64 +
                                            "\" />"
                                            + "<input type=\"hidden\" name=\"mac\" value=\"" + _macSha512 + "\" />"
                                            + "<noscript><p>" + _noScriptMessage + "</p></noscript>"
                                            + "<input type=\"submit\" name=\"submit\" value=\"" + _submitText +
                                            "\" />"
                                            + "</form>";

        return this;
    }

    public PaymentForm SetHtmlFields()
    {
        _macSha512 = HashUtil.CreateHash(_xmlMessageBase64 + _secretWord);

        var formFields = new Dictionary<string, string>
            {
                {
                    "form_start_tag",
                    "<form name=\"paymentForm\" id=\"paymentForm\" method=\"post\" action=\"" + _url + "\">"
                },
                {"input_merchantId", "<input type=\"hidden\" name=\"merchantid\" value=\"" + _merchantid + "\" />"},
                {"input_message", "<input type=\"hidden\" name=\"message\" value=\"" + _xmlMessageBase64 + "\" />"},
                {"input_mac", "<input type=\"hidden\" name=\"mac\" value=\"" + _macSha512 + "\" />"},
                {"noscript_p_tag", "<noscript><p>" + _noScriptMessage + "</p></noscript>"},
                {"input_submit", "<input type=\"submit\" name=\"submit\" value=\"" + _submitText + "\" />"},
                {"form_end_tag", "</form>"}
            };

        foreach (var formField in formFields)
        {
            if (_formHtmlFields.ContainsKey(formField.Key))
            {
                _formHtmlFields[formField.Key] = formField.Value;
            }
            else
            {
                _formHtmlFields.Add(formField.Key, formField.Value);
            }
        }

        return this;
    }

    public string GetSubmitText()
    {
        return _submitText;
    }

    public void SetSubmitText(string submitText)
    {
        _submitText = submitText;
    }

    public string GetXmlMessage()
    {
        return _xmlMessage;
    }

    public void SetXmlMessage(string xmlMessage)
    {
        _xmlMessage = xmlMessage;
        SetMessageBase64(Base64Util.EncodeBase64String(xmlMessage));
    }

    public string GetHtmlFormMethod()
    {
        return _htmlFormMethod;
    }

    public void SetPayPageUrl(string payPageUrl)
    {
        _url = payPageUrl;
    }

    public string GetPayPageUrl()
    {
        return _url;
    }
}