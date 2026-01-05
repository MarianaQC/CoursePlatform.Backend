using CoursePlatform.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

await app.UseApplicationMiddleware(app.Environment);

app.MapControllers();

app.Run();