using Auditings.Application;
using Auditings.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls("http://+:80"); // Important!
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi



builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new() { Title = "3S Egypt Auditing Service API", Version = "v1" });
    opt.CustomSchemaIds(type => type.FullName); // Use full name for schema IDs to avoid conflicts

});

builder.Services.AddMassTransit(configure =>
{
    configure.SetKebabCaseEndpointNameFormatter();
    configure.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ") 
            //?? "rabbitmq://localhost"
             ?? "rabbitmq://guest:guest@rabbitmq:5672"
            , h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
        cfg.UseInMemoryOutbox(context); // FIX: Use the IRegistrationContext overload as recommended
    });
    configure.AddConsumers(typeof(Program).Assembly);
});

await builder.Services.InstallServices(builder.Configuration);
///////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "3S Auditing API V1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        });

    }

}

app.UseHttpsRedirection();
//===============================================================================================================
app.MapGet("/ListAll", async (AuditingDbContext db) =>
{
    var data = await db.Logs.ToListAsync();
    return data;
})
.WithName("ListAll");


app.MapGet("/Find", async (string text, AuditingDbContext db) =>
{
    var data = await db.Logs
                    .Where(e => e.Data.Contains(text) )
                    .ToListAsync();
    return data;
})
.WithName("Find");


app.MapPut("/Fail", async (bool shouldFaile, FailureService failureService) =>
{
    await failureService.Fail(shouldFaile);
    return Results.Ok();
})
.WithName("Fail");


app.MapDelete("/Reset", async (AuditingDbContext db) =>
{
    var data = db.Logs.ToList();
    db.Logs.RemoveRange(data.ToArray());
    await db.SaveChangesAsync();
    return Results.Ok();
})
.WithName("Reset");

await app.Services.ApplyMigrationAsync();
app.Run();