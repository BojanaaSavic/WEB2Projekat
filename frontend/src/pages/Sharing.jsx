import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { sharingService } from '../services/sharingService';
import { CreateSharedPlanTokenRequest, AccessType } from '../models/SharedPlanToken';
import '../styles/Sharing.css';

export default function Sharing() {
  const { planId } = useParams();
  const [sharedTokens, setSharedTokens] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [accessType, setAccessType] = useState(AccessType.VIEW);
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    fetchSharedTokens();
  }, [planId]);

  const fetchSharedTokens = async () => {
    setLoading(true);
    try {
      const tokens = await sharingService.getSharedTokens(planId);
      setSharedTokens(tokens);
    } catch (err) {
      setError(err.message || 'Failed to fetch shared tokens');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateToken = async () => {
    setError('');
    try {
      const createRequest = new CreateSharedPlanTokenRequest(planId, accessType);
      const newToken = await sharingService.createSharedToken(planId, createRequest);
      setSharedTokens(prev => [...prev, newToken]);
    } catch (err) {
      setError(err.message || 'Failed to create shared token');
    }
  };

  const handleDeleteToken = async (tokenId) => {
    if (window.confirm('Delete this shared link? People with this link will no longer have access.')) {
      try {
        await sharingService.deleteSharedToken(planId, tokenId);
        setSharedTokens(prev => prev.filter(t => t.id !== tokenId));
      } catch (err) {
        setError(err.message || 'Failed to delete token');
      }
    }
  };

  const handleCopyLink = (token) => {
    const baseUrl = window.location.origin;
    const shareUrl = `${baseUrl}/shared/${token.token}`;
    navigator.clipboard.writeText(shareUrl);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const generateQRCode = async (token) => {
    try {
      const blob = await sharingService.generateQRCode(token.token);
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `qr-code-${token.token}.png`;
      a.click();
    } catch (err) {
      setError('Failed to generate QR code');
    }
  };

  if (loading) return <div className="loading">Loading sharing options...</div>;

  return (
    <div className="sharing-page">
      <h1>Share Your Travel Plan</h1>

      {error && <div className="error-message">{error}</div>}

      <div className="create-token-section">
        <h2>Create Shared Link</h2>
        <div className="create-form">
          <div className="form-group">
            <label htmlFor="accessType">Access Level *</label>
            <select
              id="accessType"
              value={accessType}
              onChange={(e) => setAccessType(e.target.value)}
            >
              <option value={AccessType.VIEW}>View Only - Can view plan but cannot edit</option>
              <option value={AccessType.EDIT}>Can Edit - Can view and modify the plan</option>
            </select>
          </div>
          <button className="btn-primary" onClick={handleCreateToken}>
            Generate Share Link
          </button>
        </div>
      </div>

      <div className="shared-tokens-section">
        <h2>Active Share Links ({sharedTokens.length})</h2>

        {sharedTokens.length === 0 ? (
          <p>No shared links yet. Create one to share your plan with others!</p>
        ) : (
          <div className="tokens-list">
            {sharedTokens.map(token => (
              <div key={token.id} className="token-card">
                <div className="token-info">
                  <div className="token-header">
                    <h3>
                      <span className={`access-badge access-${token.accessType.toLowerCase()}`}>
                        {token.accessType}
                      </span>
                    </h3>
                    <p className="token-date">
                      Created: {new Date(token.createdAt).toLocaleDateString()}
                    </p>
                  </div>

                  <div className="token-link">
                    <input
                      type="text"
                      value={`${window.location.origin}/shared/${token.token}`}
                      readOnly
                      onClick={(e) => e.target.select()}
                    />
                    <button
                      className="btn-secondary"
                      onClick={() => handleCopyLink(token)}
                    >
                      {copied ? 'Copied!' : 'Copy Link'}
                    </button>
                  </div>

                  <div className="token-actions">
                    <button
                      className="btn-secondary"
                      onClick={() => generateQRCode(token)}
                    >
                      Download QR Code
                    </button>
                    <button
                      className="btn-danger"
                      onClick={() => handleDeleteToken(token.id)}
                    >
                      Delete Link
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="sharing-info">
        <h3>How Sharing Works</h3>
        <ul>
          <li><strong>View Only:</strong> Recipients can view your travel plan but cannot make changes.</li>
          <li><strong>Can Edit:</strong> Recipients can view and modify the plan (add activities, expenses, etc.)</li>
          <li>Each shared link is unique. You can create multiple links with different access levels.</li>
          <li>Delete a link anytime to revoke access.</li>
          <li>Share via link or QR code for easy access.</li>
        </ul>
      </div>
    </div>
  );
}
