using Webpay.Integration.Response.Hosted;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Config;

namespace Webpay.Integration.Test.Response;

[TestFixture]
public class HostedPaymentResponseTest
{
    [Test]
    public void TestDirectBankResponse()
    {
        const string testXmlResponseBase64 =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY2OTg5Ij4NCiAgICA8cGF5bWVudG1ldGhvZD5EQk5PUkRFQVNFPC9wYXltZW50bWV0aG9kPg0KICAgIDxtZXJjaGFudGlkPjExNzU8L21lcmNoYW50aWQ+DQogICAgPGN1c3RvbWVycmVmbm8+MzczNzgyMzk4N19pZF8wMDE8L2N1c3RvbWVycmVmbm8+DQogICAgPGFtb3VudD41MDA8L2Ftb3VudD4NCiAgICA8Y3VycmVuY3k+U0VLPC9jdXJyZW5jeT4NCiAgPC90cmFuc2FjdGlvbj4NCiAgPHN0YXR1c2NvZGU+MDwvc3RhdHVzY29kZT4NCjwvcmVzcG9uc2U+";
        var response = new SveaResponse(testXmlResponseBase64);

        Assert.That(response.TransactionId, Is.EqualTo("566989"));
        Assert.That(response.PaymentMethod, Is.EqualTo("DBNORDEASE"));
        Assert.That(response.MerchantId, Is.EqualTo("1175"));
        Assert.That(response.ClientOrderNumber, Is.EqualTo("3737823987_id_001"));
        Assert.That(response.Amount, Is.EqualTo(5));
        Assert.That(response.Currency, Is.EqualTo("SEK"));
        Assert.That(response.ResultCode, Is.EqualTo("0 (ORDER_ACCEPTED)"));
    }

    [Test]
    public void TestCreatePaymentResponse()
    {
        const string testXmlResponseBase64 =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY2OTIzIj4NCiAgICA8cGF5bWVudG1ldGhvZD5LT1JUQ0VSVDwvcGF5bWVudG1ldGhvZD4NCiAgICA8bWVyY2hhbnRpZD4xMTc1PC9tZXJjaGFudGlkPg0KICAgIDxjdXN0b21lcnJlZm5vPnRlc3RfMTM1OTQ2MDU3NjQ5MTwvY3VzdG9tZXJyZWZubz4NCiAgICA8YW1vdW50PjUwMDwvYW1vdW50Pg0KICAgIDxjdXJyZW5jeT5TRUs8L2N1cnJlbmN5Pg0KICAgIDxjYXJkdHlwZT5WSVNBPC9jYXJkdHlwZT4NCiAgICA8bWFza2VkY2FyZG5vPjQ0NDQzM3h4eHh4eDMzMDA8L21hc2tlZGNhcmRubz4NCiAgICA8ZXhwaXJ5bW9udGg+MDM8L2V4cGlyeW1vbnRoPg0KICAgIDxleHBpcnl5ZWFyPjIwPC9leHBpcnl5ZWFyPg0KICAgIDxhdXRoY29kZT4xNTI1ODc8L2F1dGhjb2RlPg0KICA8L3RyYW5zYWN0aW9uPg0KICA8c3RhdHVzY29kZT4wPC9zdGF0dXNjb2RlPg0KPC9yZXNwb25zZT4=";
        var response = new SveaResponse(testXmlResponseBase64);

        Assert.That(response.OrderAccepted, Is.True);
        Assert.That(response.TransactionId, Is.EqualTo("566923"));
        Assert.That(response.ClientOrderNumber, Is.EqualTo("test_1359460576491"));
        Assert.That(response.Amount, Is.EqualTo(5.00));
        Assert.That(response.Currency, Is.EqualTo("SEK"));
        Assert.That(response.CardType, Is.EqualTo("VISA"));
        Assert.That(response.ExpiryMonth, Is.EqualTo("03"));
        Assert.That(response.ExpiryYear, Is.EqualTo("20"));
        Assert.That(response.AuthCode, Is.EqualTo("152587"));
        Assert.That(response.ResultCode, Is.EqualTo("0 (ORDER_ACCEPTED)"));
    }

