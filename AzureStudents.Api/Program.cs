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

namespace AzureStudents.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ==================================================================================================================
        // Setup temporary logging capabilities to support troubleshooting in the cloud. 
        // ==================================================================================================================

        using var loggerFactory = LoggerFactory.Create(config =>
        {
            config.AddConsole();
            config.AddDebug();
        });

        // Flag to toggle logging
        bool enableLogging = true;
        ILogger? logger = enableLogging ? loggerFactory.CreateLogger<Program>() : null;

        // ==================================================================================================================
        // Import external configuration
        // ==================================================================================================================
        if (builder.Environment.IsProduction())
        {
            var keyVaultSecretsProvider = new AzureKeyVaultSecretsProvider(new Uri(builder.Configuration["KeyVault:VaultUrl"]!), logger);

            builder.Configuration.AddInMemoryCollection(keyVaultSecretsProvider.GetSecrets(
                new List<string>()
                {
                    builder.Configuration["KeyVault:JwtSigningKeySecretName"]!,
                    builder.Configuration["KeyVault:AzureStudentsDatabaseConnectionStringSecretName"]!
                })
            );            
        }

        // Configure JWT settings
        var jwtSettingsSection = builder.Configuration.GetSection("Jwt");
        builder.Services.Configure<JwtSettings>(jwtSettingsSection);


        // Configure database connection strings
        var databaseConnectionStringsSection = builder.Configuration.GetSection("ConnectionStrings");
        builder.Services.Configure<DatabaseConnectionStringsSettings>(databaseConnectionStringsSection);
        var databaseConnectionStringsSettings = databaseConnectionStringsSection.Get<DatabaseConnectionStringsSettings>()!;

        if (string.IsNullOrEmpty(databaseConnectionStringsSettings.AzureStudentsDatabaseConnectionString))
        {
            const string exitMessage = "No database connection string is configured. Shutting down application.";
            logger?.LogError(exitMessage);
            throw new InvalidOperationException(exitMessage);
        }

        // ==================================================================================================================
        // Database (DB Context, etc)
        // ==================================================================================================================
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(databaseConnectionStringsSettings.AzureStudentsDatabaseConnectionString));

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

        // Authorization service
        builder.Services.AddTransient<IUserAuthorizationService,  UserAuthorizationService>();

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

            // We currently have a database that goes into sleep during inactity to save money during development. 
            // So we need to wake the database up. 
            var delay = TimeSpan.FromSeconds(3);
            var maxTime = TimeSpan.FromMinutes(10);
            var startTime = DateTime.Now;

            while (true)
            {
                if (DateTime.Now - startTime > maxTime)
                {
                    throw new Exception("Failed to wake up the database.");
                }

                logger?.LogInformation("Checking status of the database.");

                if (context.Database.CanConnect())
                {
                    logger?.LogInformation("Database is ready.");
                    break;
                }

                logger?.LogInformation("Waiting for the database to wake up.");

                Thread.Sleep(delay);
            }

            context.Database.Migrate();
        }

        // ==================================================================================================================
        // Run application
        // ==================================================================================================================

        app.Run();
    }
}
