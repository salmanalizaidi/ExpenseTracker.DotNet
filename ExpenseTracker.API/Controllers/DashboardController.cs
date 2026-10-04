using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Returns all data needed by the dashboard UI in a single call:
    /// total balance, monthly spending vs budget, active savings goal,
    /// category distribution for the current month, and recent transactions.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDashboard()
        => Ok(await _dashboardService.GetDashboardAsync());

    /// <summary>
    /// Returns the N most recent transactions for the current user.
    /// Useful for refreshing just the Recent Transactions widget independently.
    /// </summary>
    [HttpGet("transactions/recent")]
    public async Task<IActionResult> GetRecentTransactions([FromQuery] int limit = 5)
        => Ok(await _dashboardService.GetRecentTransactionsAsync(limit));
}
