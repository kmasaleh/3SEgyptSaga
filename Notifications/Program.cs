using MassTransit;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Notifications.Application;
using Notifications.Application.DTOs;
using Notifications.Application.Sagas;
using Notifications.Persistence;
using static Notifications.Application.ServiceExt;


// Helper: defines EF saga repository for the state machine


var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls("http://+:80"); // Important!


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
    opt.SwaggerDoc("v1", new() { Title = "3S Egypt Notifications API", Version = "v1" });
    opt.CustomSchemaIds(type => type.FullName); // Use full name for schema IDs to avoid conflicts

});

builder.Services.AddMassTransit(configure =>
{
   // configure.SetSagaRepositoryProvider(new EntityFrameworkSagaRepositoryProviderSetup());
    configure.SetKebabCaseEndpointNameFormatter();
    configure.UsingRabbitMq((context, cfg) =>
    {

        //cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ") ?? "rabbitmq://localhost", h =>
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
    configure.AddSagas(typeof(Program).Assembly);
    configure.AddSagaStateMachine<NewUserOnboardSaga, NewUserOnboardSagaData>()
    .EntityFrameworkRepository(r =>
    {
        r.ExistingDbContext<NotificationsDbContext>();
        r.UseSqlServer();
    });

    //configure.AddSagaStateMachines(typeof(Program).Assembly);
});




await builder.Services.InstallServices(builder.Configuration);


//////////////////////////////////////////////////////////////////////////////////////////////////////
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "3S Notifications API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });

}

app.UseHttpsRedirection();



app.MapPost("/UserRegistered", async (UserRegisteredDto dto, IBus bus) =>
{
    await bus.Publish<UserRegisteredCommand>(new UserRegisteredCommand
    (
        Message : dto.Message,
        UserEmail: dto.UserEmail
    ));
    return Results.Ok();
})
.WithName("UserRegistered");



app.MapGet("/Find", async (string user, INotificationsService service) =>
{
    return await service.ListEventsByUserAsync(user);
})
.WithName("Find");

app.MapGet("/ListAll", async (INotificationsService service) =>
{
    return await service.ListEventsAsync();
})
.WithName("ListAll");

app.MapDelete("/Reset", async ([FromServices] INotificationsService service) =>
{
    await service.ResetAsync();
    return Results.Ok();
})
.WithName("Reset");

await app.Services.ApplyMigrationAsync();
app.Run();

