// Checklist Item model
export class ChecklistItem {
  constructor(
    id,
    travelPlanId,
    title,
    isCompleted = false,
    description = ''
  ) {
    this.id = id;
    this.travelPlanId = travelPlanId;
    this.title = title;
    this.isCompleted = isCompleted;
    this.description = description;
  }
}

export class CreateChecklistItemRequest {
  constructor(
    travelPlanId,
    title,
    description = ''
  ) {
    this.travelPlanId = travelPlanId;
    this.title = title;
    this.description = description;
  }
}

export class UpdateChecklistItemRequest {
  constructor(
    id,
    title,
    isCompleted,
    description = ''
  ) {
    this.id = id;
    this.title = title;
    this.isCompleted = isCompleted;
    this.description = description;
  }
}
