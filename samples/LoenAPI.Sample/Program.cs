using LoenAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Configuration.GetValue<string>("Loen:Env") ?? "dev";

builder.Services.AddLoenServices(builder.Configuration);

var app = builder.Build();

app = app.UseLoenAPI(env == "dev");

app.Run();
