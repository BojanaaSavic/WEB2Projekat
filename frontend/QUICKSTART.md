# Quick Start Guide - Travel Planner Frontend

## ⚡ 5-Minute Setup

### 1. Install & Configure
```bash
cd frontend
npm install
cp .env.example .env
# Edit .env: VITE_API_URL=http://localhost:8080
```

### 2. Start Dev Server
```bash
npm run dev
```
Open: http://localhost:5173

### 3. Create Account & Login
- Click "Register" to create account
- Enter name, email, password
- Login with your credentials

## 🎯 Main Features

### Dashboard
- View all your travel plans
- Create new plan with "+ New Travel Plan"
- See budget, destinations, activities at a glance

### Create Travel Plan
1. Enter plan name, description
2. Set start & end dates
3. Set planned budget
4. Add optional notes
5. Click "Create Travel Plan"

### Manage Plan Details
- **Destinations**: Add places you'll visit with dates
- **Calendar**: Plan daily activities with date/time
- **Expenses**: Track costs by category
- **Checklist**: Create packing/prep list
- **Share**: Generate links to share with friends

## 📝 Example Workflow

1. **Register** → Create account
2. **Create Plan** → "Summer Europe 2024"
3. **Add Destinations** → Paris, Rome, Barcelona
4. **Schedule Activities** → Daily tours, meals, transport
5. **Track Expenses** → Hotel, flights, food, tickets
6. **Packing List** → Passport, camera, documents
7. **Share Plan** → Generate QR code for friends

## 🔧 Environment Setup

Create `.env` file:
```env
VITE_API_URL=http://localhost:8080
```

Replace `localhost:8080` with your backend API URL.

## 📦 Build for Production

```bash
npm run build
npm run preview
```

## 🐛 Troubleshooting

| Problem | Solution |
|---------|----------|
| Blank page | Hard refresh (Ctrl+Shift+R) |
| API errors | Check VITE_API_URL |
| Can't login | Verify backend is running |
| Styling broken | Clear cache |

## 📚 Full Documentation

See [README.md](./README.md) for complete documentation.
See [FRONTEND_SETUP.md](../FRONTEND_SETUP.md) for detailed setup.

## 💪 You're All Set!

Start planning your travels! 🚀✈️

---

**Need Help?** Check README.md or contact the development team.
