# Epic Quest - Full Stack Game Application

A comprehensive full-stack game application with a RESTful API backend built with ASP.NET Core 8.0 and an interactive frontend with HTML/CSS/JavaScript. Features JWT authentication, Entity Framework Core, SQLite database, and a modern cyberpunk-themed UI.

## 🎮 Features

### Backend API
- **Complete CRUD Operations**: Manage Players, Characters, Items, and Scores
- **JWT Authentication**: Secure token-based authentication and authorization
- **Role-Based Access Control**: Admin and Player roles with different permissions
- **Database Integration**: Entity Framework Core with SQLite
- **Data Validation**: Comprehensive validation on all endpoints
- **Error Handling**: Global error handling middleware
- **Swagger Documentation**: Interactive API documentation
- **Pagination & Filtering**: Efficient data retrieval with query parameters
- **Leaderboard System**: Public leaderboards for game scores
- **Data Seeding**: Pre-populated database with sample data

### Frontend Application
- **Interactive Dashboard**: Manage all game entities through a modern UI
- **Real-time Leaderboards**: View global rankings across game modes
- **Particle Animation**: Dynamic canvas-based background effects
- **Responsive Design**: Works seamlessly on all devices
- **Toast Notifications**: User-friendly feedback system
- **Cyberpunk Theme**: Modern gaming aesthetic with neon colors

## 📋 Table of Contents

- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
  - [Backend Setup](#backend-setup)
  - [Frontend Setup](#frontend-setup)
- [API Endpoints](#api-endpoints)
- [Authentication](#authentication)
- [Frontend Features](#frontend-features)
- [Testing](#testing)
- [Database Schema](#database-schema)
- [Project Structure](#project-structure)

## 🛠 Technologies Used

### Backend
- **ASP.NET Core 8.0** - Web API Framework
- **Entity Framework Core 8.0** - ORM for database operations
- **SQLite** - Lightweight database
- **JWT Bearer Authentication** - Secure authentication
- **BCrypt.Net** - Password hashing
- **Swashbuckle (Swagger)** - API documentation
- **C# 12** - Programming language

### Frontend
- **HTML5** - Markup structure
- **CSS3** - Styling and animations
- **Vanilla JavaScript (ES6+)** - Application logic
- **Canvas API** - Particle animation system
- **Fetch API** - HTTP requests
- **LocalStorage** - Client-side data persistence

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A code editor (Visual Studio, VS Code, or Rider)
- A modern web browser (Chrome, Firefox, Edge, Safari)
- Git

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/UpadhyayPrerna15/Final-project-bootcamp.git
   cd Final-project-bootcamp/GameAPI
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string (optional)**
   
   The default connection string uses SQLite with a local file `gameapi.db`. To change it, edit `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=gameapi.db"
   }
   ```

4. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```
   
   The database will be automatically created with seeded data on first run.

5. **Run the application**
   ```bash
   dotnet run
   ```
   
   The API will start at `https://localhost:5001` (or check console output for the exact port)

6. **Access Swagger UI**
   
   Open your browser and navigate to: `https://localhost:5001`
   
   You'll see the interactive Swagger documentation where you can test all endpoints.

### Frontend Setup

1. **Navigate to Frontend directory**
   ```bash
   cd ../Frontend
   ```

2. **Start a local web server**
   
   **Option 1: Using Python**
   ```bash
   python -m http.server 8000
   ```
   
   **Option 2: Using VS Code Live Server**
   - Install "Live Server" extension
   - Right-click on `index.html` and select "Open with Live Server"

3. **Access the frontend**
   
   Open your browser and navigate to: `http://localhost:8000`

4. **Login with default credentials**
   - Username: `player1`
   - Password: `password123`

## 🎯 Frontend Features

### Home Page
- Animated hero section with glitch effect title
- Real-time statistics display
- Quick access to registration and leaderboard

### Player Dashboard
- **Player Management**: Create and view your game players with stats
- **Character Creation**: Build characters with different classes (Warrior, Mage, Ranger, Rogue, Paladin, Necromancer)
- **Inventory System**: View and manage items with rarity levels
- **Score Tracking**: Submit battle scores and view achievements

### Global Leaderboard
- Switch between game modes: Arena, Dungeon, Quest, Raid
- View top 20 players with detailed statistics
- Real-time rankings with K/D ratios and play time

### Visual Design
- Cyberpunk theme with neon cyan and magenta colors
- Animated particle background using Canvas API
- Smooth transitions and hover effects
- Glassmorphism UI elements
- Responsive design for all devices

## 🔐 Authentication

### Register a New User

**POST** `/api/auth/register`

```json
{
  "username": "newplayer",
  "email": "newplayer@example.com",
  "password": "password123"
}
```

### Login

**POST** `/api/auth/login`

```json
{
  "username": "player1",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "player1",
  "email": "player1@gameapi.com",
  "role": "Player",
  "expiration": "2025-12-05T15:30:00Z"
}
```

### Using the Token

1. **In Swagger UI**: Click the "Authorize" button at the top, enter `Bearer YOUR_TOKEN`, and click "Authorize"
2. **In Postman**: Add header `Authorization: Bearer YOUR_TOKEN`
3. **In code**: Add `Authorization: Bearer YOUR_TOKEN` to request headers

### Default Users

The database is seeded with these users (password: `password123`):

- **admin** (Admin role) - Full access to all resources
- **player1** (Player role) - Access to own resources
- **player2** (Player role) - Access to own resources

## 📡 API Endpoints

### Authentication
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login user | No |

### Players
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/players` | Get all players (with pagination) | Yes |
| GET | `/api/players/{id}` | Get player by ID | Yes |
| POST | `/api/players` | Create new player | Yes |
| PUT | `/api/players/{id}` | Update player | Yes |
| DELETE | `/api/players/{id}` | Delete player | Yes |

**Query Parameters for GET /api/players:**
- `page` (default: 1)
- `pageSize` (default: 10, max: 100)

### Characters
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/characters` | Get all characters (with filtering) | Yes |
| GET | `/api/characters/{id}` | Get character by ID | Yes |
| POST | `/api/characters` | Create new character | Yes |
| PUT | `/api/characters/{id}` | Update character | Yes |
| DELETE | `/api/characters/{id}` | Delete character | Yes |

**Query Parameters for GET /api/characters:**
- `playerId` - Filter by player
- `characterClass` - Filter by class (e.g., Warrior, Mage)
- `page` (default: 1)
- `pageSize` (default: 10)

### Items
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/items` | Get all items (with filtering) | Yes |
| GET | `/api/items/{id}` | Get item by ID | Yes |
| POST | `/api/items` | Create new item | Yes |
| PUT | `/api/items/{id}` | Update item | Yes |
| DELETE | `/api/items/{id}` | Delete item | Yes |

**Query Parameters for GET /api/items:**
- `playerId` - Filter by player
- `itemType` - Filter by type (e.g., Weapon, Armor)
- `minRarity` - Filter by minimum rarity (1-10)
- `page` (default: 1)
- `pageSize` (default: 10)

### Scores
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/scores` | Get all scores (with filtering) | Yes |
| GET | `/api/scores/{id}` | Get score by ID | Yes |
| GET | `/api/scores/leaderboard/{gameMode}` | Get public leaderboard | No |
| POST | `/api/scores` | Create new score | Yes |
| DELETE | `/api/scores/{id}` | Delete score | Yes |

**Query Parameters for GET /api/scores:**
- `playerId` - Filter by player
- `gameMode` - Filter by game mode
- `highScoresOnly` - Show only high scores
- `page` (default: 1)
- `pageSize` (default: 10)

**Query Parameters for GET /api/scores/leaderboard/{gameMode}:**
- `top` - Number of top scores (default: 10, max: 100)

## 🧪 Testing

### Manual Testing with Swagger

1. Run the application: `dotnet run`
2. Navigate to `https://localhost:5001`
3. Use the Swagger UI to test endpoints interactively

### Testing Workflow

1. **Register/Login**: Get a JWT token
2. **Authorize**: Click "Authorize" and enter token
3. **Create Player**: POST to `/api/players`
4. **Create Character**: POST to `/api/characters` with playerId
5. **Add Items**: POST to `/api/items` with playerId
6. **Submit Score**: POST to `/api/scores` with playerId
7. **View Leaderboard**: GET `/api/scores/leaderboard/Arena`

### Example: Create a Player

```bash
curl -X POST "https://localhost:5001/api/players" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name": "WarriorKing"}'
```

## 🗄 Database Schema

### User
- Id (PK)
- Username (unique)
- Email (unique)
- PasswordHash
- Role (Admin/Player)
- CreatedAt
- LastLogin

### Player
- Id (PK)
- Name
- Level
- Experience
- Gold
- Health / MaxHealth
- Mana / MaxMana
- UserId (FK → User)
- CreatedAt
- LastPlayed

### Character
- Id (PK)
- Name
- CharacterClass
- Level
- Experience
- Strength, Intelligence, Dexterity, Vitality
- Health / MaxHealth
- IsActive
- PlayerId (FK → Player)
- CreatedAt

### Item
- Id (PK)
- Name
- Description
- ItemType
- AttackBonus
- DefenseBonus
- Value
- Rarity (1-10)
- IsEquipped
- Quantity
- PlayerId (FK → Player, nullable)
- AcquiredAt

### Score
- Id (PK)
- GameMode
- Points
- Kills
- Deaths
- TimePlayed
- DifficultyLevel
- IsHighScore
- PlayerId (FK → Player)
- AchievedAt

## 📁 Project Structure

```
Final-project-bootcamp/
├── GameAPI/               # Backend API
│   ├── Controllers/       # API Controllers
│   │   ├── AuthController.cs
│   │   ├── PlayersController.cs
│   │   ├── CharactersController.cs
│   │   ├── ItemsController.cs
│   │   └── ScoresController.cs
│   ├── Data/             # Database Context
│   │   └── GameDbContext.cs
│   ├── DTOs/             # Data Transfer Objects
│   │   ├── AuthDtos.cs
│   │   ├── PlayerDtos.cs
│   │   ├── CharacterDtos.cs
│   │   ├── ItemDtos.cs
│   │   └── ScoreDtos.cs
│   ├── Models/           # Entity Models
│   │   ├── User.cs
│   │   ├── Player.cs
│   │   ├── Character.cs
│   │   ├── Item.cs
│   │   └── Score.cs
│   ├── Services/         # Business Logic Services
│   │   ├── TokenService.cs
│   │   └── PasswordHasher.cs
│   ├── Middleware/       # Custom Middleware
│   │   └── ErrorHandlingMiddleware.cs
│   ├── Migrations/       # EF Core Migrations
│   ├── Program.cs        # Application Entry Point
│   ├── appsettings.json  # Configuration
│   └── gameapi.db       # SQLite Database (generated)
├── Frontend/             # Frontend Application
│   ├── index.html        # Main HTML file
│   ├── css/
│   │   └── styles.css    # All styles and animations
│   ├── js/
│   │   ├── api.js        # API communication layer
│   │   ├── particles.js  # Particle animation system
│   │   └── app.js        # Main application logic
│   ├── assets/           # Images, icons (future)
│   └── README.md         # Frontend documentation
├── README.md             # Main documentation
├── TESTING.md            # API testing guide
├── GameAPI_Postman_Collection.json  # Postman collection
└── .gitignore
```
├── appsettings.json       # Configuration
└── gameapi.db            # SQLite Database (generated)
```

## 🔒 Security Features

- **Password Hashing**: BCrypt with salt
- **JWT Tokens**: Signed with HS256 algorithm
- **Token Expiration**: 60 minutes (configurable)
- **Role-Based Authorization**: Admin and Player roles
- **Resource Ownership**: Users can only access/modify their own data
- **HTTPS Redirection**: All traffic redirected to HTTPS
- **Input Validation**: Data annotations on all DTOs
- **SQL Injection Protection**: EF Core parameterized queries

## ⚙️ Configuration

Edit `appsettings.json` to customize:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=gameapi.db"
  },
  "JwtSettings": {
    "SecretKey": "Your-Secret-Key-Here-At-Least-32-Characters",
    "Issuer": "GameAPI",
    "Audience": "GameAPIClients",
    "ExpirationMinutes": "60"
  }
}
```

**Important**: Change the `SecretKey` in production!

## 🎯 Learning Outcomes Demonstrated

✅ **Understanding Databases**: Relational schema with proper foreign keys and relationships  
✅ **Entity Framework**: Full EF Core integration with migrations and seeding  
✅ **Building a Web API**: Complete ASP.NET Core Web API with proper middleware  
✅ **CRUD Operations**: Full Create, Read, Update, Delete for all entities  
✅ **Serializing Data**: JSON responses for all endpoints  
✅ **Routing and Endpoints**: RESTful routes following best practices  
✅ **Authentication Basics**: User registration, login, and session management  
✅ **JWT Tokens**: Token generation, validation, and refresh  
✅ **Securing Endpoints**: Authorization attributes and role-based access  

## 📝 API Response Formats

### Success Response
```json
{
  "id": 1,
  "name": "DragonSlayer",
  "level": 15,
  ...
}
```

### Error Response
```json
{
  "error": "Player not found",
  "statusCode": 404,
  "timestamp": "2025-12-05T10:30:00Z"
}
```

### Pagination Headers
```
X-Total-Count: 50
X-Page: 1
X-Page-Size: 10
```

## 🤝 Contributing

This is a bootcamp project. For issues or suggestions:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is created for educational purposes as part of a bootcamp assignment.

## 👨‍💻 Author

**Prerna Upadhyay**
- GitHub: [@UpadhyayPrerna15](https://github.com/UpadhyayPrerna15)

## 🙏 Acknowledgments

- ASP.NET Core Documentation
- Entity Framework Core Documentation
- JWT.io for token debugging
- Swagger/OpenAPI for documentation

---

**Built with ❤️ using ASP.NET Core 8.0**
