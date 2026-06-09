import api from './api';
import { Expense } from '../models/Expense';

export const expenseService = {
  // Get all expenses for a travel plan
  getExpenses: async (planId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/expenses`);
      return response.data.map(e => new Expense(
        e.id,
        e.travelPlanId,
        e.name,
        e.category,
        e.amount,
        e.date,
        e.description
      ));
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch expenses';
    }
  },

  // Get single expense
  getExpense: async (planId, expenseId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/expenses/${expenseId}`);
      const e = response.data;
      return new Expense(
        e.id,
        e.travelPlanId,
        e.name,
        e.category,
        e.amount,
        e.date,
        e.description
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch expense';
    }
  },

  // Create expense
  createExpense: async (planId, createRequest) => {
    try {
      const response = await api.post(`/api/travelplans/${planId}/expenses`, createRequest);
      const e = response.data;
      return new Expense(
        e.id,
        e.travelPlanId,
        e.name,
        e.category,
        e.amount,
        e.date,
        e.description
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to create expense';
    }
  },

  // Update expense
  updateExpense: async (planId, expenseId, updateRequest) => {
    try {
      const response = await api.put(
        `/api/travelplans/${planId}/expenses/${expenseId}`,
        updateRequest
      );
      const e = response.data;
      return new Expense(
        e.id,
        e.travelPlanId,
        e.name,
        e.category,
        e.amount,
        e.date,
        e.description
      );
    } catch (error) {
      throw error.response?.data?.message || 'Failed to update expense';
    }
  },

  // Delete expense
  deleteExpense: async (planId, expenseId) => {
    try {
      await api.delete(`/api/travelplans/${planId}/expenses/${expenseId}`);
    } catch (error) {
      throw error.response?.data?.message || 'Failed to delete expense';
    }
  },

  // Get total expenses
  getTotalExpenses: async (planId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/expenses/total`);
      return response.data;
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch total expenses';
    }
  },

  // Get expenses by category
  getExpensesByCategory: async (planId) => {
    try {
      const response = await api.get(`/api/travelplans/${planId}/expenses/category`);
      return response.data;
    } catch (error) {
      throw error.response?.data?.message || 'Failed to fetch expenses by category';
    }
  },
};
