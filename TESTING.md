# API Testing Guide

## Running the API

The API is successfully running at: **http://localhost:5220**

Swagger UI is accessible at: **http://localhost:5220/** (root path)

## Quick Start Testing

### 1. Access Swagger UI
Open your browser and go to: `http://localhost:5220/`

You'll see the interactive Swagger documentation with all endpoints.

### 2. Test Authentication

**Login with a test account:**

Click on `/api/auth/login` endpoint → Try it out

Use these credentials:
```json
{
  "username": "player1",
  "password": "password123"
}
```

**Result:** You'll receive a JWT token. Copy this token.

### 3. Authorize in Swagger

Click the **Authorize** button at the top right of Swagger UI.

Enter: `Bearer YOUR_TOKEN_HERE` (replace YOUR_TOKEN_HERE with the actual token)

Click **Authorize** and **Close**.

### 4. Test CRUD Operations

Now you can test any endpoint. All requests will include your authentication token.

**Example - Get all players:**
- Click on `/api/players` GET endpoint
- Click "Try it out"
- Click "Execute"
- You'll see the list of players

**Example - Create a new player:**
- Click on `/api/players` POST endpoint
- Click "Try it out"
- Enter:
```json
{
  "name": "MyNewPlayer"
}
```
- Click "Execute"
- You'll see your new player created

## Available Test Accounts

| Username | Password | Role | Description |
|----------|----------|------|-------------|
| admin | password123 | Admin | Full access to all resources |
| player1 | password123 | Player | Has 2 existing players with characters, items, and scores |
| player2 | password123 | Player | Has 1 existing player with character, items, and score |

## Seeded Data Overview

### Players
- DragonSlayer (Level 15, owned by player1)
- ShadowHunter (Level 10, owned by player2)

### Characters
- Thorin (Warrior, Level 15, belongs to DragonSlayer)
- Gandalf (Mage, Level 12, belongs to DragonSlayer)
- Legolas (Ranger, Level 10, belongs to ShadowHunter)

### Items
- Excalibur (Legendary sword, Rarity 10, owned by DragonSlayer)
- Iron Armor (Armor, Rarity 5, owned by DragonSlayer)
- Health Potion (Consumable, Quantity 10, owned by DragonSlayer)
- Elven Bow (Weapon, Rarity 7, owned by ShadowHunter)

### Scores
- Arena mode: 15000 points (DragonSlayer)
- Dungeon mode: 8000 points (DragonSlayer)
- Arena mode: 12000 points (ShadowHunter)

## Testing with Postman

Import the `GameAPI_Postman_Collection.json` file into Postman for a complete set of pre-configured requests.

### Steps:
1. Open Postman
2. Click Import → Upload Files
3. Select `GameAPI_Postman_Collection.json`
4. Update the `baseUrl` variable to `http://localhost:5220`
5. Run the "Login" request first to automatically set your token
6. Test all other endpoints

## Key Features to Test

### 1. Authentication
- ✅ Register new user
- ✅ Login with credentials
- ✅ JWT token generation

### 2. Authorization
- ✅ Role-based access (Admin vs Player)
- ✅ Resource ownership validation
- ✅ Protected endpoints

### 3. CRUD Operations
- ✅ Create new entities
- ✅ Read/List entities with pagination
- ✅ Update existing entities
- ✅ Delete entities

### 4. Advanced Features
- ✅ Pagination (page and pageSize parameters)
- ✅ Filtering (by playerId, itemType, gameMode, etc.)
- ✅ Public leaderboard endpoint
- ✅ High score tracking
- ✅ Error handling with meaningful messages

## Testing Pagination

Try: `GET /api/players?page=1&pageSize=5`

Response headers include:
- X-Total-Count: Total number of records
- X-Page: Current page
- X-Page-Size: Items per page

## Testing Filtering

**Characters by class:**
`GET /api/characters?characterClass=Warrior`

**Items by type and rarity:**
`GET /api/items?itemType=Weapon&minRarity=7`

**Scores by game mode:**
`GET /api/scores?gameMode=Arena&highScoresOnly=true`

## Testing Leaderboard (Public)

This endpoint doesn't require authentication:
`GET /api/scores/leaderboard/Arena?top=10`

## Expected Response Codes

- 200 OK - Successful GET request
- 201 Created - Successful POST request
- 204 No Content - Successful PUT/DELETE request
- 400 Bad Request - Validation error
- 401 Unauthorized - Missing or invalid token
- 403 Forbidden - Don't have permission for this resource
- 404 Not Found - Resource doesn't exist

## Database File

The SQLite database is stored at: `GameAPI/gameapi.db`

You can inspect it using any SQLite browser tool if needed.

## Stopping the API

Press `Ctrl+C` in the terminal where the API is running.

---

**All tests passed! The API is fully functional and ready for demonstration.**
