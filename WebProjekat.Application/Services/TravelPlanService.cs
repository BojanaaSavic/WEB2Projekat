using WebProjekat.Application.DTOs;
using WebProjekat.Application.Interfaces;
using WebProjekat.Domain.Entities;
using WebProjekat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProjekat.Application.Services
{
    public class TravelPlanService : ITravelPlanService
    {
        private readonly ApplicationDbContext _context;

        public TravelPlanService(ApplicationDbContext context)
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
                .Include(p => p.Expenses)
                .Include(p => p.Destinations)
                .Include(p => p.Activities)
                .Include(p => p.ChecklistItems)
                .ToListAsync();

            return plans.Select(MapToDto);
        }

        public async Task<TravelPlanDTO> GetByIdAsync(Guid id, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .Include(p => p.Expenses)
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
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);
            if (plan == null) return false;

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
                    .ThenInclude(p => p.Expenses)
                .Include(s => s.TravelPlan)
                    .ThenInclude(p => p.ChecklistItems)
                .FirstOrDefaultAsync(s => s.Token == token && s.IsValid());

            if (shared == null) return (null, null);

            return (MapToDto(shared.TravelPlan), shared.AccessLevel.ToString());
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

        public async Task<bool> UpdateDestinationAsync(Guid planId, Guid destinationId, DestinationDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return false;

            var dest = await _context.Destinations.FirstOrDefaultAsync(d => d.Id == destinationId && d.TravelPlanId == planId);
            if (dest == null) return false;

            dest.Update(dto.Name, dto.Location, dto.ArrivalDate, dto.DepartureDate, dto.Description);
            _context.Destinations.Update(dest);
            await _context.SaveChangesAsync();
            return true;
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

        public async Task<bool> UpdateActivityAsync(Guid planId, Guid activityId, ActivityDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return false;

            var act = await _context.Activities.FirstOrDefaultAsync(a => a.Id == activityId && a.TravelPlanId == planId);
            if (act == null) return false;

            act.Update(dto.Name, dto.ScheduledAt, dto.Location, dto.Description, dto.EstimatedCost, dto.Status);
            _context.Activities.Update(act);
            await _context.SaveChangesAsync();
            return true;
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

        // Expense
        public async Task<ExpenseDTO> AddExpenseAsync(Guid planId, ExpenseDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var expense = new Expense(plan.Id, dto.Name, dto.Category, dto.Amount, dto.Date, dto.Description);
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            dto.Id = expense.Id;
            return dto;
        }

        public async Task<bool> UpdateExpenseAsync(Guid planId, Guid expenseId, ExpenseDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return false;

            var exp = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.TravelPlanId == planId);
            if (exp == null) return false;

            exp.Update(dto.Name, dto.Category, dto.Amount, dto.Date, dto.Description);
            _context.Expenses.Update(exp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveExpenseAsync(Guid planId, Guid expenseId, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return false;

            var exp = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.TravelPlanId == planId);
            if (exp == null) return false;

            _context.Expenses.Remove(exp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ExpenseDTO> GetExpenseAsync(Guid planId, Guid expenseId, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var exp = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.TravelPlanId == planId);
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

        public async Task<IEnumerable<ExpenseDTO>> GetExpensesAsync(Guid planId, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .Include(p => p.Expenses)
                .FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);

            if (plan == null) return null;

            return plan.Expenses.Select(e => new ExpenseDTO
            {
                Id = e.Id,
                Name = e.Name,
                Category = e.Category,
                Amount = e.Amount,
                Date = e.Date,
                Description = e.Description
            }).ToList();
        }

        // Checklist
        public async Task<ChecklistItemDTO> AddChecklistItemAsync(Guid planId, ChecklistItemDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return null;

            var item = new ChecklistItem(plan.Id, dto.Name, dto.IsCompleted);
            _context.ChecklistItems.Add(item);
            await _context.SaveChangesAsync();
            dto.Id = item.Id;
            return dto;
        }

        public async Task<bool> UpdateChecklistItemAsync(Guid planId, Guid checklistItemId, ChecklistItemDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId && p.OwnerId == ownerId);
            if (plan == null) return false;

            var item = await _context.ChecklistItems.FirstOrDefaultAsync(c => c.Id == checklistItemId && c.TravelPlanId == planId);
            if (item == null) return false;

            item.Update(dto.Name, dto.IsCompleted);
            _context.ChecklistItems.Update(item);
            await _context.SaveChangesAsync();
            return true;
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
                Name = item.Name,
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
                Name = c.Name,
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
        private TravelPlanDTO MapToDto(TravelPlan p)
        {
            var totalExpenses = p.Expenses?.Sum(e => e.Amount) ?? 0m;
            var remaining = p.PlannedBudget - totalExpenses;

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
                TotalExpenses = totalExpenses,
                RemainingBudget = remaining,
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
                Expenses = p.Expenses?.Select(e => new ExpenseDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Category = e.Category,
                    Amount = e.Amount,
                    Date = e.Date,
                    Description = e.Description
                }).ToList(),
                ChecklistItems = p.ChecklistItems?.Select(c => new ChecklistItemDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsCompleted = c.IsCompleted
                }).ToList()
            };
        }
    }
}