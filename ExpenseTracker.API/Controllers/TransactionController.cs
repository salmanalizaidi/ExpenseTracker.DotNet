using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionController: ControllerBase
{
    private readonly ITransactionService _transactionService;
    
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransactionDto>>> GetAllAsync()
    {
        var response = await _transactionService.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetByIdAsync(Guid id)
    {
        var response = await _transactionService.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> CreateAsync(CreateTransactionDto dto)
    {
        var response = await _transactionService.CreateAsync(dto);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TransactionDto>> UpdateAsync(Guid id, UpdateTransactionDto dto)
    {
        var response = await _transactionService.UpdateAsync(id, dto);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await _transactionService.DeleteAsync(id);
        return NoContent();
    }
    
}