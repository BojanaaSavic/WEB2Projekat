# Frontend Application - File Purpose Reference

## 📁 Complete File Manifest

### 🔐 Authentication Layer

#### Pages
- **`src/pages/LogIn.jsx`**
  - Purpose: User login form
  - Features: Email/password input, error display, login button
  - State: Local component state + AuthContext
  - API: authService.login()

- **`src/pages/Register.jsx`**
  - Purpose: User registration form
  - Features: Name/email/password input, validation, registration
  - State: Local component state + AuthContext
  - API: authService.register()

#### Services
- **`src/services/authService.js`**
  - Purpose: Authentication API integration
  - Methods: login(), register(), getCurrentUser(), logout()
  - Uses: Axios instance
  - Error handling: Custom error messages

#### Context
- **`src/context/AuthContext.jsx`**
  - Purpose: Global authentication state
  - State: user, token, loading, error
  - Methods: login(), register(), logout()
  - Storage: localStorage for token persistence

#### Components
- **`src/components/ProtectedRoute.jsx`**
  - Purpose: Route protection wrapper
  - Logic: Redirects to login if not authenticated
  - Usage: Wraps protected pages

---

### ✈️ Travel Plan Management

#### Pages
- **`src/pages/Dashboard.jsx`**
  - Purpose: Main dashboard overview
  - Features: Display all travel plans, create new plan form
  - State: TravelPlanContext (travelPlans, currentPlan)
  - Components: TravelPlanCard (map), form inputs

- **`src/pages/PlanDetails.jsx`**
  - Purpose: Detailed travel plan view
  - Features: Plan info, tabbed interface, navigation links
  - Tabs: Overview, Destinations, Activities, Expenses, Checklist, Share
  - Calculations: Total expenses, remaining budget, days until start

