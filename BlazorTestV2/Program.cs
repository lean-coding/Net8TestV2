using BlazorTestV2.Components;
using BlazorTestV2.Service;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components.Authorization;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

//builder.Logging.SetMinimumLevel(LogLevel.Warning);

#region ¦Û­q LoggerProvider
builder.Logging.ClearProviders();
builder.Logging.AddMyLogger();
#endregion

builder.Services.AddScoped<AuthenticationStateProvider, MyAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("BlazorTestV2 program has started!");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
