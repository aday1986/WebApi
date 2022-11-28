using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Services
{
    /// <summary>
    /// EF
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions options) : base(options)
        {
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Model.AddEntityType(typeof(UserInfo));
        }

    }

   

}
