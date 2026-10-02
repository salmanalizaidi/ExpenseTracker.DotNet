using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Data;

/// <summary>
/// Seeds the database with a realistic demo user and related data.
/// Run via: dotnet run --project ExpenseTracker.API -- --seed
/// Idempotent — skips seeding if the demo user already exists.
/// </summary>
public class DataSeeder
{
    private readonly AppDbContext _db;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(AppDbContext db, ILogger<DataSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        const string email = "alex@demo.com";

        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            _logger.LogInformation("Demo user already exists — skipping seed.");
            return;
        }

        _logger.LogInformation("Seeding demo data...");

        // ── User ────────────────────────────────────────────────────────────
        var user = new User
        {
            Username    = "alex_demo",
            Email       = email,
            FirstName   = "Alex",
            LastName    = "Morgan",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo@1234"),
            LastLoginAt  = DateTime.UtcNow
        };
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync(); // flush so user.Id is populated

        // ── Categories ──────────────────────────────────────────────────────
        var categories = new[]
        {
            new Category { Name = "Housing",      Icon = "🏠", Color = "#4F46E5", UserId = user.Id },
            new Category { Name = "Groceries",    Icon = "🛒", Color = "#16A34A", UserId = user.Id },
            new Category { Name = "Transport",    Icon = "🚗", Color = "#CA8A04", UserId = user.Id },
            new Category { Name = "Dining Out",   Icon = "🍽️", Color = "#DC2626", UserId = user.Id },
            new Category { Name = "Bills",        Icon = "⚡", Color = "#7C3AED", UserId = user.Id },
            new Category { Name = "Travel",       Icon = "✈️", Color = "#0EA5E9", UserId = user.Id },
            new Category { Name = "Entertainment",Icon = "🎬", Color = "#F97316", UserId = user.Id },
            new Category { Name = "Salary",       Icon = "💼", Color = "#059669", UserId = user.Id },
            new Category { Name = "Freelance",    Icon = "💻", Color = "#0D9488", UserId = user.Id },
        };
        await _db.Categories.AddRangeAsync(categories);
        await _db.SaveChangesAsync();

        // Quick lookup helpers
        Category Cat(string name) => categories.First(c => c.Name == name);

        // ── Budget (current month) ───────────────────────────────────────────
        var now = DateTime.UtcNow;
        var budget = new Budget
        {
            MonthlyLimit = 4000.00m,
            Month  = now.Month,
            Year   = now.Year,
            UserId = user.Id
        };
        await _db.Budgets.AddAsync(budget);

