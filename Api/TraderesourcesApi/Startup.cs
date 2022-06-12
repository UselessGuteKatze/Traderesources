using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using Yoda.Application;

namespace TraderesourcesApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddQueryExecuter(Configuration);

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TRADERESOURCES API", Version = "v1" });
                //c.AddSecurityDefinition("Bearer",
                //  new OpenApiSecurityScheme {
                //      In = ParameterLocation.Header,
                //      Description = "Please enter into field the word 'Bearer' following by space and JWT",
                //      Name = "Authorization",
                //      Type = SecuritySchemeType.ApiKey
                //  });
            });


            var identityServerUrl = Configuration["IdentityServerUrl"];
            services
              .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options => {
                  options.Authority = identityServerUrl;
                  options.TokenValidationParameters = new TokenValidationParameters {
                      ValidateAudience = false
                  };
              });

            services
                .AddAuthorization(options => {
                    options.AddPolicy("TraderesourcesApiScope", policy => {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("scope", "TraderesourcesApi");
                    });
                });

            services
                .AddHealthChecks()
                .AddUrlGroup(new Uri(identityServerUrl + "/.well-known/openid-configuration"), "IdentityServerUrl");

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            if (env.EnvironmentName == "Development") {
                app.Use((context, next) => {
                    context.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                        new Claim(ClaimTypes.NameIdentifier, "000429500372"),
                        new Claim(ClaimTypes.Name, "Test user"),
                        new Claim(ClaimTypes.Email, "test@example.com"),
                        new Claim("scope", "TraderesourcesApi"),
                    }, JwtBearerDefaults.AuthenticationScheme));
                    var auth = context.User.Identity.IsAuthenticated;
                    return next();
                });
            }

            app.UseAuthorization();
            
            app.UseStaticFiles();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Avatars V1");
            });

            app.UseHealthChecks("/healthz-check-ui-endpoint", new HealthCheckOptions {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksPrometheusExporter(
                "/healthz-check-prometheus-endpoint",
                options => options.ResultStatusCodes[HealthStatus.Unhealthy] = (int)HttpStatusCode.OK
            );
        }
    }
}
