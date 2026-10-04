using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Common;

public abstract class BaseService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected Guid CurrentUserId =>
        Guid.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);
}