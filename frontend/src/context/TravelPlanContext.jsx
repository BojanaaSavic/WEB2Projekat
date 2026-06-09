import React, { createContext, useState, useCallback } from 'react';

export const TravelPlanContext = createContext();

export function TravelPlanProvider({ children }) {
  const [travelPlans, setTravelPlans] = useState([]);
  const [currentPlan, setCurrentPlan] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const setPlans = useCallback((plans) => {
    setTravelPlans(plans);
  }, []);

  const addPlan = useCallback((plan) => {
    setTravelPlans(prev => [...prev, plan]);
  }, []);

  const updatePlan = useCallback((updatedPlan) => {
    setTravelPlans(prev =>
      prev.map(plan => plan.id === updatedPlan.id ? updatedPlan : plan)
    );
    if (currentPlan?.id === updatedPlan.id) {
      setCurrentPlan(updatedPlan);
    }
  }, [currentPlan]);

  const deletePlan = useCallback((planId) => {
    setTravelPlans(prev => prev.filter(plan => plan.id !== planId));
    if (currentPlan?.id === planId) {
      setCurrentPlan(null);
    }
  }, [currentPlan]);

  const selectPlan = useCallback((plan) => {
    setCurrentPlan(plan);
  }, []);

  const addDestination = useCallback((planId, destination) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? { ...plan, destinations: [...(plan.destinations || []), destination] }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => ({
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
      setCurrentPlan(prev => ({
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
      setCurrentPlan(prev => ({
        ...prev,
        destinations: (prev.destinations || []).filter(d => d.id !== destinationId)
      }));
    }
  }, [currentPlan]);

  const addActivity = useCallback((planId, activity) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? { ...plan, activities: [...(plan.activities || []), activity] }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => ({
        ...prev,
        activities: [...(prev.activities || []), activity]
      }));
    }
  }, [currentPlan]);

  const updateActivity = useCallback((planId, updatedActivity) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? {
              ...plan,
              activities: (plan.activities || []).map(a =>
                a.id === updatedActivity.id ? updatedActivity : a
              )
            }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => ({
        ...prev,
        activities: (prev.activities || []).map(a =>
          a.id === updatedActivity.id ? updatedActivity : a
        )
      }));
    }
  }, [currentPlan]);

  const deleteActivity = useCallback((planId, activityId) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? {
              ...plan,
              activities: (plan.activities || []).filter(a => a.id !== activityId)
            }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => ({
        ...prev,
        activities: (prev.activities || []).filter(a => a.id !== activityId)
      }));
    }
  }, [currentPlan]);

  const addExpense = useCallback((planId, expense) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? { ...plan, expenses: [...(plan.expenses || []), expense] }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => ({
        ...prev,
        expenses: [...(prev.expenses || []), expense]
      }));
    }
  }, [currentPlan]);

  const updateExpense = useCallback((planId, updatedExpense) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? {
              ...plan,
              expenses: (plan.expenses || []).map(e =>
                e.id === updatedExpense.id ? updatedExpense : e
              )
            }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => ({
        ...prev,
        expenses: (prev.expenses || []).map(e =>
          e.id === updatedExpense.id ? updatedExpense : e
        )
      }));
    }
  }, [currentPlan]);

  const deleteExpense = useCallback((planId, expenseId) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? {
              ...plan,
              expenses: (plan.expenses || []).filter(e => e.id !== expenseId)
            }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => ({
        ...prev,
        expenses: (prev.expenses || []).filter(e => e.id !== expenseId)
      }));
    }
  }, [currentPlan]);

  const addChecklistItem = useCallback((planId, item) => {
    setTravelPlans(prev =>
      prev.map(plan =>
        plan.id === planId
          ? { ...plan, checklistItems: [...(plan.checklistItems || []), item] }
          : plan
      )
    );
    if (currentPlan?.id === planId) {
      setCurrentPlan(prev => ({
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
      setCurrentPlan(prev => ({
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
      setCurrentPlan(prev => ({
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
