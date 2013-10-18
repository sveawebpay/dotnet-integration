using NUnit.Framework;
using Webpay.Integration.CSharp.Response.Hosted;

namespace Webpay.Integration.CSharp.Test.Response
{
    [TestFixture]
    public class HostedPaymentResponseTest
    {
        [Test]
        public void TestDirectBankResponse()
        {
            const string testXmlResponseBase64 =
                "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY2OTg5Ij4NCiAgICA8cGF5bWVudG1ldGhvZD5EQk5PUkRFQVNFPC9wYXltZW50bWV0aG9kPg0KICAgIDxtZXJjaGFudGlkPjExNzU8L21lcmNoYW50aWQ+DQogICAgPGN1c3RvbWVycmVmbm8+MzczNzgyMzk4N19pZF8wMDE8L2N1c3RvbWVycmVmbm8+DQogICAgPGFtb3VudD41MDA8L2Ftb3VudD4NCiAgICA8Y3VycmVuY3k+U0VLPC9jdXJyZW5jeT4NCiAgPC90cmFuc2FjdGlvbj4NCiAgPHN0YXR1c2NvZGU+MDwvc3RhdHVzY29kZT4NCjwvcmVzcG9uc2U+";
            var response = new SveaResponse(testXmlResponseBase64, null);

            Assert.AreEqual("566989", response.TransactionId);
            Assert.AreEqual("DBNORDEASE", response.PaymentMethod);
            Assert.AreEqual("1175", response.MerchantId);
            Assert.AreEqual("3737823987_id_001", response.ClientOrderNumber);
            Assert.AreEqual(5, response.Amount);
            Assert.AreEqual("SEK", response.Currency);
            Assert.AreEqual("0 (ORDER_ACCEPTED)", response.ResultCode);
        }

        [Test]
        public void TestCreatePaymentResponse()
        {
            const string testXmlResponseBase64 =
                "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY2OTIzIj4NCiAgICA8cGF5bWVudG1ldGhvZD5LT1JUQ0VSVDwvcGF5bWVudG1ldGhvZD4NCiAgICA8bWVyY2hhbnRpZD4xMTc1PC9tZXJjaGFudGlkPg0KICAgIDxjdXN0b21lcnJlZm5vPnRlc3RfMTM1OTQ2MDU3NjQ5MTwvY3VzdG9tZXJyZWZubz4NCiAgICA8YW1vdW50PjUwMDwvYW1vdW50Pg0KICAgIDxjdXJyZW5jeT5TRUs8L2N1cnJlbmN5Pg0KICAgIDxjYXJkdHlwZT5WSVNBPC9jYXJkdHlwZT4NCiAgICA8bWFza2VkY2FyZG5vPjQ0NDQzM3h4eHh4eDMzMDA8L21hc2tlZGNhcmRubz4NCiAgICA8ZXhwaXJ5bW9udGg+MDM8L2V4cGlyeW1vbnRoPg0KICAgIDxleHBpcnl5ZWFyPjIwPC9leHBpcnl5ZWFyPg0KICAgIDxhdXRoY29kZT4xNTI1ODc8L2F1dGhjb2RlPg0KICA8L3RyYW5zYWN0aW9uPg0KICA8c3RhdHVzY29kZT4wPC9zdGF0dXNjb2RlPg0KPC9yZXNwb25zZT4=";
            var response = new SveaResponse(testXmlResponseBase64, null);

            Assert.AreEqual(true, response.OrderAccepted);
            Assert.AreEqual("566923", response.TransactionId);
            Assert.AreEqual("test_1359460576491", response.ClientOrderNumber);
            Assert.AreEqual(5.00, response.Amount);
            Assert.AreEqual("SEK", response.Currency);
            Assert.AreEqual("VISA", response.CardType);
            Assert.AreEqual("03", response.ExpiryMonth);
            Assert.AreEqual("20", response.ExpiryYear);
            Assert.AreEqual("152587", response.AuthCode);
        }

        [Test]
        public void TestPayPageDirectBankResponse()
        {
            const string testXmlResponseBase64 =
                "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY3MDU2Ij4NCiAgICA8cGF5bWVudG1ldGhvZD5EQk5PUkRFQVNFPC9wYXltZW50bWV0aG9kPg0KICAgIDxtZXJjaGFudGlkPjExNzU8L21lcmNoYW50aWQ+DQogICAgPGN1c3RvbWVycmVmbm8+dGVzdF8xMzU5NjIwMzExNTg0PC9jdXN0b21lcnJlZm5vPg0KICAgIDxhbW91bnQ+NTAwPC9hbW91bnQ+DQogICAgPGN1cnJlbmN5PlNFSzwvY3VycmVuY3k+DQogIDwvdHJhbnNhY3Rpb24+DQogIDxzdGF0dXNjb2RlPjA8L3N0YXR1c2NvZGU+DQo8L3Jlc3BvbnNlPg==";
            var response = new SveaResponse(testXmlResponseBase64, null);

            Assert.AreEqual(true, response.OrderAccepted);
            Assert.AreEqual("0 (ORDER_ACCEPTED)", response.ResultCode);
            Assert.AreEqual("567056", response.TransactionId);
            Assert.AreEqual("1175", response.MerchantId);
            Assert.AreEqual(5, response.Amount);
            Assert.AreEqual("test_1359620311584", response.ClientOrderNumber);
            Assert.AreEqual("SEK", response.Currency);
            Assert.AreEqual("DBNORDEASE", response.PaymentMethod);
        }

