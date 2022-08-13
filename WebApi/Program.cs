using WebApi.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
builder.Services.AddControllers(option => {
    option.Filters.Add<GlobalExceptionFilter>();
    option.Filters.Add<TokenActionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.UseSwagger();

builder.Services.UseJwt(builder.Configuration);

builder.WebHost
    .UseUrls("http://*:35000")
    .UseKestrel(options =>
    {
        //options.ConfigureHttpsDefaults(o =>
        //{
        //    o.ServerCertificate =
        //    new System.Security.Cryptography.X509Certificates.X509Certificate2($@"{root}\\SSL\\7705714_www.zkbar.cn.pfx", "xbz4548s");//֤��·��������
        //});
        options.Limits.MaxRequestLineSize = int.MaxValue;//HTTP �����е���������С�� Ĭ��Ϊ 8kb
        options.Limits.MaxRequestBufferSize = int.MaxValue;//���󻺳���������С�� Ĭ��Ϊ 1M
        options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;//�������󳤶�
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
