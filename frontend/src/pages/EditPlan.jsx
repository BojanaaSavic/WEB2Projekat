import { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { TravelPlanContext } from '../context/TravelPlanContext';
import { travelPlanService } from '../services/travelPlanService';
import '../styles/Dashboard.css';

export default function EditPlan() {
  const { planId } = useParams();
  const navigate = useNavigate();
  const { updatePlan } = useContext(TravelPlanContext);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    startDate: '',
    endDate: '',
    plannedBudget: 0,
    notes: '',
  });

    const fetchPlan = async () => {
        try {
            const plan = await travelPlanService.getPlan(planId);
            setFormData({
                name: plan.name,
                description: plan.description || '',
                startDate: plan.startDate.substring(0, 10),
                endDate: plan.endDate.substring(0, 10),
                plannedBudget: plan.plannedBudget,
                notes: plan.notes || '',
            });
        } catch (err) {
            setError('Failed to load travel plan.', err);
        } finally {
            setLoading(false);
        }
    };

  useEffect(() => {
      const loadPlans = async () => {
          await fetchPlan();
      };

      loadPlans();
  }, [planId]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    try {
      const updated = await travelPlanService.updatePlan(planId, {
        name: formData.name,
        description: formData.description,
        startDate: formData.startDate,
        endDate: formData.endDate,
        plannedBudget: parseFloat(formData.plannedBudget),
        notes: formData.notes,
      });
      updatePlan(updated);
      navigate(`/plan/${planId}`);
    } catch (err) {
      setError(typeof err === 'string' ? err : 'Failed to update travel plan.');
    }
  };

  if (loading) return <div className="loading">Loading...</div>;

  return (
    <div className="dashboard">
      <div className="dashboard-header">
        <h1>Edit Travel Plan</h1>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="create-form-container">
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="name">Plan Name *</label>
              <input
                id="name"
                type="text"
                name="name"
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
              value={formData.notes}
              onChange={handleInputChange}
              rows="3"
            />
          </div>

          <div style={{ display: 'flex', gap: '1rem' }}>
            <button type="submit" className="btn-primary">Save Changes</button>
            <button
              type="button"
              className="btn-secondary"
              onClick={() => navigate(`/plan/${planId}`)}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}