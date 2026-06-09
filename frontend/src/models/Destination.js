// Destination model
export class Destination {
  constructor(
    id,
    travelPlanId,
    name,
    location,
    arrivalDate,
    departureDate,
    description = ''
  ) {
    this.id = id;
    this.travelPlanId = travelPlanId;
    this.name = name;
    this.location = location;
    this.arrivalDate = arrivalDate;
    this.departureDate = departureDate;
    this.description = description;
  }
}

export class CreateDestinationRequest {
  constructor(
    travelPlanId,
    name,
    location,
    arrivalDate,
    departureDate,
    description = ''
  ) {
    this.travelPlanId = travelPlanId;
    this.name = name;
    this.location = location;
    this.arrivalDate = arrivalDate;
    this.departureDate = departureDate;
    this.description = description;
  }
}

export class UpdateDestinationRequest {
  constructor(
    id,
    name,
    location,
    arrivalDate,
    departureDate,
    description = ''
  ) {
    this.id = id;
    this.name = name;
    this.location = location;
    this.arrivalDate = arrivalDate;
    this.departureDate = departureDate;
    this.description = description;
  }
}
