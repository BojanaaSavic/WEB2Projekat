// Activity Status enum
export const ActivityStatus = {
  PLANNED: 'Planned',
  RESERVED: 'Reserved',
  COMPLETED: 'Completed',
  CANCELLED: 'Cancelled',
};

// Activity model
export class Activity {
  constructor(
    id,
    travelPlanId,
    name,
    scheduledAt,
    location,
    description,
    estimatedCost,
    status = ActivityStatus.PLANNED
  ) {
    this.id = id;
    this.travelPlanId = travelPlanId;
    this.name = name;
    this.scheduledAt = scheduledAt;
    this.location = location;
    this.description = description;
    this.estimatedCost = estimatedCost;
    this.status = status;
  }
}

export class CreateActivityRequest {
  constructor(
    travelPlanId,
    name,
    scheduledAt,
    location,
    description,
    estimatedCost,
    status = ActivityStatus.PLANNED
  ) {
    this.travelPlanId = travelPlanId;
    this.name = name;
    this.scheduledAt = scheduledAt;
    this.location = location;
    this.description = description;
    this.estimatedCost = estimatedCost;
    this.status = status;
  }
}

export class UpdateActivityRequest {
  constructor(
    id,
    name,
    scheduledAt,
    location,
    description,
    estimatedCost,
    status = ActivityStatus.PLANNED
  ) {
    this.id = id;
    this.name = name;
    this.scheduledAt = scheduledAt;
    this.location = location;
    this.description = description;
    this.estimatedCost = estimatedCost;
    this.status = status;
  }
}
