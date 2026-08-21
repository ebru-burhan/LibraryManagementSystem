using Library.Business.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Katman servislerinin kaydı
builder.Services.AddBusinessServices(builder.Configuration);

// Controller'ları (Garsonları) sisteme ekle
builder.Services.AddControllers();

// .NET'in yeni nesil OpenAPI desteği (Senin kodunda gelen)
builder.Services.AddOpenApi();

// 1. SWAGGER SERVİSLERİNİ EKLİYORUZ (Yeşil görsel arayüz için)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Arka plandaki API haritası

    // İŞTE EKSİK OLAN YER BURASIYDI! (Görsel arayüzü ayağa kaldırıyoruz)
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 1. Kimlik Kartını Oku
app.UseAuthentication();
// 2. Yetkiyi Kontrol Et
app.UseAuthorization();

app.MapControllers();

app.Run();