# Travel Planner - Frontend Application

Modern React-based frontend for a comprehensive travel planning web application. This application allows users to create, plan, and manage their travel trips with features for destinations, activities, expense tracking, packing lists, and plan sharing.

## Features

### ✈️ Travel Planning
- **Create & Manage Travel Plans**: Create detailed travel plans with name, description, dates, and budget
- **Destination Management**: Add multiple destinations with arrival/departure dates and descriptions
- **Activity Calendar**: Plan daily activities with a calendar view, status tracking, and cost estimation
- **Budget Tracking**: Monitor expenses against planned budget with category-wise breakdown
- **Packing List**: Create and manage checklist items for pre-trip preparation

### 👥 Collaboration
- **Plan Sharing**: Share travel plans via unique links with two access levels (View-only or Edit)
- **QR Code Generation**: Generate QR codes for easy sharing of travel plans

### 🔐 Authentication
- **User Registration**: Create new user accounts with email and password
- **User Login**: Secure login with token-based authentication
- **Protected Routes**: All features are protected and require authentication

### 📊 Dashboard
- **Quick Overview**: View all travel plans at a glance
- **Plan Cards**: See essential information like budget status, duration, and trip statistics
- **Easy Navigation**: Quick links to different sections of each plan

## Tech Stack

- **React 19**: Modern React with hooks and latest features
- **React Router v6**: Client-side routing
- **Axios**: HTTP client for API communication
- **Context API**: State management for authentication and travel plans
- **Vite**: Fast build tool and development server
- **CSS3**: Modern styling with responsive design

## Project Structure

```
frontend/
├── src/
│   ├── components/          # Reusable React components
│   │   ├── Navigation.jsx    # Navigation bar with user menu
│   │   ├── TravelPlanCard.jsx# Card component for travel plans
│   │   └── ProtectedRoute.jsx # Route protection wrapper
│   ├── pages/               # Page components
│   │   ├── LogIn.jsx        # Login page
│   │   ├── Register.jsx     # Registration page
│   │   ├── Dashboard.jsx    # Main dashboard
│   │   ├── PlanDetails.jsx  # Travel plan details view
│   │   ├── Destinations.jsx # Destination management
│   │   ├── Calendar.jsx     # Activity calendar
│   │   ├── Expenses.jsx     # Expense tracking
│   │   ├── Checklist.jsx    # Packing list
│   │   └── Sharing.jsx      # Plan sharing
│   ├── services/            # API service files
│   │   ├── api.js           # Axios instance with interceptors
│   │   ├── authService.js   # Authentication API calls
│   │   ├── travelPlanService.js # Travel plan CRUD
│   │   ├── destinationService.js # Destination CRUD
│   │   ├── activityService.js    # Activity CRUD
│   │   ├── expenseService.js     # Expense CRUD
│   │   ├── checklistService.js   # Checklist CRUD
│   │   └── sharingService.js     # Sharing & QR code
│   ├── models/              # Data models and DTOs
│   │   ├── User.js          # User and auth models
│   │   ├── TravelPlan.js    # Travel plan model
│   │   ├── Destination.js   # Destination model
│   │   ├── Activity.js      # Activity model
│   │   ├── Expense.js       # Expense model
│   │   ├── ChecklistItem.js # Checklist item model
│   │   └── SharedPlanToken.js # Sharing model
│   ├── context/             # React Context
│   │   ├── AuthContext.jsx  # Authentication context
│   │   └── TravelPlanContext.jsx # Travel plan context
│   ├── styles/              # CSS files
│   │   ├── globals.css      # Global styles
│   │   ├── Navigation.css   # Navigation styles
│   │   ├── Auth.css         # Auth pages styles
│   │   ├── Dashboard.css    # Dashboard styles
│   │   ├── TravelPlanCard.css # Card styles
│   │   ├── PlanDetails.css  # Plan details styles
│   │   ├── Destinations.css # Destinations styles
│   │   ├── Calendar.css     # Calendar styles
│   │   ├── Expenses.css     # Expenses styles
│   │   ├── Checklist.css    # Checklist styles
│   │   └── Sharing.css      # Sharing styles
│   ├── App.jsx              # Main app component with routing
│   ├── main.jsx             # Entry point
│   └── index.css            # Additional global styles
├── index.html               # HTML template
├── .env                     # Environment variables
├── .env.example             # Example environment file
├── package.json             # Dependencies
├── vite.config.js           # Vite configuration
└── eslint.config.js         # ESLint configuration
```

## Installation & Setup

### Prerequisites
- Node.js (v16 or higher)
- npm or yarn

### Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure environment variables**
   ```bash
   # Copy the example file
   cp .env.example .env
   
   # Edit .env and set the backend API URL
   VITE_API_URL=http://localhost:8080
   ```

4. **Start the development server**
   ```bash
   npm run dev
   ```
   The app will be available at `http://localhost:5173`

5. **Build for production**
   ```bash
   npm run build
   ```

## Features & Usage

### Authentication
- **Register**: Create a new account with name, email, and password
- **Login**: Use your credentials to access the application
- **Logout**: Safely logout from the application

### Travel Plans
1. Create a new travel plan from the dashboard
2. Fill in basic information (name, dates, budget)
3. View plan details and manage sub-sections

### Destinations
- Add multiple destinations to your travel plan
- Set arrival and departure dates for each destination
- Add descriptions and notes

### Activities & Calendar
- Plan daily activities with the calendar view
- Set date, time, location, and estimated cost
- Track activity status (Planned, Reserved, Completed, Cancelled)
- View activities grouped by date

### Expenses
- Track all expenses with categories (Transport, Accommodation, Food, Tickets, Shopping, Other)
- See real-time budget calculations
- View expenses by category breakdown

### Packing List
- Create checklist items for pre-trip preparation
- Mark items as complete
- Track completion percentage

### Sharing
- Generate unique share links for your travel plans
- Choose access level (View-only or Can Edit)
- Generate QR codes for easy sharing
- Manage and revoke shared links

## API Integration

The frontend communicates with the backend via REST API. All API calls are centralized in the services directory.

### API Endpoints Used

- **Authentication**: `/api/auth/login`, `/api/auth/register`, `/api/auth/me`
- **Travel Plans**: `/api/travelplans` (CRUD operations)
- **Destinations**: `/api/travelplans/{id}/destinations`
- **Activities**: `/api/travelplans/{id}/activities`
- **Expenses**: `/api/travelplans/{id}/expenses`
- **Checklist**: `/api/travelplans/{id}/checklist`
- **Sharing**: `/api/travelplans/{id}/share`, `/api/shared/{token}`

## State Management

The application uses React Context API for state management:

- **AuthContext**: Manages authentication state and user information
- **TravelPlanContext**: Manages travel plans, destinations, activities, expenses, and checklist items

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Performance Optimizations

- Lazy loading of routes
- Optimized re-renders with Context API
- Efficient API calls with proper caching
- Responsive design for all devices

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run lint` - Run ESLint

## Styling

The application uses modern CSS3 with:
- CSS Grid for layouts
- Flexbox for component alignment
- CSS variables for consistent theming
- Responsive design with media queries
- Smooth transitions and animations

## Contributing

To contribute to this project:

1. Create a new branch for your feature
2. Make your changes
3. Test thoroughly
4. Submit a pull request

## License

This project is part of a university assignment and is provided as-is.

## Support

For issues or questions, please contact the development team or refer to the project documentation.

---

**Note**: This frontend requires a running backend server. Make sure to configure the `VITE_API_URL` environment variable to point to your backend API.