        // ── Savings Goal ─────────────────────────────────────────────────────
        var savingsGoal = new SavingsGoal
        {
            Name          = "Vacation in Italy",
            TargetAmount  = 5000.00m,
            CurrentAmount = 3250.00m,
            Deadline      = new DateTime(now.Year + 1, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            UserId        = user.Id
        };
        await _db.SavingsGoals.AddAsync(savingsGoal);

        // ── Transactions ─────────────────────────────────────────────────────
        // Helper: build a UTC date at a given offset from today
        DateTime Ago(int days, int hour = 10) =>
            DateTime.SpecifyKind(DateTime.Today.AddDays(-days).AddHours(hour), DateTimeKind.Utc);

        var transactions = new List<Transaction>
        {
            // ── This month ──────────────────────────────────────────────────
            // Income
            new() { Title = "Monthly Salary",    Amount = 5500.00m, Type = TransactionType.Income,  Date = Ago(2),  UserId = user.Id, CategoryId = Cat("Salary").Id },
            new() { Title = "Freelance Project",  Amount = 800.00m,  Type = TransactionType.Income,  Date = Ago(5),  UserId = user.Id, CategoryId = Cat("Freelance").Id },

            // Expenses
            new() { Title = "Skyline Properties", Amount = 2100.00m, Type = TransactionType.Expense, Date = Ago(1),  UserId = user.Id, CategoryId = Cat("Housing").Id,    Description = "Monthly rent" },
            new() { Title = "Whole Foods Market",  Amount = 142.30m,  Type = TransactionType.Expense, Date = Ago(0, 8), UserId = user.Id, CategoryId = Cat("Groceries").Id },
            new() { Title = "Uber Trip",           Amount = 24.50m,   Type = TransactionType.Expense, Date = Ago(1),  UserId = user.Id, CategoryId = Cat("Transport").Id },
            new() { Title = "Clean Energy Co.",    Amount = 89.90m,   Type = TransactionType.Expense, Date = Ago(1),  UserId = user.Id, CategoryId = Cat("Bills").Id,      Description = "Electricity bill" },
            new() { Title = "Netflix",             Amount = 15.99m,   Type = TransactionType.Expense, Date = Ago(3),  UserId = user.Id, CategoryId = Cat("Entertainment").Id },
            new() { Title = "Sushi Palace",        Amount = 68.00m,   Type = TransactionType.Expense, Date = Ago(4),  UserId = user.Id, CategoryId = Cat("Dining Out").Id },
            new() { Title = "Flight to Rome",      Amount = 480.00m,  Type = TransactionType.Expense, Date = Ago(6),  UserId = user.Id, CategoryId = Cat("Travel").Id },
            new() { Title = "Metro Card Topup",    Amount = 30.00m,   Type = TransactionType.Expense, Date = Ago(7),  UserId = user.Id, CategoryId = Cat("Transport").Id },
            new() { Title = "Trader Joe's",        Amount = 96.45m,   Type = TransactionType.Expense, Date = Ago(8),  UserId = user.Id, CategoryId = Cat("Groceries").Id },
            new() { Title = "Internet Bill",       Amount = 49.99m,   Type = TransactionType.Expense, Date = Ago(9),  UserId = user.Id, CategoryId = Cat("Bills").Id },
            new() { Title = "Burger Barn",         Amount = 34.20m,   Type = TransactionType.Expense, Date = Ago(10), UserId = user.Id, CategoryId = Cat("Dining Out").Id },
            new() { Title = "Spotify",             Amount = 9.99m,    Type = TransactionType.Expense, Date = Ago(11), UserId = user.Id, CategoryId = Cat("Entertainment").Id },
            new() { Title = "Gas Station",         Amount = 55.00m,   Type = TransactionType.Expense, Date = Ago(12), UserId = user.Id, CategoryId = Cat("Transport").Id },

            // ── Last month ──────────────────────────────────────────────────
            new() { Title = "Monthly Salary",     Amount = 5500.00m, Type = TransactionType.Income,  Date = Ago(32), UserId = user.Id, CategoryId = Cat("Salary").Id },
            new() { Title = "Skyline Properties",  Amount = 2100.00m, Type = TransactionType.Expense, Date = Ago(31), UserId = user.Id, CategoryId = Cat("Housing").Id },
            new() { Title = "Whole Foods Market",  Amount = 187.60m,  Type = TransactionType.Expense, Date = Ago(30), UserId = user.Id, CategoryId = Cat("Groceries").Id },
            new() { Title = "Water Bill",          Amount = 38.00m,   Type = TransactionType.Expense, Date = Ago(29), UserId = user.Id, CategoryId = Cat("Bills").Id },
            new() { Title = "Lyft",                Amount = 19.00m,   Type = TransactionType.Expense, Date = Ago(28), UserId = user.Id, CategoryId = Cat("Transport").Id },
            new() { Title = "The Italian Place",   Amount = 82.00m,   Type = TransactionType.Expense, Date = Ago(27), UserId = user.Id, CategoryId = Cat("Dining Out").Id },
            new() { Title = "Amazon Prime",        Amount = 14.99m,   Type = TransactionType.Expense, Date = Ago(26), UserId = user.Id, CategoryId = Cat("Entertainment").Id },
            new() { Title = "Costco",              Amount = 212.00m,  Type = TransactionType.Expense, Date = Ago(25), UserId = user.Id, CategoryId = Cat("Groceries").Id },
            new() { Title = "Phone Bill",          Amount = 55.00m,   Type = TransactionType.Expense, Date = Ago(24), UserId = user.Id, CategoryId = Cat("Bills").Id },

            // ── Two months ago (for history) ────────────────────────────────
            new() { Title = "Monthly Salary",     Amount = 5500.00m, Type = TransactionType.Income,  Date = Ago(62), UserId = user.Id, CategoryId = Cat("Salary").Id },
            new() { Title = "Freelance Invoice",  Amount = 1200.00m, Type = TransactionType.Income,  Date = Ago(58), UserId = user.Id, CategoryId = Cat("Freelance").Id },
            new() { Title = "Skyline Properties",  Amount = 2100.00m, Type = TransactionType.Expense, Date = Ago(61), UserId = user.Id, CategoryId = Cat("Housing").Id },
            new() { Title = "Target Run",          Amount = 145.20m,  Type = TransactionType.Expense, Date = Ago(59), UserId = user.Id, CategoryId = Cat("Groceries").Id },
            new() { Title = "Train Ticket",        Amount = 22.00m,   Type = TransactionType.Expense, Date = Ago(57), UserId = user.Id, CategoryId = Cat("Transport").Id },
            new() { Title = "Electric Bill",       Amount = 94.00m,   Type = TransactionType.Expense, Date = Ago(56), UserId = user.Id, CategoryId = Cat("Bills").Id },
        };

        await _db.Transactions.AddRangeAsync(transactions);

        // ── Scheduled Payments ───────────────────────────────────────────────
        var scheduledPayments = new[]
        {
            new ScheduledPayment { Name = "Rent",            Amount = 2100.00m, DueDate = new DateTime(now.Year, now.Month + 1 > 12 ? 1 : now.Month + 1, 1, 0, 0, 0, DateTimeKind.Utc),  UserId = user.Id, CategoryId = Cat("Housing").Id },
            new ScheduledPayment { Name = "Spotify",         Amount = 9.99m,    DueDate = Ago(-15), UserId = user.Id, CategoryId = Cat("Entertainment").Id },
            new ScheduledPayment { Name = "Netflix",         Amount = 15.99m,   DueDate = Ago(-20), UserId = user.Id, CategoryId = Cat("Entertainment").Id },
            new ScheduledPayment { Name = "Internet Bill",   Amount = 49.99m,   DueDate = Ago(-10), UserId = user.Id, CategoryId = Cat("Bills").Id },
        };
        await _db.ScheduledPayments.AddRangeAsync(scheduledPayments);

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Seed complete. Login with email={Email} password=Demo@1234", email);
    }
}
