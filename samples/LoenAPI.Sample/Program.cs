using LoenAPI.Extensions;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLoenServices(builder.Configuration, DbType.PostgreSQL, true);

var app = builder.Build();

app = app.UseLoenAPI(true);

app.Run();
