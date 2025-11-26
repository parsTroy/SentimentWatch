using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using StockSentiment.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient<ISentimentClient, SentimentClient>((sp, client) =>
{
    // For now, assume API is running on the same host but different port.
    // You can move this to configuration later.
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseAddress = configuration.GetValue<string>("SentimentApiBaseUrl")
                     ?? "https://localhost:5001";

    client.BaseAddress = new Uri(baseAddress);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
