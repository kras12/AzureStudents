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
using AzureStudents.Api.Configuration;
using AzureKeyVaultConfigurationSource = AzureStudents.Api.Configuration.AzureKeyVaultSecretsConfigurationSource;

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

        // Azure Key Vault
        if (builder.Environment.IsProduction())
        {
            builder.Configuration.Sources.Add(new AzureKeyVaultConfigurationSource(new Uri(builder.Configuration["KeyVault:VaultUrl"]!), new()
            {
                builder.Configuration["KeyVault:JwtSigningKeySecretName"]!
            }));
        }

        // Authentication
        var jwtSettingsSection = builder.Configuration.GetSection("Jwt");
        builder.Services.Configure<JwtSettings>(jwtSettingsSection);

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
            var jwtSettings = jwtSettingsSection.Get<JwtSettings>()!;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SigningKey))
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
