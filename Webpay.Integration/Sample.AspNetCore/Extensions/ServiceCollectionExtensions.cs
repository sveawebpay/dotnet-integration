using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Net.Http;

namespace Sample.AspNetCore.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Sample.AspNetCore.Models;

using System.Collections.Generic;
using System.Linq;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSveaClient(this IServiceCollection services, Uri checkoutUri, Uri paymentAdminUri, string merchantId, string secret)
    {
        services.AddHttpClient("checkoutApi", client => client.BaseAddress = checkoutUri)
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(SleepDurations))
            .ConfigurePrimaryHttpMessageHandler(() => RedirectHandler);

        services.AddHttpClient("paymentAdminApi", client => client.BaseAddress = paymentAdminUri)
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(SleepDurations))
            .ConfigurePrimaryHttpMessageHandler(() => RedirectHandler);

        //services.AddTransient(s =>
        //{
        //    var httpContextAccessor= s.GetService<IHttpContextAccessor>();
        //    var marketService = s.GetService<Market>();
        //    var currentMarket = httpContextAccessor.HttpContext.Request.Query["marketId"].FirstOrDefault() ?? marketService.MarketId;
        //    var credentials = s.GetService<IOptions<List<Credentials>>>()?.Value;
        //    var credential = credentials?.FirstOrDefault(x => x.MarketId.Equals(currentMarket, StringComparison.InvariantCultureIgnoreCase));
        //    var httpClientFactory = s.GetService<IHttpClientFactory>();
        //    var checkoutApiHttpClient = httpClientFactory.CreateClient("checkoutApi");
        //    var paymentAdminApiHttpClient = httpClientFactory.CreateClient("paymentAdminApi");
        //    return new SveaWebPayClient(checkoutApiHttpClient, paymentAdminApiHttpClient, new Svea.WebPay.SDK.Credentials(credential?.MerchantId ?? merchantId, credential?.Secret ?? secret), s.GetService<ILogger>());
        //});

        return services;
    }

    private static TimeSpan[] SleepDurations =>
        new[] {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)
        };

    private static HttpClientHandler RedirectHandler => new HttpClientHandler { AllowAutoRedirect = false };
}