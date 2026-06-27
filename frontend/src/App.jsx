import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { TravelPlanProvider } from './context/TravelPlanContext';
import ProtectedRoute from './components/ProtectedRoute';
import Navigation from './components/Navigation';

// Pages
import Login from './pages/LogIn';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import PlanDetails from './pages/PlanDetails';
import Destinations from './pages/Destinations';
import Calendar from './pages/Calendar';
import Expenses from './pages/Expenses';
import Checklist from './pages/Checklist';
import Sharing from './pages/Sharing';
import SharedPlanView from './pages/SharedPlanView';
import EditPlan from './pages/EditPlan';
import AdminPanel from './pages/AdminPanel';

import './App.css';

function App() {
    return (
        <Router>
            <AuthProvider>
                <TravelPlanProvider>
                    <div className="app">
                        <Navigation />
                        <main className="app-main">
                            <Routes>
                                {/* Auth Routes */}
                                <Route path="/login" element={<Login />} />
                                <Route path="/register" element={<Register />} />
                                <Route path="/shared/:token" element={<SharedPlanView />} />

                                {/* Protected Routes */}
                                <Route
                                    path="/dashboard"
                                    element={
                                        <ProtectedRoute>
                                            <Dashboard />
                                        </ProtectedRoute>
                                    }
                                />

                                <Route
                                    path="/plan/:planId"
                                    element={
                                        <ProtectedRoute>
                                            <PlanDetails />
                                        </ProtectedRoute>
                                    }
                                />

                                <Route
                                    path="/plan/:planId/destinations"
                                    element={
                                        <ProtectedRoute>
                                            <Destinations />
                                        </ProtectedRoute>
                                    }
                                />

                                <Route
                                    path="/plan/:planId/calendar"
                                    element={
                                        <ProtectedRoute>
                                            <Calendar />
                                        </ProtectedRoute>
                                    }
                                />

                                <Route
                                    path="/plan/:planId/expenses"
                                    element={
                                        <ProtectedRoute>
                                            <Expenses />
                                        </ProtectedRoute>
                                    }
                                />

                                <Route
                                    path="/plan/:planId/checklist"
                                    element={
                                        <ProtectedRoute>
                                            <Checklist />
                                        </ProtectedRoute>
                                    }
                                />

                                <Route
                                    path="/plan/:planId/share"
                                    element={
                                        <ProtectedRoute>
                                            <Sharing />
                                        </ProtectedRoute>
                                    }
                                />

                                <Route
                                    path="/plan/:planId/edit"
                                    element={
                                        <ProtectedRoute>
                                            <EditPlan />
                                        </ProtectedRoute>
                                    }
                                />
                                <Route
                                    path="/admin"
                                    element={
                                        <ProtectedRoute>
                                            <AdminPanel />
                                        </ProtectedRoute>
                                    }
                                />
                                
                                {/* Redirect to dashboard */}
                                <Route path="/" element={<Navigate to="/dashboard" replace />} />
                            </Routes>
                        </main>
                    </div>
                </TravelPlanProvider>
            </AuthProvider>
        </Router>
    );
}

export default App;