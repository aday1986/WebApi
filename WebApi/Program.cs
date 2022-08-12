var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost
    .UseUrls("http://*:35000;")
    .UseKestrel(options =>
    {
        //options.ConfigureHttpsDefaults(o =>
        //{
        //    o.ServerCertificate =
        //    new System.Security.Cryptography.X509Certificates.X509Certificate2($@"{root}\\SSL\\7705714_www.zkbar.cn.pfx", "xbz4548s");//证书路径、密码
        //});
        options.Limits.MaxRequestLineSize = int.MaxValue;//HTTP 请求行的最大允许大小。 默认为 8kb
        options.Limits.MaxRequestBufferSize = int.MaxValue;//请求缓冲区的最大大小。 默认为 1M


        //任何请求正文的最大允许大小（以字节为单位）,默认 30,000,000 字节，大约为 28.6MB
        options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;//限制请求长度
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
