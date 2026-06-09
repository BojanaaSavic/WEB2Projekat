import { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom';
import { TravelPlanContext } from '../context/TravelPlanContext';
import { activityService } from '../services/activityService';
import { CreateActivityRequest, UpdateActivityRequest, ActivityStatus } from '../models/Activity';
import '../styles/Calendar.css';

export default function Calendar() {
  const { planId } = useParams();
  const { currentPlan, addActivity, updateActivity, deleteActivity } = useContext(TravelPlanContext);
  const [activities, setActivities] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [currentMonth, setCurrentMonth] = useState(new Date());
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    scheduledAt: '',
    location: '',
    description: '',
    estimatedCost: 0,
    status: ActivityStatus.PLANNED,
  });

  useEffect(() => {
    fetchActivities();
  }, [planId]);

  const fetchActivities = async () => {
    setLoading(true);
    try {
      const data = await activityService.getActivities(planId);
      setActivities(data);
    } catch (err) {
      setError(err.message || 'Failed to fetch activities');
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    try {
      if (editingId) {
        const updateRequest = new UpdateActivityRequest(
          editingId,
          formData.name,
          formData.scheduledAt,
          formData.location,
          formData.description,
          parseFloat(formData.estimatedCost),
          formData.status
        );
        const updated = await activityService.updateActivity(planId, editingId, updateRequest);
        setActivities(prev => prev.map(a => a.id === editingId ? updated : a));
        updateActivity(planId, updated);
      } else {
        const createRequest = new CreateActivityRequest(
          planId,
          formData.name,
          formData.scheduledAt,
          formData.location,
          formData.description,
          parseFloat(formData.estimatedCost),
          formData.status
        );
        const created = await activityService.createActivity(planId, createRequest);
        setActivities(prev => [...prev, created]);
        addActivity(planId, created);
      }
      resetForm();
    } catch (err) {
      setError(err.message || 'Operation failed');
    }
  };

  const handleEdit = (activity) => {
    setFormData({
      name: activity.name,
      scheduledAt: activity.scheduledAt,
      location: activity.location,
      description: activity.description,
      estimatedCost: activity.estimatedCost,
      status: activity.status,
    });
    setEditingId(activity.id);
    setShowForm(true);
  };

  const handleDelete = async (activityId) => {
    if (window.confirm('Delete this activity?')) {
      try {
        await activityService.deleteActivity(planId, activityId);
        setActivities(prev => prev.filter(a => a.id !== activityId));
        deleteActivity(planId, activityId);
      } catch (err) {
        setError(err.message || 'Failed to delete activity');
      }
    }
  };

  const resetForm = () => {
    setFormData({
      name: '',
      scheduledAt: '',
      location: '',
      description: '',
      estimatedCost: 0,
      status: ActivityStatus.PLANNED,
    });
    setEditingId(null);
    setShowForm(false);
  };

  // Get activities grouped by date
  const getActivitiesByDate = (date) => {
    return activities.filter(a => {
      const activityDate = new Date(a.scheduledAt).toDateString();
      return activityDate === date.toDateString();
    });
  };

  // Render calendar grid
  const renderCalendar = () => {
    const year = currentMonth.getFullYear();
    const month = currentMonth.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const daysInMonth = lastDay.getDate();
    const startingDayOfWeek = firstDay.getDay();

    const days = [];
    for (let i = 0; i < startingDayOfWeek; i++) {
      days.push(null);
    }
    for (let i = 1; i <= daysInMonth; i++) {
      days.push(new Date(year, month, i));
    }

    return days;
  };

  const calendarDays = renderCalendar();

  if (loading) return <div className="loading">Loading calendar...</div>;

  return (
    <div className="calendar-page">
      <h1>Activity Calendar</h1>

      {error && <div className="error-message">{error}</div>}

      <button className="btn-primary" onClick={() => setShowForm(!showForm)}>
        {showForm ? 'Cancel' : '+ Add Activity'}
      </button>

      {showForm && (
        <div className="form-container">
          <form onSubmit={handleSubmit}>
            <div className="form-row">
              <div className="form-group">
                <label>Activity Name *</label>
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Location</label>
                <input
                  type="text"
                  name="location"
                  value={formData.location}
                  onChange={handleInputChange}
                />
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label>Date & Time *</label>
                <input
                  type="datetime-local"
                  name="scheduledAt"
                  value={formData.scheduledAt}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Estimated Cost</label>
                <input
                  type="number"
                  name="estimatedCost"
                  value={formData.estimatedCost}
                  onChange={handleInputChange}
                  min="0"
                  step="0.01"
                />
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label>Status</label>
                <select name="status" value={formData.status} onChange={handleInputChange}>
                  <option value={ActivityStatus.PLANNED}>Planned</option>
                  <option value={ActivityStatus.RESERVED}>Reserved</option>
                  <option value={ActivityStatus.COMPLETED}>Completed</option>
                  <option value={ActivityStatus.CANCELLED}>Cancelled</option>
                </select>
              </div>
            </div>

            <div className="form-group">
              <label>Description</label>
              <textarea
                name="description"
                value={formData.description}
                onChange={handleInputChange}
                rows="3"
              />
            </div>

            <button type="submit" className="btn-primary">
              {editingId ? 'Update' : 'Add'} Activity
            </button>
          </form>
        </div>
      )}

      <div className="calendar-container">
        <div className="calendar-header">
          <button onClick={() => setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() - 1))}>←</button>
          <h2>{currentMonth.toLocaleDateString('en-US', { month: 'long', year: 'numeric' })}</h2>
          <button onClick={() => setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1))}>→</button>
        </div>

        <div className="calendar-grid">
          {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map(day => (
            <div key={day} className="calendar-day-header">{day}</div>
          ))}
          {calendarDays.map((day, idx) => (
            <div key={idx} className="calendar-day">
              {day ? (
                <>
                  <div className="day-number">{day.getDate()}</div>
                  <div className="day-activities">
                    {getActivitiesByDate(day).slice(0, 2).map(activity => (
                      <div key={activity.id} className={`activity-badge status-${activity.status.toLowerCase()}`}>
                        {activity.name.substring(0, 10)}...
                      </div>
                    ))}
                    {getActivitiesByDate(day).length > 2 && (
                      <div className="activity-more">+{getActivitiesByDate(day).length - 2}</div>
                    )}
                  </div>
                </>
              ) : null}
            </div>
          ))}
        </div>
      </div>

      <div className="activities-list">
        <h3>All Activities</h3>
        {activities.length === 0 ? (
          <p>No activities yet. Add one to get started!</p>
        ) : (
          <div className="activities-grid">
            {activities.sort((a, b) => new Date(a.scheduledAt) - new Date(b.scheduledAt)).map(activity => (
              <div key={activity.id} className="activity-card">
                <div className="activity-header">
                  <h4>{activity.name}</h4>
                  <span className={`status-badge status-${activity.status.toLowerCase()}`}>{activity.status}</span>
                </div>
                <p className="activity-date">{new Date(activity.scheduledAt).toLocaleString()}</p>
                {activity.location && <p className="activity-location">{activity.location}</p>}
                {activity.description && <p className="activity-description">{activity.description}</p>}
                {activity.estimatedCost > 0 && <p className="activity-cost">Cost: ${activity.estimatedCost.toFixed(2)}</p>}
                <div className="activity-actions">
                  <button className="btn-secondary" onClick={() => handleEdit(activity)}>Edit</button>
                  <button className="btn-danger" onClick={() => handleDelete(activity.id)}>Delete</button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
