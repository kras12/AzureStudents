using AzureStudents.Shared.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AzureStudents.Api.Mapping;
using AzureStudents.Data.DatabaseContexts;
using AzureStudents.Data.Repositories;

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
#if DEBUG
        string corsPolicyName = "LocalHostingCorsPolicy";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
#endif

        // ==================================================================================================================
        //  Repositories
        // ==================================================================================================================
        builder.Services.AddTransient<IStudentRepository, StudentRepository>();

        // ==================================================================================================================
        // Security (authentication, authorization, identity)
        // ==================================================================================================================


        // ==================================================================================================================
        //  Seeding
        // ==================================================================================================================


        // ==================================================================================================================
        //  Final configurations
        // ==================================================================================================================

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

#if DEBUG
        app.UseCors(corsPolicyName);
#endif

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

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
