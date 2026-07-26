using ExpenseTracker.Application.Common;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Services;

public class TransactionService: BaseService, ITransactionService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IExpenseRepository expenseRepository, IUnitOfWork unitOfWork, 
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllAsync() =>
        (await _expenseRepository.GetByUserIdAsync(CurrentUserId)).Adapt<IEnumerable<TransactionDto>>();

    public async Task<TransactionDto?> GetByIdAsync(Guid id) =>
        (await _expenseRepository.GetByIdAsync(id))?.Adapt<TransactionDto>();

    public async Task<TransactionDto> CreateAsync(Guid userId, CreateTransactionDto dto)
    {
        var expense = dto.Adapt<Transaction>();
        expense.UserId = userId;
        await _expenseRepository.AddAsync(expense);
        await _unitOfWork.SaveChangesAsync();
        return expense.Adapt<TransactionDto>();
    }

    public async Task<TransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto)
    {
        var expense = await _expenseRepository.GetByIdAsync(id)
                      ?? throw new KeyNotFoundException($"Expense {id} not found");
        dto.Adapt(expense);
        await _expenseRepository.UpdateAsync(expense);
        await _unitOfWork.SaveChangesAsync();
        return expense.Adapt<TransactionDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _expenseRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}