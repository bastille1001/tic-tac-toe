using EmployCity.TicTacToe.API.Context;
using EmployCity.TicTacToe.API.Services;
using EmployCity.TicTacToe.API.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<StartGameRequestValidator>(ServiceLifetime.Transient);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.Configure<GameStateDatabaseSettings>(configuration.GetSection("GameStateDatabase"));

builder.Services.AddSingleton<GameService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.MapControllers();

app.Run();