#### Services
- **`src/services/travelPlanService.js`**
  - Purpose: Travel plan CRUD operations
  - Methods: getAllPlans(), getPlan(), createPlan(), updatePlan(), deletePlan()
  - API: GET/POST/PUT/DELETE /api/travelplans/*
  - Data transformation: JSON → TravelPlan instances

#### Models
- **`src/models/TravelPlan.js`**
  - Exports: TravelPlan, CreateTravelPlanRequest, UpdateTravelPlanRequest
  - Properties: id, name, description, startDate, endDate, plannedBudget, notes
  - Helper methods: getTotalExpenses(), getRemainingBudget(), getDaysUntilStart()

#### Context
- **`src/context/TravelPlanContext.jsx`**
  - Purpose: Global travel plan state
  - State: travelPlans[], currentPlan, loading, error
  - Methods: All CRUD operations for plans, destinations, activities, expenses, checklist
  - Structure: Optimized for nested data updates

#### Components
- **`src/components/TravelPlanCard.jsx`**
  - Purpose: Visual preview card for travel plan
  - Features: Plan name, budget bar, duration, items count
  - Actions: Click to view details, delete button
  - Data: Calculated stats from plan object

---

### 🏙️ Destination Management

#### Pages
- **`src/pages/Destinations.jsx`**
  - Purpose: Destination CRUD interface
  - Features: List all destinations, add form, edit/delete actions
  - Form inputs: name, location, arrivalDate, departureDate, description
  - Actions: Create, Edit, Delete with confirmation

#### Services
- **`src/services/destinationService.js`**
  - Purpose: Destination API integration
  - Methods: getDestinations(), createDestination(), updateDestination(), deleteDestination()
  - API: /api/travelplans/{planId}/destinations/*

#### Models
- **`src/models/Destination.js`**
  - Exports: Destination, CreateDestinationRequest, UpdateDestinationRequest
  - Properties: id, name, location, arrivalDate, departureDate, description

---

### 📅 Activity & Calendar Management

#### Pages
- **`src/pages/Calendar.jsx`**
  - Purpose: Activity calendar interface
  - Features: Visual calendar grid, activity planning form, activities list
  - Calendar: Month view with activity badges
  - Form: name, location, scheduledAt, description, estimatedCost, status
  - Status: Planned, Reserved, Completed, Cancelled

#### Services
- **`src/services/activityService.js`**
  - Purpose: Activity API integration
  - Methods: getActivities(), createActivity(), updateActivity(), deleteActivity()
  - Special: getActivitiesByDate()
  - API: /api/travelplans/{planId}/activities/*

#### Models
- **`src/models/Activity.js`**
  - Exports: Activity, ActivityStatus enum, Create/UpdateActivityRequest
  - Properties: id, name, scheduledAt, location, description, estimatedCost, status
  - Status values: Planned, Reserved, Completed, Cancelled

---

### 💰 Expense & Budget Management

#### Pages
- **`src/pages/Expenses.jsx`**
  - Purpose: Expense tracking and budget management
  - Features: Budget overview card, category breakdown, expenses table
  - Form: name, category, amount, date, description
  - Calculations: Total spent, remaining budget, % used
  - Table: Sortable by date, category badges, amount formatting

#### Services
- **`src/services/expenseService.js`**
  - Purpose: Expense API integration
  - Methods: getExpenses(), createExpense(), updateExpense(), deleteExpense()
  - Special: getTotalExpenses(), getExpensesByCategory()
  - API: /api/travelplans/{planId}/expenses/*

#### Models
- **`src/models/Expense.js`**
  - Exports: Expense, ExpenseCategories enum, Create/UpdateExpenseRequest
  - Categories: Transport, Accommodation, Food, Tickets, Shopping, Other
  - Properties: id, name, category, amount, date, description

---

### ✓ Checklist / Packing List

#### Pages
- **`src/pages/Checklist.jsx`**
  - Purpose: Packing list management
  - Features: Completion stats, add form, checkbox interface
  - Completion: Separate views for Todo and Completed items
  - Form: title, description (optional)
  - Stats: X/Y items completed, percentage, progress bar

#### Services
- **`src/services/checklistService.js`**
  - Purpose: Checklist API integration
  - Methods: getChecklistItems(), createChecklistItem(), updateChecklistItem(), deleteChecklistItem()
  - Special: getCompletionPercentage()
  - API: /api/travelplans/{planId}/checklist/*

#### Models
- **`src/models/ChecklistItem.js`**
  - Exports: ChecklistItem, Create/UpdateChecklistItemRequest
  - Properties: id, title, isCompleted, description

---

### 🔗 Plan Sharing

#### Pages
- **`src/pages/Sharing.jsx`**
  - Purpose: Plan sharing management
  - Features: Create share link form, active shares list
  - Access levels: View-only, Can Edit
  - Actions: Copy link, Download QR code, Delete link
  - Display: Created date, access type badge

#### Services
- **`src/services/sharingService.js`**
  - Purpose: Sharing API integration
  - Methods: createSharedToken(), getSharedTokens(), deleteSharedToken()
  - Special: getSharedPlan(), generateQRCode()
  - API: /api/travelplans/{planId}/share/*, /api/shared/*

#### Models
- **`src/models/SharedPlanToken.js`**
  - Exports: SharedPlanToken, AccessType enum, CreateSharedPlanTokenRequest
  - Access types: VIEW (read-only), EDIT (full access)
  - Properties: id, token, accessType, createdAt, expiresAt

---

### 🎨 UI Components

#### Navigation
- **`src/components/Navigation.jsx`**
  - Purpose: Main navigation bar
  - Features: Logo, "My Plans" link, user menu, logout button
  - Responsive: Mobile toggle menu
  - Sticky: Stays at top while scrolling

---

### 🛠️ Services

#### Main API Configuration
- **`src/services/api.js`**
  - Purpose: Axios instance setup
  - Configuration: baseURL from VITE_API_URL
  - Interceptors: Token injection in headers, 401 redirect
  - Error handling: Centralized error handling

---

### 📊 State Management

#### Global Context
- **`src/context/AuthContext.jsx`**
  - State: user, token, loading, error
  - Methods: login(), register(), logout(), setAuthError()
  - Storage: localStorage persistence
  - Effects: Initialization from storage

- **`src/context/TravelPlanContext.jsx`**
  - State: travelPlans, currentPlan, loading, error
  - Plan methods: setPlans(), addPlan(), updatePlan(), deletePlan(), selectPlan()
  - Destination methods: addDestination(), updateDestination(), deleteDestination()
  - Activity methods: addActivity(), updateActivity(), deleteActivity()
  - Expense methods: addExpense(), updateExpense(), deleteExpense()
  - Checklist methods: addChecklistItem(), updateChecklistItem(), deleteChecklistItem()

---

### 🎨 Styling

#### Global
- **`src/styles/globals.css`**
  - Purpose: Global styles and utilities
  - Content: Reset, buttons, forms, utilities, responsive breakpoints

#### Component-Specific
- **`src/styles/Navigation.css`** - Navbar styling
- **`src/styles/Auth.css`** - Login/Register forms
- **`src/styles/Dashboard.css`** - Dashboard layout
- **`src/styles/TravelPlanCard.css`** - Card component
- **`src/styles/PlanDetails.css`** - Plan details view
- **`src/styles/Destinations.css`** - Destination list & form
- **`src/styles/Calendar.css`** - Calendar and activity views
- **`src/styles/Expenses.css`** - Expense tracking UI
- **`src/styles/Checklist.css`** - Checklist interface
- **`src/styles/Sharing.css`** - Sharing management UI

---

### 🔀 Routing

- **`src/App.jsx`**
  - Purpose: Main app component with routing
  - Routes:
    - `/login` → Login page
    - `/register` → Register page
    - `/dashboard` → Dashboard (protected)
    - `/plan/:planId` → Plan details (protected)
    - `/plan/:planId/destinations` → Destinations (protected)
    - `/plan/:planId/calendar` → Calendar (protected)
    - `/plan/:planId/expenses` → Expenses (protected)
    - `/plan/:planId/checklist` → Checklist (protected)
    - `/plan/:planId/share` → Sharing (protected)
    - `/` → Redirect to dashboard

---

### 📦 Configuration & Entry

- **`src/main.jsx`**
  - Purpose: Application entry point
  - Imports: React, styles, App component
  - Renders: App into root div with StrictMode

- **`src/index.css`**
  - Purpose: Additional global styles
  - Content: Root element styling

- **`vite.config.js`**
  - Purpose: Vite build configuration
  - Plugins: React plugin

- **`eslint.config.js`**
  - Purpose: Code quality linting
  - Configuration: ESLint rules for React

---

### 📄 Environment & Config

- **`.env`**
  - Purpose: Local environment variables
  - Variable: VITE_API_URL (backend API URL)

- **`.env.example`**
  - Purpose: Template for environment variables
  - Usage: Copy to .env and configure

---

### 📦 Dependencies

- **`package.json`**
  - Dependencies:
    - react: ^19.2.5
    - react-dom: ^19.2.5
    - react-router-dom: ^6.20.0
    - axios: ^1.6.2
    - date-fns: ^3.0.0
  - Scripts:
    - dev: `vite`
    - build: `vite build`
    - lint: `eslint .`
    - preview: `vite preview`

---

### 📚 Documentation

- **`README.md`**
  - Complete project documentation
  - Features list
  - Installation instructions
  - Usage guide
  - API integration details

- **`QUICKSTART.md`**
  - Quick 5-minute setup guide
  - Common workflows
  - Troubleshooting tips
  - Quick reference

- **`BUILD_SUMMARY.md`**
  - High-level project overview
  - Feature highlights
  - Architecture summary
  - Quick links

---

### 🏗️ Root HTML

- **`index.html`**
  - Purpose: HTML template
  - Content: Root div for React mounting
  - Scripts: Vite module script

---

## 🔄 Data Flow Example

### Creating a Travel Plan:
```
User Input (Dashboard.jsx)
    ↓
travelPlanService.createPlan()
    ↓
API POST /api/travelplans
    ↓
Backend creates plan
    ↓
Returns TravelPlan object
    ↓
addPlan() in TravelPlanContext
    ↓
State updates
    ↓
Dashboard re-renders with new plan
    ↓
TravelPlanCard displays plan
```

### Updating Expense:
```
User Action (Expenses.jsx)
    ↓
Form submission
    ↓
expenseService.updateExpense()
    ↓
API PUT /api/travelplans/{id}/expenses/{id}
    ↓
Backend updates expense
    ↓
Returns updated Expense
    ↓
updateExpense() in TravelPlanContext
    ↓
State and currentPlan update
    ↓
Component re-renders
    ↓
Budget calculations update
```

---

## 📊 Component Hierarchy

```
App
├── Navigation
└── Routes
    ├── Login
    ├── Register
    ├── Dashboard
    │   └── TravelPlanCard (x N)
    ├── PlanDetails
    │   ├── Overview Tab
    ├── Destinations
    ├── Calendar
    ├── Expenses
    ├── Checklist
    └── Sharing
```

---

## 🔐 Authentication Flow

```
User Registration/Login
    ↓
authService.register() / login()
    ↓
API POST /api/auth/register or /login
    ↓
Backend creates user / validates
    ↓
Returns { token, user }
    ↓
AuthContext.login() / register()
    ↓
Stores in state & localStorage
    ↓
Token added to all future requests
    ↓
Access to protected routes
```

---

## 💾 State Structure

### AuthContext
```javascript
{
  user: { id, name, email, role },
  token: "JWT_TOKEN",
  loading: boolean,
  error: string | null,
  isAuthenticated: boolean
}
```

### TravelPlanContext
```javascript
{
  travelPlans: [TravelPlan],
  currentPlan: TravelPlan | null,
  loading: boolean,
  error: string | null
}
```

---

## 🎯 File Organization Philosophy

1. **Separation of Concerns**: Each file has a single responsibility
2. **Modularity**: Components are independent and reusable
3. **Scalability**: Easy to add new features
4. **Maintainability**: Clear structure and naming
5. **Testability**: Services can be tested independently

---

**This structure makes the application scalable, maintainable, and easy to understand! 🚀**
