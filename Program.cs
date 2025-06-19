using Microsoft.EntityFrameworkCore;
//using Minimart_Api.Data;
using Minimart_Api.Repositories;
using Minimart_Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using Minimart_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Authentication_and_Authorization_Api.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Minimart_Api.Mappings;
using Minimart_Api.Services.RabbitMQ;
using Minimart_Api.Services.NotificationService;
using OpenSearch.Client;
using OpenSearch.Net;
using Microsoft.Extensions.Options;
using Minimart_Api.Services.OpenSearchService;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Amazon.Runtime;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Minimart_Api.Services.SystemSecurity;
using Minimart_Api.Repositories.SystemSecurityRepo;
using Minimart_Api.Services.SystemMerchantService;
using Minimart_Api.Repositories.SystemMerchantsRepository;
using Minimart_Api.Services.ProductService;
using Minimart_Api.Repositories.ProductRepository;
using Minimart_Api.Services.CategoriesService;
using Minimart_Api.Repositories.CategoriesRepository;
using Minimart_Api.Services.SignalR;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.DTOS.Payments;
using Minimart_Api.DTOS.Authorization;
using Minimart_Api.Repositories.Authorization;
//using Minimart_Api.Repositories.Merchants;
using Minimart_Api.Repositories.Order;
using Minimart_Api.Repositories.Reports;
using Minimart_Api.Repositories.Search;
using Minimart_Api.Services.OrderService;
using Minimart_Api.Services.ReportService;
using Minimart_Api.Services.SearchService;
using Minimart_Api.Services.OrderService.OrderService;
using Minimart_Api.Services.SearchService.SearchService;
using Minimart_Api.Services.ReportService.ReportService;
using Minimart_Api.Data;
using StackExchange.Redis;
using Minimart_Api.Services.EmailServices;
using Minimart_Api.Services.Features;
using Minimart_Api.Repositories.Features;
using Minimart_Api.Services.Cart;
using Minimart_Api.Repositories.Cart;
using Minimart_Api.Services.SimilarProducts;
using Minimart_Api.Services.Recommedation;
using Minimart_Api.Repositories.Recommendation;
using Minimart_Api.Services.Deliveries;
using Minimart_Api.Repositories.Deliveries;
using Minimart_Api.Services.Address;
using Minimart_Api.Repositories.AddressesRepo;
using Microsoft.AspNetCore.HttpOverrides;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMyService, MyService>();
builder.Services.AddScoped<IRepository, MyRepository>();
builder.Services.AddScoped<IOrderService, OrderServices>();
builder.Services.AddScoped<IorderRepository, OrderRepository>();

//builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();

builder.Services.AddScoped<ISearchService, SearchServices>();
builder.Services.AddScoped<ISearchRepo,SearchRepo>();

//builder.Services.AddScoped<IMerchantService, MerchantService>();
//builder.Services.AddScoped<IMerchantRepo, MerchantRepo>();

builder.Services.AddScoped<IReportService, ReportServices>();
builder.Services.AddScoped<IReportRepo, ReportRepo>();

builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<IFeatureRepo, FeatureRepo>();


builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartRepo, CartRepo>();

builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IDeliveriesRepo, DeliveriesRepo>();

builder.Services.AddScoped<IAddress, AddressService>();
builder.Services.AddScoped<IAddressRepo, AddressRepo>();

builder.Services.AddScoped<ISimilarProductsService, SimilarProductsService>();

builder.Services.AddScoped<IAuthentication, AuthenticationService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<ISystemSecurity, SystemSecurity>();
builder.Services.AddScoped<ISystemSecurityRepo, SystemSecurityRepo>();


builder.Services.AddScoped<IRecomedationService, RecommendationService>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ISystemMerchants, MerchantsService>();
builder.Services.AddScoped<ISystemMerchantRepo, SystemMerchantRepo>();

builder.Services.AddScoped<ICategoriesService, CategoriesNewService>();
builder.Services.AddScoped<ICategoryRepos, CategoryRepos>();

builder.Services.AddScoped<IOrderEventPublisher, OrderEventPublisher>();
builder.Services.AddHostedService<OrderEventConsumer>();

builder.Services.AddScoped<INotfication, NotificationService>();

//builder.Services.AddScoped<IOpenSearchService, OpenSearchService>();


builder.Services.AddScoped<CoreLibraries>();
builder.Services.AddScoped<OrderMapper>();

builder.Services.AddScoped<BrevoEmailService>();

builder.Services.AddMemoryCache();

// Register RabbitMQ connection
builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

// Optional: Add health check
//builder.Services.AddHealthChecks()
//    .AddRabbitMQ(provider =>
//        provider.GetRequiredService<IRabbitMqConnection>().Connection);



//configure Serilog

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

