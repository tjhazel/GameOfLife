using GameOfLife.Domain;
using GameOfLife.Domain.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace GameOfLife.Api;

public static class AppBuilder
{
   public static WebApplication BuildApp(WebApplicationBuilder builder, bool isDebug)
   {
      AddSwagger(builder);
      AddCors(builder);
      AddLogging(builder);

      RegisterHttpClients(builder);
      RegisterDependencies(builder);
      RegisterControllers(builder);

      return builder.Build();
   }

   static void AddSwagger(WebApplicationBuilder builder)
   {
      builder.Services.AddSwaggerGen(c =>
      {
         c.SwaggerDoc("v1", new OpenApiInfo
         {
            Title = "GameOfLife API",
            Version = "v1"
         });
      });
   }

   static void AddCors(WebApplicationBuilder builder)
   {
      //Set up cors
      builder.Services.AddCors(options =>
      {
         options.AddPolicy("AllowCors",
             policyBuilder => policyBuilder
                 .WithOrigins("http://localhost:53006",
                     "http://localhost:44346")
                 .SetIsOriginAllowedToAllowWildcardSubdomains()
                 .AllowAnyMethod()
                 .AllowCredentials()
                 .AllowAnyHeader());
      });
   }

   static void AddLogging(WebApplicationBuilder builder)
   {
      builder.Services.AddLogging(logging =>
      {
         logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
         logging.AddFilter("System", LogLevel.Warning);
         logging.AddConsole();
         logging.AddDebug();
      });
   }

   static void RegisterHttpClients(WebApplicationBuilder builder)
   {
      builder.Services.AddHttpClient();
   }


   /// <summary>
   /// single location to populate the core IoC container.
   /// </summary>
   /// <param name="services"></param>
   static void RegisterDependencies(WebApplicationBuilder builder)
   {
      //local dependencies
      builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      
      //project dependencies
      builder.Services.AddTransient<IBoardStateService, BoardStateService>();
      builder.Services.AddTransient<IGridService, GridService>();
      builder.Services.AddTransient<Game>();
   }

   static void RegisterControllers(WebApplicationBuilder builder)
   {
      builder.Services
         .AddControllers()
         .AddJsonOptions(options =>
         {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;

         });
   }
}
