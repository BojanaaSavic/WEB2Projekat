using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace WebProjekat.Application.Interfaces
{
    public interface ITravelPlanRemotingService : IService
    {
        Task DeletePlansByOwnerAsync(Guid ownerId);
    }
}