//configur Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug(); 
builder.Services.AddDbContext<MinimartDBContext>(options =>
{
    // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    //.LogTo(message => Log.Information(message), Microsoft.Extensions.Logging.LogLevel.Information) // Log to Serilog
    //.EnableSensitiveDataLogging(); // Enable logging of sensitive data (like parameters)

    //options.UseNpgsql(builder.Configuration.GetConnectionString("PostgressConnection"))
    //.LogTo(error => Log.Error(error))
    //.EnableSensitiveDataLogging();

    options.UseNpgsql(
    builder.Configuration.GetConnectionString("PostgressConnection"),
    npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        );
    })
    .LogTo(error => Log.Error(error))
    .EnableSensitiveDataLogging();

},
ServiceLifetime.Scoped); // Scoped lifetime for the DbContext

//Development
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();

//    var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL")
//                  ?? builder.Configuration.GetConnectionString("Redis");

//    if (string.IsNullOrWhiteSpace(redisUrl))
//        throw new InvalidOperationException("Redis connection string is missing.");

//    logger.LogInformation("Connecting to Redis via URI: {RedisUrl}", redisUrl);

//    try
//    {
//        // Parse the URL into ConfigurationOptions for better control
//        var config = ConfigurationOptions.Parse(redisUrl);

//        // Recommended settings for Upstash
//        config.AbortOnConnectFail = false; // Retry on failure
//        config.ConnectTimeout = 10000;    // 10 seconds
//        config.SyncTimeout = 5000;        // 5 seconds  
//        config.Ssl = true;                 // Force SSL (Upstash requires it)
//        config.DefaultDatabase = 0;        // Explicitly set DB if needed

//        var connection = ConnectionMultiplexer.Connect(config);

//        connection.ConnectionFailed += (_, args) =>
//            logger.LogError(args.Exception, "Redis connection failed to {Endpoint}", args.EndPoint);

//        connection.ConnectionRestored += (_, args) =>
//            logger.LogInformation("Redis connection restored to {Endpoint}", args.EndPoint);

//        logger.LogInformation("Redis connected successfully.");
//        return connection;
//    }
//    catch (Exception ex)
//    {
//        logger.LogCritical(ex, "Failed to connect to Redis.");
//        throw;
//    }
//});



//Production
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();

    // Get connection string from Render environment variable
    var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL")
                   ?? builder.Configuration.GetConnectionString("Redis")
                   ?? throw new InvalidOperationException("Missing REDIS_URL");

    logger.LogInformation("Connecting to Redis: {Host}", redisUrl);

    try
    {

        // Explicit configuration to avoid port parsing bugs
        var config = new ConfigurationOptions
        {
            // Manually specify endpoint to prevent 6379:6380 issue
            EndPoints = { "loved-airedale-34854.upstash.io:6379" },

            // Extract password from URL (or use directly)
            Password = redisUrl.Split('@')[0].Split(':')[2],

            // Critical for Upstash
            Ssl = true,
            AbortOnConnectFail = false,
            ConnectTimeout = 15000, // 15 seconds
            SyncTimeout = 5000      // 5 seconds
        };

        var connection = ConnectionMultiplexer.Connect(config);

        // Attach event handlers for reliability
        connection.ConnectionFailed += (_, e) =>
            logger.LogError(e.Exception, "Redis connection failed");

        connection.ConnectionRestored += (_, _) =>
            logger.LogInformation("Redis connection restored");

        return connection;
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Failed to connect to Redis");
        throw;
    }
});




// 2. Redis (TLS-enabled)
//builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
//    ConnectionMultiplexer.Connect(
//        Environment.GetEnvironmentVariable("REDIS_URL") ??
//        builder.Configuration.GetConnectionString("Redis"),
//        options => options.Ssl = true
//    ));

builder.Services.AddScoped(provider =>
{
    var redis = provider.GetRequiredService<IConnectionMultiplexer>();
    return redis.GetDatabase();
});
builder.Services.AddScoped<MpesaSandBox>();

//builder.Services.AddHostedService<SyncProductsToOpenSearch>();


builder.Services.Configure<MpesaSandBox>(builder.Configuration.GetSection("MpesaSandBox"));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.Configure<CelcomAfrica>(builder.Configuration.GetSection("CelcomAfrica"));



builder.Services.AddSignalR();


// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// Add authorization policies if needed
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new QueryStringApiVersionReader("api-version");

});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api",
        Version = "v1"
    });

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }});

});
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        //https://minimart-nine.vercel.app
        //http://localhost:3000
        builder.WithOrigins("https://minimart-nine.vercel.app")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});


builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // Optional for proxy environments
    options.KnownProxies.Clear();  // Optional for proxy environments
});




builder.Services.AddHttpClient();


var app = builder.Build();

// Enable forwarded headers middleware BEFORE any URL generation or redirect logic
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimart API v1");
        c.ConfigObject.AdditionalItems["servers"] = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "url", "/" } }
        };
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimart API v1");
        c.RoutePrefix = "swagger";
        c.ConfigObject.AdditionalItems["servers"] = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "url", "/" } }
        };
    });
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Create Uploads directory if it doesn't exist
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// Serve static files from the "Uploads" directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// Use refined CORS policy
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub
app.MapHub<ActivityHub>("/ActivityHub").RequireCors("AllowFrontend");

// Map controller routes
app.MapControllers();

// Use global error handler
app.UseExceptionHandler("/error");

app.Run();

