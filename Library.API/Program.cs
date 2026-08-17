using Library.Business.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Katman servislerinin kaydı
builder.Services.AddBusinessServices(builder.Configuration);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 1.Kimlik Kartını Oku
app.UseAuthentication();
// 2. Yetkiyi Kontrol Et
app.UseAuthorization();  

app.MapControllers();

app.Run();
