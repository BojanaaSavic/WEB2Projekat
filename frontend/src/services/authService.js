import api from './api';
import { User, AuthResponse } from '../models/User';

export const authService = {
  login: async (email, password) => {
    try {
      const response = await api.post('/api/auth/login', { email, password });
      const { token, userId, name, email: userEmail, role } = response.data;
      return new AuthResponse(token, new User(userId, name, userEmail, role || 'User'));
    } catch (error) {
      throw error.response?.data?.message || 'Login failed';
    }
  },

  register: async (name, email, password) => {
    try {
      const response = await api.post('/api/auth/register', { name, email, password });
      const { token, userId, name: userName, email: userEmail, role } = response.data;
      return new AuthResponse(token, new User(userId, userName, userEmail, role || 'User'));
    } catch (error) {
      throw error.response?.data?.message || 'Registration failed';
    }
  },

  getCurrentUser: async () => {
    try {
      const response = await api.get('/api/auth/me');
      const user = response.data;
      return new User(user.id, user.name, user.email, user.role);
    } catch (error) {
      const data = error.response?.data;
      throw typeof data === 'string' ? data : data?.message || 'Failed to get user';
    }
  },

  // Logout (client-side only, just clear localStorage)
  logout: () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('authUser');
  },
};
