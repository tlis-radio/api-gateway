using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tlis.WebApiGateway;
using Yarp.ReverseProxy.Swagger;
using Yarp.ReverseProxy.Swagger.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.DocumentFilter<ReverseProxyDocumentFilter>();
});

var configuration = builder.Configuration.GetSection("ReverseProxy");
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddSwagger(configuration);
    
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var config = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
    foreach (var cluster in config.Clusters)
    {
        options.SwaggerEndpoint($"/swagger/{cluster.Key}/swagger.json", cluster.Key);
    }
});

app.MapReverseProxy();
app.UseHttpsRedirection();

app.Run();
