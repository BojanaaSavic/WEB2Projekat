using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;
using WebProjekat.Application.DTOs;

namespace WebProjekat.Application.Interfaces
{
    public interface IExpenseRemotingService : IService
    {
        Task DeleteExpensesByPlanAsync(Guid planId);
        Task<decimal> GetTotalExpensesForPlanAsync(Guid planId);
        Task<List<ExpenseDTO>> GetExpensesForPlanAsync(Guid planId);
    }
}