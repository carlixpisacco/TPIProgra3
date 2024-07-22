using ClothingStore.Application.IRepositories;
using ClothingStore.Application.IServices;
using ClothingStore.Application.Services;
using ClothingStore.Infrastructure.Context;
using ClothingStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("ShopApiBearerAuth", new OpenApiSecurityScheme() //Esto va a permitir usar swagger con el token.
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "agregar token"
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ShopApiBearerAuth" } //Tiene que coincidir con el id seteado arriba en la definición
                }, new List<string>() }
    });
});

//context
builder.Services.AddDbContext<ClothingStoreDbContext>(options =>options.UseSqlite(builder.Configuration.GetConnectionString("ClothingStoreDbConnection")));
//automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"])),
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                // Controlar la respuesta en caso de que la autorización falle
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var result = JsonContent.Create(new { message = "No está autorizado para esta acción. Inicie sesión" });
                await result.CopyToAsync(context.Response.Body);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var result = JsonContent.Create(new { message = "No tiene permiso para acceder a este recurso." });
                await result.CopyToAsync(context.Response.Body);
            }
        };
    });
//inyección repositorios
builder.Services.AddScoped< IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

//inyección servicios
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
