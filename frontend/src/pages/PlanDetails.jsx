import { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { TravelPlanContext } from '../context/TravelPlanContext';
import { travelPlanService } from '../services/travelPlanService';
import '../styles/PlanDetails.css';
import { exportPlanToPdf } from '../services/pdfExportService';

export default function PlanDetails() {
  const { planId } = useParams();
  const navigate = useNavigate();
  const { selectPlan } = useContext(TravelPlanContext);
  const [plan, setPlan] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState('overview');

    const fetchPlan = async () => {
        setLoading(true);
        try {
            const fetchedPlan = await travelPlanService.getPlan(planId);
            setPlan(fetchedPlan);
            selectPlan(fetchedPlan);
        } catch (err) {
            setError(err.message || 'Failed to fetch travel plan');
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

  const handleDeletePlan = async () => {
    if (window.confirm('Are you sure you want to delete this travel plan? This action cannot be undone.')) {
      try {
        await travelPlanService.deletePlan(planId);
        navigate('/dashboard');
      } catch (err) {
        setError(err.message || 'Failed to delete travel plan');
      }
    }
    };

    const handleExportPdf = () => {
        try {
            exportPlanToPdf(plan);
        } catch (err) {
            setError('Greška pri generisanju PDF-a.', err);
        }
    };

  if (loading) {
    return <div className="loading">Loading travel plan...</div>;
  }

  if (!plan) {
    return <div className="error-message">Travel plan not found</div>;
  }

  const totalExpenses = plan.totalExpenses ?? 0;
  const remainingBudget = plan.remainingBudget ?? 0;
  const daysUntilStart = Math.ceil((new Date(plan.startDate) - new Date()) / (1000 * 60 * 60 * 24));

  return (
    <div className="plan-details">
      <div className="plan-header">
        <div className="plan-title">
          <h1>{plan.name}</h1>
          <p className="plan-description">{plan.description}</p>
        </div>
        <div className="plan-actions">
          <button className="btn-secondary" onClick={handleExportPdf}>Export PDF</button>
          <Link to={`/plan/${planId}/edit`} className="btn-secondary">Edit</Link>
          <button className="btn-danger" onClick={handleDeletePlan}>Delete</button>
        </div>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="plan-info-banner">
        <div className="info-item">
          <span className="info-label">Trip Duration</span>
          <span className="info-value">
            {new Date(plan.startDate).toLocaleDateString()} - {new Date(plan.endDate).toLocaleDateString()}
          </span>
        </div>
        <div className="info-item">
          <span className="info-label">Days Until Start</span>
          <span className="info-value">{daysUntilStart > 0 ? daysUntilStart : 'Trip Started'} days</span>
        </div>
        <div className="info-item">
          <span className="info-label">Budget</span>
          <span className="info-value">${plan.plannedBudget.toFixed(2)}</span>
        </div>
        <div className="info-item">
          <span className="info-label">Spent</span>
          <span className="info-value">${totalExpenses.toFixed(2)}</span>
        </div>
        <div className="info-item">
          <span className="info-label">Remaining</span>
          <span className={`info-value ${remainingBudget < 0 ? 'negative' : ''}`}>
            ${remainingBudget.toFixed(2)}
          </span>
        </div>
      </div>

      <div className="plan-tabs">
        <button
          className={`tab-button ${activeTab === 'overview' ? 'active' : ''}`}
          onClick={() => setActiveTab('overview')}
        >
          Overview
        </button>
        <button
          className={`tab-button ${activeTab === 'destinations' ? 'active' : ''}`}
          onClick={() => setActiveTab('destinations')}
        >
          Destinations ({plan.destinations?.length || 0})
        </button>
        <button
          className={`tab-button ${activeTab === 'activities' ? 'active' : ''}`}
          onClick={() => setActiveTab('activities')}
        >
          Activities ({plan.activities?.length || 0})
        </button>
        <button
          className={`tab-button ${activeTab === 'expenses' ? 'active' : ''}`}
          onClick={() => setActiveTab('expenses')}
        >
          Expenses ({plan.expenses?.length || 0})
        </button>
        <button
          className={`tab-button ${activeTab === 'checklist' ? 'active' : ''}`}
          onClick={() => setActiveTab('checklist')}
        >
          Checklist ({plan.checklistItems?.length || 0})
        </button>
        <button
          className={`tab-button ${activeTab === 'sharing' ? 'active' : ''}`}
          onClick={() => setActiveTab('sharing')}
        >
          Share
        </button>
      </div>

      <div className="tab-content">
        {activeTab === 'overview' && (
          <div className="overview-tab">
            <h2>Trip Overview</h2>
            <div className="overview-grid">
              <div className="overview-card">
                <h3>Notes</h3>
                <p>{plan.notes || 'No notes added'}</p>
              </div>
              <div className="overview-card">
                <h3>Quick Links</h3>
                <ul>
                  <li><Link to={`/plan/${planId}/destinations`}>Manage Destinations</Link></li>
                  <li><Link to={`/plan/${planId}/calendar`}>View Calendar</Link></li>
                  <li><Link to={`/plan/${planId}/expenses`}>Track Expenses</Link></li>
                  <li><Link to={`/plan/${planId}/checklist`}>Packing List</Link></li>
                </ul>
              </div>
            </div>
          </div>
        )}

        {activeTab === 'destinations' && (
          <div className="destinations-tab">
            <Link to={`/plan/${planId}/destinations`} className="btn-primary">
              Manage Destinations
            </Link>
          </div>
        )}

        {activeTab === 'activities' && (
          <div className="activities-tab">
            <Link to={`/plan/${planId}/calendar`} className="btn-primary">
              View Calendar & Activities
            </Link>
          </div>
        )}

        {activeTab === 'expenses' && (
          <div className="expenses-tab">
            <Link to={`/plan/${planId}/expenses`} className="btn-primary">
              Manage Expenses
            </Link>
          </div>
        )}

        {activeTab === 'checklist' && (
          <div className="checklist-tab">
            <Link to={`/plan/${planId}/checklist`} className="btn-primary">
              Manage Checklist
            </Link>
          </div>
        )}

        {activeTab === 'sharing' && (
          <div className="sharing-tab">
            <Link to={`/plan/${planId}/share`} className="btn-primary">
              Share Travel Plan
            </Link>
          </div>
        )}
      </div>
    </div>
  );
}
