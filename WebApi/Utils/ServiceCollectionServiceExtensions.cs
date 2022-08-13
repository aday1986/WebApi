using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using WebApi.Controllers;
using WebApi.Models;

namespace WebApi.Utils
{
    public static partial class ServiceCollectionServiceExtensions
    {
        public static void UseJwt(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddHttpContextAccessor();
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
            var jwtConfig = new JwtConfig();
            Configuration.Bind("JwtConfig", jwtConfig);
            services.AddAuthentication(option =>
                {
                    //认证middleware配置
                    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                        ValidateLifetime = true,
                    };
                });
           
            services.AddScoped<ServiceConfig>();
        }

        public static void UseSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                #region 启用swagger验证功能
                //添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称一致即可。
                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",

                            },
                        },
                    new string[] { }
                    }
                 });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 在下方输入Bearer {token} 即可，注意两者之间有空格",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",

                });
                #endregion


                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MessageServerApi", Version = "v1" });
                // 获取xml文件路径
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Api.xml");
                // 添加控制器层注释，true表示显示控制器注释
                options.IncludeXmlComments(xmlPath, true);
            });
        }

    }

    /// <summary>
    /// 接收消息体
    /// </summary>
    public class ServiceConfig
    {

        public ServiceConfig(IHttpContextAccessor httpContext)
        {
            var user = httpContext.HttpContext.User;
            User = new UserInfo()
            {
                UserNo = user.FindFirst("UserNo")?.Value,
                UserName = user.FindFirst("UserName")?.Value,
                DepName = user.FindFirst("DepName")?.Value,
                DepNo = user.FindFirst("DepNo")?.Value,
                JobName = user.FindFirst("JobName")?.Value,
                JobNo = user.FindFirst("JobNo")?.Value,
            };
        }

        /// <summary>
        /// 帐号登录信息
        /// </summary>
        public UserInfo User { get; set; }

    }
}
