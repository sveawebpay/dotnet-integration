using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Hosted.Payment;
using Webpay.Integration.CSharp.Order.Identity;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Order.Validator;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Payment;

namespace Webpay.Integration.CSharp.Order.Create
{
    /// <summary>
    /// Builds the order up in the fluent api by using create order methods.
    /// End by choosing payment type.
    /// </summary>
    public class CreateOrderBuilder : OrderBuilder<CreateOrderBuilder>
    {
        private OrderValidator _validator;

        private string _clientOrderNumber;
        private string _customerReference;
        private DateTime _orderDate;
        private string _currency;
        private long? _campaignCode;
        private bool _sendAutomaticGiroPaymentForm;
        private bool _hasSetCountryCode;

        protected CustomerIdentity CustomerId;

        public CreateOrderBuilder(IConfigurationProvider config)
        {
            _config = config;
        }

        public OrderValidator GetValidator()
        {
            return _validator;
        }

        public CreateOrderBuilder SetValidator(OrderValidator validator)
        {
            _validator = validator;
            return this;
        }

        /// <summary>
        /// Start build order request to create an order for all payments.
        /// </summary>
        /// <returns>Unique order number from client string</returns>
        public string GetClientOrderNumber()
        {
            return _clientOrderNumber;
        }

        public CreateOrderBuilder SetClientOrderNumber(string clientOrderNumber)
        {
            _clientOrderNumber = clientOrderNumber;
            return this;
        }

        public string GetCustomerReference()
        {
            return _customerReference;
        }

        public CreateOrderBuilder SetCustomerReference(string customerReference)
        {
            _customerReference = customerReference;
            return this;
        }

        public DateTime GetOrderDate()
        {
            return _orderDate;
        }

        public CreateOrderBuilder SetOrderDate(DateTime orderDate)
        {
            _orderDate = orderDate;
            return this;
        }

        public override CreateOrderBuilder SetFixedDiscountRows(List<FixedDiscountBuilder> fixedDiscountRows)
        {
            FixedDiscountRows = fixedDiscountRows;
            return this;
        }

        public override CreateOrderBuilder SetRelativeDiscountRows(List<RelativeDiscountBuilder> relativeDiscountRows)
        {
            RelativeDiscountRows = relativeDiscountRows;
            return this;
        }

        public override CreateOrderBuilder AddOrderRow(OrderRowBuilder itemOrderRow)
        {
            OrderRows.Add(itemOrderRow);
            return this;
        }

        public override CreateOrderBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            if (CustomerId != null)
            {
                CustomerId.CountryCode = countryCode.ToString().ToUpper();

                if (_countryCode == CountryCode.SE && CustomerId.CustomerType == CustomerType.Company)
                {
                    CustomerId.NationalIdNumber = CustomerId.CompanyIdentity.CompanyVatNumber ??
                                                  CustomerId.NationalIdNumber;
                    CustomerId.CompanyIdentity = null;
                }
            }

            _hasSetCountryCode = true;
            return this;
        }

        public override CreateOrderBuilder AddOrderRows(IEnumerable<OrderRowBuilder> itemOrderRow)
        {
            OrderRows.AddRange(itemOrderRow);
            return this;
        }

        public override CreateOrderBuilder AddDiscount(IRowBuilder itemDiscount)
        {
            if (itemDiscount is FixedDiscountBuilder)
            {
                FixedDiscountRows.Add(itemDiscount as FixedDiscountBuilder);
            }
            else
            {
                RelativeDiscountRows.Add(itemDiscount as RelativeDiscountBuilder);
            }
            return this;
        }

        public override CreateOrderBuilder AddFee(IRowBuilder itemFee)
        {
            if (itemFee.GetType() == new ShippingFeeBuilder().GetType())
            {
                ShippingFeeRows.Add((ShippingFeeBuilder) itemFee);
            }
            else
            {
                InvoiceFeeRows.Add((InvoiceFeeBuilder) itemFee);
            }
            return this;
        }

        public string GetCurrency()
        {
            return _currency;
        }

        public CreateOrderBuilder SetCurrency(Currency currency)
        {
            _currency = currency.ToString().ToUpper();
            return this;
        }

        public long? GetCampaignCode()
        {
            return _campaignCode;
        }

        public CreateOrderBuilder SetCampaignCode(long? campaignCode)
        {
            _campaignCode = campaignCode;
            return this;
        }

        public bool GetSendAutomaticGiroPaymentForm()
        {
            return _sendAutomaticGiroPaymentForm;
        }

        public CreateOrderBuilder SetSendAutomaticGiroPaymentForm(bool sendAutomaticGiroPaymentForm)
        {
            _sendAutomaticGiroPaymentForm = sendAutomaticGiroPaymentForm;
            return this;
        }

