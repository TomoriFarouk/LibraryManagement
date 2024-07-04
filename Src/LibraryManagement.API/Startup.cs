using System;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Application.Common.Interface;
using LibraryManagement.Infrastructure.Services;
using LibraryManagement.Core.Interface.Query;
using LibraryManagement.Infrastructure.Repository.Query;
using LibraryManagement.Core.Interface.COMMAND;
using LibraryManagement.Infrastructure.Repository.Command;
using NLog.Extensions.Logging;
using LibraryManagement.Core.Entities.Identity;
using static LibraryManagement.Application.Handler.CommandHandler.BookCommandHandler;
using System.Reflection;

namespace LibraryManagement.API
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
            services.AddControllers();

            // For authentication
            var _key = Configuration["Jwt:Key"];
            var _issuer = Configuration["Jwt:Issuer"];
            var _audience = Configuration["Jwt:Audience"];
            var _expirtyMinutes = Configuration["Jwt:ExpiryMinutes"];



            // Configuration for token
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = _audience,
                    ValidIssuer = _issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                    ClockSkew = TimeSpan.FromMinutes(Convert.ToDouble(_expirtyMinutes))

                };
            });

            services.AddAuthorization();
            // Configure for Sqlite
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
               b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                // Add authentication parameters to Swagger requests
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // Configure the authorization requirement for endpoints
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false; // For special character
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
            });

            // Register dependencies
            services.AddAutoMapper(typeof(Startup));
            services.AddMediatR(typeof(CreateBookHandler).GetTypeInfo().Assembly);
            services.AddMemoryCache();
            services.AddSingleton<CacheManager>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IPatronQuery, PatronQueryRepository>();
            services.AddScoped<IBookQuery, BookQueryRepository>();
            services.AddScoped<IBorrowingRecordQuery, BorrowingRecordQueryRepository>();
            services.AddScoped<IPatronCommand, PatronCommandRepository>();
            services.AddScoped<IBookCommand, BookCommandRepository>();
            services.AddScoped<IBorrowingRecordCommand, BorrowingRecordCommandRepository>();
            

            // services.AddSingleton<ILoggerManager, LoggerManager>();
            //Register the NLog logger service
            services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(provider =>
            {
                var factory = provider.GetRequiredService<ILoggerFactory>();
                return factory.CreateLogger("DefaultLogger");
            });
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(Configuration);
            });
            //// Background service
            //services.AddHostedService<NotificationBackgroundService>();
            //// Dependency injection with key
            services.AddSingleton<ITokenGenerator>(new TokenGenerator(_key, _issuer, _audience, _expirtyMinutes));

            // Add in-memory caching
            

            // For distributed caching with Redis
            // services.AddStackExchangeRedisCache(options =>
            // {
            //     options.Configuration = "localhost:6379"; // Set your Redis configuration here
            //     options.InstanceName = "LibraryCache_";
            // });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LibraryManagement.API v1"));
            }

            app.UseHttpsRedirection();

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

