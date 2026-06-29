using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
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
    public class TravelPlanService : ITravelPlanService
    {
        private readonly TravelPlanDbContext _context;

        public TravelPlanService(TravelPlanDbContext context)
        {
            _context = context;
        }

        public async Task<TravelPlanDTO> CreateAsync(CreateTravelPlanDTO dto, Guid ownerId)
        {
            if (dto.EndDate < dto.StartDate) throw new ArgumentException("End date cannot be before start date.");
            if (dto.PlannedBudget < 0) throw new ArgumentException("Budget cannot be negative.");

            var plan = new TravelPlan(dto.Name, dto.Description, dto.StartDate, dto.EndDate, dto.PlannedBudget, dto.Notes, ownerId);
            _context.TravelPlans.Add(plan);
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                Console.WriteLine(
                    $"{entry.Entity.GetType().Name} - {entry.State}"
                );
            }
            await _context.SaveChangesAsync();

            return MapToDto(plan);
        }

        public async Task<IEnumerable<TravelPlanDTO>> GetAllForUserAsync(Guid ownerId)
        {
            var plans = await _context.TravelPlans
                .Where(p => p.OwnerId == ownerId)
                .Include(p => p.Destinations)
                .Include(p => p.Activities)
                .Include(p => p.ChecklistItems)
                .ToListAsync();

            return plans.Select(MapToDto);
        }

        public async Task<TravelPlanDTO> GetByIdAsync(Guid id, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .Include(p => p.Destinations)
                .Include(p => p.Activities)
                .Include(p => p.ChecklistItems)
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);

            if (plan == null) return null;

            return MapToDto(plan);
        }

        public async Task<TravelPlanDTO> UpdateAsync(Guid id, CreateTravelPlanDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);
            if (plan == null) return null;

            plan.Update(dto.Name, dto.Description, dto.StartDate, dto.EndDate, dto.PlannedBudget, dto.Notes, ownerId);
            _context.TravelPlans.Update(plan);
            await _context.SaveChangesAsync();
            return MapToDto(plan);
        }



        public async Task<bool> DeleteAsync(Guid id, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);
            if (plan == null) return false;

            try
            {
                var expenseService = ServiceProxy.Create<IExpenseRemotingService>(
                    new Uri("fabric:/WebProjekat/ExpenseService"),
                    new ServicePartitionKey());
                await expenseService.DeleteExpensesByPlanAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not delete expenses via remoting: {ex.Message}");
            }

            _context.TravelPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> SharePlanAsync(Guid id, Guid ownerId, string accessLevel, TimeSpan validFor)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);
            if (plan == null) return null;

            if (!Enum.TryParse<ShareAccessLevel>(accessLevel, true, out var level))
                level = ShareAccessLevel.View;

            var token = new SharedPlanToken(id, level, validFor);
            _context.Add(token);
            await _context.SaveChangesAsync();
            return token.Token;
        }

        public async Task<(TravelPlanDTO Plan, string AccessLevel)> GetByShareTokenAsync(string token)
        {
            var shared = await _context.SharedPlanTokens
                .Include(s => s.TravelPlan)
                    .ThenInclude(p => p.Destinations)
                .Include(s => s.TravelPlan)
                    .ThenInclude(p => p.Activities)
                .Include(s => s.TravelPlan)
                    .ThenInclude(p => p.ChecklistItems)
                .FirstOrDefaultAsync(s => s.Token == token && s.ExpiresAt >= DateTime.UtcNow);

            if (shared == null) return (null, null);

            return (MapToDto(shared.TravelPlan), shared.AccessLevel.ToString());
        }

        public async Task<IEnumerable<SharedTokenDTO>> GetSharedTokensAsync(Guid planId, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var tokens = await _context.SharedPlanTokens
                .Where(t => t.TravelPlanId == planId && t.ExpiresAt >= DateTime.UtcNow)
                .ToListAsync();

            return tokens.Select(t => new SharedTokenDTO
            {
                Id = t.Id,
                Token = t.Token,
                AccessType = t.AccessLevel.ToString(),
                CreatedAt = t.CreatedAt,
                ExpiresAt = t.ExpiresAt
            });
        }

        public async Task<bool> DeleteSharedTokenAsync(Guid planId, Guid tokenId, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return false;

            var token = await _context.SharedPlanTokens
                .FirstOrDefaultAsync(t => t.Id == tokenId && t.TravelPlanId == planId);
            if (token == null) return false;

            _context.SharedPlanTokens.Remove(token);
            await _context.SaveChangesAsync();
            return true;
        }

        // Destination
        public async Task<DestinationDTO> AddDestinationAsync(Guid planId, DestinationDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var dest = new Destination(plan.Id, dto.Name, dto.Location, dto.ArrivalDate, dto.DepartureDate, dto.Description);
            _context.Destinations.Add(dest);
            await _context.SaveChangesAsync();
            dto.Id = dest.Id;
            return dto;
        }

        public async Task<DestinationDTO> UpdateDestinationAsync(Guid planId, Guid destinationId, DestinationDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var dest = await _context.Destinations.FirstOrDefaultAsync(d => d.Id == destinationId && d.TravelPlanId == planId);
            if (dest == null) return null;

            dest.Update(dto.Name, dto.Location, dto.ArrivalDate, dto.DepartureDate, dto.Description);
            _context.Destinations.Update(dest); //posibly won't need check with db
            await _context.SaveChangesAsync();
            return new DestinationDTO
            {
                Id = dest.Id,
                Name = dest.Name,
                Location = dest.Location,
                ArrivalDate = dest.ArrivalDate,
                DepartureDate = dest.DepartureDate,
                Description = dest.Description
            };
        }

        public async Task<bool> RemoveDestinationAsync(Guid planId, Guid destinationId, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return false;

            var dest = await _context.Destinations.FirstOrDefaultAsync(d => d.Id == destinationId && d.TravelPlanId == planId);
            if (dest == null) return false;

            _context.Destinations.Remove(dest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DestinationDTO> GetDestinationAsync(Guid planId, Guid destinationId, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var dest = await _context.Destinations.FirstOrDefaultAsync(d => d.Id == destinationId && d.TravelPlanId == planId);
            if (dest == null) return null;

            return new DestinationDTO
            {
                Id = dest.Id,
                Name = dest.Name,
                Location = dest.Location,
                ArrivalDate = dest.ArrivalDate,
                DepartureDate = dest.DepartureDate,
                Description = dest.Description
            };
        }

        public async Task<IEnumerable<DestinationDTO>> GetDestinationsAsync(Guid planId, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .Include(p => p.Destinations)
                .FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);

            if (plan == null) return null;

            return plan.Destinations.Select(d => new DestinationDTO
            {
                Id = d.Id,
                Name = d.Name,
                Location = d.Location,
                ArrivalDate = d.ArrivalDate,
                DepartureDate = d.DepartureDate,
                Description = d.Description
            }).ToList();
        }

        // Activity
        public async Task<ActivityDTO> AddActivityAsync(Guid planId, ActivityDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var activity = new Activity(plan.Id, dto.Name, dto.ScheduledAt, dto.Location, dto.Description, dto.EstimatedCost, dto.Status);
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            dto.Id = activity.Id;
            return dto;
        }

        public async Task<ActivityDTO> UpdateActivityAsync(Guid planId, Guid activityId, ActivityDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var act = await _context.Activities.FirstOrDefaultAsync(a => a.Id == activityId && a.TravelPlanId == planId);
            if (act == null) return null;

            act.Update(dto.Name, dto.ScheduledAt, dto.Location, dto.Description, dto.EstimatedCost, dto.Status);
            _context.Activities.Update(act);
            await _context.SaveChangesAsync();
            return new ActivityDTO
            {
                Id = act.Id,
                Name = act.Name,
                ScheduledAt = act.ScheduledAt,
                Location = act.Location,
                Description = act.Description,
                EstimatedCost = act.EstimatedCost,
                Status = act.Status
            };
        }

        public async Task<bool> RemoveActivityAsync(Guid planId, Guid activityId, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return false;

            var act = await _context.Activities.FirstOrDefaultAsync(a => a.Id == activityId && a.TravelPlanId == planId);
            if (act == null) return false;

            _context.Activities.Remove(act);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ActivityDTO> GetActivityAsync(Guid planId, Guid activityId, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var act = await _context.Activities.FirstOrDefaultAsync(a => a.Id == activityId && a.TravelPlanId == planId);
            if (act == null) return null;

            return new ActivityDTO
            {
                Id = act.Id,
                Name = act.Name,
                ScheduledAt = act.ScheduledAt,
                Location = act.Location,
                Description = act.Description,
                EstimatedCost = act.EstimatedCost,
                Status = act.Status
            };
        }

        public async Task<IEnumerable<ActivityDTO>> GetActivitiesAsync(Guid planId, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);

            if (plan == null) return null;

            return plan.Activities.Select(a => new ActivityDTO
            {
                Id = a.Id,
                Name = a.Name,
                ScheduledAt = a.ScheduledAt,
                Location = a.Location,
                Description = a.Description,
                EstimatedCost = a.EstimatedCost,
                Status = a.Status
            }).ToList();
        }

        // Checklist
        public async Task<ChecklistItemDTO> AddChecklistItemAsync(Guid planId, ChecklistItemDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var item = new ChecklistItem(plan.Id, dto.Title, dto.IsCompleted, dto.Description);
            _context.ChecklistItems.Add(item);
            await _context.SaveChangesAsync();
            dto.Id = item.Id;
            return dto;
        }

        public async Task<ChecklistItemDTO> UpdateChecklistItemAsync(Guid planId, Guid checklistItemId, ChecklistItemDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var item = await _context.ChecklistItems.FirstOrDefaultAsync(c => c.Id == checklistItemId && c.TravelPlanId == planId);
            if (item == null) return null;

            item.Update(dto.Title, dto.IsCompleted, dto.Description);
            _context.ChecklistItems.Update(item);
            await _context.SaveChangesAsync();
            return new ChecklistItemDTO
            {
                Id = item.Id,
                Title = item.Title,
                IsCompleted = item.IsCompleted,
                Description = item.Description,
            };
        }

        public async Task<bool> RemoveChecklistItemAsync(Guid planId, Guid checklistItemId, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return false;

            var item = await _context.ChecklistItems.FirstOrDefaultAsync(c => c.Id == checklistItemId && c.TravelPlanId == planId);
            if (item == null) return false;

            _context.ChecklistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ChecklistItemDTO> GetChecklistItemAsync(Guid planId, Guid checklistItemId, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var item = await _context.ChecklistItems.FirstOrDefaultAsync(c => c.Id == checklistItemId && c.TravelPlanId == planId);
            if (item == null) return null;

            return new ChecklistItemDTO
            {
                Id = item.Id,
                Title = item.Title,
                IsCompleted = item.IsCompleted
            };
        }

        public async Task<IEnumerable<ChecklistItemDTO>> GetChecklistItemsAsync(Guid planId, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .Include(p => p.ChecklistItems)
                .FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);

            if (plan == null) return null;

            return plan.ChecklistItems.Select(c => new ChecklistItemDTO
            {
                Id = c.Id,
                Title = c.Title,
                IsCompleted = c.IsCompleted
            }).ToList();
        }

        public async Task<IEnumerable<ActivityCalendarDayDTO>> GetActivitiesCalendarAsync(Guid planId, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);

            if (plan == null) return null;

            var grouped = plan.Activities
                .OrderBy(a => a.ScheduledAt)
                .GroupBy(a => a.ScheduledAt.Date)
                .Select(g => new ActivityCalendarDayDTO
                {
                    Date = g.Key,
                    Activities = g.Select(a => new ActivityDTO
                    {
                        Id = a.Id,
                        Name = a.Name,
                        ScheduledAt = a.ScheduledAt,
                        Location = a.Location,
                        Description = a.Description,
                        EstimatedCost = a.EstimatedCost,
                        Status = a.Status
                    }).ToList()
                })
                .OrderBy(d => d.Date)
                .ToList();

            return grouped;
        }

        public async Task<IEnumerable<AdminTravelPlanDTO>> GetAllPlansAdminAsync()
        {
            var plans = await _context.TravelPlans
                .Include(p => p.Destinations)
                .Include(p => p.Activities)
                .Include(p => p.ChecklistItems)
                .ToListAsync();

            return plans.Select(p => new AdminTravelPlanDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                PlannedBudget = p.PlannedBudget,
                OwnerName = p.OwnerId.ToString(), // OwnerId jer nemamo pristup UserService bazi
                OwnerEmail = "",
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<bool> DeletePlanAdminAsync(Guid planId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(p => p.Id == planId);
            if (plan == null) return false;

            // Briši troškove iz ExpenseService via remoting
            try
            {
                var expenseService = ServiceProxy.Create<IExpenseRemotingService>(
                    new Uri("fabric:/WebProjekat/ExpenseService"),
                    new ServicePartitionKey());
                await expenseService.DeleteExpensesByPlanAsync(planId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not delete expenses via remoting: {ex.Message}");
            }

            _context.TravelPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AdminStatisticsDTO> GetAdminStatisticsAsync()
        {
            var totalPlans = await _context.TravelPlans.CountAsync();
            var totalDestinations = await _context.Destinations.CountAsync();
            var totalActivities = await _context.Activities.CountAsync();

            decimal totalExpenses = 0;
            try
            {
                var planIds = await _context.TravelPlans
                    .Select(p => p.Id)
                    .ToListAsync();

                var expenseService = ServiceProxy.Create<IExpenseRemotingService>(
                    new Uri("fabric:/WebProjekat/ExpenseService"),
                    new ServicePartitionKey());

                foreach (var planId in planIds)
                {
                    totalExpenses += await expenseService.GetTotalExpensesForPlanAsync(planId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not fetch total expenses: {ex.Message}");
            }

            return new AdminStatisticsDTO
            {
                TotalPlans = totalPlans,
                TotalDestinations = totalDestinations,
                TotalActivities = totalActivities,
                TotalExpenses = totalExpenses
            };
        }
        private TravelPlanDTO MapToDto(TravelPlan p)
        {
            var activityCosts = p.Activities?.Sum(a => a.EstimatedCost) ?? 0;

            return new TravelPlanDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                PlannedBudget = p.PlannedBudget,
                Notes = p.Notes,
                OwnerId = p.OwnerId,
                TotalExpenses = activityCosts, 
                RemainingBudget = p.PlannedBudget - activityCosts,
                Destinations = p.Destinations?.Select(d => new DestinationDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Location = d.Location,
                    ArrivalDate = d.ArrivalDate,
                    DepartureDate = d.DepartureDate,
                    Description = d.Description
                }).ToList(),
                Activities = p.Activities?.Select(a => new ActivityDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    ScheduledAt = a.ScheduledAt,
                    Location = a.Location,
                    Description = a.Description,
                    EstimatedCost = a.EstimatedCost,
                    Status = a.Status
                }).ToList(),
                Expenses = new List<ExpenseDTO>(),
                ChecklistItems = p.ChecklistItems?.Select(c => new ChecklistItemDTO
                {
                    Id = c.Id,
                    Title = c.Title,
                    IsCompleted = c.IsCompleted
                }).ToList()
            };
        }
    }
}