    [Test]
    public void TestPayPageDirectBankResponse()
    {
        const string testXmlResponseBase64 =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY3MDU2Ij4NCiAgICA8cGF5bWVudG1ldGhvZD5EQk5PUkRFQVNFPC9wYXltZW50bWV0aG9kPg0KICAgIDxtZXJjaGFudGlkPjExNzU8L21lcmNoYW50aWQ+DQogICAgPGN1c3RvbWVycmVmbm8+dGVzdF8xMzU5NjIwMzExNTg0PC9jdXN0b21lcnJlZm5vPg0KICAgIDxhbW91bnQ+NTAwPC9hbW91bnQ+DQogICAgPGN1cnJlbmN5PlNFSzwvY3VycmVuY3k+DQogIDwvdHJhbnNhY3Rpb24+DQogIDxzdGF0dXNjb2RlPjA8L3N0YXR1c2NvZGU+DQo8L3Jlc3BvbnNlPg==";
        var response = new SveaResponse(testXmlResponseBase64);

        Assert.That(response.OrderAccepted, Is.True);
        Assert.That(response.ResultCode, Is.EqualTo("0 (ORDER_ACCEPTED)"));
        Assert.That(response.TransactionId, Is.EqualTo("567056"));
        Assert.That(response.MerchantId, Is.EqualTo("1175"));
        Assert.That(response.Amount, Is.EqualTo(5));
        Assert.That(response.ClientOrderNumber, Is.EqualTo("test_1359620311584"));
        Assert.That(response.Currency, Is.EqualTo("SEK"));
        Assert.That(response.PaymentMethod, Is.EqualTo("DBNORDEASE"));
    }

    [Test]
    public void TestPayPageDirectBankInterruptedResponse()
    {
        const string testXmlResponseBase64 =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY3MDYyIj4NCiAgICA8cGF5bWVudG1ldGhvZD5EQk5PUkRFQVNFPC9wYXltZW50bWV0aG9kPg0KICAgIDxtZXJjaGFudGlkPjExNzU8L21lcmNoYW50aWQ+DQogICAgPGN1c3RvbWVycmVmbm8+dGVzdF8xMzU5NjIzMDIyMTQzPC9jdXN0b21lcnJlZm5vPg0KICAgIDxhbW91bnQ+NTAwPC9hbW91bnQ+DQogICAgPGN1cnJlbmN5PlNFSzwvY3VycmVuY3k+DQogIDwvdHJhbnNhY3Rpb24+DQogIDxzdGF0dXNjb2RlPjEwNzwvc3RhdHVzY29kZT4NCjwvcmVzcG9uc2U+DQo=";
        var response = new SveaResponse(testXmlResponseBase64);
        Assert.That(response.ResultCode, Is.EqualTo("107 (DENIED_BY_BANK)"));
    }

    [Test]
    public void TestPayPageCardPaymentResponse()
    {
        const string testXmlResponseBase64 =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY3MDU4Ij4NCiAgICA8cGF5bWVudG1ldGhvZD5LT1JUQ0VSVDwvcGF5bWVudG1ldGhvZD4NCiAgICA8bWVyY2hhbnRpZD4xMTc1PC9tZXJjaGFudGlkPg0KICAgIDxjdXN0b21lcnJlZm5vPnRlc3RfMTM1OTYyMTQ2NTk5MDwvY3VzdG9tZXJyZWZubz4NCiAgICA8YW1vdW50PjUwMDwvYW1vdW50Pg0KICAgIDxjdXJyZW5jeT5TRUs8L2N1cnJlbmN5Pg0KICAgIDxjYXJkdHlwZT5WSVNBPC9jYXJkdHlwZT4NCiAgICA8bWFza2VkY2FyZG5vPjQ0NDQzM3h4eHh4eDMzMDA8L21hc2tlZGNhcmRubz4NCiAgICA8ZXhwaXJ5bW9udGg+MDM8L2V4cGlyeW1vbnRoPg0KICAgIDxleHBpcnl5ZWFyPjIwPC9leHBpcnl5ZWFyPg0KICAgIDxhdXRoY29kZT43NjQ4Nzc8L2F1dGhjb2RlPg0KICA8L3RyYW5zYWN0aW9uPg0KICA8c3RhdHVzY29kZT4wPC9zdGF0dXNjb2RlPg0KPC9yZXNwb25zZT4NCg==";
        var response = new SveaResponse(testXmlResponseBase64);

        Assert.That(response.TransactionId, Is.EqualTo("567058"));
        Assert.That(response.PaymentMethod, Is.EqualTo("KORTCERT"));
        Assert.That(response.MerchantId, Is.EqualTo("1175"));
        Assert.That(response.ClientOrderNumber, Is.EqualTo("test_1359621465990"));
        Assert.That(response.Amount, Is.EqualTo(5));
        Assert.That(response.Currency, Is.EqualTo("SEK"));
        Assert.That(response.CardType, Is.EqualTo("VISA"));
        Assert.That(response.MaskedCardNumber, Is.EqualTo("444433xxxxxx3300"));
        Assert.That(response.ExpiryMonth, Is.EqualTo("03"));
        Assert.That(response.ExpiryYear, Is.EqualTo("20"));
        Assert.That(response.AuthCode, Is.EqualTo("764877"));
        Assert.That(response.ResultCode, Is.EqualTo("0 (ORDER_ACCEPTED)"));
    }

