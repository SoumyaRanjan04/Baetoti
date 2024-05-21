using Baetoti.API.Filters;
using Baetoti.API.Helpers;
using Baetoti.API.Middlewares;
using Baetoti.Core.Interface.Base;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.IoC;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.JwtConfig;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Baetoti.API
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        private readonly SymmetricSecurityKey _signingKey = null;
        private string allowedOrigin;

        private string AllowedOrigin => allowedOrigin;

        private void SetAllowedOrigin(string value)
        {
            allowedOrigin = value;
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var key = configuration?.GetSection("JwtConfiguration")["Key"];
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            SetAllowedOrigin(configuration?.GetSection("AllowedOrigins").Value);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });
            //For Cors Setting
            services.AddCors(options =>
            {
                options.AddPolicy("Trusted", p =>
                {
                    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();//.AllowCredentials();
                });
            });


            services.AddDataProtection()
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
            {
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });

            services.AddAutoMapper(typeof(Startup));
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Baetoti API", Version = "v1" });
                c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                        In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });

            services.AddSignalR();
            services.AddControllers();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = true;//to use UseMvc() to configure routes
                options.Filters.Add(new ValidateModelStateFilterAttribute());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    ServiceProvider provider = services.BuildServiceProvider();

                    var dbLogger = provider.GetRequiredService<IAppLogger<ValidationProblemDetails>>();
                    var logger = provider.GetRequiredService<ILogger<ValidationProblemDetails>>();
                    var errorMessages = ModelStateHelper.GetValidationErrorMessages(context);
                    var response = GenericResponses.InvalidModelStateResponse(errorMessages);

                    //logging request to DB
                    dbLogger.LogValidationErrorAsync
                    (
                        response.Message, context.HttpContext.User.GetUserId(),
                        context.HttpContext?.Request?.Path ?? null,
                        null,
                        JsonConvert.SerializeObject(errorMessages),
                        logger
                    ).ConfigureAwait(true);
                    return new BadRequestObjectResult(response);
                };
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //services.AddRouting();
            services.AddDbContext<BaetotiDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("Default"), sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly("Baetoti.Infrastructure");
                });
            });

            services.AddDbContext<LoggingDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("Default"));

            }, ServiceLifetime.Transient);

            var jwtAppSettingOptions = Configuration.GetSection("JwtConfiguration");
            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                    ValidateAudience = true,
                    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,

                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                configureOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
                configureOptions.SaveToken = true;
                configureOptions.RequireHttpsMetadata = false;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Role", policy => policy.RequireClaim(JwtCustomClaimNames.Role, "Admin"));
                options.AddPolicy("Role", policy => policy.RequireClaim(JwtCustomClaimNames.Role, "Client"));
            });

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            DependencyContainer.RegisterBaetotiApiServices(services);
            DependencyContainer.RegisterBaetotiApiRepositories(services);

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IAppLogger<Startup> logger)
        {
            app.UseMiddleware<BasicAuthenticationHandler>();
            app.UseSwagger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseExceptionMiddleware(logger);
            app.UseRouting();
            app.UseCors("Trusted");
            app.UseHttpsRedirection();

            if (Convert.ToString(Configuration["LogAPICalls"]) == "1")
                app.UseRequestLogging();

            app.UseAuthentication();
            app.UseAuthorization();

            const string cacheMaxAge = "604800";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, @"wwwroot\Uploads")),
                RequestPath = "/Uploads",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(
                         "Cache-Control", $"public, max-age={cacheMaxAge}");
                }
            });

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            contentTypeProvider.Mappings[".pdf"] = "application/pdf";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, @"wwwroot\Contracts")),
                RequestPath = "/Contracts",
                ContentTypeProvider = contentTypeProvider,
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(
                        "Cache-Control", $"public, max-age={cacheMaxAge}");
                }
            });

            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Baetoti - Swagger UI";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Baetoti V1");
                c.RoutePrefix = string.Empty;
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<UserHub>("/userHub");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }

    }
}
