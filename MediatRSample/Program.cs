using MediatRHandler;
using MediatRHandler.Repositories;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;
using System.Threading.RateLimiting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using MediatRSample.CustomEnrichers;
using System.Configuration;
using Serilog.Sinks.MSSqlServer;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;




var builder = WebApplication.CreateBuilder(args);


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 1024;
    options.MaximumKeyLength = 1024;
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };
}) ;

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("RedisConnectionString");
});

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.RegisterRequestHandlers();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<HttpRequestAndCorrelationContextEnricher>();

Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));


builder.Host.UseSerilog((_, serviceProvider, loggerConfiguration) =>
{
    var enricher = serviceProvider.GetRequiredService<HttpRequestAndCorrelationContextEnricher>();

    loggerConfiguration.Enrich.FromLogContext()
        .Enrich.With(enricher) // Register custom enricher
        .WriteTo.Console(new JsonFormatter(), LogEventLevel.Debug)
        .WriteTo.Seq("http://localhost:5341")
        .WriteTo.MSSqlServer(
            connectionString: configuration.GetConnectionString("DefaultConnection"),
            sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true },
            restrictedToMinimumLevel: LogEventLevel.Debug,

            columnOptions: null, // You can configure the column options if needed
            appConfiguration: null) // Optional, but you can pass configuration here
        .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
            .WriteTo.File("logs\\error.txt", rollingInterval: RollingInterval.Day)
        );

    Log.Information("Hello, {Name}!", Environment.UserName);
});


// Adding Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 4;
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    options.RejectionStatusCode = 429;
});
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("Sliding", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(30);
        opt.SegmentsPerWindow = 3;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 10;
    });

    options.RejectionStatusCode = 429;
});
builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("Token", opt =>
    {
        opt.TokenLimit = 100;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 10;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        opt.TokensPerPeriod = 10; //Rate at which you want to fill
        opt.AutoReplenishment = true;
    });

    options.RejectionStatusCode = 429;
});
builder.Services.AddRateLimiter(options =>
{
    options.AddConcurrencyLimiter("Concurrency", opt =>
    {
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 10;
        opt.PermitLimit = 100;
    });

    options.RejectionStatusCode = 429;
});


builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultPolicy = new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromMilliseconds(10)
    };
    options.AddPolicy("threesecondpolicy", TimeSpan.FromSeconds(1));
    options.AddPolicy("customstatuscode", new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromMilliseconds(2),
        TimeoutStatusCode = 503
    });
    options.AddPolicy("customdelegatepolicy", new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromMilliseconds(3),
        TimeoutStatusCode = 503,
        WriteTimeoutResponse = async (HttpContext context) =>
        {
            context.Response.ContentType = "application/json";
            var errorResponse = new
            {
                error = "Request time out from custom delegate policy",
                status = 503
            };
            var jsonResponse = JsonConvert.SerializeObject(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    });
});


//Caching
//builder.Services.AddOutputCache();
//Caching
builder.Services.AddOutputCache(options =>
{
    // By default all the outputs will be cached for 30 seconds
    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromSeconds(30)));

    //Which ever endpoint using this policy that paritcular endpoint will be cached for 10 seconds
    options.AddPolicy("CacheForTenSeconds", builder =>
        builder.Expire(TimeSpan.FromSeconds(10)));

});

builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRequestTimeouts();
app.UseRateLimiter();


app.UseAuthorization();

app.MapControllers();
app.UseOutputCache();

app.Run();
 
