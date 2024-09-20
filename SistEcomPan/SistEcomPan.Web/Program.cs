using IOC;


using Microsoft.AspNetCore.Authentication.Cookies;
using SistEcomPan.Web.Tools.Handler;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options=>
    
    {
        options.IdleTimeout = TimeSpan.FromMinutes(20);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Login";
        option.LogoutPath = "/Acceso/Salir";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        option.AccessDeniedPath = "/Home/Privacy";
    });

builder.Services.InyectarDependencias();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.Use(async(context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache,no-store,must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.UseWebSockets(); // Habilitar soporte para WebSockets
app.Map("/ws", (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = context.WebSockets.AcceptWebSocketAsync().Result;
        return Task.Run(() => MensajeWebSocketHandler.HandleWebSocketAsync(webSocket));
    }
    else
    {
        context.Response.StatusCode = 400;
        return Task.CompletedTask;
    }
});


app.Run();
