import { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom';
import { TravelPlanContext } from '../context/TravelPlanContext';
import { checklistService } from '../services/checklistService';
import { CreateChecklistItemRequest, UpdateChecklistItemRequest } from '../models/ChecklistItem';
import '../styles/Checklist.css';

export default function Checklist() {
  const { planId } = useParams();
  const { addChecklistItem, updateChecklistItem, deleteChecklistItem } = useContext(TravelPlanContext);
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
  });


    const fetchChecklistItems = async () => {
        setLoading(true);
        try {
            const data = await checklistService.getChecklistItems(planId);
            setItems(data);
        } catch (err) {
            setError(err.message || 'Failed to fetch checklist items');
        } finally {
            setLoading(false);
        }
    };

  useEffect(() => {
      const loadPlans = async () => {
          await fetchChecklistItems();
      };

      loadPlans();
  }, [planId]);

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
        const updateRequest = new UpdateChecklistItemRequest(
          editingId,
          formData.title,
          items.find(i => i.id === editingId).isCompleted,
          formData.description
        );
        const updated = await checklistService.updateChecklistItem(planId, editingId, updateRequest);
        setItems(prev => prev.map(i => i.id === editingId ? updated : i));
        updateChecklistItem(planId, updated);
      } else {
        const createRequest = new CreateChecklistItemRequest(
          planId,
          formData.title,
          formData.description
        );
        const created = await checklistService.createChecklistItem(planId, createRequest);
        setItems(prev => [...prev, created]);
        addChecklistItem(planId, created);
      }
      resetForm();
    } catch (err) {
      setError(err.message || 'Operation failed');
    }
  };

  const handleToggleComplete = async (item) => {
    try {
      const updateRequest = new UpdateChecklistItemRequest(
        item.id,
        item.title,
        !item.isCompleted,
        item.description
      );
      const updated = await checklistService.updateChecklistItem(planId, item.id, updateRequest);
      setItems(prev => prev.map(i => i.id === item.id ? updated : i));
      updateChecklistItem(planId, updated);
    } catch (err) {
      setError(err.message || 'Failed to update item');
    }
  };

  const handleEdit = (item) => {
    setFormData({
      title: item.title,
      description: item.description,
    });
    setEditingId(item.id);
    setShowForm(true);
  };

  const handleDelete = async (itemId) => {
    if (window.confirm('Delete this item?')) {
      try {
        await checklistService.deleteChecklistItem(planId, itemId);
        setItems(prev => prev.filter(i => i.id !== itemId));
        deleteChecklistItem(planId, itemId);
      } catch (err) {
        setError(err.message || 'Failed to delete item');
      }
    }
  };

  const resetForm = () => {
    setFormData({
      title: '',
      description: '',
    });
    setEditingId(null);
    setShowForm(false);
  };

  if (loading) return <div className="loading">Loading checklist...</div>;

  const completedCount = items.filter(i => i.isCompleted).length;
  const totalCount = items.length;
  const completionPercentage = totalCount > 0 ? (completedCount / totalCount) * 100 : 0;

  return (
    <div className="checklist-page">
      <h1>Packing List & Checklist</h1>

      {error && <div className="error-message">{error}</div>}

      <div className="checklist-stats">
        <div className="stat">
          <h3>Completion</h3>
          <div className="stat-value">{completedCount}/{totalCount}</div>
          <div className="progress-bar">
            <div
              className="progress-fill"
              style={{ width: `${completionPercentage}%` }}
            />
          </div>
          <p>{completionPercentage.toFixed(0)}% complete</p>
        </div>
      </div>

      <button className="btn-primary" onClick={() => setShowForm(!showForm)}>
        {showForm ? 'Cancel' : '+ Add Item'}
      </button>

      {showForm && (
        <div className="form-container">
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Item Title *</label>
              <input
                type="text"
                name="title"
                placeholder="e.g., Passport, Camera, Charger"
                value={formData.title}
                onChange={handleInputChange}
                required
              />
            </div>

            <div className="form-group">
              <label>Description</label>
              <textarea
                name="description"
                placeholder="Add any notes about this item"
                value={formData.description}
                onChange={handleInputChange}
                rows="3"
              />
            </div>

            <button type="submit" className="btn-primary">
              {editingId ? 'Update' : 'Add'} Item
            </button>
          </form>
        </div>
      )}

      <div className="checklist-items">
        {items.length === 0 ? (
          <p>No items in your checklist. Add items to prepare for your trip!</p>
        ) : (
          <>
            <h3>To Do ({items.filter(i => !i.isCompleted).length})</h3>
            {items
              .filter(i => !i.isCompleted)
              .map(item => (
                <div key={item.id} className="checklist-item">
                  <input
                    type="checkbox"
                    checked={item.isCompleted}
                    onChange={() => handleToggleComplete(item)}
                  />
                  <div className="item-content">
                    <h4>{item.title}</h4>
                    {item.description && <p>{item.description}</p>}
                  </div>
                  <div className="item-actions">
                    <button className="btn-secondary" onClick={() => handleEdit(item)}>Edit</button>
                    <button className="btn-danger" onClick={() => handleDelete(item.id)}>Delete</button>
                  </div>
                </div>
              ))}

            {items.filter(i => i.isCompleted).length > 0 && (
              <>
                <h3>Completed ({items.filter(i => i.isCompleted).length})</h3>
                {items
                  .filter(i => i.isCompleted)
                  .map(item => (
                    <div key={item.id} className="checklist-item completed">
                      <input
                        type="checkbox"
                        checked={item.isCompleted}
                        onChange={() => handleToggleComplete(item)}
                      />
                      <div className="item-content">
                        <h4>{item.title}</h4>
                        {item.description && <p>{item.description}</p>}
                      </div>
                      <div className="item-actions">
                        <button className="btn-secondary" onClick={() => handleEdit(item)}>Edit</button>
                        <button className="btn-danger" onClick={() => handleDelete(item.id)}>Delete</button>
                      </div>
                    </div>
                  ))}
              </>
            )}
          </>
        )}
      </div>
    </div>
  );
}
