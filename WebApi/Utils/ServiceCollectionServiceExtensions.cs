using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using WebApi.Models;

namespace WebApi.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ServiceCollectionServiceExtensions
    {
        /// <summary>
        /// 使用JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
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

        /// <summary>
        /// 使用Swagger
        /// </summary>
        /// <param name="services"></param>
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
                    Description = "JWT授权,在下方输入Bearer Token",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.Http,
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
}
