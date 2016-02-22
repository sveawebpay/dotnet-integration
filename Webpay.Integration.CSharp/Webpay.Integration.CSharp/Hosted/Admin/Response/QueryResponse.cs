using System;
using System.Collections.Generic;
using System.Xml;
using Webpay.Integration.CSharp.Order.Row;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public class QueryResponse : CustomerRefNoResponseBase
    {
        public readonly Transaction Transaction;

        public QueryResponse(XmlDocument response) : base(response)
        {
            if (response.SelectSingleNode("/response/transaction") != null)
            {
                TransactionCustomer customer = new TransactionCustomer(
                    AttributeString(response, "/response/transaction/customer", "id"),
                    TextString(response, "/response/transaction/customer/firstname"),
                    TextString(response, "/response/transaction/customer/lastname"),
                    TextString(response, "/response/transaction/customer/initials"),
                    TextString(response, "/response/transaction/customer/fullname"),
                    TextString(response, "/response/transaction/customer/email"),
                    TextString(response, "/response/transaction/customer/ssn"),
                    TextString(response, "/response/transaction/customer/address"),
                    TextString(response, "/response/transaction/customer/address2"),
                    TextString(response, "/response/transaction/customer/city"),
                    TextString(response, "/response/transaction/customer/country"),
                    TextString(response, "/response/transaction/customer/zip"),
                    TextString(response, "/response/transaction/customer/phone"),
                    TextString(response, "/response/transaction/customer/vatnumber"),
                    TextString(response, "/response/transaction/customer/housenumber"),
                    TextString(response, "/response/transaction/customer/companyname")
                    );

                Transaction = new Transaction(
                    TextString(response, "/response/transaction/customerrefno"),
                    TextInt(response, "/response/transaction/merchantid").Value,
                    TextString(response, "/response/transaction/status"),
                    MinorCurrencyToDecimalAmount(TextInt(response, "/response/transaction/amount").Value),
                    TextString(response, "/response/transaction/currency"),
                    MinorCurrencyToDecimalAmount(TextInt(response, "/response/transaction/vat")),
                    MinorCurrencyToDecimalAmount(TextInt(response, "/response/transaction/capturedamount")),
                    MinorCurrencyToDecimalAmount(TextInt(response, "/response/transaction/authorizedamount")),
                    DateTime.Parse(TextString(response, "/response/transaction/created")),
                    TextString(response, "/response/transaction/creditstatus"),
                    MinorCurrencyToDecimalAmount(TextInt(response, "/response/transaction/creditedamount")),
                    TextInt(response, "/response/transaction/merchantresponsecode"),
                    TextString(response, "/response/transaction/paymentmethod"),
                    TextString(response, "/response/transaction/callbackurl"),
                    TextString(response, "/response/transaction/subscriptionid"),
                    TextString(response, "/response/transaction/subscriptiontype"),
                    TextString(response, "/response/transaction/eci"),
                    TextString(response, "/response/transaction/mdstatus"),
                    TextString(response, "/response/transaction/expiryyear"),
                    TextString(response, "/response/transaction/expirymonth"),
                    TextString(response, "/response/transaction/ch_name"),
                    TextString(response, "/response/transaction/authcode"),
                    customer
                    );

                var enumerator = response.SelectNodes("/response/transaction/orderrows/row").GetEnumerator();
                var rowNumber = 1;  // NumberedOrderRows are 1-indexed
                while (enumerator.MoveNext())
                {
                    var xmlNode = (XmlNode)enumerator.Current;

                    Transaction.OrderRows.Add(new TransactionOrderRow(
                        TextString(xmlNode, "./id"),
                        TextString(xmlNode, "./name"),
                        MinorCurrencyToDecimalAmount(TextInt(xmlNode, "./amount").Value),
                        MinorCurrencyToDecimalAmount(TextInt(xmlNode, "./vat")),
                        TextString(xmlNode, "./description"),
                        TextDecimal(xmlNode, "./quantity").GetValueOrDefault(1),
                        TextString(xmlNode, "./sku"),
                        TextString(xmlNode, "./unit")
                        ));

                    decimal row_amount = MinorCurrencyToDecimalAmount(TextInt(xmlNode, "./amount").Value);
                    decimal row_vat = MinorCurrencyToDecimalAmount(TextInt(xmlNode, "./vat")) ?? 0M;
                    decimal row_amountExVat = (row_amount - row_vat);
                    decimal row_vatPercent = (row_vat / (row_amountExVat)) *100;

                    var numberedOrderRow = new NumberedOrderRowBuilder()
                        .SetRowNumber( rowNumber++ )
                        .SetCreditInvoiceId(null)
                        .SetInvoiceId(null)
                        .SetStatus(null)
                        .SetArticleNumber(TextString(xmlNode, "./sku"))
                        .SetName(TextString(xmlNode, "./name"))
                        .SetDescription(TextString(xmlNode, "./description"))
                        .SetAmountExVat(row_amountExVat)
                        .SetAmountIncVat(row_amount)
                        .SetVatPercent(row_vatPercent)
                        .SetQuantity(TextDecimal(xmlNode, "./quantity").GetValueOrDefault(1))
                        .SetUnit(TextString(xmlNode, "./unit"))
                        .SetVatDiscount(0)
                        .SetDiscountPercent(0)
                    ;
                    Transaction.NumberedOrderRows.Add(numberedOrderRow);
                }

            }
        }

    }
    public class Transaction
    {
        public readonly string CustomerRefNo;
        public readonly string ClientOrderNumber;
        public readonly int MerchantId;
        public readonly string Status;
        public readonly decimal Amount;
        public readonly string Currency;
        public readonly decimal? Vat;
        public readonly decimal? CapturedAmount;
        public readonly decimal? AuthorizedAmount;
        public readonly DateTime Created;
        public readonly string CreditStatus;
        public readonly decimal? CreditedAmount;
        public readonly int? MerchantResponseCode;
        public readonly string PaymentMethod;
        public readonly string CallbackUrl;
        public readonly string SubscriptionId;
        public readonly string SubscriptionType;
        public readonly TransactionCustomer Customer;
        public readonly string Eci;
        public readonly string MdStatus;
        public readonly string ExpiryYear;
        public readonly string ExpiryMonth;
        public readonly string ChName;
        public readonly string AuthCode;

        public readonly IList<TransactionOrderRow> OrderRows; // TODO set to private
        public readonly IList<NumberedOrderRowBuilder> NumberedOrderRows; 

        public Transaction(string customerRefNo, int merchantId, string status, decimal amount, string currency, decimal? vat, decimal? capturedAmount, decimal? authorizedAmount, DateTime created, string creditStatus, decimal? creditedAmount, int? merchantResponseCode, string paymentMethod, string callbackUrl, string subscriptionId, string subscriptionType, string eci, string mdStatus, string expiryYear, string expiryMonth, string chName, string authCode, TransactionCustomer customer)
        {
            CustomerRefNo = customerRefNo;
            ClientOrderNumber = CustomerRefNo;
            MerchantId = merchantId;
            Status = status;
            Amount = amount;
            Currency = currency;
            Vat = vat;
            CapturedAmount = capturedAmount;
            AuthorizedAmount = authorizedAmount;
            Created = created;
            CreditStatus = creditStatus;
            CreditedAmount = creditedAmount;
            MerchantResponseCode = merchantResponseCode;
            PaymentMethod = paymentMethod;
            CallbackUrl = callbackUrl;
            SubscriptionId = subscriptionId;
            SubscriptionType = subscriptionType;
            Customer = customer;
            Eci = eci;
            MdStatus = mdStatus;
            ExpiryYear = expiryYear;
            ExpiryMonth = expiryMonth;
            ChName = chName;
            AuthCode = authCode;

            OrderRows = new List<TransactionOrderRow>();
            NumberedOrderRows = new List<NumberedOrderRowBuilder>();
        }


    }

    public class TransactionOrderRow
    {
        public readonly string Id;
        public readonly string Name;
        public readonly decimal Amount;
        public readonly decimal? Vat;
        public readonly string Description;
        public readonly decimal Quantity;
        public readonly string Sku;
        public readonly string Unit;

        public TransactionOrderRow(string id, string name, decimal amount, decimal? vat, string description, decimal quantity, string sku, string unit)
        {
            Id = id;
            Name = name;
            Amount = amount;
            Vat = vat;
            Description = description;
            Quantity = quantity;
            Sku = sku;
            Unit = unit;
        }
    }

    public class TransactionCustomer
    {
        public TransactionCustomer(string id, string firstName, string lastName, string initials, string fullName, string email, string ssn, string address, string address2, string city, string country, string zip, string phone, string vatNumber, string houseNumber, string companyName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Initials = initials;
            FullName = fullName;
            Email = email;
            Ssn = ssn;
            Address = address;
            Address2 = address2;
            City = city;
            Country = country;
            Zip = zip;
            Phone = phone;
            VatNumber = vatNumber;
            HouseNumber = houseNumber;
            CompanyName = companyName;
        }

        public readonly string Id;
        public readonly string FirstName;
        public readonly string LastName;
        public readonly string Initials;
        public readonly string FullName;
        public readonly string Email;
        public readonly string Ssn;
        public readonly string Address;
        public readonly string Address2;
        public readonly string City;
        public readonly string Country;
        public readonly string Zip;
        public readonly string Phone;
        public readonly string VatNumber;
        public readonly string HouseNumber;
        public readonly string CompanyName;
    }
}