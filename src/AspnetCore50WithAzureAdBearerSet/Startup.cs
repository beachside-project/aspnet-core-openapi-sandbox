using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AspnetCore50WithAzureAdBearerSet
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
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = AuthenticationFailed
                    };
                });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SwaggerSandbox.AspnetCore50WithAzureAdBearerSet", Version = "v1" });

                c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "IdP ����擾�����g�[�N�����Z�b�g���܂�(�擪�� 'Bearer' + space �͕s�v)�B",
                });

                //��������邱�Ƃł��ׂĂ� API �ɔF�؂�K�p����
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SwaggerSandbox.AspnetCore50WithAzureAdBearerSet v1"));
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

        private static async Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            var message = $"AuthenticationFailed: {arg.Exception.Message}";
            arg.Response.ContentLength = message.Length;
            arg.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await arg.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(message), 0, message.Length);
        }
    }
}