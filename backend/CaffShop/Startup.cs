using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CaffShop.Helpers;
using CaffShop.Helpers.Wrappers;
using CaffShop.Interfaces;
using CaffShop.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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
            services.AddDbContext<CaffShopContext>(options => options.UseMySql(GetConnectionStringFromEnv()));
            
            // Add auto mapper profile
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICaffItemService, CaffItemService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ICaffParserWrapper, CaffParserWrapper>();
            
            RegisterJwt(services);

            // Allow _myOrigin CORS profile
            services.AddCors(options =>
            {
                options.AddPolicy("_myOrigin", builder =>
                {
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowAnyOrigin();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(
                    c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CaffShop V1")
                );
            }

            app.UseHttpsRedirection();

            // Use _myOrigin CORS profile
            app.UseCors("_myOrigin");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            MigrateDatabase(app);
        }
        
        private static string GetConnectionStringFromEnv()
        {
            var host = HelperFunctions.GetEnvironmentValueOrException("DB_HOST");
            var name = HelperFunctions.GetEnvironmentValueOrException("DB_NAME");
            var user = HelperFunctions.GetEnvironmentValueOrException("DB_USER");
            var pass = HelperFunctions.GetEnvironmentValueOrException("DB_PASS");
            var port = HelperFunctions.GetEnvironmentValueOrException("DB_PORT");

            return $"server={host};port={port};database={name};user={user};password={pass}";
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
            if (Environment.GetEnvironmentVariable("DB_MIGRATE") != "TRUE") return;
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<CaffShopContext>();
            context.Database.Migrate();
        }

        private static void RegisterJwt(IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(HelperFunctions.GetEnvironmentValueOrException("JWT_SECRET"));
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.RequireHttpsMetadata = false;
                    jwtBearerOptions.SaveToken = true;
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };
                });
        }

        
    }
}