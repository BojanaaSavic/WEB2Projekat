import { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { sharingService } from '../services/sharingService';
import { AuthContext } from '../context/AuthContext';

export default function SharedPlanView() {
    const { token } = useParams();
    const navigate = useNavigate();
    const { isAuthenticated } = useContext(AuthContext);
    const [plan, setPlan] = useState(null);
    const [accessLevel, setAccessLevel] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const fetchSharedPlan = async () => {
        try {
            const data = await sharingService.getSharedPlan(token);
            setPlan(data.plan);
            setAccessLevel(data.accessLevel);

            if (data.accessLevel === 'Edit' && !isAuthenticated) {
                navigate(`/login?redirect=/shared/${token}`);
                return;
            }
        } catch (err) {
            setError('This link is invalid or has expired.', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const loadPlans = async () => {
            await fetchSharedPlan();
        };

        loadPlans();
    }, [token, isAuthenticated]);

    if (loading) return <div className="loading">Loading shared plan...</div>;
    if (error) return <div className="error-message">{error}</div>;

    const canEdit = accessLevel === 'Edit' && isAuthenticated;

    return (
        <div className="plan-details">
            <div className="plan-header">
                <h1>{plan.name}</h1>
                <span className={`access-badge access-${accessLevel?.toLowerCase()}`}>
                    {canEdit ? '✏️ Can Edit' : '👁 View Only'}
                </span>
                {accessLevel === 'Edit' && !isAuthenticated && (
                    <div className="error-message">
                        You must be logged in to edit this plan.{' '}
                        <button
                            className="btn-primary"
                            onClick={() => navigate(`/login?redirect=/shared/${token}`)}
                        >
                            Log in
                        </button>
                    </div>
                )}
            </div>

            <div className="plan-info-banner">
                <div className="info-item">
                    <span className="info-label">Duration</span>
                    <span className="info-value">
                        {new Date(plan.startDate).toLocaleDateString()} -{' '}
                        {new Date(plan.endDate).toLocaleDateString()}
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
                            <p>
                                {new Date(d.arrivalDate).toLocaleDateString()} -{' '}
                                {new Date(d.departureDate).toLocaleDateString()}
                            </p>
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
                            {/* Popravka: item.title umjesto item.name */}
                            <span>{item.isCompleted ? '✅' : '⬜'} {item.title}</span>
                        </div>
                    ))}
                </div>
            )}

            {canEdit && (
                <div className="overview-card">
                    <h3>Edit Access</h3>
                    <p>You have edit access to this plan. Contact the owner to make changes directly.</p>
                </div>
            )}
        </div>
    );
}