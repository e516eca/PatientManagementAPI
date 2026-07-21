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
            /*
            // Configure FHIR Server options with validation
            builder.Services
                .AddOptions<FhirServerOptions>()
                .Bind(builder.Configuration.GetSection(FhirServerOptions.SectionName))
                .Validate(
                    options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri)
                               && uri.Scheme == Uri.UriSchemeHttps,
                    "FhirServer:BaseUrl must be an absolute HTTPS URL.")
                .Validate(
                    options => !string.IsNullOrWhiteSpace(options.BearerToken),
                    "FhirServer:BearerToken is required.")
                .Validate(
                    options => options.TimeoutSeconds is >= 5 and <= 300,
                    "FhirServer:TimeoutSeconds must be between 5 and 300.")
                .ValidateOnStart();
            */

            // Add services to the container.
            /*
            builder.Services.AddCors(opt => opt.AddPolicy("AllowAll",
            opt => opt.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin())
            );*/

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://192.168.1.241:7005", "http://127.0.0.1:4200","http://localhost:4200", "https://pzt7znfh-4200.use.devtunnels.ms") // Your Angular URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials() // Required if Angular sends cookies/tokens
                     // Crucial step for the loopback error:
                          .SetPreflightMaxAge(TimeSpan.FromSeconds(5600));
                });

              
            });
            //    KeyFileStorage codeStorage = new KeyFileStorage();
            //   var code = codeStorage.Read();

            var customKey = Environment.GetEnvironmentVariable("DbSettings__ConnectionString");

            builder.Services.AddScoped<IPatientService, PatientService > ();
            builder.Services.AddScoped<IPatientDetailsService, PatientDetailsService>();

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

            // Configure HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //   app.UseCors("AllowAll");
            app.UseCors("AllowAngular");
         //   app.UseHttpsRedirection();
           // app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
         
            app.Run();
        }
    }
}
