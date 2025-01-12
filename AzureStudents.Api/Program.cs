using AzureStudents.Shared.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AzureStudents.Api.Mapping;
using AzureStudents.Data.DatabaseContexts;
using AzureStudents.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AzureStudents.Api.Services;
using AzureStudents.Api.Constants;
using AzureStudents.Shared.Constants;

namespace AzureStudents.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ==================================================================================================================
        // DB Context
        // ==================================================================================================================
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(ConfigurationHelper.GetConnectionString()));

        // ==================================================================================================================
        // API Documentation
        // ==================================================================================================================

        // Swagger
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Azure Students API", Version = "v1" });

            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            option.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        // ==================================================================================================================
        // Mapping
        // ==================================================================================================================
        builder.Services.AddAutoMapper(
            typeof(AutoMapperProfile)
        );

        // ==================================================================================================================
        // Network (converters, cors, data transfers, filters)
        // ==================================================================================================================

        // Cors policy
        string corsPolicyName = "DefaultCorsPolicy";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, builder => builder
                .AllowAnyOrigin()  
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        // ==================================================================================================================
        //  Repositories
        // ==================================================================================================================
        builder.Services.AddTransient<IStudentRepository, StudentRepository>();

        // ==================================================================================================================
        // Security (authentication, authorization, identity)
        // ==================================================================================================================
        
        // Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
            options.DefaultChallengeScheme =
            options.DefaultForbidScheme =
            options.DefaultScheme =
            options.DefaultSignInScheme =
            options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["JWT:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!))
            };
        });

        // Authorization Policies
        builder.Services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(ApplicationUserPolicies.FrontEndApplicationPolicy, policy =>
                policy.RequireClaim(ApplicationUserJwtClaims.ApplicationId, ApplicationConstants.FrontEndApplicationId));
        });

        // Token service
        builder.Services.AddTransient<ITokenService, TokenService>();

        // ==================================================================================================================
        //  Seeding
        // ==================================================================================================================


        // ==================================================================================================================
        //  Final configurations
        // ==================================================================================================================

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseCors(corsPolicyName);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        // ==================================================================================================================
        // Migration
        // ==================================================================================================================
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        // ==================================================================================================================
        // Run application
        // ==================================================================================================================

        app.Run();
    }
}
