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
             * 2021-01-26���_�� NOT WORKING.
             * NuGet package �̖�� (ui ���̖���������ۂ�)
             * https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1235
             *
             * x-tokenName support �� issue ���Ĕ����Ă�݂����ŃJ�I�X�ɂȂ��Ă�̂ŁA���ۂɗ��p����̂ɂ͂��΂炭�l�q���B
             * implicit flow �g���Ă�Ȃ��� bearer token �Z�b�g��������g�����������S�B
             *
             * ���Ȃ݂Ɉȉ��̎����̂悤�ɁAScope �� openid �� api �� scope ��ǉ�����ƁA
             * authorize endpoint �� request ����ۂ� query parameter �� "response_type=token" �ɂ͂Ȃ邯�ǂȂ��� id token ���擾�ł��āA
             * id token �� bearer �ɃZ�b�g���� api �� call ���Ă����̂ŁA������蓮�������Ƃ͂ł���B
             * (�������� "response_type=token" �� id token �Ƃ�Ă�̂�������������...)
             *
             * azure ad �� beachside-sandbox > swagger-app-implicit �Ō��ؒ�
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

                //��������邱�Ƃł��ׂĂ� API �ɔF�؂�K�p����
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            //Extensions = new Dictionary<string, IOpenApiExtension> { { "x-tokenName", new OpenApiString("id_token") } },
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id ="Azure AD - Implicit Flow"},
                        },
                        // Scope �͕K�v�ɉ����ē��͂���
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