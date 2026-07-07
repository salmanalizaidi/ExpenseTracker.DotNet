using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<ICategoryService, CategoryService>();
        
        return services;
    }
}