import api from './api';
import { SharedPlanToken } from '../models/SharedPlanToken';

export const sharingService = {
    createSharedToken: async (planId, createRequest) => {
        try {
            const accessParam = createRequest.accessType === 'Edit' ? 'Edit' : 'View';
            const response = await api.post(`/api/travelplans/${planId}/share?access=${accessParam}&validDays=7`);
            const t = response.data;
            return new SharedPlanToken(
                t.id,
                planId,
                t.token,
                t.accessType,
                t.createdAt,
                t.expiresAt
            );
        } catch (error) {
            throw error.response?.data?.message || 'Failed to create shared token';
        }
    },

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

  deleteSharedToken: async (planId, tokenId) => {
    try {
      await api.delete(`/api/travelplans/${planId}/share/${tokenId}`);
    } catch (error) {
      throw error.response?.data?.message || 'Failed to delete shared token';
    }
  },

  getSharedPlan: async (token) => {
    try {
      const response = await api.get(`/api/travelplans/shared/${token}`);
      return response.data;
    } catch (error) {
      throw error.response?.data?.message || 'Invalid or expired token';
    }
  },

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

    updateSharedPlan: async (token, dto) => {
        try {
            const response = await api.put(`/api/travelplans/shared/${token}`, dto);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to update travel plan';
        }
    },

    addSharedDestination: async (token, destination) => {
        try {
            const response = await api.post(`/api/travelplans/shared/${token}/destinations`, destination);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to add destination';
        }
    },
    updateSharedDestination: async (token, destinationId, destination) => {
        try {
            const response = await api.put(`/api/travelplans/shared/${token}/destinations/${destinationId}`, destination);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to update destination';
        }
    },

    addSharedActivity: async (token, activity) => {
        try {
            const response = await api.post(`/api/travelplans/shared/${token}/activities`, activity);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to add activity';
        }
    },
    updateSharedActivity: async (token, activityId, activity) => {
        try {
            const response = await api.put(`/api/travelplans/shared/${token}/activities/${activityId}`, activity);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to update activity';
        }
    },

    addSharedChecklistItem: async (token, item) => {
        try {
            const response = await api.post(`/api/travelplans/shared/${token}/checklist`, item);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to add checklist item';
        }
    },
    updateSharedChecklistItem: async (token, itemId, item) => {
        try {
            const response = await api.put(`/api/travelplans/shared/${token}/checklist/${itemId}`, item);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to update checklist item';
        }
    },
    deleteSharedDestination: async (token, destinationId) => {
        try {
            await api.delete(`/api/travelplans/shared/${token}/destinations/${destinationId}`);
        } catch (error) {
            throw error.response?.data?.message || 'Failed to delete destination';
        }
    },
    deleteSharedActivity: async (token, activityId) => {
        try {
            await api.delete(`/api/travelplans/shared/${token}/activities/${activityId}`);
        } catch (error) {
            throw error.response?.data?.message || 'Failed to delete activity';
        }
    },
    deleteSharedChecklistItem: async (token, itemId) => {
        try {
            await api.delete(`/api/travelplans/shared/${token}/checklist/${itemId}`);
        } catch (error) {
            throw error.response?.data?.message || 'Failed to delete checklist item';
        }
    },
};
