export const ExpenseCategories = {
  TRANSPORT: 'Transport',
  ACCOMMODATION: 'Accommodation',
  FOOD: 'Food',
  TICKETS: 'Tickets',
  SHOPPING: 'Shopping',
  OTHER: 'Other',
};

export class Expense {
  constructor(
    id,
    travelPlanId,
    name,
    category,
    amount,
    date,
    description = ''
  ) {
    this.id = id;
    this.travelPlanId = travelPlanId;
    this.name = name;
    this.category = category;
    this.amount = amount;
    this.date = date;
    this.description = description;
  }
}

export class CreateExpenseRequest {
  constructor(
    travelPlanId,
    name,
    category,
    amount,
    date,
    description = ''
  ) {
    this.travelPlanId = travelPlanId;
    this.name = name;
    this.category = category;
    this.amount = amount;
    this.date = date;
    this.description = description;
  }
}

export class UpdateExpenseRequest {
  constructor(
    id,
    name,
    category,
    amount,
    date,
    description = ''
  ) {
    this.id = id;
    this.name = name;
    this.category = category;
    this.amount = amount;
    this.date = date;
    this.description = description;
  }
}
