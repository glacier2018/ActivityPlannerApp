using Application.Activities;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService
        (
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddControllers( opt => 
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                opt.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            services.AddDbContext<DataContext>(opt =>
               opt.UseSqlite(config.GetConnectionString("DefaultConnection")));

            services.AddMediatR(typeof(GetAll.Handler).Assembly);
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            return services;
        }
    }
}