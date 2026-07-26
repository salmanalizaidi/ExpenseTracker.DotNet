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
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, 
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllAsync() =>
        (await _transactionRepository.GetByUserIdAsync(CurrentUserId)).Adapt<IEnumerable<TransactionDto>>();

    public async Task<TransactionDto?> GetByIdAsync(Guid id) =>
        (await _transactionRepository.GetByIdAsync(id))?.Adapt<TransactionDto>();

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
    {
        var transaction = dto.Adapt<Transaction>();
        transaction.UserId = CurrentUserId;
        await _transactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        return transaction.Adapt<TransactionDto>();
    }

    public async Task<TransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id)
                          ?? throw new KeyNotFoundException($"Transaction {id} not found");
        dto.Adapt(transaction);
        await _transactionRepository.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        return transaction.Adapt<TransactionDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _transactionRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}