    [Test]
    public void TestSetErrorParamsCode101()
    {
        const string responseXmlBase64 =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+DQogIDx0cmFuc2FjdGlvbiBpZD0iNTY3MDU4Ij4NCiAgICA8cGF5bWVudG1ldGhvZD5LT1JUQ0VSVDwvcGF5bWVudG1ldGhvZD4NCiAgICA8bWVyY2hhbnRpZD4xMTc1PC9tZXJjaGFudGlkPg0KICAgIDxjdXN0b21lcnJlZm5vPnRlc3RfMTM1OTYyMTQ2NTk5MDwvY3VzdG9tZXJyZWZubz4NCiAgICA8YW1vdW50PjUwMDwvYW1vdW50Pg0KICAgIDxjdXJyZW5jeT5TRUs8L2N1cnJlbmN5Pg0KICAgIDxjYXJkdHlwZT5WSVNBPC9jYXJkdHlwZT4NCiAgICA8bWFza2VkY2FyZG5vPjQ0NDQzM3h4eHh4eDMzMDA8L21hc2tlZGNhcmRubz4NCiAgICA8ZXhwaXJ5bW9udGg+MDM8L2V4cGlyeW1vbnRoPg0KICAgIDxleHBpcnl5ZWFyPjIwPC9leHBpcnl5ZWFyPg0KICAgIDxhdXRoY29kZT43NjQ4Nzc8L2F1dGhjb2RlPg0KICA8L3RyYW5zYWN0aW9uPg0KICA8c3RhdHVzY29kZT4xMDE8L3N0YXR1c2NvZGU+DQo8L3Jlc3BvbnNlPg==";     
        var response = new SveaResponse(responseXmlBase64);

        Assert.That(response.ErrorMessage, Is.EqualTo("Invalid XML."));
    }

    [Test]
    public void TestMacValidationSuccess()
    {
        const string responseXmlBase64 =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+PHRyYW5zYWN0aW9uIGlkPSI2MDgyMjYiPjxwYXltZW50bWV0aG9kPlNWRUFDQVJEUEFZPC9wYXltZW50bWV0aG9kPjxtZXJjaGFudGlkPjExMTA8L21lcmNoYW50aWQ+PGN1c3RvbWVycmVmbm8+MjAxNi0wMS0wNVQxMjo1MzozNSAwMDowMDwvY3VzdG9tZXJyZWZubz48YW1vdW50PjEyNDk5PC9hbW91bnQ+PGN1cnJlbmN5PlNFSzwvY3VycmVuY3k+PGNhcmR0eXBlPlZJU0E8L2NhcmR0eXBlPjxtYXNrZWRjYXJkbm8+NDQ0NDMzeHh4eHh4MTEwMDwvbWFza2VkY2FyZG5vPjxleHBpcnltb250aD4wNTwvZXhwaXJ5bW9udGg+PGV4cGlyeXllYXI+MjE8L2V4cGlyeXllYXI+PGF1dGhjb2RlPjM3MjUyNzwvYXV0aGNvZGU+PGN1c3RvbWVyPjxmaXJzdG5hbWUvPjxsYXN0bmFtZS8+PGluaXRpYWxzLz48ZW1haWwvPjxzc24vPjxhZGRyZXNzLz48YWRkcmVzczIvPjxjaXR5Lz48Y291bnRyeT5TRTwvY291bnRyeT48emlwLz48cGhvbmUvPjx2YXRudW1iZXIvPjxob3VzZW51bWJlci8+PGNvbXBhbnluYW1lLz48ZnVsbG5hbWUvPjwvY3VzdG9tZXI+PC90cmFuc2FjdGlvbj48c3RhdHVzY29kZT4wPC9zdGF0dXNjb2RlPjwvcmVzcG9uc2U+";
        const string macToValidate = "0b76a25c5eb5d66b837178f52f4c13f5f19bb7367ca3759b55b4cbd8d7d22000aebf971f9bdd73e5268ad3df6f932ef4a20eafbb3eccc07730c9e270a5197e55";
        CountryCode countryCode = CountryCode.SE;
        var response = new SveaResponse(responseXmlBase64, macToValidate, countryCode, SveaConfig.GetDefaultConfig());
        Assert.That(response.MacValidation, Is.EqualTo(1));
        Assert.That(response.OrderAccepted, Is.True);
        Assert.That(response.ResultCode, Is.EqualTo("0 (ORDER_ACCEPTED)"));

    }

