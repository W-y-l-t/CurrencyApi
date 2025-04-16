using System.Reflection;
using System.Text.Json.Serialization;
using Audit.Core;
using Audit.Http;
using Fuse8.BackendInternship.InternalApi.Contracts.Services;
using Fuse8.BackendInternship.InternalApi.DataAccess.DbContexts;
using Fuse8.BackendInternship.InternalApi.Exceptions.ExceptionFilters;
using Fuse8.BackendInternship.InternalApi.Extensions.ValidationExtensions;
using Fuse8.BackendInternship.InternalApi.Interceptors;
using Fuse8.BackendInternship.InternalApi.Middlewares;
using Fuse8.BackendInternship.InternalApi.Models.Converters;
using Fuse8.BackendInternship.InternalApi.Models.ModelBinderProviders;
using Fuse8.BackendInternship.InternalApi.Profiles;
using Fuse8.BackendInternship.InternalApi.Services;
using Fuse8.BackendInternship.InternalApi.Services.CacheWarmupServices;
using Fuse8.BackendInternship.InternalApi.Services.GrpcServices;
using Fuse8.BackendInternship.InternalApi.Settings;
using Fuse8.BackendInternship.InternalApi.Validators;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

namespace Fuse8.BackendInternship.InternalApi;

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
			cfg.AddProfile<CurrencyGrpcServiceResponseMapping>();
		});
	
		services.AddTransient<RequestLoggingMiddleware>();
		
		var connectionString = _configuration.GetConnectionString("CurrencyDb");
		services.AddDbContext<CurrencyCacheContext>(options =>
		{
			options.UseNpgsql(connectionString, npgsqlOptions =>
			{
				npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "cur");
			});
		});
		
		services.AddScoped<ICachedCurrencyApiService, CachedCurrencyApiService>();
		services.AddHostedService<CacheWarmupService>();
		services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
		
		services.AddHttpClient<ICurrencyApiService, CurrencyApiService>()
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
		
		services.AddGrpc(options =>
		{
			options.Interceptors.Add<ValidateInterceptor>();
		});
		
		services.AddControllers(options => 
		{
			options.ModelBinderProviders.Insert(0, new DateOnlyModelBinderProvider());
					
			options.Filters.Add<GlobalExceptionFilter>();
		})
		.AddJsonOptions(options =>
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
		
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();
		
		services.AddTransient<CurrencySettingsValidator>();
		services.AddTransient<ApiSettingsValidator>();
		
		services.AddOptions<ApiSettings>()
			.Bind(_configuration.GetSection("ApiSettings"))
			.ValidateFluent<ApiSettings, ApiSettingsValidator>()
			.ValidateOnStart();
		services.AddOptions<CurrencySettings>()
			.Bind(_configuration.GetSection("CurrencySettings"))
			.ValidateFluent<CurrencySettings, CurrencySettingsValidator>()
			.ValidateOnStart();
		
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
			var db = scope.ServiceProvider.GetRequiredService<CurrencyCacheContext>();
			db.Database.Migrate();
		}

		app.UseRouting();
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
			endpoints.MapGrpcService<CurrencyGrpcService>();
		});
	}
}