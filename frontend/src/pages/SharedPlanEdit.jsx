import { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import { sharingService } from '../services/sharingService';
import '../styles/Dashboard.css';
import '../styles/Checklist.css';

const emptyDestination = { name: '', location: '', arrivalDate: '', departureDate: '', description: '' };
const emptyActivity = { name: '', scheduledAt: '', location: '', description: '', estimatedCost: 0, status: 'Planned' };
const emptyChecklistItem = { title: '', isCompleted: false, description: '' };

export default function SharedPlanEdit() {
    const { token } = useParams();
    const navigate = useNavigate();
    const { isAuthenticated } = useContext(AuthContext);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState('');

    const [formData, setFormData] = useState({
        name: '', description: '', startDate: '', endDate: '', plannedBudget: 0, notes: '',
    });

    const [destinations, setDestinations] = useState([]);
    const [activities, setActivities] = useState([]);
    const [checklistItems, setChecklistItems] = useState([]);

    const [newDestination, setNewDestination] = useState(emptyDestination);
    const [newActivity, setNewActivity] = useState(emptyActivity);
    const [newChecklistItem, setNewChecklistItem] = useState(emptyChecklistItem);

    const [editingDestinationId, setEditingDestinationId] = useState(null);
    const [editingActivityId, setEditingActivityId] = useState(null);
    const [editingChecklistId, setEditingChecklistId] = useState(null);

    useEffect(() => {
        const loadPlan = async () => {
            try {
                const data = await sharingService.getSharedPlan(token);
                if (data.accessLevel !== 'Edit') {
                    setError('You do not have edit access to this plan.');
                    return;
                }
                const plan = data.plan;
                setFormData({
                    name: plan.name,
                    description: plan.description || '',
                    startDate: plan.startDate.substring(0, 10),
                    endDate: plan.endDate.substring(0, 10),
                    plannedBudget: plan.plannedBudget,
                    notes: plan.notes || '',
                });
                setDestinations(plan.destinations || []);
                setActivities(plan.activities || []);
                setChecklistItems(plan.checklistItems || []);
            } catch (err) {
                setError('This link is invalid or has expired.', err);
            } finally {
                setLoading(false);
            }
        };
        loadPlan();
    }, [token, isAuthenticated]);

    // ----- Osnovni podaci plana -----
    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
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

        setSaving(true);
        try {
            await sharingService.updateSharedPlan(token, {
                name: formData.name,
                description: formData.description,
                startDate: formData.startDate,
                endDate: formData.endDate,
                plannedBudget: parseFloat(formData.plannedBudget),
                notes: formData.notes,
            });
            navigate(`/shared/${token}`);
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to update travel plan.');
        } finally {
            setSaving(false);
        }
    };

    // ----- Destinacije -----
    const handleAddDestination = async () => {
        try {
            const created = await sharingService.addSharedDestination(token, newDestination);
            setDestinations(prev => [...prev, created]);
            setNewDestination(emptyDestination);
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to add destination.');
        }
    };

    const handleUpdateDestination = async (destinationId, dto) => {
        try {
            const updated = await sharingService.updateSharedDestination(token, destinationId, dto);
            setDestinations(prev => prev.map(d => d.id === destinationId ? updated : d));
            setEditingDestinationId(null);
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to update destination.');
        }
    };

    // ----- Aktivnosti -----
    const handleAddActivity = async () => {
        try {
            const created = await sharingService.addSharedActivity(token, newActivity);
            setActivities(prev => [...prev, created]);
            setNewActivity(emptyActivity);
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to add activity.');
        }
    };

    const handleUpdateActivity = async (activityId, dto) => {
        try {
            const updated = await sharingService.updateSharedActivity(token, activityId, dto);
            setActivities(prev => prev.map(a => a.id === activityId ? updated : a));
            setEditingActivityId(null);
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to update activity.');
        }
    };

    // ----- Checklist -----
    const handleAddChecklistItem = async () => {
        try {
            const created = await sharingService.addSharedChecklistItem(token, newChecklistItem);
            setChecklistItems(prev => [...prev, created]);
            setNewChecklistItem(emptyChecklistItem);
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to add checklist item.');
        }
    };

    const handleUpdateChecklistItem = async (itemId, dto) => {
        try {
            const updated = await sharingService.updateSharedChecklistItem(token, itemId, dto);
            setChecklistItems(prev => prev.map(c => c.id === itemId ? updated : c));
            setEditingChecklistId(null);
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to update checklist item.');
        }
    };

    const handleToggleChecklistItem = (item) => {
        handleUpdateChecklistItem(item.id, {
            title: item.title,
            isCompleted: !item.isCompleted,
            description: item.description,
        });
    };

    // ----- Deleting -----
    const handleDeleteDestination = async (destinationId) => {
        if (!window.confirm('Delete this destination?')) return;
        try {
            await sharingService.deleteSharedDestination(token, destinationId);
            setDestinations(prev => prev.filter(d => d.id !== destinationId));
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to delete destination.');
        }
    };

    const handleDeleteActivity = async (activityId) => {
        if (!window.confirm('Delete this activity?')) return;
        try {
            await sharingService.deleteSharedActivity(token, activityId);
            setActivities(prev => prev.filter(a => a.id !== activityId));
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to delete activity.');
        }
    };

    const handleDeleteChecklistItem = async (itemId) => {
        if (!window.confirm('Delete this checklist item?')) return;
        try {
            await sharingService.deleteSharedChecklistItem(token, itemId);
            setChecklistItems(prev => prev.filter(c => c.id !== itemId));
        } catch (err) {
            setError(typeof err === 'string' ? err : 'Failed to delete checklist item.');
        }
    };

    if (loading) return <div className="loading">Loading...</div>;
    if (error && destinations.length === 0 && activities.length === 0) {
        return <div className="error-message">{error}</div>;
    }

    return (
        <div className="dashboard">
            <div className="dashboard-header">
                <h1>Edit Shared Travel Plan</h1>
            </div>

            {error && <div className="error-message">{error}</div>}

            {/* Osnovni podaci plana */}
            <div className="create-form-container">
                <form onSubmit={handleSubmit}>
                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="name">Plan Name *</label>
                            <input id="name" type="text" name="name" value={formData.name} onChange={handleInputChange} required />
                        </div>
                        <div className="form-group">
                            <label htmlFor="description">Description</label>
                            <input id="description" type="text" name="description" value={formData.description} onChange={handleInputChange} />
                        </div>
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="startDate">Start Date *</label>
                            <input id="startDate" type="date" name="startDate" value={formData.startDate} onChange={handleInputChange} required />
                        </div>
                        <div className="form-group">
                            <label htmlFor="endDate">End Date *</label>
                            <input id="endDate" type="date" name="endDate" value={formData.endDate} onChange={handleInputChange} required />
                        </div>
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="plannedBudget">Planned Budget</label>
                            <input id="plannedBudget" type="number" name="plannedBudget" value={formData.plannedBudget} onChange={handleInputChange} min="0" step="0.01" />
                        </div>
                    </div>

                    <div className="form-group">
                        <label htmlFor="notes">Notes</label>
                        <textarea id="notes" name="notes" value={formData.notes} onChange={handleInputChange} rows="3" />
                    </div>

                    <div style={{ display: 'flex', gap: '1rem' }}>
                        <button type="submit" className="btn-primary" disabled={saving}>
                            {saving ? 'Saving...' : 'Save Changes'}
                        </button>
                        <button type="button" className="btn-secondary" onClick={() => navigate(`/shared/${token}`)}>
                            Cancel
                        </button>
                    </div>
                </form>
            </div>

            {/* Destinacije */}
            <div className="overview-card">
                <h3>Destinations</h3>
                {destinations.map(d => (
                    editingDestinationId === d.id ? (
                        <DestinationEditRow
                            key={d.id}
                            destination={d}
                            onSave={(dto) => handleUpdateDestination(d.id, dto)}
                            onCancel={() => setEditingDestinationId(null)}
                        />
                    ) : (
                        <div key={d.id} className="destination-card">
                            <h4>{d.name}</h4>
                            <p>{d.location}</p>
                            <p>{new Date(d.arrivalDate).toLocaleDateString()} - {new Date(d.departureDate).toLocaleDateString()}</p>
                            {d.description && <p>{d.description}</p>}
                            <div style={{ display: 'flex', gap: '0.5rem' }}>
                                <button className="btn-secondary" onClick={() => setEditingDestinationId(d.id)}>Edit</button>
                                <button className="btn-danger" onClick={() => handleDeleteDestination(d.id)}>Delete</button>
                            </div>
                        </div>
                    )
                ))}

                <h4>Add Destination</h4>
                <div className="form-row">
                    <div className="form-group">
                        <label>Name *</label>
                        <input value={newDestination.name} onChange={e => setNewDestination(prev => ({ ...prev, name: e.target.value }))} />
                    </div>
                    <div className="form-group">
                        <label>Location</label>
                        <input value={newDestination.location} onChange={e => setNewDestination(prev => ({ ...prev, location: e.target.value }))} />
                    </div>
                </div>
                <div className="form-row">
                    <div className="form-group">
                        <label>Arrival Date *</label>
                        <input type="date" value={newDestination.arrivalDate} onChange={e => setNewDestination(prev => ({ ...prev, arrivalDate: e.target.value }))} />
                    </div>
                    <div className="form-group">
                        <label>Departure Date *</label>
                        <input type="date" value={newDestination.departureDate} onChange={e => setNewDestination(prev => ({ ...prev, departureDate: e.target.value }))} />
                    </div>
                </div>
                <div className="form-group">
                    <label>Description</label>
                    <input value={newDestination.description} onChange={e => setNewDestination(prev => ({ ...prev, description: e.target.value }))} />
                </div>
                <button className="btn-primary" onClick={handleAddDestination}>Add Destination</button>
            </div>

            {/* Aktivnosti */}
            <div className="overview-card">
                <h3>Activities</h3>
                {activities.map(a => (
                    editingActivityId === a.id ? (
                        <ActivityEditRow
                            key={a.id}
                            activity={a}
                            onSave={(dto) => handleUpdateActivity(a.id, dto)}
                            onCancel={() => setEditingActivityId(null)}
                        />
                    ) : (
                        <div key={a.id} className="activity-card">
                            <h4>{a.name}</h4>
                            <p>{new Date(a.scheduledAt).toLocaleString()}</p>
                            {a.location && <p>{a.location}</p>}
                            <p>Status: {a.status} | Cost: ${a.estimatedCost?.toFixed(2)}</p>
                            <div style={{ display: 'flex', gap: '0.5rem' }}>
                                <button className="btn-secondary" onClick={() => setEditingActivityId(a.id)}>Edit</button>
                                <button className="btn-danger" onClick={() => handleDeleteActivity(a.id)}>Delete</button>
                            </div>
                        </div>
                    )
                ))}

                <h4>Add Activity</h4>
                <div className="form-row">
                    <div className="form-group">
                        <label>Name *</label>
                        <input value={newActivity.name} onChange={e => setNewActivity(prev => ({ ...prev, name: e.target.value }))} />
                    </div>
                    <div className="form-group">
                        <label>Scheduled At *</label>
                        <input type="datetime-local" value={newActivity.scheduledAt} onChange={e => setNewActivity(prev => ({ ...prev, scheduledAt: e.target.value }))} />
                    </div>
                </div>
                <div className="form-row">
                    <div className="form-group">
                        <label>Location</label>
                        <input value={newActivity.location} onChange={e => setNewActivity(prev => ({ ...prev, location: e.target.value }))} />
                    </div>
                    <div className="form-group">
                        <label>Estimated Cost</label>
                        <input type="number" min="0" step="0.01" value={newActivity.estimatedCost} onChange={e => setNewActivity(prev => ({ ...prev, estimatedCost: parseFloat(e.target.value) }))} />
                    </div>
                </div>
                <div className="form-row">
                    <div className="form-group">
                        <label>Status</label>
                        <select value={newActivity.status} onChange={e => setNewActivity(prev => ({ ...prev, status: e.target.value }))}>
                            <option value="Planned">Planned</option>
                            <option value="Reserved">Reserved</option>
                            <option value="Completed">Completed</option>
                            <option value="Cancelled">Cancelled</option>
                        </select>
                    </div>
                </div>
                <div className="form-group">
                    <label>Description</label>
                    <input value={newActivity.description} onChange={e => setNewActivity(prev => ({ ...prev, description: e.target.value }))} />
                </div>
                <button className="btn-primary" onClick={handleAddActivity}>Add Activity</button>
            </div>

            {/* Checklist */}
            <div className="overview-card shared-checklist-section">
                <h3>Checklist</h3>
                <div className="shared-checklist-list">
                    {checklistItems.map(item => (
                        editingChecklistId === item.id ? (
                            <ChecklistEditRow
                                key={item.id}
                                item={item}
                                onSave={(dto) => handleUpdateChecklistItem(item.id, dto)}
                                onCancel={() => setEditingChecklistId(null)}
                            />
                        ) : (
                            <div key={item.id} className="checklist-item shared-checklist-item">
                                <label className="shared-checklist-label">
                                    <input
                                        type="checkbox"
                                        checked={item.isCompleted}
                                        onChange={() => handleToggleChecklistItem(item)}
                                    />
                                    {' '}{item.title}
                                    {item.description}
                                </label>
                                <div className="shared-checklist-actions">
                                    <button className="btn-secondary" onClick={() => setEditingChecklistId(item.id)}> Edit </button>
                                    <button className="btn-danger" onClick={() => handleDeleteChecklistItem(item.id)}> Delete </button>
                                </div>
                            </div>
                        )
                    ))}
                </div>

                <div className="form-row">
                    <div className="form-group">
                        <label>Title *</label>
                        <input
                            value={newChecklistItem.title}
                            onChange={e => setNewChecklistItem(prev => ({ ...prev, title: e.target.value }))}
                        />
                    </div>
                    <div className="form-group">
                        <label>Description</label>
                        <input
                            value={newChecklistItem.description}
                            onChange={e => setNewChecklistItem(prev => ({ ...prev, description: e.target.value }))}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
}

// ----- Pomocne komponente za inline edit -----

function DestinationEditRow({ destination, onSave, onCancel }) {
    const [dto, setDto] = useState({
        name: destination.name,
        location: destination.location,
        arrivalDate: destination.arrivalDate?.substring(0, 10),
        departureDate: destination.departureDate?.substring(0, 10),
        description: destination.description || '',
    });

    return (
        <div className="destination-card">
            <div className="form-row">
                <div className="form-group">
                    <label>Name</label>
                    <input value={dto.name} onChange={e => setDto(prev => ({ ...prev, name: e.target.value }))} />
                </div>
                <div className="form-group">
                    <label>Location</label>
                    <input value={dto.location} onChange={e => setDto(prev => ({ ...prev, location: e.target.value }))} />
                </div>
            </div>
            <div className="form-row">
                <div className="form-group">
                    <label>Arrival</label>
                    <input type="date" value={dto.arrivalDate} onChange={e => setDto(prev => ({ ...prev, arrivalDate: e.target.value }))} />
                </div>
                <div className="form-group">
                    <label>Departure</label>
                    <input type="date" value={dto.departureDate} onChange={e => setDto(prev => ({ ...prev, departureDate: e.target.value }))} />
                </div>
            </div>
            <div className="form-group">
                <label>Description</label>
                <input value={dto.description} onChange={e => setDto(prev => ({ ...prev, description: e.target.value }))} />
            </div>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
                <button className="btn-primary" onClick={() => onSave(dto)}>Save</button>
                <button className="btn-secondary" onClick={onCancel}>Cancel</button>
            </div>
        </div>
    );
}

function ActivityEditRow({ activity, onSave, onCancel }) {
    const [dto, setDto] = useState({
        name: activity.name,
        scheduledAt: activity.scheduledAt?.substring(0, 18),
        location: activity.location || '',
        description: activity.description || '',
        estimatedCost: activity.estimatedCost,
        status: activity.status,
    });

    return (
        <div className="activity-card">
            <div className="form-row">
                <div className="form-group">
                    <label>Name</label>
                    <input value={dto.name} onChange={e => setDto(prev => ({ ...prev, name: e.target.value }))} />
                </div>
                <div className="form-group">
                    <label>Scheduled At</label>
                    <input type="datetime-local" value={dto.scheduledAt} onChange={e => setDto(prev => ({ ...prev, scheduledAt: e.target.value }))} />
                </div>
            </div>
            <div className="form-row">
                <div className="form-group">
                    <label>Location</label>
                    <input value={dto.location} onChange={e => setDto(prev => ({ ...prev, location: e.target.value }))} />
                </div>
                <div className="form-group">
                    <label>Estimated Cost</label>
                    <input type="number" min="0" step="0.01" value={dto.estimatedCost} onChange={e => setDto(prev => ({ ...prev, estimatedCost: parseFloat(e.target.value) }))} />
                </div>
            </div>
            <div className="form-row">
                <div className="form-group">
                    <label>Status</label>
                    <select value={dto.status} onChange={e => setDto(prev => ({ ...prev, status: e.target.value }))}>
                        <option value="Planned">Planned</option>
                        <option value="Reserved">Reserved</option>
                        <option value="Completed">Completed</option>
                        <option value="Cancelled">Cancelled</option>
                    </select>
                </div>
            </div>
            <div className="form-group">
                <label>Description</label>
                <input value={dto.description} onChange={e => setDto(prev => ({ ...prev, description: e.target.value }))} />
            </div>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
                <button className="btn-primary" onClick={() => onSave(dto)}>Save</button>
                <button className="btn-secondary" onClick={onCancel}>Cancel</button>
            </div>
        </div>
    );
}

function ChecklistEditRow({ item, onSave, onCancel }) {
    const [dto, setDto] = useState({
        title: item.title,
        isCompleted: item.isCompleted,
        description: item.description || '',
    });

    return (
        <div className="checklist-item">
            <div classname="form-row">
            <div className="form-group">
                <label>Title</label>
                <input value={dto.title} onChange={e => setDto(prev => ({ ...prev, title: e.target.value }))} />
            </div>
            <div className="form-group">
                <label>Description</label>
                <input value={dto.description} onChange={e => setDto(prev => ({ ...prev, description: e.target.value}))}/>
            </div>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
                <button className="btn-primary" onClick={() => onSave(dto)}>Save</button>
                <button className="btn-secondary" onClick={onCancel}>Cancel</button>
            </div>
            </div>
        </div>
    );
}