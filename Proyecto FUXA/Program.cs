using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Components;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<ImputacionService>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped<ServicioMaquina>();
builder.Services.AddScoped<ServicioProductividad>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:1881/") });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("-------------------------------------------------");
    Console.WriteLine("💥 ERROR FATAL QUE ESTÁ MATANDO EL SERVIDOR: ");
    Console.WriteLine(ex.ToString());
    Console.WriteLine("-------------------------------------------------");
    Console.WriteLine("Presiona ENTER para cerrar esta ventana...");
    Console.ReadLine();
}