        [Test]
        public void TestPayPageDirectBankInterruptedResponse()
        {
            const string testXmlResponseBase64 =
                "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY3MDYyIj4NCiAgICA8cGF5bWVudG1ldGhvZD5EQk5PUkRFQVNFPC9wYXltZW50bWV0aG9kPg0KICAgIDxtZXJjaGFudGlkPjExNzU8L21lcmNoYW50aWQ+DQogICAgPGN1c3RvbWVycmVmbm8+dGVzdF8xMzU5NjIzMDIyMTQzPC9jdXN0b21lcnJlZm5vPg0KICAgIDxhbW91bnQ+NTAwPC9hbW91bnQ+DQogICAgPGN1cnJlbmN5PlNFSzwvY3VycmVuY3k+DQogIDwvdHJhbnNhY3Rpb24+DQogIDxzdGF0dXNjb2RlPjEwNzwvc3RhdHVzY29kZT4NCjwvcmVzcG9uc2U+DQo=";

            var response = new SveaResponse(testXmlResponseBase64, null);
            Assert.AreEqual("107 (DENIED_BY_BANK)", response.ResultCode);
        }

        [Test]
        public void TestPayPageCardPaymentResponse()
        {
            const string testXmlResponseBase64 =
                "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY3MDU4Ij4NCiAgICA8cGF5bWVudG1ldGhvZD5LT1JUQ0VSVDwvcGF5bWVudG1ldGhvZD4NCiAgICA8bWVyY2hhbnRpZD4xMTc1PC9tZXJjaGFudGlkPg0KICAgIDxjdXN0b21lcnJlZm5vPnRlc3RfMTM1OTYyMTQ2NTk5MDwvY3VzdG9tZXJyZWZubz4NCiAgICA8YW1vdW50PjUwMDwvYW1vdW50Pg0KICAgIDxjdXJyZW5jeT5TRUs8L2N1cnJlbmN5Pg0KICAgIDxjYXJkdHlwZT5WSVNBPC9jYXJkdHlwZT4NCiAgICA8bWFza2VkY2FyZG5vPjQ0NDQzM3h4eHh4eDMzMDA8L21hc2tlZGNhcmRubz4NCiAgICA8ZXhwaXJ5bW9udGg+MDM8L2V4cGlyeW1vbnRoPg0KICAgIDxleHBpcnl5ZWFyPjIwPC9leHBpcnl5ZWFyPg0KICAgIDxhdXRoY29kZT43NjQ4Nzc8L2F1dGhjb2RlPg0KICA8L3RyYW5zYWN0aW9uPg0KICA8c3RhdHVzY29kZT4wPC9zdGF0dXNjb2RlPg0KPC9yZXNwb25zZT4NCg==";
            var response = new SveaResponse(testXmlResponseBase64, null);

            Assert.AreEqual("567058", response.TransactionId);
            Assert.AreEqual("KORTCERT", response.PaymentMethod);
            Assert.AreEqual("1175", response.MerchantId);
            Assert.AreEqual("test_1359621465990", response.ClientOrderNumber);
            Assert.AreEqual(5, response.Amount);
            Assert.AreEqual("SEK", response.Currency);
            Assert.AreEqual("VISA", response.CardType);
            Assert.AreEqual("444433xxxxxx3300", response.MaskedCardNumber);
            Assert.AreEqual("03", response.ExpiryMonth);
            Assert.AreEqual("20", response.ExpiryYear);
            Assert.AreEqual("764877", response.AuthCode);
            Assert.AreEqual("0 (ORDER_ACCEPTED)", response.ResultCode);
        }

        [Test]
        public void TestSetErrorParamsCode101()
        {
            const string responseXmlBase64 =
                "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY3MDU4Ij4NCiAgICA8cGF5bWVudG1ldGhvZD5LT1JUQ0VSVDwvcGF5bWVudG1ldGhvZD4NCiAgICA8bWVyY2hhbnRpZD4xMTc1PC9tZXJjaGFudGlkPg0KICAgIDxjdXN0b21lcnJlZm5vPnRlc3RfMTM1OTYyMTQ2NTk5MDwvY3VzdG9tZXJyZWZubz4NCiAgICA8YW1vdW50PjUwMDwvYW1vdW50Pg0KICAgIDxjdXJyZW5jeT5TRUs8L2N1cnJlbmN5Pg0KICAgIDxjYXJkdHlwZT5WSVNBPC9jYXJkdHlwZT4NCiAgICA8bWFza2VkY2FyZG5vPjQ0NDQzM3h4eHh4eDMzMDA8L21hc2tlZGNhcmRubz4NCiAgICA8ZXhwaXJ5bW9udGg+MDM8L2V4cGlyeW1vbnRoPg0KICAgIDxleHBpcnl5ZWFyPjIwPC9leHBpcnl5ZWFyPg0KICAgIDxhdXRoY29kZT43NjQ4Nzc8L2F1dGhjb2RlPg0KICA8L3RyYW5zYWN0aW9uPg0KICA8c3RhdHVzY29kZT4xMDE8L3N0YXR1c2NvZGU+DQo8L3Jlc3BvbnNlPg==";
            var response = new SveaResponse(responseXmlBase64, null);

            Assert.AreEqual("Invalid XML.", response.ErrorMessage);
        }
    }
}