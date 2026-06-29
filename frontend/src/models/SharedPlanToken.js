// Shared Plan Token model for sharing with QR code
export const AccessType = {
  VIEW: 'View',
  EDIT: 'Edit',
};

export class SharedPlanToken {
  constructor(
    id,
    travelPlanId,
    token,
    accessType,
    createdAt,
    expiresAt = null
  ) {
    this.id = id;
    this.travelPlanId = travelPlanId;
    this.token = token;
    this.accessType = accessType;
    this.createdAt = createdAt;
    this.expiresAt = expiresAt;
  }
}

export class CreateSharedPlanTokenRequest {
  constructor(
    travelPlanId,
    accessType = AccessType.VIEW
  ) {
    this.travelPlanId = travelPlanId;
    this.accessType = accessType;
  }
}
