using System;

namespace Sample.AspNetCore.Models;

public class MerchantSettings
{
    public Uri PushUri { get; set; }
    public Uri TermsUri { get; set; }
    public Uri CheckoutUri { get; set; }
    public Uri ConfirmationUri { get; set; }
    public Uri CheckoutValidationCallbackUri { get; set; }
    public Uri WebhookUri { get; set; }
}
