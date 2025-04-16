using System.Reflection;
using System.Text.Json.Serialization;
using Audit.Core;
using Audit.Http;
using Fuse8.BackendInternship.InternalApi.Grpc.CurrencyProto;
using Fuse8.BackendInternship.PublicApi.Contracts.Services;
using Fuse8.BackendInternship.PublicApi.DataAccess.DbContexts;
using Fuse8.BackendInternship.PublicApi.Exceptions.ExceptionFilters;
using Fuse8.BackendInternship.PublicApi.Extensions.ValidationExtensions;
using Fuse8.BackendInternship.PublicApi.Middlewares;
using Fuse8.BackendInternship.PublicApi.Models.Converters;
using Fuse8.BackendInternship.PublicApi.Models.ModelBinderProviders;
using Fuse8.BackendInternship.PublicApi.Profiles;
using Fuse8.BackendInternship.PublicApi.Services;
using Fuse8.BackendInternship.PublicApi.Settings;
using Fuse8.BackendInternship.PublicApi.Validators;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

namespace Fuse8.BackendInternship.PublicApi;

public class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddAutoMapper(cfg =>
		{
			cfg.AddProfile<CurrencyGrpcTypesMapping>();
		});
		
		services.AddTransient<RequestLoggingMiddleware>();
		
		var connectionString = _configuration.GetConnectionString("CurrencyDb");
		services.AddDbContext<FavoriteCurrencyContext>(options =>
		{
			options.UseNpgsql(connectionString, npgsqlOptions =>
			{
				npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "user");
			});
		});

		services
			.AddGrpcClient<CurrencyService.CurrencyServiceClient>(options =>
			{
				options.Address = new Uri(_configuration["InternalApi:GrpcUrl"] ?? "https://localhost:5889");
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = 
					HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			})
			.AddPolicyHandler(
				HttpPolicyExtensions
					.HandleTransientHttpError()
					.WaitAndRetryAsync(
						retryCount: 3,
						sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) - 1)))
			.AddAuditHandler(
				audit => audit
					.IncludeRequestBody()
					.IncludeResponseBody()
					.IncludeContentHeaders()
					.IncludeRequestHeaders()
					.IncludeResponseHeaders());

		services.AddScoped<ICurrencyApiService, CurrencyApiService>();
		services.AddScoped<IFavoriteCurrencyApiService, FavoriteCurrencyApiService>();
		
		services
			.AddControllers(
				options =>
				{
					options.ModelBinderProviders.Insert(0, new DateOnlyModelBinderProvider());
					
					options.Filters.Add<GlobalExceptionFilter>();
				})
			.AddJsonOptions(
				options =>
				{
					options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				});

		services.AddSwaggerGen(c =>
		{
			var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

			c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
		});
		
		services.AddTransient<CurrencySettingsValidator>();
		
		services.AddOptions<CurrencySettings>()
			.Bind(_configuration.GetSection("CurrencySettings"))
			.ValidateFluent<CurrencySettings, CurrencySettingsValidator>()
			.ValidateOnStart();

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();
		
		Configuration.Setup().UseSerilog();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency API v1");
				options.RoutePrefix = string.Empty;
			});
		}
		
		app.ApplicationServices.GetRequiredService<AutoMapper.IConfigurationProvider>().AssertConfigurationIsValid();

		app.UseMiddleware<RequestLoggingMiddleware>();
		
		using (var scope = app.ApplicationServices.CreateScope())
		{
			var db = scope.ServiceProvider.GetRequiredService<FavoriteCurrencyContext>();
			db.Database.Migrate();
		}

		app.UseRouting();
		app.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}