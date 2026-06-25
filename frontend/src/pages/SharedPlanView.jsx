import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { sharingService } from '../services/sharingService';

export default function SharedPlanView() {
  const { token } = useParams();
  const [plan, setPlan] = useState(null);
  const [accessLevel, setAccessLevel] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchSharedPlan();
  }, [token]);

  const fetchSharedPlan = async () => {
    try {
      const data = await sharingService.getSharedPlan(token);
      setPlan(data.plan);
      setAccessLevel(data.accessLevel);
    } catch (err) {
      setError('This link is invalid or has expired.');
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className="loading">Loading shared plan...</div>;
  if (error) return <div className="error-message">{error}</div>;

  return (
    <div className="plan-details">
      <div className="plan-header">
        <h1>{plan.name}</h1>
        <span className={`access-badge access-${accessLevel?.toLowerCase()}`}>
          {accessLevel === 'View' ? '👁 View Only' : '✏️ Can Edit'}
        </span>
      </div>

      <div className="plan-info-banner">
        <div className="info-item">
          <span className="info-label">Duration</span>
          <span className="info-value">
            {new Date(plan.startDate).toLocaleDateString()} - {new Date(plan.endDate).toLocaleDateString()}
          </span>
        </div>
        <div className="info-item">
          <span className="info-label">Budget</span>
          <span className="info-value">${plan.plannedBudget?.toFixed(2)}</span>
        </div>
        <div className="info-item">
          <span className="info-label">Spent</span>
          <span className="info-value">${plan.totalExpenses?.toFixed(2)}</span>
        </div>
        <div className="info-item">
          <span className="info-label">Remaining</span>
          <span className="info-value">${plan.remainingBudget?.toFixed(2)}</span>
        </div>
      </div>

      {plan.description && (
        <div className="overview-card">
          <h3>Description</h3>
          <p>{plan.description}</p>
        </div>
      )}

      {plan.destinations?.length > 0 && (
        <div className="overview-card">
          <h3>Destinations</h3>
          {plan.destinations.map(d => (
            <div key={d.id} className="destination-card">
              <h4>{d.name}</h4>
              <p>{d.location}</p>
              <p>{new Date(d.arrivalDate).toLocaleDateString()} - {new Date(d.departureDate).toLocaleDateString()}</p>
            </div>
          ))}
        </div>
      )}

      {plan.activities?.length > 0 && (
        <div className="overview-card">
          <h3>Activities</h3>
          {plan.activities
            .sort((a, b) => new Date(a.scheduledAt) - new Date(b.scheduledAt))
            .map(a => (
              <div key={a.id} className="activity-card">
                <h4>{a.name}</h4>
                <p>{new Date(a.scheduledAt).toLocaleString()}</p>
                {a.location && <p>{a.location}</p>}
              </div>
            ))}
        </div>
      )}

      {plan.checklistItems?.length > 0 && (
        <div className="overview-card">
          <h3>Checklist</h3>
          {plan.checklistItems.map(item => (
            <div key={item.id} className="checklist-item">
              <span>{item.isCompleted ? '✅' : '⬜'} {item.name}</span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}