    [Test]
    public void TestMacValidationFailed()
    {
        const string responseXmlBase64 =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+PHRyYW5zYWN0aW9uIGlkPSI2MDgyMjYiPjxwYXltZW50bWV0aG9kPktPUlRDRVJUPC9wYXltZW50bWV0aG9kPjxtZXJjaGFudGlkPjExMzA8L21lcmNoYW50aWQ+PGN1c3RvbWVycmVmbm8+MjAxNi0wMS0wNVQxMjo1MzozNSAwMDowMDwvY3VzdG9tZXJyZWZubz48YW1vdW50PjEyNDk5PC9hbW91bnQ+PGN1cnJlbmN5PlNFSzwvY3VycmVuY3k+PGNhcmR0eXBlPlZJU0E8L2NhcmR0eXBlPjxtYXNrZWRjYXJkbm8+NDQ0NDMzeHh4eHh4MTEwMDwvbWFza2VkY2FyZG5vPjxleHBpcnltb250aD4wNTwvZXhwaXJ5bW9udGg+PGV4cGlyeXllYXI+MjE8L2V4cGlyeXllYXI+PGF1dGhjb2RlPjM3MjUyNzwvYXV0aGNvZGU+PGN1c3RvbWVyPjxmaXJzdG5hbWUvPjxsYXN0bmFtZS8+PGluaXRpYWxzLz48ZW1haWwvPjxzc24vPjxhZGRyZXNzLz48YWRkcmVzczIvPjxjaXR5Lz48Y291bnRyeT5TRTwvY291bnRyeT48emlwLz48cGhvbmUvPjx2YXRudW1iZXIvPjxob3VzZW51bWJlci8+PGNvbXBhbnluYW1lLz48ZnVsbG5hbWUvPjwvY3VzdG9tZXI+PC90cmFuc2FjdGlvbj48c3RhdHVzY29kZT4wPC9zdGF0dXNjb2RlPjwvcmVzcG9uc2U+";
        const string macToValidate = "wrong mac";
        CountryCode countryCode = CountryCode.SE;
        var response = new SveaResponse(responseXmlBase64, macToValidate, countryCode, SveaConfig.GetDefaultConfig());
        Assert.That(response.MacValidation, Is.EqualTo(2));
        Assert.That(response.OrderAccepted, Is.False);
        Assert.That(response.ErrorMessage, Is.EqualTo("Mac validation failed."));
    }

    [Test]
    public void TestSetErrorParamsCode150()
    {
        const string responseXmlBase64 =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48cmVzcG9uc2U+CiAgPHRyYW5zYWN0aW9uIGlkPSI1NjY5MjMiPgogICAgPHBheW1lbnRtZXRob2Q+S09SVENFUlQ8L3BheW1lbnRtZXRob2Q+CiAgICA8bWVyY2hhbnRpZD4xMTc1PC9tZXJjaGFudGlkPgogICAgPGN1c3RvbWVycmVmbm8+dGVzdF8xMzU5NDYwNTc2NDkxPC9jdXN0b21lcnJlZm5vPgogICAgPGFtb3VudD41MDA8L2Ftb3VudD4KICAgIDxjdXJyZW5jeT5TRUs8L2N1cnJlbmN5PgogICAgPGNhcmR0eXBlPlZJU0E8L2NhcmR0eXBlPgogICAgPG1hc2tlZGNhcmRubz40NDQ0MzN4eHh4eHgzMzAwPC9tYXNrZWRjYXJkbm8+CiAgICA8ZXhwaXJ5bW9udGg+MDM8L2V4cGlyeW1vbnRoPgogICAgPGV4cGlyeXllYXI+MjA8L2V4cGlyeXllYXI+CiAgICA8YXV0aGNvZGU+MTUyNTg3PC9hdXRoY29kZT4KICA8L3RyYW5zYWN0aW9uPgogIDxzdGF0dXNjb2RlPjE1MDwvc3RhdHVzY29kZT4KPC9yZXNwb25zZT4=";
        var response = new SveaResponse(responseXmlBase64);
        Assert.That(response.OrderAccepted, Is.True);
        Assert.That(response.ResultCode, Is.EqualTo("150 (CREDIT_PENDING)"));
    }
}