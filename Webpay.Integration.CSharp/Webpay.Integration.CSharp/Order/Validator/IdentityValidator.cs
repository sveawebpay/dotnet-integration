using Webpay.Integration.CSharp.Order.Create;

namespace Webpay.Integration.CSharp.Order.Validator
{
    public class IdentityValidator
    {
        public string ValidateNordicIdentity(CreateOrderBuilder order)
        {
            //check Company identity
            if (order.GetCustomerIdentity().NationalIdNumber == null)
            {
                return order.GetIsCompanyIdentity()
                           ? "MISSING VALUE - Organisation number is required for company customers when countrycode is SE, NO, DK or FI. Use SetNationalIdNumber(...).\n"
                           : "MISSING VALUE - National number(ssn) is required for individual customers when countrycode is SE, NO, DK or FI. Use SetNationalIdNumber(...).\n";
            }

            return "";
        }

        public string ValidateNlIdentity(CreateOrderBuilder order)
        {
            string errors = "";
            //Individual
            if (!order.GetIsCompanyIdentity())
            {
                if (order.GetIndividualCustomer().GetInitials() == null)
                {
                    errors +=
                        "MISSING VALUE - Initials is required for individual customers when countrycode is NL. Use SetInitials().\n";
                }
                if (order.GetIndividualCustomer().GetBirthDate() == null)
                {
                    errors +=
                        "MISSING VALUE - Birth date is required for individual customers when countrycode is NL. Use SetBirthDate().\n";
                }
                if (order.GetIndividualCustomer().GetFirstName() == null ||
                    order.GetIndividualCustomer().GetLastName() == null)
                {
                    errors +=
                        "MISSING VALUE - Name is required for individual customers when countrycode is NL. Use SetName().\n";
                }
            }

            //Company
            if (order.GetIsCompanyIdentity())
            {
                if (order.GetCompanyCustomer().GetVatNumber() == null)
                {
                    errors +=
                        "MISSING VALUE - Vat number is required for company customers when countrycode is NL. Use SetVatNumber().\n";
                }
                if (order.GetCompanyCustomer().GetCompanyName() == null)
                {
                    errors +=
                        "MISSING VALUE - Company name is required for individual customers when countrycode is NL. Use SetName().\n";
                }
            }

            //Individual and Company
            if (order.GetCustomerIdentity().Street == null || order.GetCustomerIdentity().HouseNumber == null)
            {
                errors +=
                    "MISSING VALUE - Street address and house number is required for all customers when countrycode is NL. Use SetStreetAddress().\n";
            }
            if (order.GetCustomerIdentity().Locality == null)
            {
                errors +=
                    "MISSING VALUE - Locality is required for all customers when countrycode is NL. Use SetLocality().\n";
            }
            if (order.GetCustomerIdentity().ZipCode == null)
            {
                errors +=
                    "MISSING VALUE - Zip code is required for all customers when countrycode is NL. Use SetZipCode().\n";
            }

            return errors;
        }

        public string ValidateDeIdentity(CreateOrderBuilder order)
        {
            string errors = "";
            //Individual
            if (!order.GetIsCompanyIdentity())
            {
                if (order.GetIndividualCustomer().GetBirthDate() == null)
                {
                    errors +=
                        "MISSING VALUE - Birth date is required for individual customers when countrycode is DE. Use SetBirthDate().\n";
                }
                if (order.GetIndividualCustomer().GetFirstName() == null ||
                    order.GetIndividualCustomer().GetLastName() == null)
                {
                    errors +=
                        "MISSING VALUE - Name is required for individual customers when countrycode is DE. Use SetName().\n";
                }
            }

            //Company
            if (order.GetIsCompanyIdentity() &&
                order.GetCompanyCustomer().GetVatNumber() == null)
            {
                errors +=
                    "MISSING VALUE - Vat number is required for company customers when countrycode is DE. Use SetVatNumber().\n";
            }

            //Individual and Company
            if (order.GetCustomerIdentity().Street == null || order.GetCustomerIdentity().HouseNumber == null)
            {
                errors +=
                    "MISSING VALUE - Street address is required for all customers when countrycode is DE. Use SetStreetAddress().\n";
            }
            if (order.GetCustomerIdentity().Locality == null)
            {
                errors +=
                    "MISSING VALUE - Locality is required for all customers when countrycode is DE. Use SetLocality().\n";
            }
            if (order.GetCustomerIdentity().ZipCode == null)
            {
                errors +=
                    "MISSING VALUE - Zip code is required for all customers when countrycode is DE. Use SetCustomerZipCode().\n";
            }
            return errors;
        }
    }
}