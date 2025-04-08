using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using redis.WebAPi.Service.IService;
using redis.WebAPi.Service.AzureShared;
using redis.WebAPi.Service;
using redis.WebAPi.Repository.AppDbContext;
using redis.WebAPI.Service;
using redis.WebAPi.Filters;
using redis.WebAPi.Hubs;
using redis.WebAPi.Service.Benchmark;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AuthFilter>();
// Add services to the container.
builder.Services.AddControllers();
// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// SignalR service
builder.Services.AddSignalR();
builder.Services.AddScoped<ConnectionVMService>();
// Add CORS policy
builder.Services.AddCors();
// JWT Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Development environments may not force HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

// Configure database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<BenchmarkContent>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Using Autofac as a Dependency Injection Container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register CreateHub as singleton
    containerBuilder.RegisterType<CreateHub>().AsSelf().SingleInstance();

    containerBuilder.RegisterType<TimerService>().InstancePerLifetimeScope();

    // Register other services
    //containerBuilder.RegisterType<BenchmarkQueueProcessor>().As<IHostedService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ConnectionVMService>().InstancePerDependency();
    containerBuilder.RegisterType<AzureClientFactory>().SingleInstance();
    containerBuilder.RegisterType<SubscriptionResourceService>().As<ISubscriptionResourceService>().SingleInstance();
    containerBuilder.RegisterType<RedisCollectionService>().As<IRedisCollection>().SingleInstance();
    containerBuilder.RegisterType<StackExchangeService>().As<IStackExchangeService>().SingleInstance();
    containerBuilder.RegisterType<ResourceDeletionService>().As<IResourceDeletionService>().SingleInstance();
    containerBuilder.RegisterType<MedianService>().As<IMedianService>().SingleInstance();
    containerBuilder.RegisterType<CreationService>().As<ICreationService>().SingleInstance();
    //containerBuilder.RegisterType<ConnectionVMService>().As<IConnectionVMService>().SingleInstance();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000").AllowCredentials();
    opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:3000").AllowCredentials();
    opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://172.29.20.156:3000");
});

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authentication and authorization

//app.UseAuthentication();
app.UseAuthorization();

// Map SignalR Hub
app.MapHub<CreateHub>("/api/createHub");

// Map controllers
app.MapControllers();

app.Run();
