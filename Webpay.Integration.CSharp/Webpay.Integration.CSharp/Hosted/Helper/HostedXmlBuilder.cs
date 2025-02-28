﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Webpay.Integration.CSharp.Hosted.Payment;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Xml;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Hosted.Helper
{
    public class HostedXmlBuilder
    {
        public string GetXml(HostedPayment payment)
        {
            var xmlOutput = new Utf8StringWriter(Encoding.UTF8);
            var order = payment.GetCreateOrderBuilder();
            var rows = payment.GetRowBuilder();

            var xmlSettings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8
                };

            using (var xmlw = XmlWriter.Create(xmlOutput, xmlSettings))
            {
                Action<string, string> doWriteSimple = (name, value) => WriteSimpleElement(name, value, xmlw);

                xmlw.WriteStartDocument();
                xmlw.WriteComment("Message generated by Integration package C#");
                xmlw.WriteStartElement("payment");

                payment.WritePaymentSpecificXml(xmlw);

                doWriteSimple("customerrefno", order.GetClientOrderNumber()); //This is not an error. Do not confuse with order.GetCustomerReference().
                doWriteSimple("currency", order.GetCurrency());
                doWriteSimple("amount", payment.GetAmount().ToString(CultureInfo.InvariantCulture));
                doWriteSimple("vat", payment.GetVat().ToString(CultureInfo.InvariantCulture));
                doWriteSimple("lang", payment.GetPayPageLanguageCode().ToLower());
                doWriteSimple("returnurl", payment.GetReturnUrl());
                doWriteSimple("cancelurl", payment.GetCancelUrl());
                doWriteSimple("callbackurl", payment.GetCallbackUrl());
                doWriteSimple("iscompany", order.GetIsCompanyIdentity().ToString().ToLower());
                doWriteSimple("ipaddress", payment.GetIpAddress());
                doWriteSimple("payeralias", payment.GetPayerAlias());
                doWriteSimple("message", payment.GetMessage());

                SerializeCustomer(order, xmlw);
                SerializeRows(rows, xmlw);
                SerializeExcludedPaymentMethods(payment.GetExcludedPaymentMethod(), xmlw);

                doWriteSimple("addinvoicefee", "false");
                xmlw.WriteEndDocument();
            }

            return xmlOutput.ToString();
        }

        private void SerializeUnknownCustomer(CreateOrderBuilder order, XmlWriter xmlw)
        {
            Action<string, string> doWriteSimple = (name, value) => WriteSimpleElement(name, value, xmlw);

            xmlw.WriteStartElement("customer");
            doWriteSimple("unknowncustomer", "true");
            doWriteSimple("country", order.GetCountryCode().ToString().ToUpper());
            xmlw.WriteEndElement();
        }

        private void SerializeCustomer(CreateOrderBuilder order, XmlWriter xmlw)
        {
            Action<string, string> doWriteSimple = (name, value) => WriteSimpleElement(name, value, xmlw);

            if (order.GetCustomerIdentity() == null)
            {
                return;
            }

            CustomerIdentity customer;


            if (order.GetIsCompanyIdentity())
            {
                customer = order.GetCompanyCustomer();
            }
            else
            {
                customer = order.GetIndividualCustomer();
            }

            xmlw.WriteStartElement("customer");

            if (customer.NationalIdNumber != null) //nordic country individual customer type
            {
                doWriteSimple("ssn", customer.NationalIdNumber);
            }
            else if (!order.GetIsCompanyIdentity()) //euro country individual
            {
                doWriteSimple("ssn", customer.IndividualIdentity.BirthDate);
            }
            else if (order.GetIsCompanyIdentity() && order.GetCountryCode() != CountryCode.SE)
                //euro country, Company customer and nationalId not set
            {
                doWriteSimple("ssn", customer.CompanyIdentity.CompanyVatNumber);
            }

            //Individual customer
            if (!order.GetIsCompanyIdentity())
            {
                var individualIdentity = customer.IndividualIdentity;
                if (individualIdentity != null)
                {
                    doWriteSimple("firstname", individualIdentity.FirstName);
                    doWriteSimple("lastname", individualIdentity.LastName);
                    doWriteSimple("initials", individualIdentity.Initials);
                }
            }
            else //Company customer
            {
                doWriteSimple("firstname", customer.FullName);
            }

            //Address
            doWriteSimple("phone", customer.PhoneNumber);
            doWriteSimple("email", customer.Email);
            doWriteSimple("address", customer.Street);
            doWriteSimple("housenumber", customer.HouseNumber);
            doWriteSimple("address2", customer.CoAddress);
            doWriteSimple("zip", customer.ZipCode);
            doWriteSimple("city", customer.Locality);

            if (order.GetCountryCode() != CountryCode.NONE)
            {
                doWriteSimple("country", order.GetCountryCode().ToString().ToUpper());
            }

            xmlw.WriteEndElement();

            doWriteSimple("ipaddress",
                          order.GetIsCompanyIdentity()
                              ? order.GetCompanyCustomer().GetIpAddress()
                              : order.GetIndividualCustomer().GetIpAddress());
        }

        /// <summary>
        /// SerializeRows
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="xmlw"></param>
        private void SerializeRows(ICollection<HostedOrderRowBuilder> rows, XmlWriter xmlw)
        {
            if (rows == null || rows.Count == 0)
            {
                return;
            }

            xmlw.WriteStartElement("orderrows");

            foreach (var row in rows)
            {
                SerializeRow(row, xmlw);
            }

            xmlw.WriteEndElement();
        }

        /// <summary>
        /// SerializeRow
        /// </summary>
        /// <param name="row"></param>
        /// <param name="xmlw"></param>
        private void SerializeRow(HostedOrderRowBuilder row, XmlWriter xmlw)
        {
            xmlw.WriteStartElement("row");

            WriteSimpleElement("sku", row.GetSku() ?? "", xmlw);
            WriteSimpleElement("name", row.GetName() ?? "", xmlw);
            WriteSimpleElement("description", row.GetDescription() ?? "", xmlw);

            WriteSimpleElement("amount", row.GetAmount().ToString(CultureInfo.InvariantCulture), xmlw);
            WriteSimpleElement("vat", row.GetVat().ToString(CultureInfo.InvariantCulture), xmlw);

            if (row.GetQuantity() > 0)
            {
                WriteSimpleElement("quantity", row.GetQuantity().ToString(CultureInfo.InvariantCulture), xmlw);
            }

            WriteSimpleElement("unit", row.GetUnit(), xmlw);

            xmlw.WriteEndElement();
        }

        private void SerializeExcludedPaymentMethods(IEnumerable<string> excludedPaymentMethod, XmlWriter xmlw)
        {
            if (excludedPaymentMethod == null)
            {
                return;
            }

            xmlw.WriteStartElement("excludepaymentMethods");

            foreach (var str in excludedPaymentMethod)
            {
                WriteSimpleElement("exclude", str, xmlw);
            }

            xmlw.WriteEndElement();
        }

        private static void WriteSimpleElement(string name, string value, XmlWriter xmlw)
        {
            if (value == null)
            {
                return;
            }
            xmlw.WriteElementString(name, value);
        }
    }
}