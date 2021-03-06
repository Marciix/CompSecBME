using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CaffShop.Helpers;
using CaffShop.Helpers.Wrappers;
using CaffShop.Interfaces;
using CaffShop.Models.Options;
using CaffShop.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffShop
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
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(RegisterSwaggerGeneration);

            // Lowercase urls
            services.AddRouting(options => options.LowercaseUrls = true);

            // Get Connection string from ENV variables

            var mySqlServerOptions = Configuration.GetSection(MySqlServerOptions.OptionsName);
            services.Configure<MySqlServerOptions>(mySqlServerOptions);
            var connectionString = mySqlServerOptions.Get<MySqlServerOptions>().ConnectionString;
            services.AddDbContext<CaffShopContext>(options => options.UseMySql(connectionString));

            services.Configure<UploadOptions>(Configuration.GetSection(UploadOptions.OptionsName));

            // Add auto mapper profile
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICaffItemService, CaffItemService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ICaffUploadService, CaffUploadService>();

            services.Configure<JwtOptions>(Configuration.GetSection(JwtOptions.OptionsName));

            services.AddJwtMiddleware(_validateUserAfterTokenCheck);

            switch (Environment.GetEnvironmentVariable("CAFF_PARSER"))
            {
                case "MOCK":
                    services.AddScoped<ICaffParserWrapper, CaffParserWrapperMock>();
                    break;
                case "MOCK-FAIL":
                    services.AddScoped<ICaffParserWrapper, CaffParserWrapperFailMock>();
                    break;
                default:
                    services.AddScoped<ICaffParserWrapper, CaffParserWrapper>();
                    break;
            }
        }

        private readonly Func<TokenValidatedContext, Task> _validateUserAfterTokenCheck = context =>
        {
            var s = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
            if (!s.IsUserAbleToLogin(context.Principal.Identity.Name).Result)
            {
                context.Fail("Unauthorized: User does not exists");
            }

            return Task.CompletedTask;
        };


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IOptions<MySqlServerOptions> dbOptions,
            IOptions<UploadOptions> upOptions
        )
        {
            if (env.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(
                    c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CaffShop V1")
                );
            }

            // Use _myOrigin CORS profile
            app.UseCors(builder =>
            {
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
                builder.AllowAnyOrigin();
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            if (dbOptions.Value.DoMigration)
            {
                MigrateDatabase(app);
            }

            InitUploadDirectories(upOptions.Value);
        }


        private static void RegisterSwaggerGeneration(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo {Title = "CaffShop API", Version = "v1"});
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme.<br /> 
                      Enter 'Bearer' [space] &lt;token string&gt;",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        }

        private static void MigrateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<CaffShopContext>();
            context.Database.Migrate();
        }


        private static void InitUploadDirectories(UploadOptions options)
        {
            Directory.CreateDirectory(options.UploadBaseDir);
            Directory.CreateDirectory(options.TempDirPath);
            Directory.CreateDirectory(options.CaffDirPath);
            Directory.CreateDirectory(options.PrevDirPath);
        }
    }
}