# C#/.Net Integration Package API for SveaWebPay

## Index
* [1. Introduction](https://github.com/sveawebpay/dotnet-integration/tree/master#1-introduction)
* [2. Build](https://github.com/sveawebpay/dotnet-integration/tree/master#2-build)
* [3. Configuration](https://github.com/sveawebpay/dotnet-integration/tree/master#3-configuration)
* [4. CreateOrder](https://github.com/sveawebpay/dotnet-integration/tree/master#4-createorder)
    * [Specify order](https://github.com/sveawebpay/dotnet-integration/tree/master#41-specify-order)
    * [Customer identity](https://github.com/sveawebpay/dotnet-integration/tree/master#42-customer-identity)
    * [Other values](https://github.com/sveawebpay/dotnet-integration/tree/master#43-other-values)
    * [Choose payment](https://github.com/sveawebpay/dotnet-integration/tree/master#44-choose-payment)
* [5. GetPaymentPlanParams](https://github.com/sveawebpay/dotnet-integration/tree/master#5-getpaymentplanparams)
    * [PaymentPlanPricePerMonth](https://github.com/sveawebpay/dotnet-integration/tree/master#51-paymentplanpricepermonth)
* [6. GetAddresses](https://github.com/sveawebpay/dotnet-integration/tree/master#6-getaddresses)
* [7. DeliverOrder](https://github.com/sveawebpay/dotnet-integration/tree/master#7-deliverorder)
    * [Deliver Invoice order](https://github.com/sveawebpay/dotnet-integration/tree/master#71-deliver-invoice-order)
    * [Other values](https://github.com/sveawebpay/dotnet-integration/tree/master#72-other-values)
* [8. Credit Invoice](https://github.com/sveawebpay/dotnet-integration/tree/master#8-credit-invoice)
* [9. CloseOrder](https://github.com/sveawebpay/dotnet-integration/tree/master#9-closeorder)
* [10. Response handler](https://github.com/sveawebpay/dotnet-integration/tree/master#10-response-handler)
* [APPENDIX](https://github.com/sveawebpay/dotnet-integration/tree/master#appendix)


## 1. Introduction                                                             
This integration package is built for developers to simplify the integration of Svea WebPay services. 
Using this package will make your implementation sustainable and unaffected by changes
in our payment system. Just make sure to update the package regularly.

The API is built as a *Fluent API*, ie. you can use *method chaining* when implementing it in your code.

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 2. Build                                                             

To build the dll file, load up the project in Visual studio.
Choose 'Release' as your solution configuration and press F6.
(The solution is compatible and will compile properly with .NET Framework 4.0 and higher.)

This will build the dll and place it in bin/release sub-folder of the project.

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 3. Configuration 

The Configuration needed to be set differs of how many different payment methods and countries you have in the same installation. 
The authorization values are recieved from Svea Ekonomi when creating an account. If no configuration is created, use SveaConfig.GetDefaultConfig().

**To configure Svea authorization:**
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

## 4. CreateOrder                                                            
Creates an order and performs payment for all payment forms. Invoice and payment plan will perform 
a synchronous payment and return a response. 
Other hosted payments, like card, direct bank and payments from the *PayPage*
on the other hand are asynchronous. They will return an html form with formatted message to send from your store.
For every new payment type implementation, you follow the steps from the beginning and chose your payment type preffered in the end:
Build order -> choose payment type -> DoRequest/GetPaymentForm

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
//Continue as a direct bank payment		
.UsePayPageDirectBankOnly()
	...
	.GetPaymentForm();
//Continue as a PayPage payment
.UsePayPage()
	...
	.GetPaymentForm();
//Continue as a PayPage payment
.UsePaymentMethod(PaymentMethod.DBSEBSE) //see APPENDIX for Constants
	...
	.GetPaymentForm();
//Continue as an invoice payment
.UseInvoicePayment()
	...
	.DoRequest();
//Continue as a payment plan payment
.UsePaymentPlanPayment("campaigncode", false)
	...
	.DoRequest();
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

	
### 4.1 Specify order                                                        
Continue by adding values for products and other. You can add order row, fee and discount. Chose the right Item object as parameter.
You can use the *add* functions with an Item object or a List of Item objects as parameters. See types of Item objects in examples in 4.1.2 - 4.1.5 and 4.3.1 - 4.3.2.

```csharp
.AddOrderRow(Item.OrderRow(). ...)

//or

var orderRows = new List<OrderRowBuilder>();
orderRows.Add(Item.OrderRow(). ...)
...
CreateOrder.AddOrderRows(orderRows);
```
	
#### 4.1.1 OrderRow
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

#### 4.1.2 ShippingFee
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
#### 4.1.3 InvoiceFee
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
#### 4.1.4 Fixed Discount
When discount or coupon is a fixed amount on total product amount.
```csharp
.AddDiscount(Item.FixedDiscount()                
	.SetAmountIncVat(100.00M)               //Required
	.SetDiscountId("1")                    //Optional        
	.SetUnit("st")                         //Optional
	.SetDescription("FixedDiscount")       //Optional
	.SetName("Fixed"))                     //Optional    
```
#### 4.1.5 Relative Discount
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

### 4.2 Customer Identity   
Customer identity is required for invoice and payment plan orders. Required values varies 
depending on country and customer type. For SE, NO, DK and FI national id number is required. Email and ip address are desirable.

####4.2.1 Options for individual customers
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

####4.2.2 Options for company customers
```csharp
.AddCustomerDetails(Item.CompanyCustomer()
    .SetNationalIdNumber("2345234")			//Required for company customers in SE, NO, DK, FI
    .SetVatNumber("NL2345234")				//Required for NL and DE
    .SetCompanyName("TestCompagniet")) 		//Required for Eu countries like NL and DE
	.SetAddressSelector("7fd7768")          //Optional. Recieved from getAddresses		
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

###4.3 Other values  
```csharp
.SetCountryCode(CountryCode.SE)         	//Required
.SetCurrency(Currency.SEK)                     	//Required for card payment, direct payment and PayPage payment.
.SetClientOrderNumber("33")           		//Required for card payment, direct payment, PaymentMethod payment and PayPage payments. Must be uniqe.
.SetOrderDate(new DateTime(2012, 12, 12))   //Required for synchronous payments
.SetCustomerReference("ref33")             	//Optional
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

###4.4 Choose payment 
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


#### Asynchronous payments - Hosted solutions
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

.SetReturnUrl() When a hosted payment transaction completes (regardless of outcome, i.e. accepted or denied), the payment service will answer with a response xml message sent to the return url specified.

.SetCallbackUrl() In case the hosted payment transaction completes, but the service is unable to return a response to the return url, the payment service will retry several times using the callback url as a fallback, if specified. This may happen if i.e. the user closes the browser before the payment service redirects back to the shop.

.SetCancelUrl() In case the hosted payment service is cancelled by the user, the payment service will redirect back to the cancel url. Unless a return url is specified, no cancel button will be presented at the payment service.

####4.4.1 PayPage with card payment options
*PayPage* with availible card payments only.

#####4.4.1.1 Request

```csharp
PaymentForm form = WebpayConnection.CreateOrder()
.AddOrderRow(Item.OrderRow()
	.SetArticleNumber("1")
	.SetName("Prod")
	.SetDescription("Specification")
	.SetQuantity(2)
	.SetUnit("st")
	.SetAmountExVat(100.00M)
	.SetVatPercent(25.00M)
	.SetDiscountPercent(0))
		
.SetCountryCode(CountryCode.SE)							//Required
.SetClientOrderNumber("33")
.SetOrderDate(new DateTime(2012, 12, 12))
.SetCurrency(Currency.SEK)
.UsePayPageCardOnly()
	.SetReturnUrl("http://myurl.se")                   	//Required
	.SetCallbackUrl("http://myurl.se")                   	//Optional
	.SetCancelUrl("http://myurl.se")                   	//Optional
	.GetPaymentForm();

```
#####4.4.1.2 Return
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
            

####4.4.2 PayPage with direct bank payment options
*PayPage* with available direct bank payments only.
                
#####4.4.2.1 Request

```csharp
PaymentForm form = WebpayConnection.CreateOrder()
.AddOrderRow(Item.OrderRow()
	.SetArticleNumber("1")
	.SetName("Prod")
	.SetDescription("Specification")
	.SetQuantity(2)
	.SetUnit("st")
	.SetAmountExVat(100.00M)
	.SetVatPercent(25.00M)
	.SetDiscountPercent(0))
		
.SetCountryCode(CountryCode.SE)						   //Required
.SetCustomerReference("ref33")
.SetOrderDate(new DateTime(2012, 12, 12))
.SetCurrency(Currency.SEK)
.UsePayPageDirectBankOnly()	
	.SetReturnUrl("http://myurl.se")                   //Required		
	.SetCancelUrl("http://myurl.se")                   //Optional
	.GetPaymentForm();
```
#####4.4.2.2 Return
Returns object type PaymentForm:
           
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
 
 
####4.4.3 PayPagePayment
*PayPage* with all available payments. You can also custom the *PayPage* by using one of the methods for *PayPagePayments*:
SetPaymentMethod, IncludePaymentMethod, ExcludeCardPaymentMethod or ExcludeDirectPaymentMethod.
                
#####4.4.3.1 Request
```csharp
PaymentForm form = WebpayConnection.CreateOrder()
.AddOrderRow(Item.OrderRow()
	.SetArticleNumber("1")
	.SetName("Prod")
	.SetDescription("Specification")
	.SetQuantity(2)
	.SetUnit("st")
	.SetAmountExVat(100.00M)
	.SetVatPercent(25.00M)
	.SetDiscountPercent(0))   
	
.SetCountryCode(CountryCode.SE)							//Required
.SetCustomerReference("ref33")
.SetOrderDate(new DateTime(2012, 12, 12))
.SetCurrency(Currency.SEK)
.UsePayPage()
	.SetReturnUrl("http://myurl.se")                   	//Required	
	.SetCancelUrl("http://myurl.se")                   	//Optional
	.SetPayPageLanguageCode(LanguageCode.sv)					//Optional, English is default. LanguageCode, see APPENDIX
	.GetPaymentForm();
```               

######4.4.3.1.1 Exclude specific payment methods
Optional if you want to include specific payment methods for *PayPage*.
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
######4.4.3.1.2 Include specific payment methods
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

######4.4.3.1.3 Exclude Card payments
Optional if you want to exclude all card payment methods from *PayPage*.
```csharp
.UsePayPage()
	.SetReturnUrl("http://myurl.se")                   //Required
	.ExcludeCardPaymentMethod()                       //Optional
	.GetPaymentForm();
```

######4.4.3.1.4 Exclude Direct payments
Optional if you want to exclude all direct bank payments methods from *PayPage*.
```csharp  
.UsePayPage()
    .SetReturnUrl("http://myurl.se")                   //Required
    .ExcludeDirectPaymentMethod()                     //Optional
    .GetPaymentForm();
```
#####4.4.3.6 Return
Returns object type *PaymentForm*:
                
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
 

#### 4.4.4 PaymentMethod specified
Go direct to specified payment method without the step *PayPage*.

##### 4.4.4.1 Request
Set your store authorization here.
```csharp
PaymentForm form = WebpayConnection.CreateOrder()
.AddOrderRow(Item.OrderRow()
	.SetArticleNumber("1")
	.SetName("Prod")
	.SetDescription("Specification")
	.SetQuantity(2)
	.SetUnit("st")
	.SetAmountExVat(100.00M)
	.SetVatPercent(25)
	.SetDiscountPercent(0))                  
		
.SetCountryCode(CountryCode.SE)							//Required
.SetClientOrderNumber("33")
.SetOrderDate(new DateTime(2012, 12, 12))
.SetCurrency(Currency.SEK)
.UsePaymentMethod(PaymentMethod.KORTCERT)             	//PaymentMethod see APPENDIX
	.SetReturnUrl("http://myurl.se")                  	//Required
	.SetCancelUrl("http://myurl.se")                  	//Optional
	.GetPaymentForm();

```
##### 4.4.4.2 Return
The values of *xmlMessageBase64*, *merchantid* and *mac* are to be sent as xml to SveaWebPay.
Function GetPaymentForm() returns Object type PaymentForm with accessible members:

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
            

#### Synchronous solutions - Invoice and PaymentPlan
       
#### 4.4.5 InvoicePayment
Perform an invoice payment. This payment form will perform a synchronous payment and return a response.
Returns *CreateOrderEuResponse* object. Set your store authorization here.
```csharp
CreateOrderEuResponse response = WebpayConnection.CreateOrder()
.AddOrderRow(Item.OrderRow()
.SetArticleNumber("1")
.SetName("Prod")
.SetDescription("Specification")
.SetQuantity(2)
.SetUnit("st")
.SetAmountExVat(100.00M)
.SetVatPercent(25.00M)
.SetDiscountPercent(0))

.SetCountryCode(CountryCode.SE)						//Required
.SetCustomerReference("ref33")
.SetOrderDate(new DateTime(2012, 12, 12))
.SetCurrency(Currency.SEK)
.UseInvoicePayment()
	.DoRequest();
```
#### 4.4.6 PaymentPlanPayment
Perform *PaymentPlanPayment*. This payment form will perform a synchronous payment and return a response.
Returns a *CreateOrderEuResponse* object. Preceded by WebPay.GetPaymentPlanParams(...).
Set your store authorization here.
Param: Campaign code recieved from GetPaymentPlanParams().
```csharp
CreateOrderEuResponse response = WebpayConnection.CreateOrder()
.AddOrderRow(Item.OrderRow()
	.SetArticleNumber("1")
	.SetName("Prod")
	.SetDescription("Specification")
	.SetQuantity(2)
	.SetUnit("st")
	.SetAmountExVat(100.00M)
	.SetVatPercent(25.00M)
	.SetDiscountPercent(0))   
	
.SetCountryCode(CountryCode.SE)						//Required
.SetCustomerReference("ref33")
.SetOrderDate(new DateTime(2012, 12, 12))
.SetCurrency(Currency.SEK)
.UsePaymentPlanPayment(1234L, false)              	//Parameter1: campaign code recieved from getPaymentPlanParams
													//Paremeter2: True if Automatic autogiro form will be sent with the first notification		   
   .DoRequest();
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 5. GetPaymentPlanParams   
Use this function to retrieve campaign codes for possible payment plan options. Use prior to create payment plan payment.
Returns *PaymentPlanParamsResponse* object. Set your store authorization here.

```csharp
GetPaymentPlanParamsEuResponse response = WebpayConnection.GetPaymentPlanParams()	
	.SetCountryCode(CountryCode.SE)					//Required	
	.DoRequest();
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 5.1 PaymentPlanPricePerMonth
Use this function to calculate the prices per month of the payment plan campaigns received from GetPaymentPlanParams.
*paymentPlanParams* is the response object from GetPaymentPlanParams.
```csharp
   List<Dictionary<string, string>> pricePerMonth = WebpayConnection.PaymentPlanPricePerMonth(amount, paymentPlanParams);
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 6. GetAddresses 
Returns *getAddressesResponse* object with an *AddressSelector* for the associated addresses for a specific security number. 
Can be used when creating an order. Only applicable for SE, NO and DK. In Norway, only getAddresses of companies is supported.
Set your store authorization here.

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 6.1 Order type 
```csharp
    .SetOrderTypeInvoice()         //Required if this is an invoice order
or
    .SetOrderTypePaymentPlan()     //Required if this is a payment plan order
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 6.2 Customer type 
```csharp
    .SetIndividual("194605092222") //Required if this is an individual customer
or
    .SetCompany("companyId")       //Required if this is a company customer
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 6.3                                                                      	
```csharp
GetAddressesEuResponse response = WebpayConnection.GetAddresses(myConfig)		//see more about Configuration chapt.3       	
	.SetCountryCode(CountryCode.SE)                                 //Required
	.SetOrderTypeInvoice()                                          //See 6.1   	
	.SetIndividual("194605092222")                                  //See 6.2
    .SetZipCode("9999")                                             //Required	
	.DoRequest();
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

## 7. DeliverOrder                                                           
Use the WebpayConnection.DeliverOrder request to deliver to the customer invoices for fulfilled orders.
Svea will invoice the customer upon receiving the DeliverOrder request.
A DeliverOrder request may also be used to partly deliver an order on Invoice orders.
Add rows that you want delivered. The rows will automatically be matched with the rows that was sent when creating the order.
When Svea receives the DeliverOrder request the status on the previous created order is set to *delivered*.
The DeliverOrder functionallity is only applicable to invoice and payment plan payment method payments.

Returns *DeliverOrderEuResponse* object. Set your store authorization here.

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 7.1 Deliver Invoice order
This works more or less like WebpayConnection.CreateOrder above, and makes use of the same order item information.
Add the corresponding order id and the order rows that you want delivered before making the DeliverOrder request.
The specified rows will automatically be matched with the previous rows that was sent when creating the order.
We recommend storing the order row data to ensure that matching orderrows can be recreated in the DeliverOrder request.

If an item is left out from the DeliverOrder request that was present in the CreateOrder request, a new invoice will be created as the order is assumed to be partially fulfilled.
Any left out items should not be delivered physically, as they will not be invoiced when the DeliverOrder request is sent.

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
        .DeliverInvoiceOrder()
            .DoRequest();
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

#### 7.1.1 OrderRow
All products and other items. It is required to have a minimum of one row.
```csharp
.AddOrderRow(Item.OrderRow()
   .SetArticleNumber("1")               //Optional
   .SetName("Prod")                 	//Optional
   .SetDescription("Specification")	    //Optional
   .SetQuantity(2)              	    //Required
   .SetUnit("st")                      	//Optional
   .SetAmountExVat(100.00M)  	        //Required
   .SetVatPercent(25.00M)    	        //Required
   .SetDiscountPercent(0))             	//Optional
```

#### 7.1.2 ShippingFee
```csharp
.AddFee(Item.ShippingFee()
	.SetAmountExVat(50)               	//Required
	.SetVatPercent(25.00M)              //Required
	.SetShippingId("33")               	//Optional
	.SetName("shipping")               	//Optional
	.SetDescription("Specification")   	//Optional        
	.SetUnit("st")                     	//Optional        
	.SetDiscountPercent(0))
```
#### 7.1.3 InvoiceFee
```csharp
.AddFee(Item.InvoiceFee()
	.SetAmountExVat(50)            		//Required
	.SetVatPercent(25.00M)          	//Required
	.SetName("Svea fee")           		//Optional
	.SetDescription("invoice fee") 		//Optional       
	.SetUnit("st")                 		//Optional
	.SetDiscountPercent(0))        		//Optional
```
[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 7.2 Other values  
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

## 8. Credit Invoice
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

## 9. CloseOrder                                                             
Use when you want to cancel an undelivered order. Valid only for invoice and payment plan orders. 
Required is the order id received when creating the order. Set your store authorization here.

[<< To top](https://github.com/sveawebpay/dotnet-integration/tree/master#cnet-integration-package-api-for-sveawebpay)

### 9.1 Close by payment type                                                
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

## 10. Response handler                                                       
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

## APPENDIX 

### PaymentMethods
Enumeration, used in *UsePaymentMethod(PaymentMethod PaymentMethod)*, *.UsePayPage()*, 
*.IncludePaymentMethod(Collection<PaymentMethod> PaymentMethods)*, *.IncludePaymentMethod()*, 
*.ExcludePaymentMethods(Collection<PaymentMethod> PaymentMethods)*,
*.ExcludeDirectPaymentMethod()* and *.ExcludeCardPaymentMethod()*.

| Payment method                   | Description                                   |
|----------------------------------|-----------------------------------------------|
| PaymentMethod.BANKAXESS	       | Direct bank payment, Norway.       		   | 
| PaymentMethod.NORDEA_SE	       | Direct bank payment, Nordea, Sweden.          | 
| PaymentMethod.SEB_SE	           | Direct bank payment, private, SEB, Sweden.    |
| PaymentMethod.SEBFTG_SE 	       | Direct bank payment, company, SEB, Sweden.    |
| PaymentMethod.SHB_SE	           | Direct bank payment, Handelsbanken, Sweden.   |
| PaymentMethod.SWEDBANK_SE	       | Direct bank payment, Swedbank, Sweden.        |
| PaymentMethod.KORTCERT           | Card payments, Certitrade.                    |
| PaymentMethod.PAYPAL             | Paypal                                        |
| PaymentMethod.SKRILL             | Card payment with Dankort, Skrill.            |
| PaymentMethod.INVOICE			   | Invoice by PayPage.                		   |
| PaymentMethod.PAYMENTPLAN        | PaymentPlan by PayPage.			           |


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
