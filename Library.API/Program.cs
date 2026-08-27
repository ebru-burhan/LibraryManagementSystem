using Library.Business.Extensions;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);


// Business katmanındaki servisler
builder.Services.AddBusinessServices(builder.Configuration);

builder.Services.AddControllers();


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library API",
        Version = "v1"
    });

    // JWT tanımı
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header kullanır. Örnek: Bearer {token}",

        Name = "Authorization",

        In = ParameterLocation.Header,

        Type = SecuritySchemeType.Http,

        Scheme = "Bearer",

        BearerFormat = "JWT"
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.UseRouting();


// CORS
app.UseCors("ReactPolicy");


app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();


// Uygulama ayağa kalkarken otomatik seed mekanizmasını tetikler
Library.Business.SeedData.DatabaseSeed.Seed(app);

app.Run();
