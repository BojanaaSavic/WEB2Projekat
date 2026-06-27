using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace WebProjekat.Application.Interfaces
{
    public interface IExpenseRemotingService : IService
    {
        Task DeleteExpensesByPlanAsync(Guid planId);
        Task<decimal> GetTotalExpensesForPlanAsync(Guid planId);
    }
}