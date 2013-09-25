using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Order.Identity
{
    public abstract class Customer<T>:CustomerIdentity
    {
        public string GetEmail()
        {
            return Email;
        }

        public string GetFullName()
        {
            return FullName;
        }

        public string GetPhoneNumber()
        {
            return PhoneNumber;
        }

        public string GetZipCode()
        {
            return ZipCode;
        }

        public string GetIpAddress()
        {
            return IpAddress;
        }

        public string GetStreetAddress()
        {
            return Street;
        }

        public string GetHouseNumber()
        {
            return HouseNumber;
        }

        public string GetLocality()
        {
            return Locality;
        }        

        public string GetCoAddress()
        {
            return CoAddress;
        }

        protected void SetFullName(string firstName, string lastName)
        {
            FullName = firstName + " " + lastName;
        }

        public abstract T SetCoAddress(string coAddress);

        /// <summary>
        /// Optional but desirable
        /// </summary>
        /// <param name="email"></param>
        /// <returns>CustomerIdentity</returns>
        public abstract T SetEmail(string email);


        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>CustomerIdentity</returns>
        public abstract T SetPhoneNumber(string phoneNumber);

        /// <summary>
        /// Required for company and private customers in NL and DE
        /// </summary>
        /// <param name="locality"></param>
        /// <returns>CustomerIdentity</returns>
        public abstract T SetLocality(string locality);

        /// <summary>
        /// Required for company customers in NL and DE
        /// </summary>
        /// <param name="streetAddress"></param>
        /// /// <param name="houseNumber"></param>
        /// <returns>CustomerIdentity</returns>
        public abstract T SetStreetAddress(string streetAddress, string houseNumber);

        /// <summary>
        /// Required for company and private customers in NL and DE
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns>CustomerIdentity</returns>
        public abstract T SetZipCode(string zipCode);

        public abstract T SetIpAddress(string ipAddress);

        public string GetNationalIdNumber()
        {
            return NationalIdNumber;
        }
    }
}