using System;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Validator
{
    public class WebServiceOrderValidator : OrderValidator
    {
        protected bool IsCompany;

        public override string Validate(CreateOrderBuilder order)
        {
            try
            {
                if (order.GetCustomerIdentity() == null)
                {
                    Errors += "MISSING VALUE - CustomerIdentity must be set.\n";
                }

                if (order.GetCustomerIdentity() != null
                    && order.GetIsCompanyIdentity()
                    && (order.GetCustomerIdentity().NationalIdNumber != null
                        || order.GetCompanyCustomer().GetVatNumber() != null
                        || order.GetCompanyCustomer().GetCompanyName() != null))
                {
                    IsCompany = true;
                }

                var identityValidator = new IdentityValidator();

                if (order.GetCountryCode() != 0)
                {
                    if (order.GetCountryCode().Equals(CountryCode.SE)
                        || order.GetCountryCode().Equals(CountryCode.NO)
                        || order.GetCountryCode().Equals(CountryCode.DK)
                        || order.GetCountryCode().Equals(CountryCode.FI))
                    {
                        Errors += identityValidator.ValidateNordicIdentity(order);
                    }
                    else if (order.GetCountryCode().Equals(CountryCode.DE))
                    {
                        Errors += identityValidator.ValidateDeIdentity(order);
                    }
                    else if (order.GetCountryCode().Equals(CountryCode.NL))
                    {
                        Errors += identityValidator.ValidateNlIdentity(order);
                    }
                    else
                    {
                        Errors += "NOT VALID - Given countrycode does not exist in our system.\n";
                    }
                }
                else
                {
                    Errors += "MISSING VALUE - CountryCode is required. Use SetCountryCode().\n";
                }

                ValidateRequiredFieldsForOrder(order);
                ValidateOrderRow(order);
                if (order.GetOrderDate() == null)
                {
                    Errors += "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
                }
            }
            catch (NullReferenceException ex)
            {
                Errors += "MISSING VALUE - CustomerIdentity must be set.\n";
            }
            return Errors;
        }
    }
}
