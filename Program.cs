using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using OpenAI;
using PatientManagementAPI.Models;
using PatientManagementAPI.Options;
using PatientManagementAPI.Services;

namespace PatientManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
     
            builder.Configuration.AddEnvironmentVariables();
            // Add Application Insights Telemetry
            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights");
            });

          

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAngular", policy => {
                    policy.WithOrigins(
                        "http://192.168.1.241:7005",
                        "http://127.0.0.1:4200",
                        "http://localhost:4200",
                        "https://pzt7znfh-4200.use.devtunnels.ms",
                        "https://agreeable-forest-077c6b40f.7.azurestaticapps.net" // Added Azure Static App origin
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(5600));
                });
            });
            //    KeyFileStorage codeStorage = new KeyFileStorage();
            //   var code = codeStorage.Read();

            /*
            builder.Services.AddSingleton(_ =>
            {
                var apiKey = builder.Configuration["OpenAI:ApiKey"];
                return new OpenAIClient(apiKey);
            });
            */

            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IPatientDetailsService, PatientDetailsService>();


            // 1. Use builder.Configuration to fetch the key safely across all environments
            string? customKey = builder.Configuration["OpenAI:ApiKey"];

            // 2. Fail fast with a clear exception message if the key is missing
            if (string.IsNullOrEmpty(customKey))
            {
                throw new InvalidOperationException("OpenAI:ApiKey configuration is missing or null.");
            }

            builder.Services.AddSingleton(_ =>
            {
                return new OpenAIClient(customKey);
            });


            //     builder.Services.AddHttpClient<OpenAiService>();
            builder.Services.AddScoped<IOpenAiService, OpenAiService>();

            // Add API services
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            

            var app = builder.Build();

            builder.Services.AddControllers();
            app.Logger.LogInformation("customKey:", customKey);


            // Configure HTTP request pipeline
            //     if (app.Environment.IsDevelopment())
            //    {
            app.UseSwagger();
            app.UseSwaggerUI();
            //   } 

            //   app.UseCors("AllowAll");
            app.UseCors("AllowAngular");
            //   app.UseHttpsRedirection();
            // app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            /*
            if (string.IsNullOrEmpty(customKey))
            {
                // 2. Intercept pipeline and return error to client
                app.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var errorResponse = new { error = "Configuration error. Required environment variable is missing." };
                    await context.Response.WriteAsJsonAsync(errorResponse);
                });
            }
            else
            {
                app.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status202Accepted;
                    context.Response.ContentType = "application/json";

                    var errorResponse = new { error = "Checking" };
                    await context.Response.WriteAsJsonAsync(customKey);
                });


            }
            */

            app.Run();
        }
    }
}