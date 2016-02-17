# C#/.Net Integration Package API for SveaWebPay

## Index
* [1. Introduction](https://github.com/sveawebpay/dotnet-integration/tree/master#1-introduction)
* [2. Build and Configuration](https://github.com/sveawebpay/dotnet-integration/tree/master#2-build-and-configuration)
* [3. Building an order](https://github.com/sveawebpay/dotnet-integration/tree/master#3-building-an-order)
	* [3.1 Specify order](https://github.com/sveawebpay/dotnet-integration/tree/master#31-specify-order)
	* [3.2 Customer identity](https://github.com/sveawebpay/dotnet-integration/tree/master#32-customer-identity)
	* [3.3 Other values](https://github.com/sveawebpay/dotnet-integration/tree/master#33-other-values)
	* [3.4 Choose payment](https://github.com/sveawebpay/dotnet-integration/tree/master#34-choose-payment)
[4. Payment method reference]
    * [4.1 Svea Invoice payment]
    * [4.2 Svea Payment plan payment]
* [4.3 Card payment]
  * [4.4 Direct bank payment]
 * [4.5 Using the Svea PayPage]
[4. Item reference](http://sveawebpay.github.io/php-integration#i4)
    * [4.2 Item::orderRow()](https://github.com/sveawebpay/dotnet-integration/tree/master#42)
    * [4.3 Item::shippingFee()](https://github.com/sveawebpay/dotnet-integration/tree/master#43)
    * [4.4 Item::invoiceFee()](https://github.com/sveawebpay/dotnet-integration/tree/master#44)
    * [4.5 Item::fixedDiscount()](https://github.com/sveawebpay/dotnet-integration/tree/master#45)
    * [4.6 Item::relativeDiscount](https://github.com/sveawebpay/dotnet-integration/tree/master#46)
    * [4.7 Item::individualCustomer()](https://github.com/sveawebpay/dotnet-integration/tree/master#47)
    * [4.8 Item::companyCustomer()](https://github.com/sveawebpay/dotnet-integration/tree/master#48)
    * [4.9 Item::numberedOrderRow()](https://github.com/sveawebpay/dotnet-integration/tree/master#49)
* [5. WebpayConnection entrypoint method reference](https://github.com/sveawebpay/dotnet-integration/tree/master#5-webpayconnection-entrypoint-method-reference)
    * [5.1 WebpayConnection.CreateOrder()](https://github.com/sveawebpay/dotnet-integration/tree/master#51-webpayconnectioncreateorder)
    * [5.2 WebpayConnection.GetPaymentPlanParams()](https://github.com/sveawebpay/dotnet-integration/tree/master#52-webpayconnectiongetpaymentplanparams)
    * [5.3 WebpayConnection.PaymentPlanPricePerMonth()](https://github.com/sveawebpay/dotnet-integration/tree/master#53-webpayconnectionpaymentplanpricepermonth)
    * [5.4 WebpayConnection.GetAddresses()](https://github.com/sveawebpay/dotnet-integration/tree/master#54-webpayconnectiongetaddresses)
    * [5.5 WebpayConnection.DeliverOrder](https://github.com/sveawebpay/dotnet-integration/tree/master#55-webpayconnectiondeliverorder)
        * [5.5.1 Deliver Invoice order](https://github.com/sveawebpay/dotnet-integration/tree/master#551-deliver-invoice-order)
        * [5.5.2 Other values](https://github.com/sveawebpay/dotnet-integration/tree/master#552-other-values)
        * [5.5.3 Credit Invoice](https://github.com/sveawebpay/dotnet-integration/tree/master#553-credit-invoice)
    * [5.6 WebpayConnection.CloseOrder](https://github.com/sveawebpay/dotnet-integration/tree/master#56-webpayconnectioncloseorder)
* [6. Response handler](https://github.com/sveawebpay/dotnet-integration/tree/master#6-response-handler)
* [7. WebpayAdmin entrypoint method reference](https://github.com/sveawebpay/dotnet-integration/tree/master#7-webpayadmin-entrypoint-method-reference)
* [7.1 WebpayAdmin.QueryOrder()](https://github.com/sveawebpay/dotnet-integration/tree/master#71-webpayadmin-queryorder)
* [7.2 WebpayAdmin.DeliverOrderRows()](https://github.com/sveawebpay/dotnet-integration/tree/master#72-webpayadmin-deliverorderrows)
* [7.3 WebpayAdmin.CancelOrder()](https://github.com/sveawebpay/dotnet-integration/tree/master#73-webpayadmin-cancelorder)
* [APPENDIX](https://github.com/sveawebpay/dotnet-integration/tree/master#appendix)


## 1. Introduction
The WebpayConnection and WebpayAdmin classes make up the Svea API. Together they provide unified entrypoints to the various Svea web services. The API also encompass the support classes IConfigurationProvider, SveaResponse and Item, as well as various constant container classes and support classes.

The WebpayConnection class methods contains the functions needed to create orders and perform payment requests using Svea payment methods. It contains methods to define order contents, send order requests, as well as support methods needed to do this.

The WebPayAdmin class methods are used to administrate orders after they have been accepted by Svea. It includes functions to update, deliver, cancel and credit orders et.al. Note! This entrypoint is under construction and all functions not yet implemented.

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 2. Build and Configuration

To build the dll file, load up the project in Visual studio.
Choose 'Release' as your solution configuration and press F6.
(The solution is compatible and will compile properly with .NET Framework 4.0 and higher.)

This will build the dll and place it in bin/release sub-folder of the project.

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

The Configuration needed to be set differs of how many different payment methods and countries you have in the same installation.
The authorization values are recieved from Svea Ekonomi when creating an account. If no configuration is created, use SveaConfig.GetDefaultConfig().

*To configure Svea authorization:*
Create a class (eg. one for testing values, one for production) that implements the IConfigurationProvider Interface. Let the implemented methods
return the authorization values asked for.
Later when starting a WebpayConnection action in your integration file, put an instance of your class as parameter to the constructor.

*NOTE:* This solution may change in future updates!

Step 1:

```csharp

using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace SveaWebpayIntegration
{
    public class MyConfigTest : IConfigurationProvider
    {
        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>user name string</returns>
        public string GetUsername(PaymentType type, CountryCode country)
        {
            return myUserName;
        }

        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>password string</returns>
        public string GetPassword(PaymentType type, CountryCode country)
        {
            return myPassword;
        }

        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>client number int</returns>
        public int GetClientNumber(PaymentType type, CountryCode country)
        {
            return myClientNumber;
        }

        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>merchant id string</returns>
        public string GetMerchantId(PaymentType type, CountryCode country)
        {
            return myMerchantId;
        }

        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>secret word string</returns>
        public string GetSecretWord(PaymentType type, CountryCode country)
        {
            return mySecretWord;
        }

        /// <summary>
        /// Constants for the end point url found in the class SveaConfig
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <returns>end point url</returns>
        public string GetEndPoint(PaymentType type)
        {
            return type == PaymentType.HOSTED ? SveaConfig.GetTestPayPageUrl() : SveaConfig.GetTestWebserviceUrl();
        }
    }
}

```

Step 2: Put an instance of your configuration object as a parameter to the request.

```csharp
		//Create a class including test authorization
		MyConfigTest confTest = new MyConfigTest();
		//Create a class including production authorization
		MyConfigProd confProd = new MyConfigProd();

		//Create your CreateOrder object with selected and continue building your order. Se next steps.
		private CreateOrderEuResponse response = WebpayConnection.CreateOrder(confTest);
	.....

```

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 3. Building an order
We show how to specify an order, working through the various steps and options along the way:
Start by creating an order using the WebpayConnection.CreateOrder() method. Pass in your configuration and get an instance of CreateOrderBuilder in return.

```csharp
CreateOrderEuResponse response = WebpayConnection.CreateOrder(myConfig)

```

### 3.1 Specify order
Continue by adding values for products and other. You can add order row, fee and discount. Chose the right Item object as parameter.
You can use the *add* functions with an Item object or a List of Item objects as parameters. See types of Item objects in examples in 4.1.2 - 4.1.5 and 4.3.1 - 4.3.2.

```csharp
.AddOrderRow(Item.OrderRow(). ...)

//or

var orderRows = new List<OrderRowBuilder>();
orderRows.Add(Item.OrderRow(). ...)
...
CreateOrder.AddOrderRows(orderRows);


For every new payment type implementation, you follow the steps from the beginning and chose your payment type preferred in the end:
Build order -> choose payment type -> DoRequest/GetPaymentForm/PreparePayment

```csharp
CreateOrderEuResponse response = WebpayConnection.CreateOrder(myConfig)		//See Configuration chapt.3
//For all products and other items
.AddOrderRow(Item.OrderRow()...)
//If shipping fee
.AddFee(Item.ShippingFee()...)
//If invoice with invoice fee
.AddFee(Item.InvoiceFee()...)
//If discount or coupon with fixed amount
.AddDiscount(Item.FixedDiscount()...)
//If discount or coupon with percent discount
.AddDiscount(Item.RelativeDiscount()...)
//Individual customer values
.AddCustomerDetails(Item.IndividualCustomer()...)
//Company customer values
.AddCustomerDetails(Item.CompanyCustomer()...)
//Other values
.SetCountryCode(CountryCode.SE)
.SetOrderDate(new DateTime(2012, 12, 12))
.SetCustomerReference("ref33")
.SetClientOrderNumber("33")
.SetCurrency(Currency.SEK)

//Continue by choosing one of the following paths
//Continue as a card payment
.UsePayPageCardOnly()
	...
	.GetPaymentForm();
    or
    .PreparePayment("234.214.2.23")

//Continue as a direct bank payment
.UsePayPageDirectBankOnly()
	...
	.GetPaymentForm();
    or
    .PreparePayment("234.214.2.23")
//Continue as a PayPage payment
.UsePayPage()
	...
	.GetPaymentForm();
    or
    .PreparePayment("234.214.2.23")
//Continue as a PayPage payment
.UsePaymentMethod(PaymentMethod.DBSEBSE) //see APPENDIX for Constants
	...
	.GetPaymentForm()
    or
    .PreparePayment("234.214.2.23")
//Continue as an invoice payment
.UseInvoicePayment()
	...
	.DoRequest();
//Continue as a payment plan payment
.UsePaymentPlanPayment("campaigncode", false)
	...
	.DoRequest();
```

If you want to prepare a payment and send a link to the user for the payment. In this case a hosted payment will be used.
The `.PreparePayment()`-method will return a `Uri` for the customer to use to complete the payment of the order.

### 3.2 Customer Identity
Customer identity is required for invoice and payment plan orders. Required values varies
depending on country and customer type. For SE, NO, DK and FI national id number is required. Email and ip address are desirable.

#### 3.2.1 Options for individual customers
```csharp
.AddCustomerDetails(Item.IndividualCustomer()
    .SetNationalIdNumber("194605092222")	//Required for individual customers in SE, NO, DK and FI
    .SetBirthDate("19460509")        		//Required for individual customers in NL and DE
    .SetName("Tess", "Testson")        		//Required for individual customers in NL and DE
    .SetInitials("SB")                 		//Required for individual customers in NL
    .SetStreetAddress("Gatan", "23")     		//Required in NL and DE
    .SetCoAddress("c/o Eriksson")      		//Optional
    .SetZipCode("9999")                  		//Required in NL and DE
    .SetLocality("Stan")               		//Required in NL and DE
    .SetPhoneNumber("999999"))           	//Optional
    .SetEmail("test@svea.com")         		//Optional but desirable
    .SetIpAddress("123.123.123")       		//Optional but desirable
```

#### 3.2.2 Options for company customers
```csharp
.AddCustomerDetails(Item.CompanyCustomer()
    .SetNationalIdNumber("2345234")			//Required for company customers in SE, NO, DK, FI
    .SetVatNumber("NL2345234")				//Required for NL and DE
    .SetCompanyName("TestCompagniet")) 		//Required for Eu countries like NL and DE
	.SetAddressSelector("7fd7768")          //Optional. Recieved from getAddresses
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)


### 3.3 Other values
```csharp
.SetCountryCode(CountryCode.SE)         	//Required
.SetCurrency(Currency.SEK)                     	//Required for card payment, direct payment and PayPage payment.
.SetClientOrderNumber("33")           		//Required for card payment, direct payment, PaymentMethod payment and PayPage payments. Must be uniqe.
.SetOrderDate(new DateTime(2012, 12, 12))   //Required for synchronous payments
.SetCustomerReference("ref33")             	//Optional
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 3.4 Choose payment
End process by choosing the payment method you desire.

Invoice and payment plan will perform a synchronous payment and return an object as response.

Other payments(card, direct bank and payments from the *PayPage*) on the other hand are asynchronous. They will return an html form with formatted message to send from your store.

The response is then returned to the return url you have specified in function *SetReturnUrl()*. The response may also be sent to the url specified with *SetCallbackUrl()* in case the customer doesn't return to the store after the transaction has concluded at the bank/card payment page.

If you pass the xml response to an instance of SveaResponse, you will receive a formatted response object as well.

#### Which payment method to choose?
Invoice and/or payment plan payments.
>The preferable way is to use [`.UseInvoicePayment()`](https://github.com/sveawebpay/dotnet-integration/tree/master#455-invoicepayment) and
>[`.UsePaymentPlanPayment(...)`](https://github.com/sveawebpay/dotnet-integration/tree/master#456-paymentplanpayment).
>These payments are synchronous and will give you an instant response.

Card and/or direct bank payments
>Go by *PayPage* by using [`.UsePayPageCardOnly()`](https://github.com/sveawebpay/dotnet-integration/tree/master#451-paypage-with-card-payment-options)
>and [`.UsePayPageDirectBankOnly()`](https://github.com/sveawebpay/dotnet-integration/tree/master#452-paypage-with-direct-bank-payment-options).
>If you only for example only have one specific bank payment, go direct to that specific bank payment by using
>[`.UsePaymentMethod(PaymentMethod)`](https://github.com/sveawebpay/dotnet-integration/tree/master#454-PaymentMethod-specified)

Using all payments.
>The most effective way is to use [`.UseInvoicePayment()`](https://github.com/sveawebpay/dotnet-integration/tree/master#455-invoicepayment)
>and [`.UsePaymentPlanPayment(...)`](https://github.com/sveawebpay/dotnet-integration/tree/master#456-paymentplanpayment) for the synchronous payments,
>and use the *PayPage* for the asynchronous requests by using [`.UsePayPageCardOnly()`](https://github.com/sveawebpay/dotnet-integration/tree/master#451-paypage-with-card-payment-options)
>and [`.UsePayPageDirectBankOnly()`](https://github.com/sveawebpay/dotnet-integration/tree/master#452-paypage-with-direct-bank-payment-options).

Using more than one payment and want them gathered on one place.
>Go by PayPage and choose show all your payments here, or modify to exclude or include one or more payments. Use [`.UsePayPage()`]
>(https://github.com/sveawebpay/dotnet-integration/tree/master#453-paypagepayment) where you can custom your own *PayPage*.
Note that Invoice and Payment plan payments will return an asynchronous response from here.

## 4. Payment method reference
Select payment method to use with the CreateOrderBuilder class useXX() methods, which return an instance of the appropriate payment request class.

### 4.1 InvoicePayment
Perform an invoice payment. This payment form will perform a synchronous payment and return a response.
Returns *CreateOrderEuResponse* object. Set your store authorization here.


```csharp
CreateOrderEuResponse response = WebpayConnection.CreateOrder()
.AddOrderRow(...)                           // required, one or more
.AddCustomerDetails(...)                    // required, individualCustomer or companyCustomer
.SetCountryCode(CountryCode.SE)             // required
.SetOrderDate(new DateTime(2012, 12, 12))   // required

.UseInvoicePayment()                         // requires the above attributes in the order
	.DoRequest();
```

### 4.2 PaymentPlanPayment
Perform *PaymentPlanPayment*. This payment form will perform a synchronous payment and return a response.
Returns a *CreateOrderEuResponse* object. Preceded by WebPay.GetPaymentPlanParams(...).
Set your store authorization here.
Param: Campaign code recieved from GetPaymentPlanParams().

```csharp
CreateOrderEuResponse response = WebpayConnection.CreateOrder()
.AddOrderRow(...)                           // required, one or more
.AddCustomerDetails(...)                    // required, individualCustomer or companyCustomer
.SetCountryCode(CountryCode.SE)             // required
.SetOrderDate(new DateTime(2012, 12, 12))   // required

.UsePaymentPlanPayment(1234L, false)              	//Parameter1: campaign code recieved from getPaymentPlanParams
							//Paremeter2: True if Automatic autogiro form will be sent with the first notification
   .DoRequest();
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 4.3 Card payment
Select i.e.
.UsePaymentMethod(PaymentMethod.SVEACARDPAY) //see APPENDIX for Constants
to perform a card payment via the SveaCardPay card payment provider.

```csharp
PaymentForm form = WebpayConnection.CreateOrder()
.AddOrderRow(...)                       //required
.SetCountryCode(CountryCode.SE)         // required
.SetClientOrderNumber("33")
.SetOrderDate(new DateTime(2012, 12, 12))
.UsePaymentMethod(PaymentMethod.SVEACARDPAY)
	.SetReturnUrl("http://myurl.se")                   	//Required
	.SetCallbackUrl("http://myurl.se")                   	//Optional
	.SetCancelUrl("http://myurl.se")                   	//Optional
            .GetPaymentForm();

```
.SetReturnUrl() When a hosted payment transaction completes (regardless of outcome, i.e. accepted or denied), the payment service will answer with a response xml message sent to the return url specified.

.SetCallbackUrl() In case the hosted payment transaction completes, but the service is unable to return a response to the return url, the payment service will retry several times using the callback url as a fallback, if specified. This may happen if i.e. the user closes the browser before the payment service redirects back to the shop.

.SetCancelUrl() In case the hosted payment service is cancelled by the user, the payment service will redirect back to the cancel url. Unless a return url is specified, no cancel button will be presented at the payment service.


### 4.4 Direct bank payment

Select i.e.
.UsePaymentMethod(PaymentMethod.NORDEASE) //see APPENDIX for Constants
to perform a bank payment redirected direct to the selected bank landing page.

```csharp
PaymentForm form = WebpayConnection.CreateOrder()
.AddOrderRow(...)                       //required
.SetCountryCode(CountryCode.SE)         // required
.SetClientOrderNumber("33")
.SetOrderDate(new DateTime(2012, 12, 12))
.UsePaymentMethod(PaymentMethod.NORDEASE)
	.SetReturnUrl("http://myurl.se")                   	//Required
	.SetCallbackUrl("http://myurl.se")                   	//Optional
	.SetCancelUrl("http://myurl.se")                   	//Optional

```

.SetReturnUrl() When a hosted payment transaction completes (regardless of outcome, i.e. accepted or denied), the payment service will answer with a response xml message sent to the return url specified.

.SetCallbackUrl() In case the hosted payment transaction completes, but the service is unable to return a response to the return url, the payment service will retry several times using the callback url as a fallback, if specified. This may happen if i.e. the user closes the browser before the payment service redirects back to the shop.

.SetCancelUrl() In case the hosted payment service is cancelled by the user, the payment service will redirect back to the cancel url. Unless a return url is specified, no cancel button will be presented at the payment service.


### 4.5 PayPage
*PayPage* with all available payments. You can also custom the *PayPage* by using one of the methods for *PayPagePayments*:
SetPaymentMethod, IncludePaymentMethod, ExcludeCardPaymentMethod or ExcludeDirectPaymentMethod.

```csharp
PaymentForm form = WebpayConnection.CreateOrder()
.AddOrderRow(...)                       //required
.SetCountryCode(CountryCode.SE)         // required
.SetClientOrderNumber("33")
.SetOrderDate(new DateTime(2012, 12, 12))
.UsePayPage()
	.SetReturnUrl("http://myurl.se")                   	//Required
	.SetCallbackUrl("http://myurl.se")                   	//Optional
	.SetCancelUrl("http://myurl.se")                   	//Optional

```

.SetReturnUrl() When a hosted payment transaction completes (regardless of outcome, i.e. accepted or denied), the payment service will answer with a response xml message sent to the return url specified.

.SetCallbackUrl() In case the hosted payment transaction completes, but the service is unable to return a response to the return url, the payment service will retry several times using the callback url as a fallback, if specified. This may happen if i.e. the user closes the browser before the payment service redirects back to the shop.

.SetCancelUrl() In case the hosted payment service is cancelled by the user, the payment service will redirect back to the cancel url. Unless a return url is specified, no cancel button will be presented at the payment service.


#### 4.5.1 Exclude specific payment methods
Optional if you want to exlude specific payment methods for *PayPage*.
```csharp
.UsePayPage()
	.SetReturnUrl("http://myurl.se")                                            //Required
	.SetCancelUrl("http://myurl.se")                                            //Optional
	.ExcludePaymentMethods(new List<PaymentMethod>								//Optional
            {
                PaymentMethod.SEBSE,
                PaymentMethod.INVOICE
            })
	.GetPaymentForm();
```
#### 4.5.2 Include specific payment methods
Optional if you want to include specific payment methods for *PayPage*.
```csharp
.UsePayPage()
	.SetReturnUrl("http://myurl.se")                                            //Required
	.IncludePaymentMethod(new List<PaymentMethod>								//Optional
            {
                PaymentMethod.SEBSE,
                PaymentMethod.INVOICE
            })
	.GetPaymentForm();
```

#### 4.5.3 Exclude Card payments
Optional if you want to exclude all card payment methods from *PayPage*.
```csharp
.UsePayPage()
	.SetReturnUrl("http://myurl.se")                   //Required
	.ExcludeCardPaymentMethod()                       //Optional
	.GetPaymentForm();
```

#### 4.5.4 Exclude Direct payments
Optional if you want to exclude all direct bank payments methods from *PayPage*.
```csharp
.UsePayPage()
    .SetReturnUrl("http://myurl.se")                   //Required
    .ExcludeDirectPaymentMethod()                     //Optional
    .GetPaymentForm();
```

### 4.6 Return
Build order and recieve a *PaymentForm* object. Send the *PaymentForm* parameters: *merchantid*, *xmlMessageBase64* and *mac* by POST with *url*. The *PaymentForm* object also contains a complete html form as string
and the html form element as array.

```html
    <form name="paymentForm" id="paymentForm" method="post" action=" + url +">
        <input type="hidden" name="merchantid" value=" + merchantId + " />
        <input type="hidden" name="message" value=" + xmlMessageBase64 + " />
        <input type="hidden" name="mac" value=" + mac + "/>
        <noscript><p>Javascript is inactivated.</p></noscript>
        <input type="submit" name="submit" value="Submit" />
    </form>
```

The values of *xmlMessageBase64*, *merchantid* and *mac* are to be sent as xml to SveaWebPay.
Function GetPaymentForm() returns object type *PaymentForm* with accessible members:

| Value                 | Returns    | Description                               |
|-----------------------|----------- |-------------------------------------------|
| GetXmlMessageBase64() | string     | Payment information in XML-format, Base64 encoded.|
| GetXmlMessage()       | string     | Payment information in XML-format.        |
| GetMerchantId()       | string     | Authorization                             |
| GetSecretWord()       | string     | Authorization                             |
| GetMacSha512()        | string     | Message authentication code.              |
| GetUrl()				| string     | Url to preselected server, test or production.|
| GetCompleteForm()		| string     | A complete Html form with method= "post" with submit button to include in your code. |
| GetFormHtmlFields()   | Dictionary<string, string>   | Map with html tags as keys with of Html form fields to include. |


The response will be returned as http GET or POST to the returnUrl and the callbackUrl.

## 5.  Item reference
The WebPayItem class provides entrypoint methods to the different row items that make up an order, as well as the customer identity information items.

An order must contain one or more order rows. You may add invoice fees, shipping fees and discounts to an order.

For relative discounts, or fixed discounts specified using only setAmountIncVat() or only setAmountExVat() there may be several discount rows added, should the order include more than one different vat rate. It is not recommended to specify more than one relative discount row per order, or more than one fixed discount specified using only setAmountIncVat() or only setAmountExVat().

### 5.1 Item.OrderRow
All products and other items. It?s required to have a minimum of one order row.
**The price can be set in a combination by using a minimum of two out of three functions: SetAmountExVat(), SetAmountIncVat() and SetVatPercent().**
```csharp
.AddOrderRow(Item.OrderRow()
	.SetQuantity(2)                        //Required
	.SetAmountExVat(100.00M)               //Optional, see info above
	.SetAmountIncVat(125.00M)              //Optional, see info above
	.SetVatPercent(25.00M)                 //Optional, see info above
	.SetArticleNumber("1")                 //Optional
	.SetDescription("Specification")       //Optional
	.SetName("Prod")                       //Optional
	.SetUnit("st")                         //Optional
	.SetDiscountPercent(0))                //Optional
```

### 5.2 Item.ShippingFee
**The price can be set in a combination by using a minimum of two out of three functions: setAmountExVat(), setAmountIncVat()and setVatPercent().**
```csharp
.AddFee(Item.ShippingFee()
	.SetAmountExVat(50)                    //Optional, see info above
	.SetAmountIncVat(62.50M)                //Optional, see info above
	.SetVatPercent(25.00M)                  //Optional, see info above
	.SetShippingId("33")                   //Optional
	.SetName("shipping")                   //Optional
	.SetDescription("Specification")       //Optional
	.SetUnit("st")                         //Optional
	.SetDiscountPercent(0))                //Optional
```
### 5.3 Item.InvoiceFee
**The price can be set in a combination by using a minimum of two out of three functions: setAmountExVat(), setAmountIncVat() and setVatPercent().**
```csharp
.AddFee(Item.InvoiceFee()
	.SetAmountExVat(50)                    //Optional, see info above
	.SetAmountIncVat(62.50M)                //Optional, see info above
	.SetVatPercent(25.00M)       	       //Optional, see info above
	.SetName("Svea fee")                   //Optional
	.SetDescription("Fee for invoice")     //Optional
	.SetUnit("st")                         //Optional
	.SetDiscountPercent(0))                //Optional
```
### 5.4 Item.FixedDiscount
When discount or coupon is a fixed amount on total product amount.
```csharp
.AddDiscount(Item.FixedDiscount()
	.SetAmountIncVat(100.00M)               //Required
	.SetDiscountId("1")                    //Optional
	.SetUnit("st")                         //Optional
	.SetDescription("FixedDiscount")       //Optional
	.SetName("Fixed"))                     //Optional
```
### 5.5 Item.RelativeDiscount
When discount or coupon is a percentage on total product amount.
```csharp
.AddDiscount(Item.RelativeDiscount()
	.SetDiscountPercent(50)                //Required
	.SetDiscountId("1")                    //Optional
	.SetUnit("st")                         //Optional
	.SetName("Relative")                   //Optional
	.SetDescription("RelativeDiscount"))   //Optional
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 6. WebpayConnection entrypoint method reference
The WebpayConnection class methods contains the functions needed to create orders and perform payment requests using Svea payment methods.
It contains entrypoint methods used to define order contents, send order requests, as well as various support methods needed to do this.

* [6.1 WebpayConnection.CreateOrder()](https://github.com/sveawebpay/dotnet-integration/tree/master#61-webpayconnectioncreateorder)
* [6.2 WebpayConnection.GetPaymentPlanParams()](https://github.com/sveawebpay/dotnet-integration/tree/master#62-webpayconnectiongetpaymentplanparams)
* [6.3 WebpayConnection.PaymentPlanPricePerMonth()](https://github.com/sveawebpay/dotnet-integration/tree/master#63-webpayconnectionpaymentplanpricepermonth)
* [6.4 WebpayConnection.GetAddresses()](https://github.com/sveawebpay/dotnet-integration/tree/master#64-webpayconnectiongetaddresses)
* [6.5 WebpayConnection.DeliverOrder](https://github.com/sveawebpay/dotnet-integration/tree/master#65-webpayconnectiondeliverorder)
    * [6.5.1 Deliver Invoice order](https://github.com/sveawebpay/dotnet-integration/tree/master#651-deliver-invoice-order)
    * [6.5.2 Other values](https://github.com/sveawebpay/dotnet-integration/tree/master#652-other-values)
    * [6.5.3 Credit Invoice](https://github.com/sveawebpay/dotnet-integration/tree/master#653-credit-invoice)
* [6.6 WebpayConnection.CloseOrder](https://github.com/sveawebpay/dotnet-integration/tree/master#66-webpayconnectioncloseorder)

## 6.1 WebpayConnection.CreateOrder()

Use WebpayConnection.CreateOrder() to create an order using invoice, payment plan, card, or direct bank payment methods.

See the CreateOrderBuilder class for more info on methods used to specify the order builder contents
and chosing a payment method to use, followed by sending the request to Svea and parsing the response.

See the CreateOrderBuilder class for more info on methods used to specify the order builder contents
and chosing a payment method to use, followed by sending the request to Svea and parsing the response.

Invoice and Payment plan orders will perform a synchronous payment on doRequest(), and will return a response
object immediately.

Card, Direct bank, and other hosted methods accessed via PayPage are asynchronous. They provide an html form
containing a formatted message to send to Svea, which in turn will send a request response to a given return url,
where the response can be parsed using the SveaResponse class.

```csharp
CreateOrderEuResponse response = WebpayConnection.CreateOrder(myConfig)		//See Configuration chapt.3
//For all products and other items
.AddOrderRow(Item.OrderRow()...)
//If shipping fee
.AddFee(Item.ShippingFee()...)
//If invoice with invoice fee
.AddFee(Item.InvoiceFee()...)
//If discount or coupon with fixed amount
.AddDiscount(Item.FixedDiscount()...)
//If discount or coupon with percent discount
.AddDiscount(Item.RelativeDiscount()...)
//Individual customer values
.AddCustomerDetails(Item.IndividualCustomer()...)
//Company customer values
.AddCustomerDetails(Item.CompanyCustomer()...)
//Other values
.SetCountryCode(CountryCode.SE)
.SetOrderDate(new DateTime(2012, 12, 12))
.SetCustomerReference("ref33")
.SetClientOrderNumber("33")
.SetCurrency(Currency.SEK)
```

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 6.2 WebpayConnection.GetPaymentPlanParams()
Use this function to retrieve campaign codes for possible payment plan options. Use prior to create payment plan payment.
Returns *PaymentPlanParamsResponse* object. Set your store authorization here.

```csharp
GetPaymentPlanParamsEuResponse response = WebpayConnection.GetPaymentPlanParams()
	.SetCountryCode(CountryCode.SE)					//Required
	.DoRequest();
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 6.3 WebpayConnection.PaymentPlanPricePerMonth()
Use this function to calculate the prices per month of the payment plan campaigns received from GetPaymentPlanParams.
*paymentPlanParams* is the response object from GetPaymentPlanParams.
```csharp
   List<Dictionary<string, string>> pricePerMonth = WebpayConnection.PaymentPlanPricePerMonth(amount, paymentPlanParams);
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 6.4 WebpayConnection.GetAddresses()
Returns *getAddressesResponse* object with an *AddressSelector* for the associated addresses for a specific security number.
Can be used when creating an order. Only applicable for SE, NO and DK. In Norway, only getAddresses of companies is supported.
Set your store authorization here.

Set order type

```csharp
    .SetOrderTypeInvoice()         //Required if this is an invoice order
or
    .SetOrderTypePaymentPlan()     //Required if this is a payment plan order
```

Customer type
```csharp
    .SetIndividual("194605092222") //Required if this is an individual customer
or
    .SetCompany("companyId")       //Required if this is a company customer

```csharp
GetAddressesEuResponse response = WebpayConnection.GetAddresses(myConfig)		//see more about Configuration chapt.3
	.SetCountryCode(CountryCode.SE)                                 //Required
	.SetOrderTypeInvoice()                                          //Required. Used by Configurationprovider to get the right config
	.SetIndividual("194605092222")                                  //See Individual or Company
        .SetZipCode("9999")                                             //Required
	.DoRequest();
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 6.5 WebpayConnection.DeliverOrder()
Use the WebpayConnection.DeliverOrder request to deliver to the customer invoices for fulfilled orders.
Svea will invoice the customer upon receiving the DeliverOrder request.
A DeliverOrder request may also be used to partly deliver an order on Invoice orders.
Add rows that you want delivered. The rows will automatically be matched with the rows that was sent when creating the order.
When Svea receives the DeliverOrder request the status on the previous created order is set to *delivered*.
The DeliverOrder functionallity is only applicable to invoice and payment plan payment method payments.

Returns *DeliverOrderEuResponse* object. Set your store authorization here.














### 6.5.1 Deliver Invoice order
This works more or less like WebpayConnection.CreateOrder above, and makes use of the same order item information.
Add the corresponding order id and the order rows that you want delivered before making the DeliverOrder request.
The specified rows will automatically be matched with the previous rows that was sent when creating the order.
We recommend storing the order row data to ensure that matching orderrows can be recreated in the DeliverOrder request.

If an item is left out from the DeliverOrder request that was present in the CreateOrder request, a new invoice will be created as the order is assumed to be partially fulfilled.
Any left out items should not be delivered physically, as they will not be invoiced when the DeliverOrder request is sent.

```csharp
	var response = WebpayConnection.DeliverOrder(myConfig)
        .AddOrderRow()
        .SetOrderId(1234L) //Recieved from CreateOrder request

     // then select the corresponding request class and send request
     var response = response.DeliverInvoiceOrder().DoRequest();       // returns DeliverOrdersResponse (no rows) or DeliverOrderResult (with rows)
     var response = response.DeliverPaymentPlanOrder.DoRequest();   // returns DeliverOrdersResponse (no rows) or DeliverOrderResult (with rows)
     var response = response.DeliverCardOrder().DoRequest();          // returns ConfirmTransactionResponse
```

You can add OrderRow, Fee and Discount. Choose the right Item as parameter.
You can use the **.Add** functions with an Item or list of Items as parameters.

```csharp
.AddOrderRow(Item.OrderRow(). ...)

//or

List<OrderRowBuilder> orderRows = new List<OrderRowBuilder>(); //or use another preferrable List object
orderRows.Add(Item.OrderRow(). ...)
...
DeliverOrder.AddOrderRows(orderRows);
```

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 6.5.2 Other values
Required is the order id received when creating the order. Required for invoice orders are *InvoiceDistributionType*.
If invoice order is credit invoice use setCreditInvoice(invoiceId) and setNumberOfCreditDays(creditDaysAsInt)
```csharp
    .SetOrderId(orderId)                   								//Required. Received when creating order.
	.SetCountryCode(CountryCode.SE)		   								//Required
    .SetNumberOfCreditDays(1)              								//Use for invoice orders.
    .SetInvoiceDistributionType(InvoiceDistributionType.Post)  			//Use for invoice orders. DISTRIBUTIONTYPE see APPENDIX
    .SetCreditInvoice()                    								//Use for invoice orders, if this should be a credit invoice.
```

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 6.5.3. Credit Invoice
When you want to credit an invoice. The order must first be delivered. When doing [DeliverOrder](https://github.com/sveawebpay/dotnet-integration/tree/master#7-deliverorder)
you will recieve an *InvoiceId* in the Response. To credit the invoice you follow the steps as in [7. DeliverOrder](https://github.com/sveawebpay/dotnet-integration/tree/master#7-deliverorder)
 but you add the call `.SetCreditInvoice(invoiceId)`:

```csharp
	var response = WebpayConnection.DeliverOrder(myConfig)
    .AddOrderRow(
        Item.OrderRow()
            .SetArticleNumber("1")
            .SetQuantity(2)
            .SetAmountExVat(100.00M)
            .SetDescription("Specification")
            .SetName("Prod")
            .SetUnit("st")
            .SetVatPercent(25)
            .SetDiscountPercent(0)
        )
        .SetOrderId(1234L) //Recieved from CreateOrder request
        .SetInvoiceDistributionType(InvoiceDistributionType.Post)
        //Credit invoice flag. Note that you first must deliver the order and recieve an InvoiceId, then do the deliver request again but with this call:
        .SetCreditInvoice(4321L) //Use for invoice orders, if this should be a credit invoice. Params: InvoiceId recieved from when doing DeliverOrder
        .DeliverInvoiceOrder()
            .DoRequest();
```

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 6.6. CloseOrder
Use when you want to cancel an undelivered order. Valid only for invoice and payment plan orders.
Required is the order id received when creating the order. Set your store authorization here.

#### Close by payment type
```csharp
    .CloseInvoiceOrder()
or
    .ClosePaymentPlanOrder()
```

```csharp
CloseOrderEuResponse response =  WebpayConnection.CloseOrder()
	.SetOrderId(orderId)						//Required, received when creating an order.
	.SetCountryCode(CountryCode.SE)			//Required
	.CloseInvoiceOrder()
	.DoRequest();
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 6. Response handler
All synchronous responses are handled through *SveaResponse* and structured into objects.
Asynchronous responses recieved after sending the values *merchantid* and *xmlMessageBase64* to
hosted solutions can also be processed through the *SveaResponse* class.

The response from server will be sent to the *returnUrl* with POST or GET. The response contains the parameters:
*response* and *merchantid*.
Class *SveaResponse* will return an object structured similar to the synchronous answer.

Params:
* The POST or GET message Base64 encoded
```csharp
	var respObject = new SveaResponse(responseXmlBase64);
```

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 7. WebpayAdmin entrypoint method reference
The WebpayConnection class methods contains the functions needed to handle orders for Webservice payments as well as hosted payments.
It contains entrypoint methods used to define order contents, send order requests, as well as various support methods needed to do this.

### 7.1 WebpayAdmin.QueryOrder
The WebpayAdmin.QueryOrder entrypoint method is used to get information about an order.

```csharp
  QueryOrderBuilder request = WebpayAdmin.QueryOrder(config)
        .SetOrderId()                 // required
        .SetTransactionId()           // optional, card or direct bank only, alias for SetOrderId
        .SetCountryCode()             // required
        ;
        // then select the corresponding request class and send request
        response = request.QueryInvoiceOrder().DoRequest();         // returns AdminWS.GetOrdersResponse
        response = request.QueryPaymentPlanOrder().DoRequest();     // returns AdminWS.GetOrdersResponse
        response = request.QueryCardOrder().DoRequest();            // returns Hosted.Admin.Actions.QueryResponse
        response = request.QueryDirectBankOrder().DoRequest();      // returns Hosted.Admin.Actions.QueryResponse

```

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 7.2 WebpayAdmin.DeliverOrderRows
The WebpayAdmin.DeliverOrderRows entrypoint method is used to deliver individual order rows. Supports invoice and card orders.

For Invoice orders, the order row status is updated at Svea following each successful request.

For card orders, an order can only be delivered once, and any non-delivered order rows will be cancelled (i.e. the order amount will be lowered by the sum of the non-delivered order rows). A delivered card order has status CONFIRMED at Svea.

Get an order builder instance using the WebpayAdmin.DeliverOrderRows() entrypoint, then provide more information about the transaction and send the request using the DeliverOrderRowsBuilder methods:

Use SetRowToDeliver() or SetRowsToDeliver() to specify the order row(s) to deliver. The order row indexes should correspond to those returned by i.e. WebpayAdmin.QueryOrder();

For card orders, use AddNumberedOrderRow() or AddNumberedOrderRows() to pass in a copy of the original order rows. The original order rows can be retrieved using WebpayAdmin.QueryOrder; the numberedOrderRows attribute contains the serverside order rows w/indexes. Note that if a card order has been modified (i.e. rows cancelled or credited) after the initial order creation, the returned order rows will not be accurate.


```csharp
  DeliverOrderRowsBuilder request = WebpayAdmin.DeliverOrderRows(config)
        .SetOrderId()                 // required
        .SetTransactionId()           // optional, card or direct bank only, alias for SetOrderId
        .SetCountryCode()             // required
        .SetInvoiceDistributionType() // required for invoice only
        .SetRowToDeliver()            // required, index of original order rows you wish to deliver
        .AddNumberedOrderRow()        // required for card orders, should match original row indexes
        ;
        // then select the corresponding request class and send request
        response = request.DeliverInvoiceOrderRows().DoRequest();   // returns AdminWS.DeliverOrderRowsResponse
        response = request.DeliverCardOrder().DoRequest();          // returns Hosted.Admin.Actions.ConfirmResponse

```

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 7.3 WebpayAdmin.CancelOrder
The  WebpayAdmin.CancelOrder() entrypoint method is used to cancel an order with Svea,
that has not yet been delivered (invoice, payment plan) or confirmed (card).

Supports Invoice, Payment Plan and Card orders.

Get an instance using the WebpayAdmin.CancelOrder entrypoint, then provide more information about the order and send
the request using the CancelOrderBuilder methods:

```csharp
    CancelOrderBuilder request = WebpayAdmin.CancelOrder(config)
        .SetOrderId()                 // required, use SveaOrderId recieved with createOrder response
        .SetTransactionId()           // optional, card or direct bank only, alias for SetOrderId
        .SetCountryCode()             // required
        ;
        // then select the corresponding request class and send request
        response = request.CancelInvoiceOrder().DoRequest();       // returns AdminWS.CancelOrderResponse
        response = request.CancelPaymentPlanOrder().DoRequest();   // returns AdminWS.CancelOrderResponse
        response = request.CancelCardOrder().DoRequest();          // returns Hosted.Admin.Actions.AnnulResponse

```

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## APPENDIX

### PaymentMethods
Enumeration, used in *UsePaymentMethod(PaymentMethod PaymentMethod)*, *.UsePayPage()*,
*.IncludePaymentMethod(Collection<PaymentMethod> PaymentMethods)*, *.IncludePaymentMethod()*,
*.ExcludePaymentMethods(Collection<PaymentMethod> PaymentMethods)*,
*.ExcludeDirectPaymentMethod()* and *.ExcludeCardPaymentMethod()*.

| Payment method                    | Description                                   |
|-----------------------------------|-----------------------------------------------|
| PaymentMethod.BANKAXESS           | Direct bank payment, Norway.                  |
| PaymentMethod.NORDEA_SE           | Direct bank payment, Nordea, Sweden.          |
| PaymentMethod.SEB_SE              | Direct bank payment, private, SEB, Sweden.    |
| PaymentMethod.SEBFTG_SE           | Direct bank payment, company, SEB, Sweden.    |
| PaymentMethod.SHB_SE              | Direct bank payment, Handelsbanken, Sweden.   |
| PaymentMethod.SWEDBANK_SE         | Direct bank payment, Swedbank, Sweden.        |
| PaymentMethod.KORTCERT            | Card payments, Certitrade.                    |
| PaymentMethod.SVEACARDPAY         | Card payments, Svea.                          |
| PaymentMethod.PAYPAL              | Paypal                                        |
| PaymentMethod.SKRILL              | Card payment with Dankort, Skrill.            |
| PaymentMethod.INVOICE             | Invoice by PayPage.                           |
| PaymentMethod.PAYMENTPLAN         | PaymentPlan by PayPage.                       |


[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### CountryCode
Enumeration, used in .SetCountryCode(...). Using ISO 3166-1 standard.

| CountryCode						| Country					|
|-----------------------------------|---------------------------|
| CountryCode.DE					| Germany					|
| CountryCode.DK					| Denmark 					|
| CountryCode.FI					| Finland					|
| CountryCode.NL					| Netherlands				|
| CountryCode.NO					| Norway					|
| CountryCode.SE					| Sweden					|

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### LanguageCode
Enumeration, used in .SetPayPageLanguage(...). Using ISO 639-1 standard.

| LanguageCode						| Language name				|
|-----------------------------------|---------------------------|
| LanguageCode.da					| Danish					|
| LanguageCode.de					| German					|
| LanguageCode.en					| English					|
| LanguageCode.es					| Spanish					|
| LanguageCode.fr					| French					|
| LanguageCode.fi					| Finnish					|
| LanguageCode.it					| Italian					|
| LanguageCode.nl					| Dutch						|
| LanguageCode.no					| Norwegian					|
| LanguageCode.sv					| Swedish					|

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### Currency
Enumeration, used in .SetCurrency(...). Using ISO 4217 standard.

| CurrencyCode						| Currency name				|
|-----------------------------------|---------------------------|
| Currency.DKK						| Danish krone				|
| Currency.EUR						| Euro						|
| Currency.NOK						| Norwegian krone			|
| Currency.SEK						| Swedish krona				|

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### Invoice Distribution Type
Enumeration, used in .SetInvoiceDistributionType(...).

| DistributionType					| Description				|
|-----------------------------------|---------------------------|
| Post								| Invoice is sent by mail	|
| Email								| Invoice is sent by e-mail	|

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)
