using CoursePlatform.Api.Middleware;
using CoursePlatform.Infrastructure.Data;
using CoursePlatform.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> UseApplicationMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Course Platform API v1");
                options.RoutePrefix = "swagger";
            });
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Course Platform API v1");
                options.RoutePrefix = "swagger";
            });
        }

        await app.ApplyMigrationsAsync();
        await app.SeedDataAsync();

        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    private static async Task<IApplicationBuilder> ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "Error al aplicar migraciones");
        }

        return app;
    }

    private static async Task<IApplicationBuilder> SeedDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        try
        {
            await DataSeeder.SeedAsync(serviceProvider);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "Error al ejecutar el seeder");
        }

        return app;
    }
}