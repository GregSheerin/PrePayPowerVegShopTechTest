using CsvHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrePay.VegetableShop.API.ServiceCollectionExtensions;
using System.Net;

namespace PrePay.VegetableShop.API
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
            services.AddDomainServices();
            services.AddDatabaseRepository();
            services.AddControllers(opt => opt.AddSylvanCsvFormatters()); //Add in the csv formatter from the Sylvan package
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering(); //incase Csv being posted are large, buffing enables 30k + file sizes to be stored on the disc. 
                return next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //Enable swagger to serve as a entry point to test during development
                app.UseSwagger();
                app.UseSwaggerUI();

                //TODO : seperate into its own class
                app.UseExceptionHandler(c => c.Run(async context =>
                {
                    var ex = context.Features
                        .Get<IExceptionHandlerPathFeature>()
                        .Error;

                    if (ex.GetType() == typeof(CsvHelperException))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync("Error: Data is incorrect, please check that all products order are available from the store");
                    }

                    var response = new { error = ex.Message };
                    await context.Response.WriteAsync(response.error);
                }));
            }

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