        /// <summary>
        /// Start creating card payment via PayPage. Returns Payment form to integrate in shop.
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>CardPayment</returns>
        public CardPayment UsePayPageCardOnly()
        {
            return new CardPayment(this);
        }

        /// <summary>
        /// Start creating direct bank payment via PayPage. Returns Payment form to integrate in shop.
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>DirectPayment</returns>
        public DirectPayment UsePayPageDirectBankOnly()
        {
            return new DirectPayment(this);
        }

        /// <summary>
        /// Start creating payment through PayPage. You will need to customize the PayPage.
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>PayPagePayment</returns>
        public PayPagePayment UsePayPage()
        {
            return new PayPagePayment(this);
        }

        /// <summary>
        /// Start creating payment with a specific payment method. This method will directly direct to the specified payment method.
        /// Payment methods are found in appendix in our documentation.
        /// </summary>
        /// <param name="paymentMethod"></param>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>PaymentMethodPayment</returns>
        public PaymentMethodPayment UsePaymentMethod(PaymentMethod paymentMethod)
        {
            return new PaymentMethodPayment(this, paymentMethod);
        }

        /// <summary>
        /// Start create invoicePayment
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>PaymentPlanPayment</returns>
        public InvoicePayment UseInvoicePayment()
        {
            return new InvoicePayment(this);
        }

        /// <summary>
        /// Start creating payment plan payment
        /// </summary>
        /// <param name="campaignCode"></param>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>PaymentPlanPayment</returns>
        public PaymentPlanPayment UsePaymentPlanPayment(long? campaignCode)
        {
            if (campaignCode == null)
            {
                throw new SveaWebPayValidationException(
                    "MISSING VALUE - Campaign code must be set. Add parameter in .UsePaymentPlanPayment(campaignCode)");
            }
            if (CustomerId is CompanyCustomer)
            {
                throw new SveaWebPayValidationException(
                    "ERROR - CompanyCustomer is not allowed to use payment plan option.");
            }

            return UsePaymentPlanPayment(campaignCode, false);
        }

        /// <summary>
        /// Start creating payment plan payment
        /// </summary>
        /// <param name="campaignCode"></param>
        /// <param name="sendAutomaticGiroPaymentForm"></param>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>PaymentPlanPayment</returns>
        public PaymentPlanPayment UsePaymentPlanPayment(long? campaignCode, bool sendAutomaticGiroPaymentForm)
        {
            _campaignCode = campaignCode;
            _sendAutomaticGiroPaymentForm = sendAutomaticGiroPaymentForm;
            return new PaymentPlanPayment(this);
        }

        public CreateOrderBuilder AddCustomerDetails(CustomerIdentity customerIdentity)
        {
            CustomerId = customerIdentity;
            if (_hasSetCountryCode)
            {
                CustomerId.CountryCode = _countryCode.ToString().ToUpper();
            }
            return this;
        }

        public bool GetIsCompanyIdentity()
        {
            if (CustomerId == null)
            {
                return false;
            }
            return CustomerId.CustomerType == CustomerType.Company;
        }

        public CompanyCustomer GetCompanyCustomer()
        {
            return (CompanyCustomer) GetCustomerIdentity();
        }

        public IndividualCustomer GetIndividualCustomer()
        {
            return (IndividualCustomer) GetCustomerIdentity();
        }

        public CustomerIdentity GetCustomerIdentity()
        {
            return CustomerId;
        }

        public CustomerIdentity GetSoapPurifiedCustomer()
        {
            var customer = new CustomerIdentity
            {
                CoAddress = CustomerId.CoAddress,
                CompanyIdentity =
                    (CustomerId.CountryCode != null && (
                            CustomerId.CountryCode != "DE" &&
                            CustomerId.CountryCode != "NL"))
                        ? null
                        : CustomerId.CompanyIdentity,
                CountryCode = CustomerId.CountryCode,
                CustomerType = CustomerId.CustomerType,
                Email = CustomerId.Email,
                FullName = CustomerId.FullName,
                HouseNumber = CustomerId.HouseNumber,
                IndividualIdentity =
                    (CustomerId.CountryCode != null && (
                            CustomerId.CountryCode != "DE" &&
                            CustomerId.CountryCode != "NL"))
                        ? null
                        : CustomerId.IndividualIdentity,
                IpAddress = CustomerId.IpAddress,
                Locality = CustomerId.Locality,
                NationalIdNumber = CustomerId.NationalIdNumber,
                PhoneNumber = CustomerId.PhoneNumber,
                Street = CustomerId.Street,
                ZipCode = CustomerId.ZipCode
            };

            return customer;
        }

        /// <summary>
        /// Build
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>CreateOrderBuilder</returns>
        public CreateOrderBuilder Build()
        {
            _validator.Validate(this);
            return this;
        }

    }
}