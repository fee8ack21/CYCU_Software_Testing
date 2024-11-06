using App.Common.Models.MapperProfiles;
using App.PL.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;
using Serilog.Exceptions;
using App.PL;

// Init Logger
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Debug()
    .WriteTo.Console()
    .Enrich.WithProperty("Environment", environment!)
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Starting up.");

    var builder = WebApplication.CreateBuilder(args);
    var connectionString = builder.Configuration.GetConnectionString("CYCUSoftwareTesting")!;

    var jwtConfig = builder.ConfigureConfigs();

    // The builder.Host.UseSerilog() call will redirect all log events through your Serilog pipeline.
    builder.Host.UseSerilog();

    builder.Services.AddSwagger();
    builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    }));
    builder.Services.AddServices();
    builder.ConfigureDatabase();
    builder.Services.ConfigureIdentity(jwtConfig);

    // https://learn.microsoft.com/zh-tw/dotnet/api/system.text.json.serialization.jsonstringenumconverter?view=net-6.0
    // �N�C�|�ȻP�r��ۤ��ഫ�C
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

    // AutoMapper - �[�J App.Common
    builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(AccountMapperProfile)));

    // �O����֨�
    builder.Services.AddMemoryCache();

    // SignalR
    builder.Services.AddSignalR();

    var app = builder.Build();

    app.InitDatabaseAndCache();

    app.UseSwaggerService();

    // �����C�@�� Request ��T
    app.UseSerilogRequestLogging();

    // ������~�B�z
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseCors("CorsPolicy");

    app.UseHttpsRedirection();

    // �K�[�{�һP���v������
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception.");
}
finally
{
    Log.Information("Shut down complete.");
    Log.CloseAndFlush();
}