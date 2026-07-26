using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}