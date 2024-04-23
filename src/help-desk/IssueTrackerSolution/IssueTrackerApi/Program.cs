using FluentValidation;
using IssueTrackerApi.Controllers.Issues;
using Marten;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var databaseConnectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("No Connection String");
Console.WriteLine("using the connection string " + databaseConnectionString);
builder.Services.AddMarten(options =>
{
    options.Connection(databaseConnectionString);
}).UseLightweightSessions();

builder.Services.AddValidatorsFromAssemblyContaining<CreateIssueRequestModelValidator>(); // we were this from FluentValidation.AspNetCore but they moved it to FluentValidation.DependencyInjectionExtensions
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// the stuff above this line - configuration of the api and all the stuff inside of it.
var app = builder.Build();
// the stuff after this line. - it is how it takes requests and makes them into responses.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers(); // Use .NET Reflection to go find all the routes to create the route table.

app.Run(); // This is a "blocking" message pump.

