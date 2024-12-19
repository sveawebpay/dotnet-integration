using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.AspNetCore.Data;
using Sample.AspNetCore.Models;
using System.Collections.Generic;

namespace Sample.AspNetCore;

using JsonSerializer = System.Text.Json.JsonSerializer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<List<MarketSettings>>(Configuration.GetSection("Markets"));
        services.AddDbContext<StoreDbContext>(options => options.UseInMemoryDatabase("Products"));
        services.AddControllersWithViews();
        services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
        services.AddDistributedMemoryCache();

        services.AddScoped(provider => SessionCart.GetCart(provider));
        services.AddScoped(provider => SessionMarket.GetMarket(provider));

        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        services.AddSession();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "products",
                "{controller=Products}/{action=Index}/{id?}");
            endpoints.MapControllers();
        });
    }
}
