import api from './api';
import { SharedPlanToken } from '../models/SharedPlanToken';

export const sharingService = {
  // Create shared token for plan
    createSharedToken: async (planId, createRequest) => {
        try {
            const accessParam = createRequest.accessType === 'EDIT' ? 'Edit' : 'View';

            const response = await api.post(`/api/travelplans/${planId}/share?access=${accessParam}&validDays=7`);

            return {
                id: crypto.randomUUID(),
                travelPlanId: planId,
                token: response.data.token,
                accessType: createRequest.accessType,
                createdAt: new Date().toISOString(),
                expiresAt: null
            };
        } catch (error) {
            throw error.response?.data?.message || 'Failed to create shared token';
        }
    },

  // Get all shared tokens for plan
  getSharedTokens: async (planId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/share`);
      return response.data.map(t => new SharedPlanToken(
        t.id,
        t.travelPlanId,
        t.token,
        t.accessType,
        t.createdAt,
        t.expiresAt
      ));
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch shared tokens';
    }
  },

  // Delete shared token
  deleteSharedToken: async (planId, tokenId) => {
    try {
      await api.delete(`/api/travelplans/${planId}/share/${tokenId}`);
    } catch (error) {
      throw error.response?.data?.message || 'Failed to delete shared token';
    }
  },

  // Access shared plan by token
  getSharedPlan: async (token) => {
    try {
      const response = await api.get(`/api/travelplans/shared/${token}`);
      return response.data;
    } catch (error) {
      throw error.response?.data?.message || 'Invalid or expired token';
    }
  },

  // Generate QR code for token
  generateQRCode: async (token) => {
    try {
        const response = await api.get(`/api/travelplans/shared/${token}/qr`, {
        responseType: 'blob',
      });
      return response.data;
    } catch (error) {
      throw error.response?.data?.message || 'Failed to generate QR code';
    }
  },
};
