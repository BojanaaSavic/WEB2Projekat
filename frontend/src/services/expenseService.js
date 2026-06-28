import api from './api';
import { Expense } from '../models/Expense';

export const expenseService = {
    getExpenses: async (planId) => {
        try {
            const response = await api.get(`/api/travel-plans/${planId}/expenses`);
            return response.data.map(e => new Expense(
                e.id, e.travelPlanId, e.name, e.category, e.amount, e.date, e.description
            ));
        } catch (error) {
            throw error.response?.data?.message || 'Failed to fetch expenses';
        }
    },

    getExpense: async (planId, expenseId) => {
        try {
            const response = await api.get(`/api/travel-plans/${planId}/expenses/${expenseId}`);
            const e = response.data;
            return new Expense(e.id, e.travelPlanId, e.name, e.category, e.amount, e.date, e.description);
        } catch (error) {
            throw error.response?.data?.message || 'Failed to fetch expense';
        }
    },

    createExpense: async (planId, createRequest) => {
        try {
            const response = await api.post(`/api/travel-plans/${planId}/expenses`, createRequest);
            const e = response.data;
            return new Expense(e.id, e.travelPlanId, e.name, e.category, e.amount, e.date, e.description);
        } catch (error) {
            throw error.response?.data?.message || 'Failed to create expense';
        }
    },

    updateExpense: async (planId, expenseId, updateRequest) => {
        try {
            const response = await api.put(`/api/travel-plans/${planId}/expenses/${expenseId}`, updateRequest);
            const e = response.data;
            return new Expense(e.id, e.travelPlanId, e.name, e.category, e.amount, e.date, e.description);
        } catch (error) {
            throw error.response?.data?.message || 'Failed to update expense';
        }
    },

    deleteExpense: async (planId, expenseId) => {
        try {
            await api.delete(`/api/travel-plans/${planId}/expenses/${expenseId}`);
        } catch (error) {
            throw error.response?.data?.message || 'Failed to delete expense';
        }
    },

    getTotalExpenses: async (planId) => {
        try {
            const response = await api.get(`/api/travel-plans/${planId}/expenses/total`);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to fetch total expenses';
        }
    },

    getExpensesByCategory: async (planId) => {
        try {
            const response = await api.get(`/api/travel-plans/${planId}/expenses/by-category`);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to fetch expenses by category';
        }
    },
};