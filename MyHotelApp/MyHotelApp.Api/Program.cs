using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyHotelApp.Infrastructure.Data;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Infrastructure.Repositories;
using MyHotelApp.Infrastructure.Services;
using MyHotelApp.Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyHotelApp.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

// builder.Services.AddOpenApi(); // 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My Hotel API", Version = "v1" });

    // 1. Define the Security Scheme (Tell Swagger we use JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // 2. Add the Security Requirement (Apply the scheme to endpoints)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)
            )
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHomeService, HomeService>();

// Email Service Configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  
    // app.MapOpenApi(); 
    app.UseSwagger();       
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

// Seeder Logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var context = services.GetRequiredService<AppDbContext>();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred seeding the DB: {ex.Message}");
    }
}

app.Run();