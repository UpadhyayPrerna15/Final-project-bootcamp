# Epic Quest - Game Frontend

A modern, interactive web frontend for the Game API built with vanilla HTML, CSS, and JavaScript.

## 🎨 Features

- **Animated Particle Background**: Dynamic canvas-based particle system
- **Responsive Design**: Works on desktop and mobile devices
- **Modern UI**: Cyberpunk/gaming-themed design with smooth animations
- **Real-time Data**: Live updates from the Game API
- **Interactive Dashboard**: Manage players, characters, items, and scores
- **Global Leaderboards**: View top players across different game modes
- **User Authentication**: Secure login and registration
- **Toast Notifications**: User-friendly feedback for all actions

## 🚀 Getting Started

### Prerequisites

- The Game API running on `https://localhost:5001`
- A modern web browser (Chrome, Firefox, Edge, Safari)
- A local web server (or use Live Server extension in VS Code)

### Installation

1. **Navigate to the Frontend directory**
   ```bash
   cd Frontend
   ```

2. **Start a local web server**

   **Option 1: Using Python**
   ```bash
   # Python 3
   python -m http.server 8000
   
   # Python 2
   python -m SimpleHTTPServer 8000
   ```

   **Option 2: Using Node.js**
   ```bash
   npx http-server -p 8000
   ```

   **Option 3: Using VS Code Live Server**
   - Install "Live Server" extension
   - Right-click on `index.html`
   - Select "Open with Live Server"

3. **Open in browser**
   Navigate to `http://localhost:8000`

## 📁 Project Structure

```
Frontend/
├── index.html           # Main HTML file
├── css/
│   └── styles.css      # All styles and animations
├── js/
│   ├── api.js          # API communication layer
│   ├── particles.js    # Particle animation system
│   └── app.js          # Main application logic
└── assets/             # Future: images, icons, etc.
```

## 🎮 Features Guide

### Home Page
- Welcome hero section with animated title
- Quick stats showing active players, characters, and battles
- Call-to-action buttons for registration and leaderboard

### Authentication
- **Register**: Create a new account with username, email, and password
- **Login**: Access your account with username and password
- Secure JWT token-based authentication

### Player Dashboard
Access your personal dashboard to:
- **Manage Players**: Create and view your game players
- **Create Characters**: Build characters with different classes (Warrior, Mage, Ranger, etc.)
- **Inventory**: View your items and equipment
- **Submit Scores**: Record your battle achievements

### Leaderboard
- View top players across different game modes (Arena, Dungeon, Quest, Raid)
- See rankings, points, kill/death ratios, and difficulty levels
- Real-time updates when new scores are submitted

## 🎨 Design Features

### Visual Elements
- **Cyberpunk Theme**: Neon colors with primary cyan (#00f0ff) and magenta (#ff00ff)
- **Glassmorphism**: Frosted glass effect on cards and modals
- **Particle System**: Dynamic background with connected particles
- **Smooth Animations**: Hover effects, transitions, and entrance animations

### Typography
- **Primary Font**: Orbitron (futuristic, tech-inspired)
- **Accent Font**: Press Start 2P (for retro gaming elements)

### Color Scheme
```css
Primary: #00f0ff (Cyan)
Secondary: #ff00ff (Magenta)
Accent: #ffff00 (Yellow)
Background: #0a0e27 (Dark Blue)
Text: #ffffff (White)
```

## 🔧 Configuration

### API Endpoint
To change the API endpoint, edit `js/api.js`:
```javascript
const API_BASE_URL = 'https://localhost:5001/api';
```

### CORS Configuration
Make sure your API allows CORS from your frontend URL. The API is configured to allow all origins in development.

## 📱 Responsive Design

The frontend is fully responsive and optimized for:
- Desktop (1920px+)
- Laptop (1366px - 1920px)
- Tablet (768px - 1366px)
- Mobile (320px - 768px)

## 🔐 Security

- JWT tokens stored in localStorage
- Automatic token inclusion in authenticated requests
- Secure password input fields
- Session management with logout functionality

## 🎯 Usage Examples

### Creating a Player
1. Login to your account
2. Navigate to Dashboard
3. Click "Create New Player"
4. Enter player name
5. Submit

### Submitting a Score
1. Ensure you have created a player
2. Click "Submit Score" in dashboard
3. Select your player
4. Choose game mode
5. Enter battle statistics
6. Submit

### Viewing Leaderboard
1. Click "Leaderboard" in navigation
2. Select game mode (Arena, Dungeon, Quest, Raid)
3. View top players and their scores

## 🐛 Troubleshooting

### API Connection Issues
- Ensure the Game API is running on `https://localhost:5001`
- Check browser console for CORS errors
- Verify API configuration in `js/api.js`

### HTTPS Certificate Errors
- Accept the self-signed certificate in your browser
- Navigate to `https://localhost:5001` first and accept the certificate
- Then return to the frontend

### Authentication Issues
- Clear localStorage: `localStorage.clear()`
- Check if token is expired
- Try logging in again

## 🚀 Future Enhancements

Potential features to add:
- [ ] Profile editing
- [ ] Advanced inventory management
- [ ] Character leveling system
- [ ] Real-time multiplayer features
- [ ] Achievement system
- [ ] Social features (friends, guilds)
- [ ] Battle simulation
- [ ] Item crafting
- [ ] Sound effects and background music

## 📄 Browser Compatibility

Tested and working on:
- ✅ Chrome 90+
- ✅ Firefox 88+
- ✅ Safari 14+
- ✅ Edge 90+

## 👨‍💻 Development

### Adding New Features
1. Add HTML structure to `index.html`
2. Add styles to `css/styles.css`
3. Add API methods to `js/api.js` if needed
4. Add application logic to `js/app.js`

### Code Structure
- **Modular Design**: Separate concerns (API, UI, Animation)
- **Event-Driven**: Uses event listeners for interactions
- **Async/Await**: Modern JavaScript for API calls
- **Class-Based**: OOP approach for maintainability

## 📝 License

This project is created for educational purposes as part of a bootcamp assignment.

## 🙏 Credits

- Design inspired by modern gaming interfaces
- Particle system adapted from canvas tutorials
- Icons: Unicode emoji characters

---

**Built with ❤️ using vanilla HTML, CSS, and JavaScript**
