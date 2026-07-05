import { createContext, useState, useCallback } from 'react';
import { TravelPlan } from '../models/TravelPlan';

export const TravelPlanContext = createContext();

export function TravelPlanProvider({ children }) {
  const [travelPlans, setTravelPlans] = useState([]);
  const [currentPlan, setCurrentPlan] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const recalculatePlanBudget = useCallback((plan) => {
    if (!plan) return plan;

    const normalizedPlan = plan instanceof TravelPlan
      ? plan
      : new TravelPlan(
          plan.id,
          plan.name,
          plan.description,
          plan.startDate,
          plan.endDate,
          plan.plannedBudget,
          plan.notes,
          plan.ownerId,
          plan.destinations || [],
          plan.activities || [],
          plan.expenses || [],
          plan.checklistItems || []
        );

    const expenseTotal = (normalizedPlan.expenses || []).reduce((sum, expense) => sum + (Number(expense?.amount) || 0), 0);
    const activityCosts = (normalizedPlan.activities || [])
      .filter(activity => activity?.status !== 'Cancelled')
      .reduce((sum, activity) => sum + (Number(activity?.estimatedCost) || 0), 0);
    const totalSpent = expenseTotal + activityCosts;

    normalizedPlan.totalExpenses = totalSpent;
    normalizedPlan.remainingBudget = (Number(normalizedPlan.plannedBudget) || 0) - totalSpent;

    return normalizedPlan;
  }, []);

  const setPlans = useCallback((plans) => {
    setTravelPlans((plans || []).map(recalculatePlanBudget));
  }, [recalculatePlanBudget]);

  const addPlan = useCallback((plan) => {
    setTravelPlans(prev => [...prev, recalculatePlanBudget(plan)]);
  }, [recalculatePlanBudget]);

  const updatePlan = useCallback((updatedPlan) => {
    const normalizedPlan = recalculatePlanBudget(updatedPlan);
    setTravelPlans(prev =>
      prev.map(plan => plan.id === normalizedPlan.id ? normalizedPlan : plan)
    );
    if (currentPlan?.id === normalizedPlan.id) {
      setCurrentPlan(normalizedPlan);
    }
  }, [currentPlan, recalculatePlanBudget]);

  const deletePlan = useCallback((planId) => {
    setTravelPlans(prev => prev.filter(plan => plan.id !== planId));
    if (currentPlan?.id === planId) {
      setCurrentPlan(null);
    }
  }, [currentPlan]);

  const selectPlan = useCallback((plan) => {
    setCurrentPlan(recalculatePlanBudget(plan));
  }, [recalculatePlanBudget]);

  const addDestination = useCallback((planId, destination) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? { ...plan, destinations: [...(plan.destinations || []), destination] }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        destinations: [...(prev.destinations || []), destination]
      }));
    }
  }, [currentPlan]);

  const updateDestination = useCallback((planId, updatedDestination) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? {
              ...plan,
              destinations: (plan.destinations || []).map(d =>
                d.id === updatedDestination.id ? updatedDestination : d
              )
            }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        destinations: (prev.destinations || []).map(d =>
          d.id === updatedDestination.id ? updatedDestination : d
        )
      }));
    }
  }, [currentPlan]);

  const deleteDestination = useCallback((planId, destinationId) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? {
              ...plan,
              destinations: (plan.destinations || []).filter(d => d.id !== destinationId)
            }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        destinations: (prev.destinations || []).filter(d => d.id !== destinationId)
      }));
    }
  }, [currentPlan]);

  const addActivity = useCallback((planId, activity) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? recalculatePlanBudget({ ...plan, activities: [...(plan.activities || []), activity] })
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        activities: [...(prev.activities || []), activity]
      }));
    }
  }, [currentPlan, recalculatePlanBudget]);

  const updateActivity = useCallback((planId, updatedActivity) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? recalculatePlanBudget({
              ...plan,
              activities: (plan.activities || []).map(a =>
                a.id === updatedActivity.id ? updatedActivity : a
              )
            })
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        activities: (prev.activities || []).map(a =>
          a.id === updatedActivity.id ? updatedActivity : a
        )
      }));
    }
  }, [currentPlan, recalculatePlanBudget]);

  const deleteActivity = useCallback((planId, activityId) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? recalculatePlanBudget({
              ...plan,
              activities: (plan.activities || []).filter(a => a.id !== activityId)
            })
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        activities: (prev.activities || []).filter(a => a.id !== activityId)
      }));
    }
  }, [currentPlan, recalculatePlanBudget]);

  const addExpense = useCallback((planId, expense) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? recalculatePlanBudget({ ...plan, expenses: [...(plan.expenses || []), expense] })
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        expenses: [...(prev.expenses || []), expense]
      }));
    }
  }, [currentPlan, recalculatePlanBudget]);

  const updateExpense = useCallback((planId, updatedExpense) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? recalculatePlanBudget({
              ...plan,
              expenses: (plan.expenses || []).map(e =>
                e.id === updatedExpense.id ? updatedExpense : e
              )
            })
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        expenses: (prev.expenses || []).map(e =>
          e.id === updatedExpense.id ? updatedExpense : e
        )
      }));
    }
  }, [currentPlan, recalculatePlanBudget]);

  const deleteExpense = useCallback((planId, expenseId) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? recalculatePlanBudget({
              ...plan,
              expenses: (plan.expenses || []).filter(e => e.id !== expenseId)
            })
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        expenses: (prev.expenses || []).filter(e => e.id !== expenseId)
      }));
    }
  }, [currentPlan, recalculatePlanBudget]);

  const addChecklistItem = useCallback((planId, item) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? { ...plan, checklistItems: [...(plan.checklistItems || []), item] }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        checklistItems: [...(prev.checklistItems || []), item]
      }));
    }
  }, [currentPlan]);

  const updateChecklistItem = useCallback((planId, updatedItem) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? {
              ...plan,
              checklistItems: (plan.checklistItems || []).map(i =>
                i.id === updatedItem.id ? updatedItem : i
              )
            }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        checklistItems: (prev.checklistItems || []).map(i =>
          i.id === updatedItem.id ? updatedItem : i
        )
      }));
    }
  }, [currentPlan]);

  const deleteChecklistItem = useCallback((planId, itemId) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? {
              ...plan,
              checklistItems: (plan.checklistItems || []).filter(i => i.id !== itemId)
            }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => recalculatePlanBudget({
        ...prev,
        checklistItems: (prev.checklistItems || []).filter(i => i.id !== itemId)
      }));
    }
  }, [currentPlan]);

  return (
    <TravelPlanContext.Provider value={{
      travelPlans,
      currentPlan,
      loading,
      error,
      setPlans,
      addPlan,
      updatePlan,
      deletePlan,
      selectPlan,
      addDestination,
      updateDestination,
      deleteDestination,
      addActivity,
      updateActivity,
      deleteActivity,
      addExpense,
      updateExpense,
      deleteExpense,
      addChecklistItem,
      updateChecklistItem,
      deleteChecklistItem,
    }}>
      {children}
    </TravelPlanContext.Provider>
  );
}
