import api from './api';
import { ChecklistItem } from '../models/ChecklistItem';

export const checklistService = {
  // Get all checklist items for a travel plan
  getChecklistItems: async (planId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/checklist`);
      return response.data.map(i => new ChecklistItem(
        i.id,
        i.travelPlanId,
        i.title,
        i.isCompleted,
        i.description
      ));
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch checklist items';
    }
  },

  // Get single checklist item
  getChecklistItem: async (planId, itemId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/checklist/${itemId}`);
      const i = response.data;
      return new ChecklistItem(
        i.id,
        i.travelPlanId,
        i.title,
        i.isCompleted,
        i.description
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch checklist item';
    }
  },

  // Create checklist item
  createChecklistItem: async (planId, createRequest) => {
    try {
      const response = await api.post(`/api/travelplans/${planId}/checklist`, createRequest);
      const i = response.data;
      return new ChecklistItem(
        i.id,
        i.travelPlanId,
        i.title,
        i.isCompleted,
        i.description
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to create checklist item';
    }
  },

  // Update checklist item
  updateChecklistItem: async (planId, itemId, updateRequest) => {
    try {
      const response = await api.put(
        `/api/travelplans/${planId}/checklist/${itemId}`,
        updateRequest
      );
      const i = response.data;
      return new ChecklistItem(
        i.id,
        i.travelPlanId,
        i.title,
        i.isCompleted,
        i.description
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to update checklist item';
    }
  },

  // Delete checklist item
  deleteChecklistItem: async (planId, itemId) => {
    try {
      await api.delete(`/api/travelplans/${planId}/checklist/${itemId}`);
    } catch (error) {
      throw error.response?.data?.message || 'Failed to delete checklist item';
    }
  },

  // Get completion percentage
  getCompletionPercentage: async (planId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/checklist/completion`);
      return response.data;
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch completion percentage';
    }
  },
};
