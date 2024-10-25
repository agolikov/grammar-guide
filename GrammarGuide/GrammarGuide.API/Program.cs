using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotnetGeminiSDK;
using GrammarGuide.API.Validation;
using GrammarGuide.ContentBuilder;
using GrammarGuide.ContentBuilder.LLMClients;
using GrammarGuide.ContentBuilder.Prompts;
using GrammarGuide.ContentBuilder.Services;
using GrammarGuide.Services;
using GrammarGuide.Services.Entities;
using GrammarGuide.Services.Services;
using GrammarGuide.Services.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection(nameof(DbSettings)));
builder.Services.Configure<BlobSettings>(builder.Configuration.GetSection(nameof(BlobSettings)));
builder.Services.Configure<SupportedLanguages>(builder.Configuration.GetSection(nameof(SupportedLanguages)));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var settings = new AppSettings();
builder.Configuration.GetSection("AppSettings").Bind(settings);

builder.Services.AddGeminiClient(config =>
{
    config.ApiKey = settings.GeminiApiKey;
});

builder.Services.AddSignalR(); 

builder.Services.AddTransient<IApiClient, ApiClient>();
builder.Services.AddTransient<ContentGenerator>();
builder.Services.AddTransient<PromptExecutor>();
builder.Services.AddScoped<PromptBuilder>();
builder.Services.AddTransient<IBlobService, BlobService>();
builder.Services.AddTransient<LanguageValidator>();
builder.Services.AddTransient<IPromptDataProvider, PromptDataProvider>();
builder.Services.AddTransient<IBlobGenerator, BlobGenerator>();

builder.Services.AddSingleton<IBlobService, BlobService>();
builder.Services.AddSingleton<IBlobGenerator, BlobGenerator>();

builder.Services.AddTransient<ICache, MongoCache>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    var db = client.GetDatabase(settings.DatabaseName);
    return new MongoCache(settings.LlmCacheCollectionName, db);
});

builder.Services.AddTransient<IAppDataService, AppDataService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DbSettings>>().Value;
    var supportedLanguages = sp.GetRequiredService<IOptions<SupportedLanguages>>();
    var client = sp.GetRequiredService<IMongoClient>();
    var cache = sp.GetRequiredService<ICache>();
    var blobs = sp.GetRequiredService<IBlobService>();
    var gen = sp.GetRequiredService<ContentGenerator>();
    var blobGen = sp.GetRequiredService<IBlobGenerator>();
    var db = client.GetDatabase(settings.DatabaseName);
    var guideCollection = db.GetCollection<Guide>(settings.GuidesCollectionName);
    return new AppDataService(gen, guideCollection, cache, blobs, blobGen, supportedLanguages);
});

builder.Services.AddTransient<IAdminService, AdminService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    var contentGenerator = sp.GetRequiredService<ContentGenerator>();
    var blobs = sp.GetRequiredService<IBlobService>();
    var blobGen = sp.GetRequiredService<IBlobGenerator>();
    var db = client.GetDatabase(settings.DatabaseName);
    var cache = sp.GetRequiredService<ICache>();
    return new AdminService(blobs, db, settings.GuidesCollectionName, contentGenerator, cache, blobGen);
});


builder.Services.AddTransient<IUserService, UserService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    var db = client.GetDatabase(settings.DatabaseName);
    return new UserService(settings.UsersCollectionName, db);
});

builder.Services.AddTransient<IMongoDatabase>(sp =>
{
     var settings = sp.GetRequiredService<IOptions<DbSettings>>().Value;
     var client = sp.GetRequiredService<IMongoClient>();
     return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Language Guide API", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "x-api-key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme",
        Description = "Input your API key to access this API",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                Scheme = "ApiKeyScheme",
                Name = "ApiKey",
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use((context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization, x-api-key");
        context.Response.Headers.Append("Access-Control-Max-Age", "86400");
        
        context.Response.StatusCode = 200;
        return Task.CompletedTask;
    }

    return next();
});

if (!app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        if (!context.Request.Headers.TryGetValue("x-api-key", out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }

        if (!extractedApiKey.Equals(settings.ApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client");
            return;
        }

        await next();
    });
}
app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAnyOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "ai-app.json");

app.Run();