using Autofac;
using Autofac.Extensions.DependencyInjection;
using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.Extensions.Options;
using redis.WebAPi.Service;
using redis.WebAPi.Service.AzureShared;
using redis.WebAPi.Service.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//add configuration 0909
builder.Services.AddCors();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{

    // 注册 ArmClientFactory，并从配置中传递订阅 ID
    containerBuilder.Register(c =>
    {
        
        return new AzureClientFactory();
    }).SingleInstance();

    // 注册 SubscriptionResourceService，延迟生成 SubscriptionResource
    containerBuilder.RegisterType<SubscriptionResourceService>().As<ISubscriptionResourceService>().SingleInstance();

    containerBuilder.RegisterType<RedisCollectionService>().As<IRedisCollection>().SingleInstance();

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt=> 
    {
        //允许所有来源
        //opt=> .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
        //允许指定来源
        opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
    });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
