using System.Linq;
using System.Threading.Tasks;

namespace Sample.AspNetCore.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sample.AspNetCore.Data;
using Sample.AspNetCore.Models;

using System;

using Cart = Models.Cart;

[ApiController]
[Route("api/svea")]
public class SveaController : ControllerBase
{
    private readonly ILogger<SveaController> _logger;
    private readonly Cart _cartService;
    private readonly StoreDbContext _context;
    //private readonly SveaWebPayClient _sveaClient;

    //public SveaController(ILogger<SveaController> logger, Cart cartService, StoreDbContext context, SveaWebPayClient sveaClient)
    //{
    //    _logger = logger;
    //    _cartService = cartService;
    //    _context = context;
    //    _sveaClient = sveaClient;
    //}

    public SveaController(ILogger<SveaController> logger, Cart cartService, StoreDbContext context)
    {
        _logger = logger;
        _cartService = cartService;
        _context = context;
    }

    //[HttpPost("shippingTaxCalculation")]
    //public async Task<ActionResult> ShippingTaxCalculation(ShippingOption shippingOption)
    //{
    //    var order = await _sveaClient.Checkout.GetOrder(shippingOption.OrderId).ConfigureAwait(false);
    //    order.Cart.CalculateShippingOrderRows(shippingOption);

    //    await _sveaClient.Checkout.UpdateOrder(order.OrderId, new UpdateOrderModel(order.Cart, null, order.ShippingInformation)).ConfigureAwait(false);

    //    return Ok();
    //}

    //[HttpPost("shippingvalidation")]
    //public async Task<ActionResult> ShippingValidation(ShippingCallbackResponse shippingCallbackResponse)
    //{
    //    var order = await _sveaClient.Checkout.GetOrder(shippingCallbackResponse.OrderId).ConfigureAwait(false);

    //    if (order != null && order.Status == CheckoutOrderStatus.Final)
    //    {
    //        var existingOrder = _context.Orders.FirstOrDefault(x => x.SveaOrderId == order.OrderId.ToString());
    //        if (existingOrder == null)
    //        {
    //            return Problem(); 
    //        }

    //        existingOrder.ShippingStatus = shippingCallbackResponse.Type;
    //        existingOrder.ShippingDescription = shippingCallbackResponse.Description;

    //        await _context.SaveChangesAsync(true);
    //    }

    //    return Ok();
    //}

    //[HttpGet("validation/{orderId}")]
    //public ActionResult Validation(long? orderId)
    //{
    //    var response = new CheckoutValidationCallbackResponse(true);
    //    return Ok(response);
    //}

    //[HttpPost("push/{orderId}")]
    //public async Task<IActionResult> Push(long? orderId)
    //{
    //    try
    //    {
    //        if (orderId.HasValue)
    //        {
    //            var order = await _sveaClient.Checkout.GetOrder(orderId.Value).ConfigureAwait(false);

    //            if (order != null && order.Status == CheckoutOrderStatus.Final)
    //            {
    //                _cartService.SveaOrderId = order.OrderId.ToString();
    //                _cartService.Update();

    //                var products = _cartService.CartLines.Select(p => p.Product);

    //                _context.Products.AttachRange(products);

    //                var existing = _context.Orders.Find(order.OrderId.ToString());
    //                if (existing != null)
    //                {
    //                    return Ok();
    //                }

    //                _context.Orders.Add(new Order
    //                {
    //                    SveaOrderId = _cartService.SveaOrderId,
    //                    Lines = _cartService.CartLines.ToList(),
    //                    ShippingStatus = _cartService.ShippingStatus
    //                });
    //                
    //                await _context.SaveChangesAsync(true);
    //            }
    //        }

    //        return Ok();
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError(e, "Callback failed");
    //        return Ok();
    //    }
    //}
}
