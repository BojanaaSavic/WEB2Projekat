import { useState, useEffect, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import api from '../services/api';

export default function AdminPanel() {
  const { user } = useContext(AuthContext);
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('statistics');
  const [statistics, setStatistics] = useState(null);
  const [users, setUsers] = useState([]);
  const [plans, setPlans] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!user || user.role !== 'Admin') {
      navigate('/dashboard');
      return;
    }
    fetchStatistics();
  }, [user]);

  useEffect(() => {
    if (activeTab === 'statistics') fetchStatistics();
    if (activeTab === 'users') fetchUsers();
    if (activeTab === 'plans') fetchPlans();
  }, [activeTab]);

  const fetchStatistics = async () => {
    setLoading(true);
    try {
      const res = await api.get('/api/admin/statistics');
      setStatistics(res.data);
    } catch {
      setError('Failed to load statistics.');
    } finally {
      setLoading(false);
    }
  };

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const res = await api.get('/api/admin/users');
      setUsers(res.data);
    } catch {
      setError('Failed to load users.');
    } finally {
      setLoading(false);
    }
  };

  const fetchPlans = async () => {
    setLoading(true);
    try {
      const res = await api.get('/api/admin/travel-plans');
      setPlans(res.data);
    } catch {
      setError('Failed to load plans.');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteUser = async (userId) => {
    if (!window.confirm('Delete this user and all their data?')) return;
    try {
      await api.delete(`/api/admin/users/${userId}`);
      setUsers(prev => prev.filter(u => u.id !== userId));
    } catch {
      setError('Failed to delete user.');
    }
  };

  const handleDeletePlan = async (planId) => {
    if (!window.confirm('Delete this travel plan?')) return;
    try {
      await api.delete(`/api/admin/travel-plans/${planId}`);
      setPlans(prev => prev.filter(p => p.id !== planId));
    } catch {
      setError('Failed to delete plan.');
    }
  };

  return (
    <div className="dashboard">
      <div className="dashboard-header">
        <h1>Admin Panel</h1>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="plan-tabs">
        <button
          className={`tab-button ${activeTab === 'statistics' ? 'active' : ''}`}
          onClick={() => setActiveTab('statistics')}
        >
          Statistics
        </button>
        <button
          className={`tab-button ${activeTab === 'users' ? 'active' : ''}`}
          onClick={() => setActiveTab('users')}
        >
          Users
        </button>
        <button
          className={`tab-button ${activeTab === 'plans' ? 'active' : ''}`}
          onClick={() => setActiveTab('plans')}
        >
          Travel Plans
        </button>
      </div>

      {loading && <div className="loading">Loading...</div>}

      {activeTab === 'statistics' && statistics && (
        <div className="overview-grid" style={{ marginTop: '1rem' }}>
          {[
            { label: 'Total Users', value: statistics.totalUsers },
            { label: 'Total Plans', value: statistics.totalPlans },
            { label: 'Total Destinations', value: statistics.totalDestinations },
            { label: 'Total Activities', value: statistics.totalActivities },
            { label: 'Total Expenses', value: `$${statistics.totalExpenses?.toFixed(2)}` },
          ].map(stat => (
            <div key={stat.label} className="overview-card">
              <h3>{stat.label}</h3>
              <p style={{ fontSize: '2rem', fontWeight: 'bold' }}>{stat.value}</p>
            </div>
          ))}
        </div>
      )}

      {activeTab === 'users' && (
        <div className="expenses-list" style={{ marginTop: '1rem' }}>
          <div className="expenses-table">
            <div className="table-header">
              <div className="col-name">Name</div>
              <div className="col-category">Email</div>
              <div className="col-amount">Role</div>
              <div className="col-actions">Actions</div>
            </div>
            {users.map(u => (
              <div key={u.id} className="table-row">
                <div className="col-name">{u.name}</div>
                <div className="col-category">{u.email}</div>
                <div className="col-amount">{u.role}</div>
                <div className="col-actions">
                  <button
                    className="btn-danger"
                    onClick={() => handleDeleteUser(u.id)}
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {activeTab === 'plans' && (
        <div className="expenses-list" style={{ marginTop: '1rem' }}>
          <div className="expenses-table">
            <div className="table-header">
              <div className="col-name">Plan Name</div>
              <div className="col-category">Owner</div>
              <div className="col-amount">Budget</div>
              <div className="col-date">Dates</div>
              <div className="col-actions">Actions</div>
            </div>
            {plans.map(p => (
              <div key={p.id} className="table-row">
                <div className="col-name">{p.name}</div>
                <div className="col-category">{p.ownerName}</div>
                <div className="col-amount">${p.plannedBudget?.toFixed(2)}</div>
                <div className="col-date">
                  {new Date(p.startDate).toLocaleDateString()} - {new Date(p.endDate).toLocaleDateString()}
                </div>
                <div className="col-actions">
                  <button
                    className="btn-danger"
                    onClick={() => handleDeletePlan(p.id)}
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}