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

            await app.ApplyMigrationsAsync();
            await app.SeedDataAsync();
        }

        app.UseCors("AllowFrontend");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    private static async Task<IApplicationBuilder> ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }

        return app;
    }

    private static async Task<IApplicationBuilder> SeedDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        await DataSeeder.SeedAsync(serviceProvider);

        return app;
    }
}