using Webpay.Integration.Order.Validator;

namespace Webpay.Integration.Webservice.Helper;

public class PeppolId
{
    public static bool IsValidPeppolId(string peppolId)
    {
        if(WebServiceOrderValidator.ValidatePeppolIdString(peppolId) == "")
        {
            return true;
        }
        return false;
    }
}
