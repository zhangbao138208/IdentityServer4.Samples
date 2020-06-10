using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API1RESOURCE
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddAuthorization();
                //.AddJsonFormatters();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "http://localhost:5000";
                options.ApiName = "api1";
                options.RequireHttpsMetadata = false;
                options.ApiSecret = "api1 secret";
            });
            //.AddJwtBearer("Bearer", options =>
            //{
            //    options.Authority = "http://localhost:5000";
            //    options.RequireHttpsMetadata = false;

            //    options.Audience = "api1";
            //    options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(1);
            //    options.TokenValidationParameters.RequireExpirationTime = true;
            //});

            services.AddCors(options=> {
                options.AddPolicy("ReactClientOrigin",
                    buider=>buider.WithOrigins("http://localhost:8084")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });
            //services.Configure<MvcOptions>(options=> {
            //    options.Filters.Add(new CorsAuthorizationFilterFactory("ReactClientOrigin"));
            //});
        }

        public void Configure(IApplicationBuilder app,IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(apoBuilder =>
                {
                    apoBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected Error!");
                    });
                }

                    );
            }
            app.UseCors("ReactClientOrigin");
            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}