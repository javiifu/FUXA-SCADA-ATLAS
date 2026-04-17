using Microsoft.EntityFrameworkCore;

using Proyecto_FUXA;
using Proyecto_FUXA.Components;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(defaultConnection))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'ConnectionStrings:DefaultConnection'. " +
        "Configúrala en appsettings.json, appsettings.Development.json o variables de entorno.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        defaultConnection,
        sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        }));

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<ImputacionService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ServicioEmpleado>();
builder.Services.AddScoped<ServicioMaquina>();
builder.Services.AddScoped<ServicioIncidencia>();
builder.Services.AddScoped<ServicioProductividad>();
builder.Services.AddScoped<ServicioPlantaVisual>();
builder.Services.AddScoped<FuxaService>();



builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:1881")
});
builder.Services.AddRadzenComponents();
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