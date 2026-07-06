import api from './api';
import { Activity } from '../models/Activity';

export const activityService = {
  getActivities: async (planId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/activities`);
      return response.data.map(a => new Activity(
        a.id,
        a.travelPlanId,
        a.name,
        a.scheduledAt,
        a.location,
        a.description,
        a.estimatedCost,
        a.status
      ));
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch activities';
    }
  },

  getActivity: async (planId, activityId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/activities/${activityId}`);
      const a = response.data;
      return new Activity(
        a.id,
        a.travelPlanId,
        a.name,
        a.scheduledAt,
        a.location,
        a.description,
        a.estimatedCost,
        a.status
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch activity';
    }
  },

  createActivity: async (planId, createRequest) => {
    try {
      const response = await api.post(`/api/travelplans/${planId}/activities`, createRequest);
      const a = response.data;
      return new Activity(
        a.id,
        a.travelPlanId,
        a.name,
        a.scheduledAt,
        a.location,
        a.description,
        a.estimatedCost,
        a.status
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to create activity';
    }
  },

  updateActivity: async (planId, activityId, updateRequest) => {
    try {
      const response = await api.put(
        `/api/travelplans/${planId}/activities/${activityId}`,
        updateRequest
      );
      const a = response.data;
      return new Activity(
        a.id,
        a.travelPlanId,
        a.name,
        a.scheduledAt,
        a.location,
        a.description,
        a.estimatedCost,
        a.status
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to update activity';
    }
  },

  deleteActivity: async (planId, activityId) => {
    try {
      await api.delete(`/api/travelplans/${planId}/activities/${activityId}`);
    } catch (error) {
      throw error.response?.data?.message || 'Failed to delete activity';
    }
  },

    getActivitiesByDate: async (planId, date) => {
        try {
            const response = await api.get(`/api/travelplans/${planId}/activities/calendar`);

            return response.data
                .filter(day => new Date(day.date).toDateString() === new Date(date).toDateString())
                .flatMap(day => day.activities)
                .map(a => new Activity(
                    a.id, a.travelPlanId, a.name, a.scheduledAt,
                    a.location, a.description, a.estimatedCost, a.status
                ));
        } catch (error) {
            throw error.response?.data?.message || 'Failed to fetch activities for date';
        }
    },
};
