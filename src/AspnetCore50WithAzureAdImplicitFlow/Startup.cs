using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace AspnetCore50WithAzureAdImplicitFlow
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



            // TODO: MEMO
            /***
             * 2021-01-26時点は NOT WORKING.
             * NuGet package の問題 (ui 側の問題もあるっぽい)
             * https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1235
             *
             * x-tokenName support の issue も再発してるみたいでカオスになってるので、実際に利用するのにはしばらく様子見。
             * implicit flow 使ってるなら手で bearer token セットする方を使った方が安全。
             *
             * ちなみに以下の実装のように、Scope に openid と api の scope を追加すると、
             * authorize endpoint に request する際の query parameter は "response_type=token" にはなるけどなぜか id token も取得できて、
             * id token を bearer にセットして api を call してくれるので、無理やり動かすことはできる。
             * (そもそも "response_type=token" で id token とれてるのもおかしいけど...)
             *
             * azure ad は beachside-sandbox > swagger-app-implicit で検証中
             ***/



            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspnetCore50WithAzureAdImplicitFlow", Version = "v1" });

                c.AddSecurityDefinition("Azure AD - Implicit Flow", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    //Extensions = new Dictionary<string, IOpenApiExtension> { { "x-tokenName", new OpenApiString("id_token") } },
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(Configuration.GetValue<string>("AzureAd:authorizationUrl")),
                            Scopes = new Dictionary<string, string>
                            {
                                ["openid"] = "Sign In Permissions",
                                [Configuration.GetValue<string>("AzureAd:apiScope")] = "API permission"
                            }
                        }
                    },
                    Description = "Azure AD Authorization Code Flow authorization",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                });

                //これをつけることですべての API に認証を適用する
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            //Extensions = new Dictionary<string, IOpenApiExtension> { { "x-tokenName", new OpenApiString("id_token") } },
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id ="Azure AD - Implicit Flow"},
                        },
                        // Scope は必要に応じて入力する
                        new string[] {}
                    }
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
                    c.OAuthClientId(Configuration.GetValue<string>("AzureAd:clientId"));
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspnetCore50WithAzureAdImplicitFlow v1");
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