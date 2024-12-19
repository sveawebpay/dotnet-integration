namespace Webpay.Integration.Hosted.Payment;

public class SubscriptionType
{
    public string Value { get; private set; }

    /// <summary>
    /// Only available for scandinavian acquirers
    /// </summary>
    public static readonly SubscriptionType RECURRING = new SubscriptionType("RECURRING");
    public static readonly SubscriptionType RECURRINGCAPTURE = new SubscriptionType("RECURRINGCAPTURE");
    /// <summary>
    /// Only available for scandinavian acquirers
    /// </summary>
    public static readonly SubscriptionType ONECLICK = new SubscriptionType("ONECLICK");
    public static readonly SubscriptionType ONECLICKCAPTURE = new SubscriptionType("ONECLICKCAPTURE");

    private SubscriptionType(string value)
    {
        Value = value;
    }
}