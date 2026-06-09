using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebProjekat.Application.Interfaces;
using WebProjekat.Application.Services;
using WebProjekat.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Web1
{
    internal sealed class Web1 : StatelessService
    {
        public Web1(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();

                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.WebHost
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);

                        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
                        builder.Services.AddScoped<IUserService, UserService>();
                        builder.Services.AddScoped<ITravelPlanService, TravelPlanService>();

                        // JWT Authentication
                        var jwtKey = builder.Configuration["Jwt:Key"];
                        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
                        var jwtAudience = builder.Configuration["Jwt:Audience"];
                        if (!string.IsNullOrWhiteSpace(jwtKey))
                        {
                            var key = Encoding.UTF8.GetBytes(jwtKey);
                            builder.Services.AddAuthentication(options =>
                            {
                                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                            })
                            .AddJwtBearer(options =>
                            {
                                options.RequireHttpsMetadata = false;
                                options.SaveToken = true;
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
                                    ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = jwtIssuer,
                                    ValidAudience = jwtAudience,
                                    IssuerSigningKey = new SymmetricSecurityKey(key)
                                };
                            });
                        }

                        builder.Services.AddCors(options =>
                        {
                            options.AddPolicy("AllowAll",
                                policy => policy.AllowAnyOrigin()
                                              .AllowAnyMethod()
                                              .AllowAnyHeader());
                        });
                        builder.Services.AddControllers();
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen(options =>
                        {
                            options.SwaggerDoc("v1", new OpenApiInfo
                            {
                                Title = "Travel Planning API",
                                Version = "v1",
                                Description = "API for managing travel plans, activities, destinations, expenses, and checklists",
                                Contact = new OpenApiContact
                                {
                                    Name = "Travel Planner Support"
                                }
                            });

                            // Security scheme for JWT
                            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                            {
                                Name = "Authorization",
                                Type = SecuritySchemeType.Http,
                                Scheme = "bearer",
                                BearerFormat = "JWT",
                                In = ParameterLocation.Header,
                                Description = "Enter your JWT token in the format: Bearer {token}"
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
                                        }
                                    },
                                    Array.Empty<string>()
                                }
                            });

                            options.OrderActionsBy((apiDesc) =>
                            {
                                var tags = apiDesc.ActionDescriptor.EndpointMetadata
                                    .OfType<Microsoft.AspNetCore.Mvc.TagsAttribute>()
                                    .FirstOrDefault();
                                return tags?.Tags.FirstOrDefault() ?? "Other";
                            });
                        });
                        var app = builder.Build();
                        if (app.Environment.IsDevelopment())
                        {
                            app.UseSwagger();
                            app.UseSwaggerUI();
                        }
                        app.UseCors("AllowAll");
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.MapControllers();

                        return app;

                    }))
            };
        }
    }
}