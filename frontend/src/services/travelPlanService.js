import api from './api';
import { TravelPlan } from '../models/TravelPlan';

export const travelPlanService = {
  // Get all travel plans for current user
  getAllPlans: async () => {
    try {
      const response = await api.get('/api/travelplans');
      return response.data.map(p => new TravelPlan(
        p.id,
        p.name,
        p.description,
        p.startDate,
        p.endDate,
        p.plannedBudget,
        p.notes,
        p.ownerId,
        p.destinations || [],
        p.activities || [],
        p.expenses || [],
        p.checklistItems || []
      ));
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch travel plans';
    }
  },

  // Get single travel plan
  getPlan: async (planId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}`);
      const p = response.data;
      return new TravelPlan(
        p.id,
        p.name,
        p.description,
        p.startDate,
        p.endDate,
        p.plannedBudget,
        p.notes,
        p.ownerId,
        p.destinations || [],
        p.activities || [],
        p.expenses || [],
        p.checklistItems || []
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch travel plan';
    }
  },

  // Create new travel plan
  createPlan: async (createRequest) => {
    try {
      const response = await api.post('/api/travelplans', createRequest);
      const p = response.data;
      return new TravelPlan(
        p.id,
        p.name,
        p.description,
        p.startDate,
        p.endDate,
        p.plannedBudget,
        p.notes,
        p.ownerId,
        p.destinations || [],
        p.activities || [],
        p.expenses || [],
        p.checklistItems || []
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to create travel plan';
    }
  },

  // Update travel plan
  updatePlan: async (planId, updateRequest) => {
    try {
      const response = await api.put(`/api/travelplans/${planId}`, updateRequest);
      const p = response.data;
      return new TravelPlan(
        p.id,
        p.name,
        p.description,
        p.startDate,
        p.endDate,
        p.plannedBudget,
        p.notes,
        p.ownerId,
        p.destinations || [],
        p.activities || [],
        p.expenses || [],
        p.checklistItems || []
      );
    } catch (error) {
      const data = error.response?.data;
      throw typeof data === 'string' ? data : data?.message || 'Failed to update travel plan';
    }
  },

  // Delete travel plan
  deletePlan: async (planId) => {
    try {
      await api.delete(`/api/travelplans/${planId}`);
    } catch (error) {
      throw error.response?.data?.message || 'Failed to delete travel plan';
    }
  },
};
