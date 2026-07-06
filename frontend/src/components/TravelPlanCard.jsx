import { useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { TravelPlanContext } from '../context/TravelPlanContext';
import '../styles/TravelPlanCard.css';
import { travelPlanService } from '../services/travelPlanService';
import { locationIcon, calendarIcon, expenseIcon } from '../assets';

export default function TravelPlanCard({ plan }) {
  const navigate = useNavigate();
  const { deletePlan } = useContext(TravelPlanContext);

    const handleDelete = async (e) => {
        e.stopPropagation();
        if (window.confirm(`Delete "${plan.name}"?`)) {
            try {
                await travelPlanService.deletePlan(plan.id);
                deletePlan(plan.id);
            } catch (err) {
                console.error('Failed to delete plan:', err);
            }
        }
    };

  const daysUntilStart = plan.getDaysUntilStart();
  const totalExpenses = plan.totalExpenses ?? plan.getTotalExpenses();
  const remainingBudget = plan.remainingBudget ?? plan.getRemainingBudget();

  return (
    <div
      className="travel-plan-card"
      onClick={() => navigate(`/plan/${plan.id}`)}
    >
      <div className="card-header">
        <h3>{plan.name}</h3>
        <button
          className="btn-close"
          onClick={handleDelete}
          title="Delete plan"
        >
          ✕
        </button>
      </div>

      <p className="card-description">{plan.description}</p>

      <div className="card-stats">
        <div className="stat">
          <span className="stat-label">Duration</span>
          <span className="stat-value">
            {Math.ceil(
              (new Date(plan.endDate) - new Date(plan.startDate)) / (1000 * 60 * 60 * 24)
            ) + 1} days
          </span>
        </div>
        <div className="stat">
          <span className="stat-label">Days Until</span>
          <span className="stat-value">
            {daysUntilStart > 0 ? daysUntilStart : 'Ongoing'}
          </span>
        </div>
      </div>

      <div className="card-budget">
        <div className="budget-label">Budget</div>
        <div className="budget-bar">
          <div
            className="budget-used"
            style={{
              width: `${Math.min((totalExpenses / plan.plannedBudget) * 100, 100)}%`,
            }}
          />
        </div>
        <div className="budget-info">
          <span>${totalExpenses.toFixed(2)} of ${plan.plannedBudget.toFixed(2)}</span>
          <span className={remainingBudget < 0 ? 'negative' : ''}>
            ${Math.abs(remainingBudget).toFixed(2)} {remainingBudget < 0 ? 'over' : 'remaining'}
          </span>
        </div>
      </div>

      <div className="card-items">
        <span>
          <img src={locationIcon} alt="Destinations" className="card-icon" />
          {plan.destinations?.length || 0} Destinations
        </span>
        <span>
          <img src={calendarIcon} alt="Activities" className="card-icon" />
          {plan.activities?.length || 0} Activities
        </span>
        <span>
          <img src={expenseIcon} alt="Expenses" className="card-icon" />
          {plan.expenses?.length || 0} Expenses
        </span>
      </div>

      <button className="card-action-button">View Details →</button>
    </div>
  );
}
