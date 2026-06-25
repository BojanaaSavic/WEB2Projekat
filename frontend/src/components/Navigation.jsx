import { useContext, useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import '../styles/Navigation.css';

export default function Navigation() {
  const { user, logout, isAuthenticated } = useContext(AuthContext);
  const navigate = useNavigate();
  const [showMenu, setShowMenu] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  if (!isAuthenticated) {
    return null;
  }

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <Link to="/dashboard" className="navbar-brand">
          ✈️ Travel Planner
        </Link>

        <button
          className="navbar-toggle"
          onClick={() => setShowMenu(!showMenu)}
        >
          ☰
        </button>

        <div className={`navbar-menu ${showMenu ? 'active' : ''}`}>
          <Link to="/dashboard" className="nav-link">
            My Plans
          </Link>

          <div className="user-section">
            <span className="user-name">{user?.name}</span>
             {user?.role === 'Admin' && (
               <Link to="/admin" className="nav-link">
                 Admin Panel
               </Link>
            )}
            <button className="btn-logout" onClick={handleLogout}>
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}
