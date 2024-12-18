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

#region 自訂 LoggerProvider
builder.Logging.ClearProviders();
builder.Logging.AddMyLogger();
#endregion

builder.Services.AddIdleCircuitHandler(options =>
    options.IdleTimeout = TimeSpan.FromSeconds(20));

builder.Services.AddSingleton<MySyncService>();
builder.Services.AddScoped<AuthenticationStateProvider, MyAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddSingleton<Member>();

//Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc(setupAction: options => options.EnableEndpointRouting = false);
//    .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);


var app = builder.Build();

#region 記錄系統啟動時間
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("BlazorTestV2 program has started!");
#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Swager
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Web API
app.UseRouting();
app.UseMvcWithDefaultRoute();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
