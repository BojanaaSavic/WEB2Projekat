import { useState, useEffect, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import { TravelPlanContext } from '../context/TravelPlanContext';
import { travelPlanService } from '../services/travelPlanService';
import { CreateTravelPlanRequest } from '../models/TravelPlan';
import TravelPlanCard from '../components/TravelPlanCard';
import '../styles/Dashboard.css';

export default function Dashboard() {
  const { user, isAuthenticated } = useContext(AuthContext);
  const { travelPlans, setPlans, addPlan } = useContext(TravelPlanContext);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    startDate: '',
    endDate: '',
    plannedBudget: 0,
    notes: '',
  });
  const navigate = useNavigate();

    const fetchTravelPlans = async () => {
        setLoading(true);
        try {
            const plans = await travelPlanService.getAllPlans();
            setPlans(plans);
        } catch (err) {
            setPlans([]);
            console.log(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (!isAuthenticated) {
            navigate('/login');
            return;
        }

        const loadPlans = async () => {
            await fetchTravelPlans();
        };

        loadPlans();
    }, [isAuthenticated, navigate]);

  const handleCreatePlan = async (e) => {
    e.preventDefault();
    setError('');

    const today = new Date().toISOString().split('T')[0];

    if (!formData.name.trim()) {
      setError('Travel plan name is required.');
      return;
    }

    if (!formData.startDate) {
      setError('Start date is required.');
      return;
    }

    if (!formData.endDate) {
      setError('End date is required.');
      return;
    }

    if (formData.startDate < today) {
      setError('Start date cannot be in the past.');
      return;
    }

    if (formData.endDate < formData.startDate) {
      setError('End date cannot be before start date.');
      return;
    }

    try {
      const createRequest = new CreateTravelPlanRequest(
        formData.name,
        formData.description,
        formData.startDate,
        formData.endDate,
        parseFloat(formData.plannedBudget),
        formData.notes
      );

      const newPlan = await travelPlanService.createPlan(createRequest);
      addPlan(newPlan);
      setFormData({
        name: '',
        description: '',
        startDate: '',
        endDate: '',
        plannedBudget: 0,
        notes: '',
      });
      setShowCreateForm(false);
    } catch (err) {
      setError(err.message || 'Failed to create travel plan');
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  if (loading) {
    return <div className="loading">Loading travel plans...</div>;
  }

  return (
    <div className="dashboard">
      <div className="dashboard-header">
        <h1>Welcome, {user?.name}!</h1>
        <button className="btn-primary" onClick={() => setShowCreateForm(!showCreateForm)}>
          {showCreateForm ? 'Cancel' : '+ New Travel Plan'}
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      {showCreateForm && (
        <div className="create-form-container">
          <form onSubmit={handleCreatePlan}>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="name">Plan Name *</label>
                <input
                  id="name"
                  type="text"
                  name="name"
                  placeholder="e.g., Summer Europe Trip"
                  value={formData.name}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="description">Description</label>
                <input
                  id="description"
                  type="text"
                  name="description"
                  placeholder="Brief description of your trip"
                  value={formData.description}
                  onChange={handleInputChange}
                />
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label htmlFor="startDate">Start Date *</label>
                <input
                  id="startDate"
                  type="date"
                  name="startDate"
                  value={formData.startDate}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="endDate">End Date *</label>
                <input
                  id="endDate"
                  type="date"
                  name="endDate"
                  value={formData.endDate}
                  onChange={handleInputChange}
                  required
                />
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label htmlFor="plannedBudget">Planned Budget</label>
                <input
                  id="plannedBudget"
                  type="number"
                  name="plannedBudget"
                  placeholder="0"
                  value={formData.plannedBudget}
                  onChange={handleInputChange}
                  min="0"
                  step="0.01"
                />
              </div>
            </div>

            <div className="form-group">
              <label htmlFor="notes">Notes</label>
              <textarea
                id="notes"
                name="notes"
                placeholder="Add any additional notes about your trip"
                value={formData.notes}
                onChange={handleInputChange}
                rows="3"
              />
            </div>

            <button type="submit" className="btn-primary">Create Travel Plan</button>
          </form>
        </div>
      )}

      <div className="travel-plans-container">
        {travelPlans.length === 0 ? (
          <div className="empty-state">
            <p>No travel plans yet. Create one to get started!</p>
          </div>
        ) : (
          <div className="travel-plans-grid">
            {travelPlans.map(plan => (
              <TravelPlanCard key={plan.id} plan={plan} />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
