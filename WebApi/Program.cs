using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using WebApi.Services;
using WebApi.Utils;
using App;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.UseSwagger();
builder.Services.UseJwt(builder.Configuration);
builder.Services.AddSingleton<ICompiler, Compiler>();
builder.Services.AddSingleton<DynamicChangeTokenProvider>();
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(provider => provider.GetRequiredService<DynamicChangeTokenProvider>());
InMemoryDatabaseRoot _databaseRoot = new InMemoryDatabaseRoot();
string _connectionString = Guid.NewGuid().ToString();
builder.Services.AddEntityFrameworkInMemoryDatabase();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(_connectionString, _databaseRoot);
});
builder.WebHost
    .UseUrls("http://*:5000")
    .UseKestrel(options =>
    {
        //options.ConfigureHttpsDefaults(o =>
        //{
        //    o.ServerCertificate =
        //    new System.Security.Cryptography.X509Certificates.X509Certificate2($@"{System.Environment.CurrentDirectory}\\SSL\\7705714_www.zkbar.cn.pfx", "xbz4548s");//证书路径、密码
        //});
        options.Limits.MaxRequestLineSize = int.MaxValue;//HTTP 请求行的最大允许大小。 默认为 8kb
        options.Limits.MaxRequestBufferSize = int.MaxValue;//请求缓冲区的最大大小。 默认为 1M
        options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;//限制请求长度
    });
var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
