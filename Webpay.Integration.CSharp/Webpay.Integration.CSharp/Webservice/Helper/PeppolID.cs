using Webpay.Integration.CSharp.Order.Validator;

namespace Webpay.Integration.CSharp.Webservice.Helper
{
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
}
