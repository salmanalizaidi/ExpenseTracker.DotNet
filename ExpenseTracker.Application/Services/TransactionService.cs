using ExpenseTracker.Application.Common;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Services;

public class TransactionService : BaseService, ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateTransactionDto> _createValidator;
    private readonly IValidator<UpdateTransactionDto> _updateValidator;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IValidator<CreateTransactionDto> createValidator,
        IValidator<UpdateTransactionDto> updateValidator) : base(httpContextAccessor)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllAsync() =>
        (await _transactionRepository.GetByUserIdAsync(CurrentUserId)).Adapt<IEnumerable<TransactionDto>>();

    public async Task<TransactionDto?> GetByIdAsync(Guid id) =>
        (await _transactionRepository.GetByIdAsync(id))?.Adapt<TransactionDto>();

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var transaction = dto.Adapt<Transaction>();
        transaction.UserId = CurrentUserId;
        await _transactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        return transaction.Adapt<TransactionDto>();
    }

    public async Task<TransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var transaction = await _transactionRepository.GetByIdAsync(id)
                          ?? throw new KeyNotFoundException($"Transaction {id} not found");

        // Ownership check — prevent users from editing other users' transactions
        if (transaction.UserId != CurrentUserId)
            throw new UnauthorizedAccessException("You do not have permission to update this transaction.");

        dto.Adapt(transaction);
        await _transactionRepository.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        return transaction.Adapt<TransactionDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id)
                          ?? throw new KeyNotFoundException($"Transaction {id} not found");

        // Ownership check — prevent users from deleting other users' transactions
        if (transaction.UserId != CurrentUserId)
            throw new UnauthorizedAccessException("You do not have permission to delete this transaction.");

        await _transactionRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
