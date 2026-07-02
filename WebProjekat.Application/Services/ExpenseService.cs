using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProjekat.Application.DTOs;
using WebProjekat.Application.Interfaces;
using WebProjekat.Domain.Entities;
using WebProjekat.Infrastructure.Persistence;

namespace WebProjekat.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ExpenseDbContext _context;

        public ExpenseService(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<ExpenseDTO> AddExpenseAsync(Guid planId, ExpenseDTO dto)
        {
            if (dto.Amount < 0) throw new ArgumentException("Amount cannot be negative.");

            var expense = new Expense(planId, dto.Name, dto.Category, dto.Amount, dto.Date, dto.Description);
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return new ExpenseDTO
            {
                Id = expense.Id,
                Name = expense.Name,
                Category = expense.Category,
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description
            };
        }

        public async Task<ExpenseDTO> GetExpenseAsync(Guid planId, Guid expenseId)
        {
            var exp = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.TravelPlanId == planId);
            if (exp == null) return null;

            return new ExpenseDTO
            {
                Id = exp.Id,
                Name = exp.Name,
                Category = exp.Category,
                Amount = exp.Amount,
                Date = exp.Date,
                Description = exp.Description
            };
        }

        public async Task<IEnumerable<ExpenseDTO>> GetExpensesAsync(Guid planId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.TravelPlanId == planId)
                .ToListAsync();

            return expenses.Select(e => new ExpenseDTO
            {
                Id = e.Id,
                Name = e.Name,
                Category = e.Category,
                Amount = e.Amount,
                Date = e.Date,
                Description = e.Description
            }).ToList();
        }

        public async Task<ExpenseDTO> UpdateExpenseAsync(Guid planId, Guid expenseId, ExpenseDTO dto)
        {
            var exp = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.TravelPlanId == planId);
            if (exp == null) return null;

            exp.Update(dto.Name, dto.Category, dto.Amount, dto.Date, dto.Description);
            _context.Expenses.Update(exp);
            await _context.SaveChangesAsync();

            return new ExpenseDTO
            {
                Id = exp.Id,
                Name = exp.Name,
                Category = exp.Category,
                Amount = exp.Amount,
                Date = exp.Date,
                Description = exp.Description
            };
        }

        public async Task<bool> RemoveExpenseAsync(Guid planId, Guid expenseId)
        {
            var exp = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.TravelPlanId == planId);
            if (exp == null) return false;

            _context.Expenses.Remove(exp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RemoveAllExpensesForPlanAsync(Guid planId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.TravelPlanId == planId)
                .ToListAsync();

            _context.Expenses.RemoveRange(expenses);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalExpensesAsync(Guid planId)
        {
            return await _context.Expenses
                .Where(e => e.TravelPlanId == planId)
                .SumAsync(e => e.Amount);
        }

        public async Task<IEnumerable<ExpenseCategoryDTO>> GetExpensesByCategoryAsync(Guid planId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.TravelPlanId == planId)
                .ToListAsync();

            return expenses
                .GroupBy(e => e.Category)
                .Select(g => new ExpenseCategoryDTO
                {
                    Category = g.Key,
                    Total = g.Sum(e => e.Amount),
                    Count = g.Count()
                }).ToList();
        }
    }
}