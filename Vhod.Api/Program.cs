using Microsoft.EntityFrameworkCore;
using Vhod.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TradeContext>(o =>
    o.UseSqlServer("Server=.;Database=Trade2;Trusted_Connection=True;TrustServerCertificate=True"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseExceptionHandler(e => e.Run(async ctx =>
{
    ctx.Response.StatusCode = 500;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { error = "Внутренняя ошибка сервера" });
}));

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.MapControllers();
app.Run();