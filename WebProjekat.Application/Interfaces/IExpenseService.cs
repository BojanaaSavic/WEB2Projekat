using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebProjekat.Application.DTOs;

namespace WebProjekat.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseDTO> AddExpenseAsync(Guid planId, ExpenseDTO dto);
        Task<ExpenseDTO> GetExpenseAsync(Guid planId, Guid expenseId);
        Task<IEnumerable<ExpenseDTO>> GetExpensesAsync(Guid planId);
        Task<ExpenseDTO> UpdateExpenseAsync(Guid planId, Guid expenseId, ExpenseDTO dto);
        Task<bool> RemoveExpenseAsync(Guid planId, Guid expenseId);
        Task<decimal> GetTotalExpensesAsync(Guid planId);
        Task<IEnumerable<ExpenseCategoryDTO>> GetExpensesByCategoryAsync(Guid planId);
        Task RemoveAllExpensesForPlanAsync(Guid planId);
    }
}