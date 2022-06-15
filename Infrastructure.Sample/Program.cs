using DN.WebApi.Infrastructure;
using Infrastructure.sample;
using Infrastructure.Sample;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
Log.Information("Server Booting Up...");

try
{
	var builder = WebApplication.CreateBuilder(args);

	// Add services to the container.
	builder.Services.AddControllers(options => { options.EnableEndpointRouting = false; });

	builder.Host.AddConfigurations();

	//Configuring infra using shared project
	builder.Services.AddInfrastructure(builder.Configuration);

	//Configurings specific to api
	builder.Services.AddApplicationServices(builder.Configuration);

	builder.Host.UseSerilog();

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}
	else
	{
		app.UseExceptionHandler("/Error");
		app.UseHsts();
	}

	app.UseInfrastructure(builder.Configuration);
	app.UseAuthorization();
	app.UseHttpsRedirection();

	app.MapFallbackToFile("index.html");

	app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
	Log.Fatal(ex, "Unhandled exception");
}
finally
{
	Log.Information("Server Shutting down...");
	Log.CloseAndFlush();
}

