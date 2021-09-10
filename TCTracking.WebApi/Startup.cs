using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Text;
using TCTracking.Core.Implement;
using TCTracking.Core.Interface;
using TCTracking.Service.Implement;
using TCTracking.Service.Interface;
using TCTracking.WebApi.Configuration;

namespace TCTracking.WebApi
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

            services.AddSingleton<IDbClient, DbClient>();
            services.Configure<TCTrackingDatabaseSettings>(
                Configuration.GetSection(nameof(TCTrackingDatabaseSettings)));

            services.AddSingleton<ITCTrackingDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<TCTrackingDatabaseSettings>>().Value);

            services.AddTransient<ITCSService, TCSService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IPasswordGeneratorService, PasswordGeneratorService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IDisplayService, DisplayService>();
            services.AddTransient<IFileManagerService, FileManagerService>();


            // within this section we are configuring the authentication and setting the default scheme
            services.AddAuthentication(options =>
            {
               
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               
            })
            .AddJwtBearer(jwt =>
            {
                var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = false
                };
            });


            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TCTracking.WebApi", Version = "v1" });
            });

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(
                options => options.WithOrigins("http://localhost:4200",
                "http://localhost:4201", "http://localhost", "http://192.168.0.106","http://10.226.175.92")
                .AllowAnyMethod()
                .AllowAnyHeader()
                );

            //app.UseCors(x => x
            //  .AllowAnyMethod()
            //  .AllowAnyHeader()
            //  .SetIsOriginAllowed(origin => true)); // allow credentials


            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TCTracking.WebApi v1"));
            app.UseHttpsRedirection();


            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });


            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
