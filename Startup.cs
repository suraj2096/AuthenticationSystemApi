using AuthenticationSystem.DTOMapping;
using AuthenticationSystem.Identity;
using AuthenticationSystem.Repository;
using AuthenticationSystem.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer
            (Configuration.GetConnectionString("DbConnection"), b => b.MigrationsAssembly("AuthenticationSystem")));

            services.AddTransient<IRoleStore<ApplicationRoles>, ApplicationRoleStore>();
            services.AddTransient<UserManager<ApplicationUser>, ApplicationUserManager>();
            services.AddTransient<SignInManager<ApplicationUser>, ApplicationSignInManager>();
            services.AddTransient<RoleManager<ApplicationRoles>, ApplicationRoleManager>();
            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStore>();


            



            services.AddIdentity<ApplicationUser, ApplicationRoles>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddUserStore<ApplicationUserStore>()
    .AddUserManager<ApplicationUserManager>()
    .AddRoleManager<ApplicationRoleManager>()
    .AddSignInManager<ApplicationSignInManager>()
    .AddRoleStore<ApplicationRoleStore>()
    .AddDefaultTokenProviders();

            services.AddScoped<ApplicationUserStore>();
            services.AddScoped<ApplicationRoleStore>();
           
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IUserServiceRepository, UserServiceRepository>();
            services.AddScoped<IJwtManagerRepository, JwtManagerRepository>();
            services.AddAutoMapper(typeof(MappingPofile));
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            // jwt Configuration
            var appsettingSection = Configuration.GetSection("AppSettingJWT");
            services.Configure<AppSettingJWT>(appsettingSection);
            var appsetting = appsettingSection.Get<AppSettingJWT>();
            var key = Encoding.ASCII.GetBytes(appsetting.SecretKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthenticationSystem", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthenticationSystem v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // create roles by default with coding
           /* IServiceScopeFactory serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRoles>>();
                // var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                if (!await roleManager.RoleExistsAsync(SD.s_roleManager))
                {
                    var role = new ApplicationRoles();
                    role.Name = SD.s_roleManager;
                    await roleManager.CreateAsync(role);
                }
                if (!await roleManager.RoleExistsAsync(SD.s_roleHR))
                {
                    var role = new ApplicationRoles();
                    role.Name = SD.s_roleHR;
                    await roleManager.CreateAsync(role);
                }
                if (!await roleManager.RoleExistsAsync(SD.s_roleEmployee))
                {
                    var role = new ApplicationRoles();
                    role.Name = SD.s_roleEmployee;
                    await roleManager.CreateAsync(role);
                }
            }*/


            app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
        }
    }
}
