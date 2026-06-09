import { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom';
import { TravelPlanContext } from '../context/TravelPlanContext';
import { destinationService } from '../services/destinationService';
import { CreateDestinationRequest, UpdateDestinationRequest } from '../models/Destination';
import '../styles/Destinations.css';

export default function Destinations() {
  const { planId } = useParams();
  const { currentPlan, addDestination, updateDestination, deleteDestination } = useContext(TravelPlanContext);
  const [destinations, setDestinations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    location: '',
    arrivalDate: '',
    departureDate: '',
    description: '',
  });

  useEffect(() => {
    fetchDestinations();
  }, [planId]);

  const fetchDestinations = async () => {
    setLoading(true);
    try {
      const data = await destinationService.getDestinations(planId);
      setDestinations(data);
    } catch (err) {
      setError(err.message || 'Failed to fetch destinations');
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
        const updateRequest = new UpdateDestinationRequest(
          editingId,
          formData.name,
          formData.location,
          formData.arrivalDate,
          formData.departureDate,
          formData.description
        );
        const updated = await destinationService.updateDestination(planId, editingId, updateRequest);
        setDestinations(prev => prev.map(d => d.id === editingId ? updated : d));
        updateDestination(planId, updated);
      } else {
        const createRequest = new CreateDestinationRequest(
          planId,
          formData.name,
          formData.location,
          formData.arrivalDate,
          formData.departureDate,
          formData.description
        );
        const created = await destinationService.createDestination(planId, createRequest);
        setDestinations(prev => [...prev, created]);
        addDestination(planId, created);
      }
      resetForm();
    } catch (err) {
      setError(err.message || 'Operation failed');
    }
  };

  const handleEdit = (destination) => {
    setFormData({
      name: destination.name,
      location: destination.location,
      arrivalDate: destination.arrivalDate,
      departureDate: destination.departureDate,
      description: destination.description,
    });
    setEditingId(destination.id);
    setShowForm(true);
  };

  const handleDelete = async (destinationId) => {
    if (window.confirm('Delete this destination?')) {
      try {
        await destinationService.deleteDestination(planId, destinationId);
        setDestinations(prev => prev.filter(d => d.id !== destinationId));
        deleteDestination(planId, destinationId);
      } catch (err) {
        setError(err.message || 'Failed to delete destination');
      }
    }
  };

  const resetForm = () => {
    setFormData({
      name: '',
      location: '',
      arrivalDate: '',
      departureDate: '',
      description: '',
    });
    setEditingId(null);
    setShowForm(false);
  };

  if (loading) return <div className="loading">Loading destinations...</div>;

  return (
    <div className="destinations-page">
      <h1>Destinations</h1>

      {error && <div className="error-message">{error}</div>}

      <button className="btn-primary" onClick={() => setShowForm(!showForm)}>
        {showForm ? 'Cancel' : '+ Add Destination'}
      </button>

      {showForm && (
        <div className="form-container">
          <form onSubmit={handleSubmit}>
            <div className="form-row">
              <div className="form-group">
                <label>Name *</label>
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Location *</label>
                <input
                  type="text"
                  name="location"
                  value={formData.location}
                  onChange={handleInputChange}
                  required
                />
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label>Arrival Date *</label>
                <input
                  type="date"
                  name="arrivalDate"
                  value={formData.arrivalDate}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Departure Date *</label>
                <input
                  type="date"
                  name="departureDate"
                  value={formData.departureDate}
                  onChange={handleInputChange}
                  required
                />
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
              {editingId ? 'Update' : 'Add'} Destination
            </button>
          </form>
        </div>
      )}

      <div className="destinations-list">
        {destinations.length === 0 ? (
          <p>No destinations yet. Add one to get started!</p>
        ) : (
          destinations.map(dest => (
            <div key={dest.id} className="destination-card">
              <div className="destination-info">
                <h3>{dest.name}</h3>
                <p className="location">{dest.location}</p>
                <p className="dates">
                  {new Date(dest.arrivalDate).toLocaleDateString()} - {new Date(dest.departureDate).toLocaleDateString()}
                </p>
                {dest.description && <p className="description">{dest.description}</p>}
              </div>
              <div className="destination-actions">
                <button className="btn-secondary" onClick={() => handleEdit(dest)}>Edit</button>
                <button className="btn-danger" onClick={() => handleDelete(dest.id)}>Delete</button>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
