using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        private readonly IHttpClientFactory _httpClientFactory;

        public TravelPlanService(TravelPlanDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TravelPlanDTO> CreateAsync(CreateTravelPlanDTO dto, Guid ownerId)
        {
            if (dto.EndDate < dto.StartDate) throw new ArgumentException("End date cannot be before start date.");
            if (dto.PlannedBudget < 0) throw new ArgumentException("Budget cannot be negative.");

            var plan = new TravelPlan(dto.Name, dto.Description, dto.StartDate, dto.EndDate, dto.PlannedBudget, dto.Notes, ownerId);
            _context.TravelPlans.Add(plan);
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(plan);
        }

        public async Task<IEnumerable<TravelPlanDTO>> GetAllForUserAsync(Guid ownerId)
        {
            var plans = await _context.TravelPlans
                .Where(p => p.OwnerId == ownerId)
                .Include(p => p.Destinations)
                .Include(p => p.Activities)
                .Include(p => p.ChecklistItems)
                .ToListAsync();

            var dtos = await Task.WhenAll(plans.Select(MapToDtoAsync));
            return dtos;
        }

        public async Task<TravelPlanDTO> GetByIdAsync(Guid id, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .Include(p => p.Destinations)
                .Include(p => p.Activities)
                .Include(p => p.ChecklistItems)
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);

            if (plan == null) return null;

            return await MapToDtoAsync(plan);
        }

        public async Task<TravelPlanDTO> UpdateAsync(Guid id, CreateTravelPlanDTO dto, Guid ownerId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);
            if (plan == null) return null;

            plan.Update(dto.Name, dto.Description, dto.StartDate, dto.EndDate, dto.PlannedBudget, dto.Notes, ownerId);
            _context.TravelPlans.Update(plan);
            await _context.SaveChangesAsync();
            return await MapToDtoAsync(plan);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid ownerId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);
            if (plan == null) return false;

            await DeleteExpensesViaGateway(id);

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

            return (await MapToDtoAsync(shared.TravelPlan), shared.AccessLevel.ToString());
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

        
        private async Task<TravelPlan> ValidateEditTokenAsync(string token)
        {
            var shared = await _context.SharedPlanTokens
                .Include(s => s.TravelPlan)
                .FirstOrDefaultAsync(s => s.Token == token);

            if (shared == null || !shared.IsValid()) return null;
            if (shared.AccessLevel != ShareAccessLevel.Edit) return null;

            return shared.TravelPlan;
        }

        public async Task<TravelPlanDTO> UpdateViaShareTokenAsync(string token, CreateTravelPlanDTO dto, Guid editorUserId)
        {
            if (dto.EndDate < dto.StartDate) throw new ArgumentException("End date cannot be before start date.");
            if (dto.PlannedBudget < 0) throw new ArgumentException("Budget cannot be negative.");

            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return null;

            plan.Update(dto.Name, dto.Description, dto.StartDate, dto.EndDate, dto.PlannedBudget, dto.Notes, plan.OwnerId);
            _context.TravelPlans.Update(plan);
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(plan);
        }

        public async Task<DestinationDTO> AddDestinationViaShareTokenAsync(string token, DestinationDTO dto)
        {
            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return null;

            var dest = new Destination(plan.Id, dto.Name, dto.Location, dto.ArrivalDate, dto.DepartureDate, dto.Description);
            _context.Destinations.Add(dest);
            await _context.SaveChangesAsync();
            dto.Id = dest.Id;
            return dto;
        }

        public async Task<DestinationDTO> UpdateDestinationViaShareTokenAsync(string token, Guid destinationId, DestinationDTO dto)
        {
            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return null;

            var dest = await _context.Destinations.FirstOrDefaultAsync(d => d.Id == destinationId && d.TravelPlanId == plan.Id);
            if (dest == null) return null;

            dest.Update(dto.Name, dto.Location, dto.ArrivalDate, dto.DepartureDate, dto.Description);
            _context.Destinations.Update(dest);
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

        public async Task<ActivityDTO> AddActivityViaShareTokenAsync(string token, ActivityDTO dto)
        {
            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return null;

            var activity = new Activity(plan.Id, dto.Name, dto.ScheduledAt, dto.Location, dto.Description, dto.EstimatedCost, dto.Status);
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            dto.Id = activity.Id;
            return dto;
        }

        public async Task<ActivityDTO> UpdateActivityViaShareTokenAsync(string token, Guid activityId, ActivityDTO dto)
        {
            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return null;

            var act = await _context.Activities.FirstOrDefaultAsync(a => a.Id == activityId && a.TravelPlanId == plan.Id);
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

        public async Task<ChecklistItemDTO> AddChecklistItemViaShareTokenAsync(string token, ChecklistItemDTO dto)
        {
            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return null;

            var item = new ChecklistItem(plan.Id, dto.Title, dto.IsCompleted, dto.Description);
            _context.ChecklistItems.Add(item);
            await _context.SaveChangesAsync();
            dto.Id = item.Id;
            return dto;
        }

        public async Task<ChecklistItemDTO> UpdateChecklistItemViaShareTokenAsync(string token, Guid checklistItemId, ChecklistItemDTO dto)
        {
            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return null;

            var item = await _context.ChecklistItems.FirstOrDefaultAsync(c => c.Id == checklistItemId && c.TravelPlanId == plan.Id);
            if (item == null) return null;

            item.Update(dto.Title, dto.IsCompleted, dto.Description);
            _context.ChecklistItems.Update(item);
            await _context.SaveChangesAsync();

            return new ChecklistItemDTO
            {
                Id = item.Id,
                Title = item.Title,
                IsCompleted = item.IsCompleted
            };
        }

        public async Task<bool> RemoveDestinationViaShareTokenAsync(string token, Guid destinationId)
        {
            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return false;

            var dest = await _context.Destinations.FirstOrDefaultAsync(d => d.Id == destinationId && d.TravelPlanId == plan.Id);
            if (dest == null) return false;

            _context.Destinations.Remove(dest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveActivityViaShareTokenAsync(string token, Guid activityId)
        {
            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return false;

            var act = await _context.Activities.FirstOrDefaultAsync(a => a.Id == activityId && a.TravelPlanId == plan.Id);
            if (act == null) return false;

            _context.Activities.Remove(act);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveChecklistItemViaShareTokenAsync(string token, Guid checklistItemId)
        {
            var plan = await ValidateEditTokenAsync(token);
            if (plan == null) return false;

            var item = await _context.ChecklistItems.FirstOrDefaultAsync(c => c.Id == checklistItemId && c.TravelPlanId == plan.Id);
            if (item == null) return false;

            _context.ChecklistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

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
            _context.Destinations.Update(dest);
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

        public async Task<IEnumerable<AdminTravelPlanDTO>> GetAllPlansAdminAsync(string authToken)
        {
            var plans = await _context.TravelPlans
                .Include(p => p.Destinations)
                .Include(p => p.Activities)
                .Include(p => p.ChecklistItems)
                .ToListAsync();

            var client = _httpClientFactory.CreateClient("Gateway");
            if (!string.IsNullOrWhiteSpace(authToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    System.Net.Http.Headers.AuthenticationHeaderValue.Parse(authToken);
            }

            var result = new List<AdminTravelPlanDTO>();

            foreach (var p in plans)
            {
                string ownerName = p.OwnerId.ToString();
                string ownerEmail = "";

                try
                {
                    var response = await client.GetAsync($"api/admin/users/{p.OwnerId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var user = await response.Content.ReadFromJsonAsync<AdminUserDTO>();
                        if (user != null)
                        {
                            ownerName = user.Name;
                            ownerEmail = user.Email;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Gateway returned {response.StatusCode} for owner {p.OwnerId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not fetch owner info for plan {p.Id}: {ex}");
                }

                result.Add(new AdminTravelPlanDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    PlannedBudget = p.PlannedBudget,
                    OwnerName = ownerName,
                    OwnerEmail = ownerEmail,
                    CreatedAt = p.CreatedAt
                });
            }

            return result;
        }

        public async Task<bool> DeletePlanAdminAsync(Guid planId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(p => p.Id == planId);
            if (plan == null) return false;

            await DeleteExpensesViaGateway(planId);

            _context.TravelPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeletePlansByOwnerAsync(Guid ownerId)
        {
            var plans = await _context.TravelPlans
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();

            foreach (var plan in plans)
            {
                await DeleteExpensesViaGateway(plan.Id);
            }

            _context.TravelPlans.RemoveRange(plans);
            await _context.SaveChangesAsync();
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

                var client = _httpClientFactory.CreateClient("Gateway");

                foreach (var planId in planIds)
                {
                    var response = await client.GetAsync($"api/travelplans/{planId}/expenses/total");
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<TotalExpenseResponse>();
                        totalExpenses += result?.Total ?? 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not fetch total expenses: {ex}");
            }

            return new AdminStatisticsDTO
            {
                TotalPlans = totalPlans,
                TotalDestinations = totalDestinations,
                TotalActivities = totalActivities,
                TotalExpenses = totalExpenses
            };
        }

        private async Task<TravelPlanDTO> MapToDtoAsync(TravelPlan p)
        {
            var activityCosts = p.Activities?.Sum(a => a.EstimatedCost) ?? 0;

            decimal expenseTotal = 0;
            List<ExpenseDTO> planExpenses = new List<ExpenseDTO>();
            try
            {
                var client = _httpClientFactory.CreateClient("Gateway");
                var response = await client.GetAsync($"api/travelplans/{p.Id}/expenses");

                if (response.IsSuccessStatusCode)
                {
                    planExpenses = await response.Content.ReadFromJsonAsync<List<ExpenseDTO>>()
                                   ?? new List<ExpenseDTO>();
                    expenseTotal = planExpenses.Sum(e => e.Amount);
                }
                else
                {
                    Console.WriteLine($"Gateway/ExpenseService returned {response.StatusCode} for plan {p.Id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not fetch expenses for plan {p.Id}: {ex}");
            }

            var totalSpent = activityCosts + expenseTotal;

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
                TotalExpenses = totalSpent,
                RemainingBudget = p.PlannedBudget - totalSpent,
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
                Expenses = planExpenses,
                ChecklistItems = p.ChecklistItems?.Select(c => new ChecklistItemDTO
                {
                    Id = c.Id,
                    Title = c.Title,
                    IsCompleted = c.IsCompleted
                }).ToList()
            };
        }

        private async Task DeleteExpensesViaGateway(Guid planId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Gateway");
                await client.DeleteAsync($"api/travelplans/{planId}/expenses");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not delete expenses for plan {planId}: {ex}");
            }
        }

        private class TotalExpenseResponse
        {
            public Guid PlanId { get; set; }
            public decimal Total { get; set; }
        }
    }
}