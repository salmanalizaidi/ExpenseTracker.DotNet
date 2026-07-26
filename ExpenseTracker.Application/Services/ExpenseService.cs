using ExpenseTracker.Application.Common;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Services;

public class ExpenseService: BaseService, IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExpenseService(IExpenseRepository expenseRepository, IUnitOfWork unitOfWork, 
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ExpenseDto>> GetAllAsync(Guid userId) =>
        (await _expenseRepository.GetByUserIdAsync(userId)).Adapt<IEnumerable<ExpenseDto>>();

    public async Task<ExpenseDto?> GetByIdAsync(Guid id) =>
        (await _expenseRepository.GetByIdAsync(id))?.Adapt<ExpenseDto>();

    public async Task<ExpenseDto> CreateAsync(Guid userId, CreateExpenseDto dto)
    {
        var expense = dto.Adapt<Transaction>();
        expense.UserId = userId;
        await _expenseRepository.AddAsync(expense);
        await _unitOfWork.SaveChangesAsync();
        return expense.Adapt<ExpenseDto>();
    }

    public async Task<ExpenseDto> UpdateAsync(Guid id, UpdateExpenseDto dto)
    {
        var expense = await _expenseRepository.GetByIdAsync(id)
                      ?? throw new KeyNotFoundException($"Expense {id} not found");
        dto.Adapt(expense);
        await _expenseRepository.UpdateAsync(expense);
        await _unitOfWork.SaveChangesAsync();
        return expense.Adapt<ExpenseDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _expenseRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}