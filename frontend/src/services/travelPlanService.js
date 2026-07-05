import api from './api';
import { TravelPlan } from '../models/TravelPlan';

const getErrorMessage = (error, fallback) => {
    const data = error.response?.data;
    if (typeof data === 'string') return data;
    if (data?.message) return data.message;
    if (data?.errors) {
        const firstError = Object.values(data.errors).flat().find(Boolean);
        if (firstError) return firstError;
    }
    return fallback;
};

export const travelPlanService = {
    getAllPlans: async () => {
        try {
            const response = await api.get('/api/travelplans');
            return response.data.map(p => {
                const plan = new TravelPlan(
                    p.id, p.name, p.description, p.startDate, p.endDate,
                    p.plannedBudget, p.notes, p.ownerId,
                    p.destinations || [], p.activities || [],
                    p.expenses || [], p.checklistItems || []
                );
                plan.totalExpenses = p.totalExpenses ?? 0;
                plan.remainingBudget = p.remainingBudget ?? 0;
                return plan;
            });
        } catch (error) {
            throw getErrorMessage(error, 'Failed to fetch travel plans');
        }
    },

    getPlan: async (planId) => {
        try {
            const response = await api.get(`/api/travelplans/${planId}`);
            const p = response.data;
            const plan = new TravelPlan(
                p.id, p.name, p.description, p.startDate, p.endDate,
                p.plannedBudget, p.notes, p.ownerId,
                p.destinations || [], p.activities || [],
                p.expenses || [], p.checklistItems || []
            );
            plan.totalExpenses = p.totalExpenses ?? 0;
            plan.remainingBudget = p.remainingBudget ?? 0;
            return plan;
        } catch (error) {
            throw getErrorMessage(error, 'Failed to fetch travel plan');
        }
    },

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
      throw getErrorMessage(error, 'Failed to create travel plan');
    }
  },

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
      throw getErrorMessage(error, 'Failed to update travel plan');
    }
  },

  deletePlan: async (planId) => {
    try {
      await api.delete(`/api/travelplans/${planId}`);
    } catch (error) {
      throw getErrorMessage(error, 'Failed to delete travel plan');
    }
  },
};
