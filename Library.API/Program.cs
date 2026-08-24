using Library.Business.Extensions;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Katman servislerinin kaydı
builder.Services.AddBusinessServices(builder.Configuration);

// Controller'ları sisteme ekle
builder.Services.AddControllers();

// SWAGGER SERVİSLERİ VE JWT KİLİT MEKANİZMASI // nalet swagger curl ile terminalde hallettim // TODO: bi ara swagger ile ilgili şeyleri sil yok et.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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