// Travel Plan model
export class TravelPlan {
  constructor(
    id,
    name,
    description,
    startDate,
    endDate,
    plannedBudget,
    notes,
    ownerId,
    destinations = [],
    activities = [],
    expenses = [],
    checklistItems = []
  ) {
    this.id = id;
    this.name = name;
    this.description = description;
    this.startDate = startDate;
    this.endDate = endDate;
    this.plannedBudget = plannedBudget;
    this.notes = notes;
    this.ownerId = ownerId;
    this.destinations = destinations;
    this.activities = activities;
    this.expenses = expenses;
    this.checklistItems = checklistItems;
  }

  // Calculate total expenses
  getTotalExpenses() {
    return this.expenses.reduce((sum, expense) => sum + expense.amount, 0);
  }

  // Calculate remaining budget
  getRemainingBudget() {
    return this.plannedBudget - this.getTotalExpenses();
  }

  // Get days until start
  getDaysUntilStart() {
    const today = new Date();
    const start = new Date(this.startDate);
    const diffTime = start - today;
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }
}

export class CreateTravelPlanRequest {
  constructor(name, description, startDate, endDate, plannedBudget, notes = '') {
    this.name = name;
    this.description = description;
    this.startDate = startDate;
    this.endDate = endDate;
    this.plannedBudget = plannedBudget;
    this.notes = notes;
  }
}

export class UpdateTravelPlanRequest {
  constructor(id, name, description, startDate, endDate, plannedBudget, notes = '') {
    this.id = id;
    this.name = name;
    this.description = description;
    this.startDate = startDate;
    this.endDate = endDate;
    this.plannedBudget = plannedBudget;
    this.notes = notes;
  }
}
