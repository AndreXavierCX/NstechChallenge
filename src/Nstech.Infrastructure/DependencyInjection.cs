using Nstech.Application.Abstractions.Data;
using Nstech.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nstech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNstechInfrastructure(this IServiceCollection services, IConfiguration configuration, bool addOutboxPublisher, bool addRabbitConsumer)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
