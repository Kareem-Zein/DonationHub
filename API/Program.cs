using API;
using API.Extensions;
using API.Services;
using Application;
using Application.Abstractions.Interfaces.Services;
using Infrastructure.Data.Development;
using Infrastructure.Data.Seeders;
using Infrastructure.Extensions;
using Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var http = context.HttpContext;
        var problem = context.ProblemDetails;

        problem.Extensions.TryAdd("traceId", http.TraceIdentifier);
        problem.Instance ??= $"{http.Request.Method} {http.Request.Path}";
    };
});

builder.Services.PostConfigure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var factory = context.HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        var errors = context.ModelState
            .Where(kv => kv.Value?.Errors.Count > 0)
            .SelectMany(kv => kv.Value!.Errors.Select(e => new
            {
                code = e.Exception is not null
                    ? $"Binding.{kv.Key}"
                    : $"Validation.{kv.Key}",
                message = string.IsNullOrWhiteSpace(e.ErrorMessage)
                    ? e.Exception?.Message ?? "Invalid value."
                    : e.ErrorMessage
            }))
            .ToArray();

        var distinctCodes = errors.Select(e => e.code).Distinct().ToArray();
        var code = distinctCodes.Length == 1
            ? distinctCodes[0]
            : "Validation.Multiple";

        var problem = factory.CreateProblemDetails(
            context.HttpContext,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Validation Failed",
            detail: errors.Length == 1
                ? errors[0].message
                : "One or more validation errors occurred.",
            type: "https://httpstatuses.io/400");

        problem.Extensions["code"] = code;
        problem.Extensions["errors"] = errors;

        return new BadRequestObjectResult(problem);
    };
});

builder.Services
    .AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddInfrastructure(builder.Configuration)
    .RegisterApplication();

builder.Services.AddApiAuthentication();

builder.Services.AddControllers();

builder.Services.AddOpenApi("v1", options => {
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    // Used only for development to update database.
    // Do not use it in your applications.
    await DbMigrator.MigrateAsync(app.Services);
}

await RoleSeeder.SeedAsync(app.Services);

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


await app.RunAsync();
