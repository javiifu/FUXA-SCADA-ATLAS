using Proyecto_FUXA.Components;
using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ServicioMaquina>();
builder.Services.AddScoped<ServicioProductividad>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SchemaUpdate");

    try
    {
        await db.Database.ExecuteSqlRawAsync(@"
            IF COL_LENGTH('Maquina', 'IdentificadorObjetoFuxa') IS NULL
            BEGIN
                ALTER TABLE [Maquina]
                ADD [IdentificadorObjetoFuxa] NVARCHAR(100) NULL;
            END;");

        await db.Database.ExecuteSqlRawAsync(@"
            IF EXISTS (
                SELECT 1
                FROM sys.indexes
                WHERE name = 'IX_Maquina_IdentificadorObjetoFuxa'
                  AND object_id = OBJECT_ID('Maquina')
                  AND is_unique = 0
            )
            BEGIN
                DROP INDEX [IX_Maquina_IdentificadorObjetoFuxa] ON [Maquina];
            END;");

        await db.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (
                SELECT 1
                FROM sys.indexes
                WHERE name = 'IX_Maquina_IdentificadorObjetoFuxa'
                  AND object_id = OBJECT_ID('Maquina')
                  AND is_unique = 1
            )
            BEGIN
                CREATE UNIQUE INDEX [IX_Maquina_IdentificadorObjetoFuxa]
                    ON [Maquina]([IdentificadorObjetoFuxa])
                    WHERE [IdentificadorObjetoFuxa] IS NOT NULL;
            END;");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "No se pudo aplicar el ajuste de esquema para IdentificadorObjetoFuxa. Revisa permisos de ALTER TABLE/CREATE INDEX o ejecuta el script SQL manual.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore/hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
