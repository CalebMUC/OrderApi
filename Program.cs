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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .LogTo(message => Log.Information(message),Microsoft.Extensions.Logging.LogLevel.Information) // Log to Serilog
           .EnableSensitiveDataLogging(); // Enable logging of sensitive data (like parameters)
},
ServiceLifetime.Scoped); // Scoped lifetime for the DbContext
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    // Get the Redis URL from configuration or environment variable
    var redisUrl = builder.Configuration["REDIS_URL"] ??
                  Environment.GetEnvironmentVariable("REDIS_URL") ??
                  "redis://default:AYgmAAIjcDFkNzgwMWU1NjZhMzQ0NWM1YTcwYWQ2MDk5MGQ4ZDU3Y3AxMA@loved-airedale-34854.upstash.io:6379";

    // Parse the Redis URL into a ConfigurationOptions object
    var configOptions = ConfigurationOptions.Parse(redisUrl);

    // For Upstash Redis with TLS (which your connection string indicates)
    configOptions.Ssl = true;
    configOptions.AbortOnConnectFail = false;

    // Add logging
    var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();
    logger.LogInformation($"Connecting to Redis at {configOptions.EndPoints.First()}");

    return ConnectionMultiplexer.Connect(configOptions);
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

//builder.Services.Configure<OpenSearchSettings>(builder.Configuration.GetSection("OpenSearchSettings"));

//builder.Services.AddSingleton<IElasticClient>(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<OpenSearchSettings>>().Value;
//    var uri = new Uri(settings.Endpoint);

//    var connectionSettings = new ConnectionSettings(uri)
//        .BasicAuthentication("CalebMuchiri", "Caleb@2543")
//        .EnableApiVersioningHeader()  // Ensure compatibility with OpenSearch
//        .ServerCertificateValidationCallback(CertificateValidations.AllowAll); // Disable strict SSL checks for testing purposes

//    var client = new ElasticClient(connectionSettings);
//    return client;
//});

//Configure OpenSearch client
//builder.Services.AddSingleton<IOpenSearchClient>(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<OpenSearchSettings>>().Value;
//    var uri = new Uri(settings.Endpoint);
//    var connectionSettings = new ConnectionSettings(uri)
//        .BasicAuthentication("CalebMuchiri", "Caleb@2543"); // Add username/password if required
//    return new OpenSearchClient(connectionSettings);
//});

builder.Services.AddSignalR();

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<MinimartDBContext>()
//    .AddDefaultTokenProviders();



//uilder.Services.AddTransient<IEmailSender, EmailSender>();  // Update the namespace accordingly

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
// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
        builder.WithOrigins("http://localhost:3000") // Change this to match your frontend URL
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()); // Important for authentication
});


builder.Services.AddHttpClient();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimart API v1");

        // For Render's proxy handling:
        c.ConfigObject.AdditionalItems["servers"] = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "url", "/" } }
        };
    });
}
else // Production settings
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimart API v1");
        c.RoutePrefix = "swagger";  // Makes Swagger available at /swagger

        // Required for Render's reverse proxy:
        c.ConfigObject.AdditionalItems["servers"] = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "url", "/" } }
        };
    });
}
// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
//        c.RoutePrefix = "Swagger"; // Serve Swagger UI at the app's rootMpesaSandBox
//    });
//}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger"; // Ensure lowercase "swagger" for easier access
});


app.UseHttpsRedirection();

var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
if (!Directory.Exists(uploadsPath)) { 
    Directory.CreateDirectory(uploadsPath); 
}

// Enable serving of static files from the "Uploads" directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/uploads" // URL path to access the uploads
});

app.UseCors("AllowAllOrigins");


app.UseAuthentication();
app.UseAuthorization();

// Enable CORS if you're accessing the API from different origins


app.MapHub<ActivityHub>("/ActivityHub").RequireCors("AllowAllOrigins");

app.MapControllers();

app.UseExceptionHandler("/error");

 app.Run();

