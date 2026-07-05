import { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom';
import { TravelPlanContext } from '../context/TravelPlanContext';
import { expenseService } from '../services/expenseService';
import { CreateExpenseRequest, UpdateExpenseRequest, ExpenseCategories } from '../models/Expense';
import '../styles/Expenses.css';

export default function Expenses() {
    const { planId } = useParams();
    const { currentPlan, addExpense, updateExpense, deleteExpense } = useContext(TravelPlanContext);
    const [expenses, setExpenses] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showForm, setShowForm] = useState(false);
    const [editingId, setEditingId] = useState(null);
    const [formData, setFormData] = useState({
        name: '',
        category: ExpenseCategories.OTHER,
        amount: 0,
        date: '',
        description: '',
    });

    const fetchExpenses = async () => {
        setLoading(true);
        try {
            const data = await expenseService.getExpenses(planId);
            setExpenses(data);
        } catch (err) {
            setError(err.message || 'Failed to fetch expenses');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const loadPlans = async () => {
            await fetchExpenses();
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

        if (!formData.name.trim()) {
            setError('Expense name is required.');
            return;
        }

        const amount = parseFloat(formData.amount);
        if (isNaN(amount) || amount < 0) {
            setError('Expense amount must be a positive number.');
            return;
        }

        if (!formData.date) {
            setError('Expense date is required.');
            return;
        }

        try {
            if (editingId) {
                const updateRequest = new UpdateExpenseRequest(
                    editingId,
                    formData.name,
                    formData.category,
                    parseFloat(formData.amount),
                    formData.date,
                    formData.description
                );
                const updated = await expenseService.updateExpense(planId, editingId, updateRequest);
                setExpenses(prev => prev.map(e => e.id === editingId ? updated : e));
                updateExpense(planId, updated);
            } else {
                const createRequest = new CreateExpenseRequest(
                    planId,
                    formData.name,
                    formData.category,
                    parseFloat(formData.amount),
                    formData.date,
                    formData.description
                );
                const created = await expenseService.createExpense(planId, createRequest);
                setExpenses(prev => [...prev, created]);
                addExpense(planId, created);
            }
            resetForm();
        } catch (err) {
            setError(err.message || 'Operation failed');
        }
    };

    const handleEdit = (expense) => {
        setFormData({
            name: expense.name,
            category: expense.category,
            amount: expense.amount,
            date: expense.date.split('T')[0],
            description: expense.description,
        });
        setEditingId(expense.id);
        setShowForm(true);
    };

    const handleDelete = async (expenseId) => {
        if (window.confirm('Delete this expense?')) {
            try {
                await expenseService.deleteExpense(planId, expenseId);
                setExpenses(prev => prev.filter(e => e.id !== expenseId));
                deleteExpense(planId, expenseId);
            } catch (err) {
                setError(err.message || 'Failed to delete expense');
            }
        }
    };

    const resetForm = () => {
        setFormData({
            name: '',
            category: ExpenseCategories.OTHER,
            amount: 0,
            date: '',
            description: '',
        });
        setEditingId(null);
        setShowForm(false);
    };

    // Calculate totals
    const totalExpenses = expenses.reduce((sum, e) => sum + e.amount, 0);
    const expensesByCategory = Object.values(ExpenseCategories).map(category => ({
        category,
        total: expenses
            .filter(e => e.category === category)
            .reduce((sum, e) => sum + e.amount, 0),
        count: expenses.filter(e => e.category === category).length,
    }));

    if (loading) return <div className="loading">Loading expenses...</div>;

    const activityCosts = (currentPlan?.activities || [])
        .filter(a => a.status !== "Cancelled")
        .reduce((sum, a) => sum + (a.estimatedCost || 0), 0);

    const planBudget = currentPlan?.plannedBudget || 0;
    const remainingBudget = planBudget - totalExpenses - activityCosts;
    const combinedSpent = totalExpenses + activityCosts;

    return (
        <div className="expenses-page">
            <h1>Expenses & Budget</h1>

            {error && <div className="error-message">{error}</div>}

            <div className="budget-overview">
                <div className="budget-card">
                    <h3>Budget Status</h3>
                    <div className="budget-stat">
                        <span className="label">Total Budget</span>
                        <span className="value">${planBudget.toFixed(2)}</span>
                    </div>
                    <div className="budget-stat">
                        <span className="label">Expenses</span>
                        <span className="value spent">${totalExpenses.toFixed(2)}</span>
                    </div>
                    <div className="budget-stat">
                        <span className="label">Activity Costs</span>
                        <span className="value spent">${activityCosts.toFixed(2)}</span>
                    </div>
                    <div className="budget-stat">
                        <span className="label">Total Spent</span>
                        <span className="value spent">${combinedSpent.toFixed(2)}</span>
                    </div>
                    <div className="budget-stat">
                        <span className="label">Remaining</span>
                        <span className={`value ${remainingBudget < 0 ? 'over-budget' : ''}`}>
                            ${remainingBudget.toFixed(2)}
                        </span>
                    </div>
                    <div className="budget-progress">
                        <div className="progress-bar">
                            <div
                                className="progress-fill"
                                style={{
                                    width: `${Math.min((combinedSpent / planBudget) * 100, 100)}%`,
                                    backgroundColor: remainingBudget < 0 ? '#e74c3c' : '#27ae60',
                                }}
                            />
                        </div>
                        <p>{((combinedSpent / planBudget) * 100).toFixed(1)}% spent</p>
                    </div>
                </div>

                <div className="categories-breakdown">
                    <h3>By Category</h3>
                    {expensesByCategory.map(cat => (
                        cat.count > 0 && (
                            <div key={cat.category} className="category-item">
                                <span>{cat.category}</span>
                                <span>${cat.total.toFixed(2)}</span>
                            </div>
                        )
                    ))}
                </div>
            </div>

            <button className="btn-primary" onClick={() => setShowForm(!showForm)}>
                {showForm ? 'Cancel' : '+ Add Expense'}
            </button>

            {showForm && (
                <div className="form-container">
                    <form onSubmit={handleSubmit}>
                        <div className="form-row">
                            <div className="form-group">
                                <label>Expense Name *</label>
                                <input
                                    type="text"
                                    name="name"
                                    value={formData.name}
                                    onChange={handleInputChange}
                                    required
                                />
                            </div>
                            <div className="form-group">
                                <label>Category *</label>
                                <select name="category" value={formData.category} onChange={handleInputChange} required>
                                    {Object.values(ExpenseCategories).map(cat => (
                                        <option key={cat} value={cat}>{cat}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="form-row">
                            <div className="form-group">
                                <label>Amount *</label>
                                <input
                                    type="number"
                                    name="amount"
                                    value={formData.amount}
                                    onChange={handleInputChange}
                                    required
                                    min="0"
                                    step="0.01"
                                />
                            </div>
                            <div className="form-group">
                                <label>Date *</label>
                                <input
                                    type="date"
                                    name="date"
                                    value={formData.date}
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
                            {editingId ? 'Update' : 'Add'} Expense
                        </button>
                    </form>
                </div>
            )}

            <div className="expenses-list">
                {expenses.length === 0 ? (
                    <p>No expenses yet. Add one to track your spending!</p>
                ) : (
                    <div className="expenses-table">
                        <div className="table-header">
                            <div className="col-name">Name</div>
                            <div className="col-category">Category</div>
                            <div className="col-amount">Amount</div>
                            <div className="col-date">Date</div>
                            <div className="col-actions">Actions</div>
                        </div>
                        {expenses
                            .sort((a, b) => new Date(b.date) - new Date(a.date))
                            .map(expense => (
                                <div key={expense.id} className="table-row">
                                    <div className="col-name">{expense.name}</div>
                                    <div className="col-category">
                                        <span className={`category-badge cat-${expense.category.toLowerCase()}`}>
                                            {expense.category}
                                        </span>
                                    </div>
                                    <div className="col-amount">${expense.amount.toFixed(2)}</div>
                                    <div className="col-date">{new Date(expense.date).toLocaleDateString()}</div>
                                    <div className="col-actions">
                                        <button className="btn-secondary" onClick={() => handleEdit(expense)}>Edit</button>
                                        <button className="btn-danger" onClick={() => handleDelete(expense.id)}>Delete</button>
                                    </div>
                                </div>
                            ))}
                    </div>
                )}
            </div>
        </div>
    );
}
