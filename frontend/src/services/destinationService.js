import api from './api';
import { Destination } from '../models/Destination';

export const destinationService = {
  // Get all destinations for a travel plan
  getDestinations: async (planId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/destinations`);
      return response.data.map(d => new Destination(
        d.id,
        d.travelPlanId,
        d.name,
        d.location,
        d.arrivalDate,
        d.departureDate,
        d.description
      ));
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch destinations';
    }
  },

  // Get single destination
  getDestination: async (planId, destinationId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/destinations/${destinationId}`);
      const d = response.data;
      return new Destination(
        d.id,
        d.travelPlanId,
        d.name,
        d.location,
        d.arrivalDate,
        d.departureDate,
        d.description
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch destination';
    }
  },

  // Create destination
  createDestination: async (planId, createRequest) => {
    try {
      const response = await api.post(`/api/travelplans/${planId}/destinations`, createRequest);
      const d = response.data;
      return new Destination(
        d.id,
        d.travelPlanId,
        d.name,
        d.location,
        d.arrivalDate,
        d.departureDate,
        d.description
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to create destination';
    }
  },

  // Update destination
  updateDestination: async (planId, destinationId, updateRequest) => {
    try {
      const response = await api.put(
        `/api/travelplans/${planId}/destinations/${destinationId}`,
        updateRequest
      );
      const d = response.data;
      return new Destination(
        d.id,
        d.travelPlanId,
        d.name,
        d.location,
        d.arrivalDate,
        d.departureDate,
        d.description
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to update destination';
    }
  },

  // Delete destination
  deleteDestination: async (planId, destinationId) => {
    try {
      await api.delete(`/api/travelplans/${planId}/destinations/${destinationId}`);
    } catch (error) {
      throw error.response?.data?.message || 'Failed to delete destination';
    }
  },
};
