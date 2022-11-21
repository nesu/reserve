using System.IO;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Reserve.API.Attributes;
using Reserve.API.Configurations;
using Reserve.API.Filters;
using Reserve.API.Requests.Behaviours;
using Reserve.API.Services;
using Reserve.Data;

namespace Reserve.API
{
    public class Startup
    {
        private readonly IConfiguration _configurations;

        public Startup(IConfiguration configurations)
        {
            _configurations = configurations;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMySQL(_configurations);
            services.AddCors();
            
            services.AddAutoMapper(typeof(Startup));
            services.AddValidatorsFromAssemblyContaining<Startup>();
            
            services.AddMediatR(typeof(Startup));
            
            services.AddScoped(typeof(ValidationHelpers));
            services.AddScoped(typeof(Settings));
            services.AddScoped(typeof(Availability));
            
            services.AddTransient<IValidatorFactory, ServiceProviderValidatorFactory>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            services.AddHttpContextAccessor();
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
            });
            
            services.AddJWTAuthentication(_configurations);

            services.AddSingleton<RazorMailing>();
            services
                .AddFluentEmail("reserve.demonstrations@gmail.com")
                .AddMailtrapSender("7f0088477f8529", "6954a4f8d155c0", "smtp.mailtrap.io", 25);
        }

        public void Configure(IApplicationBuilder builder, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                builder.UseDeveloperExceptionPage();

            builder
                .UseCors(cors =>
                {
                    cors
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
                
                
            builder.UseHttpsRedirection();
            builder.UseRouting();
            builder.UseAuthentication();
            builder.UseAuthorization();
            
            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}