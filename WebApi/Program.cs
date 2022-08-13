using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;
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

//builder.Services.AddSwaggerGen(options =>
//{
//    //options.SwaggerDoc("v1");
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
//    {
//        Scheme = JwtBearerDefaults.AuthenticationScheme,
//        BearerFormat = "JWT",
//        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.Http
//    });
//    options.OperationFilter<AuthenticationOperationFilter>();
//    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MessageServerApi", Version = "v1" });
//    // ��ȡxml�ļ�·��
//    var xmlPath = Path.Combine(AppContext.BaseDirectory, "Api.xml");
//    // ��ӿ�������ע�ͣ�true��ʾ��ʾ������ע��
//    options.IncludeXmlComments(xmlPath, true);
//});
builder.Services.UseSwagger();
//builder.Services.UseSwagger();
builder.Services.UseJwt(builder.Configuration);
// ע����֤����
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = true, //�Ƿ���֤Issuer
//        ValidIssuer = jwtConfig.Issuer, //������Issuer
//        ValidateAudience = true, //�Ƿ���֤Audience
//        ValidAudience = jwtConfig.Audience, //������Audience
//        ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)), //SecurityKey
//        ValidateLifetime = true, //�Ƿ���֤ʧЧʱ��
//        ClockSkew = TimeSpan.FromSeconds(30), //����ʱ���ݴ�ֵ�������������ʱ�䲻ͬ�����⣨�룩
//        RequireExpirationTime = true,
//        NameClaimType = "name"
//    };
//    options.Events = new JwtBearerEvents()
//    {
//        OnChallenge = async context =>
//        {
//            context.Response.StatusCode = 401;
//            // ����
//            await context.Response.WriteAsync("401");

//            // �����Ӧ
//            //context.Handled = true;
//            context.HandleResponse();

//            //return Task.CompletedTask;
//        },
//        OnForbidden = async context =>
//        {
//            //await context.Response.WriteAsync("403");
//        }
//    };

//});
//builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

////builder.Services.AddAuthorization(options =>
////{
////    options.AddPolicy(Permissions.UserCreate, policy => policy.AddRequirements(new PermissionAuthorizationRequirement(Permissions.UserCreate)));
////    options.AddPolicy(Permissions.UserUpdate, policy => policy.AddRequirements(new PermissionAuthorizationRequirement(Permissions.UserUpdate)));
////    options.AddPolicy(Permissions.UserDelete, policy => policy.AddRequirements(new PermissionAuthorizationRequirement(Permissions.UserDelete)));
////});
//builder.Services.AddSingleton<IAuthorizationPolicyProvider, TestAuthorizationPolicyProvider>();

//builder.Services.AddSwaggerGen();
builder.WebHost
    .UseUrls("https://*:35001;http://*:35000")
    .UseKestrel(options =>
    {
        //options.ConfigureHttpsDefaults(o =>
        //{
        //    o.ServerCertificate =
        //    new System.Security.Cryptography.X509Certificates.X509Certificate2($@"{root}\\SSL\\7705714_www.zkbar.cn.pfx", "xbz4548s");//֤��·��������
        //});
        options.Limits.MaxRequestLineSize = int.MaxValue;//HTTP �����е���������С�� Ĭ��Ϊ 8kb
        options.Limits.MaxRequestBufferSize = int.MaxValue;//���󻺳���������С�� Ĭ��Ϊ 1M


        //�κ��������ĵ���������С�����ֽ�Ϊ��λ��,Ĭ�� 30,000,000 �ֽڣ���ԼΪ 28.6MB
        options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;//�������󳤶�
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
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// ���ڻ�ȡ�û���Ϣ
/// </summary>
public class TokenActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var test = context.HttpContext.Request.Path;
        string bearer = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (bearer == null || bearer.Length <= 6 || bearer.Substring(0, 6).ToLower() != "bearer")
        {
            return;
        }
        string[] jwt = bearer.Split(' ');
        var tokenObj = new JwtSecurityToken(jwt[1]);

        var claimsIdentity = new ClaimsIdentity(tokenObj.Claims);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        context.HttpContext.User = claimsPrincipal;
    }
}

/// <summary>
/// ȫ���쳣������
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    public GlobalExceptionFilter(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory;
    }
    public ILoggerFactory LoggerFactory { get; }

    public void OnException(Microsoft.AspNetCore.Mvc.Filters.ExceptionContext context)
    {
       
        var logger = LoggerFactory.CreateLogger(context.ActionDescriptor.DisplayName);
        logger.LogError(context.Exception.Message);
        if (!context.ExceptionHandled)
        {
            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(new ApiResult<string>()
            {
                Message = context.Exception.Message,
                Data = "ִ��ʧ��",
                Status = context.Exception.HResult
            });
        }

       
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}

public class AuthenticationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionScopes = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Select(attr => attr.TypeId.ToString()).Distinct();
        var controllerScopes = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>()
            .Select(attr => attr.TypeId.ToString());
        var requiredScopes = actionScopes.Union(controllerScopes).Distinct().ToArray();
        if (requiredScopes.Any())
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            //operation.Responses.Add("419", new OpenApiResponse { Description = "AuthenticationTimeout" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            var oAuthScheme = new OpenApiSecurityScheme
            {
                Scheme = JwtBearerDefaults.AuthenticationScheme, //"Bearer"
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [ oAuthScheme ] = new List<string>()
                }
            };
        }
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        var permissions = context.User.Claims.Where(_ => _.Type == "Permission").Select(_ => _.Value).ToList();
        if (permissions.Any(_ => _.StartsWith(requirement.Name)))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

public class PermissionAuthorizationRequirement : IAuthorizationRequirement
{
    public PermissionAuthorizationRequirement(string name)
    {
        Name = name;
    }
    public string Name { get; set; }
}

public class TestAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider, IAuthorizationPolicyProvider
{
    public TestAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

    public new Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => base.GetDefaultPolicyAsync();

    public new Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => base.GetFallbackPolicyAsync();

    public new Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(Permissions.User))
        {
            var policy = new AuthorizationPolicyBuilder(Array.Empty<string>()); // JwtBearerDefaults.AuthenticationScheme
            policy.AddRequirements(new PermissionAuthorizationRequirement(policyName));
            return Task.FromResult<AuthorizationPolicy?>(policy.Build());
        }
        return base.GetPolicyAsync(policyName);
    }
}

public static class Permissions
{
    public const string User = "User";
    public const string UserCreate = User + ".Create";
    public const string UserDelete = User + ".Delete";
    public const string UserUpdate = User + ".Update";
}
