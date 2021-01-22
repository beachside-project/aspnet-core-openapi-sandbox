using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AspnetCore50WithAzureAdAuthorizationCode
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.MetadataAddress = Configuration.GetValue<string>("AzureAd:MetadataAddress");
                    options.Audience = Configuration.GetValue<string>("AzureAd:clientId");
                });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspnetCore50WithAzureAdAuthorizationCode", Version = "v1" });

                c.AddSecurityDefinition("Azure AD - Authorization Code Flow", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(Configuration.GetValue<string>("AzureAd:AuthorizationUrl")),
                            TokenUrl = new Uri(Configuration.GetValue<string>("AzureAd:tokenUrl")),
                            Scopes = new Dictionary<string, string>
                            {
                                ["openid"] = "Sign In Permissions",
                                [Configuration.GetValue<string>("AzureAd:apiScope")] = "API permission"
                            },
                        }
                    },
                    Description = "Azure AD Authorization Code Flow authorization",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id ="Azure AD - Authorization Code Flow"},
            },
            // Scope ‚Í•K—v‚É‰ž‚¶‚Ä“ü—Í‚·‚é
            new string [] {}
        },

                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SwaggerSandboxAspnetCore31WithAzureAd v1");
                    c.OAuthClientId(Configuration.GetValue<string>("AzureAd:clientId"));
                    c.OAuthUsePkce();
                });
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