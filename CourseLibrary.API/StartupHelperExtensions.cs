﻿using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

namespace CourseLibrary.API;

internal static class StartupHelperExtensions {
    // Add services to the container
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder) {
        builder.Services.AddControllers(options => {
            options.ReturnHttpNotAcceptable = true;
        }).AddNewtonsoftJson().AddNewtonsoftJson(setupAction => {
            setupAction.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }).AddXmlDataContractSerializerFormatters().ConfigureApiBehaviorOptions(setupAction => {
            setupAction.InvalidModelStateResponseFactory = context => {
                // create a validation problem details object
                var problemDetailsFactory = context.HttpContext.RequestServices
                    .GetRequiredService<ProblemDetailsFactory>();

                var validationProblemDetails = problemDetailsFactory
                    .CreateValidationProblemDetails(
                        context.HttpContext,
                        context.ModelState);

                // add additional info not added by default
                validationProblemDetails.Detail =
                    "See the errors field for details.";
                validationProblemDetails.Instance =
                    context.HttpContext.Request.Path;

                // report invalid model state responses as validation issues
                validationProblemDetails.Type =
                    "https://courselibrary.com/modelvalidationproblem";
                validationProblemDetails.Status =
                    StatusCodes.Status422UnprocessableEntity;
                validationProblemDetails.Title =
                    "One or more validation errors occurred.";

                return new UnprocessableEntityObjectResult(
                    validationProblemDetails) {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });

        builder.Services.AddTransient<IPropertyMappingService, PropertyMappingService>();


        builder.Services.AddScoped<ICourseLibraryRepository,
            CourseLibraryRepository>();

        builder.Services.AddDbContext<CourseLibraryContext>(options => {
            options.UseSqlite(@"Data Source=library.db");
        });

        builder.Services.AddAutoMapper(
            AppDomain.CurrentDomain.GetAssemblies());

        return builder.Build();
    }

    // Configure the request/response pipelien
    public static WebApplication ConfigurePipeline(this WebApplication app) {
        if (app.Environment.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }
        else {
            app.UseExceptionHandler(appBuilder => {
                appBuilder.Run(async context => {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Something went wrong. Try again later.");
                });
            });
        }

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    public static async Task ResetDatabaseAsync(this WebApplication app) {
        using (var scope = app.Services.CreateScope()) {
            try {
                var context = scope.ServiceProvider.GetService<CourseLibraryContext>();
                if (context != null) {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex) {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
    }
}