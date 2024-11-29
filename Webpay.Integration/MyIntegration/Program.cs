using System;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Order.Create;
using Webpay.Integration;
using Webpay.Integration.Util.Testing;
using WebpayWS;
using Webpay.Integration.Util.Constant;

namespace MyIntegration;

class Program
{
    static async Task Main(string[] args)
    {
        await PerformRequest();
    }

    private static async Task PerformRequest()
    {
        var config = new MyConfig
        {
            MyUserName = "sverigetest",
            MyPassword = "sverigetest",
            MyClientNumber = 79021,
            MyMerchantId = string.Empty,
            MySecretWord = string.Empty
        };

        try
        {
            // GetAddresses
            var response = await WebpayConnection.GetAddresses(config)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetOrderTypeInvoice()
                .SetIndividual(TestingTool.DefaultTestIndividualNationalIdNumber)
                .DoRequestAsync();

            Console.WriteLine($"GetAddresses accepted: {response.Accepted}");
            Console.WriteLine(response.Addresses[0].FirstName);
            Console.WriteLine(response.Addresses[0].LastName);

            // CreateOrder
            var createOrderBuilder = WebpayConnection.CreateOrder(config)
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                .AddCustomerDetails(Item.IndividualCustomer()
                    .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetOrderDate(TestingTool.DefaultTestDate)
                .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                .SetCurrency(TestingTool.DefaultTestCurrency);

            var request = createOrderBuilder.UseInvoicePayment().PrepareRequest();
            //Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity == null);

            var order = await createOrderBuilder.UseInvoicePayment().DoRequestAsync();

            Console.WriteLine(order.Accepted ? "Order created successfully!" : "Failed to create order.");
            PrintCreateOrderEuResponse(order);

            // GetOrder
            var queryOrderBuilder = WebpayAdmin.QueryOrder(config)
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE);

            var answer = await queryOrderBuilder.QueryInvoiceOrder().DoRequestAsync();

            // DeliverInvoiceOrders
            var orderone = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            var orderTwo = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            var orderThree = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            var orderIdsToDeliver = new List<long> { orderone.CreateOrderResult.SveaOrderId, orderTwo.CreateOrderResult.SveaOrderId };

            var builder = WebpayAdmin.DeliverOrders(config)
                .SetOrderIds(orderIdsToDeliver)
                .SetOrderId(orderThree.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST);

            var delivery = await builder.DeliverInvoiceOrders().DoRequestAsync();
            //Assert.That(delivery.ResultCode == 0);

            var referenceNumbers = from od in delivery.OrdersDelivered
                                   where orderIdsToDeliver.Contains(od.SveaOrderId) || od.SveaOrderId == orderThree.CreateOrderResult.SveaOrderId
                                   select od.DeliveryReferenceNumber;
            //Assert.That(referenceNumbers.Count(), Is.EqualTo(3));

            Thread.Sleep(2000);

            // GetInvoices
            //var invoiceIdsToRetrieve = new List<long> { orderone.CreateOrderResult.SveaOrderId, orderTwo.CreateOrderResult.SveaOrderId, orderThree.CreateOrderResult.SveaOrderId };
            var getInvoicesBuilder = WebpayAdmin.GetInvoices(config)
                //.SetInvoiceIds(invoiceIdsToRetrieve)
                .SetInvoiceIds(referenceNumbers.ToList())
                .SetCountryCode(CountryCode.SE);

            var getInvoicesRequest = getInvoicesBuilder.Build();
            var getInvoicesResponse = await getInvoicesRequest.DoRequestAsync();

            if (getInvoicesResponse != null)
            {
                Console.WriteLine("\nInvoices retrieved successfully.");
            }
            else
            {
                Console.WriteLine("\nFailed to retrieve invoices.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred:");
            Console.WriteLine(ex.Message);
        }
    }

    public static void PrintCreateOrderEuResponse(CreateOrderEuResponse response)
    {
        Console.WriteLine("CreateOrderEuResponse:");
        Console.WriteLine($"  ResultCode: {response.ResultCode}");

        if (response.CreateOrderResult != null)
        {
            Console.WriteLine("  CreateOrderResult:");
            Console.WriteLine($"    SveaOrderId: {response.CreateOrderResult.SveaOrderId}");
            Console.WriteLine($"    OrderType: {response.CreateOrderResult.OrderType}");
            Console.WriteLine($"    SveaWillBuyOrder: {response.CreateOrderResult.SveaWillBuyOrder}");
            Console.WriteLine($"    Amount: {response.CreateOrderResult.Amount}");

            if (response.CreateOrderResult.CustomerIdentity != null)
            {
                Console.WriteLine($"    CustomerIdentity: {response.CreateOrderResult.CustomerIdentity}");
                // Etc...
            }
            else
            {
                Console.WriteLine("    CustomerIdentity: null");
            }

            if (response.CreateOrderResult.ExpirationDate.HasValue)
            {
                Console.WriteLine($"    ExpirationDate: {response.CreateOrderResult.ExpirationDate.Value}");
            }
            else
            {
                Console.WriteLine("    ExpirationDate: null");
            }

            Console.WriteLine($"    ClientOrderNumber: {response.CreateOrderResult.ClientOrderNumber}");

            if (response.CreateOrderResult.PendingReasons != null && response.CreateOrderResult.PendingReasons.Length > 0)
            {
                Console.WriteLine("    PendingReasons:");
                foreach (var reason in response.CreateOrderResult.PendingReasons)
                {
                    Console.WriteLine($"      - {reason}");
                }
            }
            else
            {
                Console.WriteLine("    PendingReasons: none");
            }
        }
        else
        {
            Console.WriteLine("  CreateOrderResult: null");
        }

        if (response.NavigationResult != null)
        {
            Console.WriteLine("  NavigationResult:");
            Console.WriteLine($"    RedirectUrl: {response.NavigationResult.RedirectUrl}");
        }
        else
        {
            Console.WriteLine("  NavigationResult: null");
        }
    }
}
