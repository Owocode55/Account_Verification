using Com.Xpresspayments.AVS.Common;
using Com.Xpresspayments.AVS.Common.Utiities;
using Com.Xpresspayments.AVS.Core;
using Com.Xpresspayments.AVS.Repository;
using Com.Xpresspayments.AVS.Services;
using Com.Xpresspayments.AVS.Services.Integrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.API
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
            services.AddDbContext<Com.Xpresspayments.AVS.Data.AppContext>(options =>
                          options.UseSqlServer(
                              Configuration.GetConnectionString("AVSConnectionString"), providerOptions => providerOptions.EnableRetryOnFailure()));
            //Register dapper in scope    
            services.AddScoped<IDapper, Dapperr>();
            services.AddScoped<IAVSRepository, AVSRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<FidelityBank, FidelityBank>();
            services.AddScoped<GTBank, GTBank>();
            services.AddScoped<Nibss, Nibss>();
            services.AddScoped<UBA, UBA>();
            services.AddScoped<SterlingBank, SterlingBank>();
            services.AddScoped<InputValidationCore, InputValidationCore>();
            services.AddScoped<AccountValidationCore, AccountValidationCore>();
            services.AddScoped<FactoryImplemetation, FactoryImplemetation>();
            services.AddHttpClient();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddSingleton<AccountValidationResponses, AccountValidationResponses>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Com.Xpresspayments.AVS.API", Version = "v1" });
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://example.com",
                                            "http://www.contoso.com");
                    });
            });
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
     
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Com.Xpresspayments.AVS.API v1"));

            app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthorization();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
