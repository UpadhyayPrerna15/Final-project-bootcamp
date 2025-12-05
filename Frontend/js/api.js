// API Configuration
const API_BASE_URL = 'https://localhost:5001/api';

class GameAPI {
    constructor() {
        this.token = localStorage.getItem('token');
        this.user = JSON.parse(localStorage.getItem('user') || 'null');
    }

    // Helper method to get headers
    getHeaders(includeAuth = true) {
        const headers = {
            'Content-Type': 'application/json'
        };
        if (includeAuth && this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }
        return headers;
    }

    // Helper method for API calls
    async request(endpoint, options = {}) {
        try {
            const response = await fetch(`${API_BASE_URL}${endpoint}`, {
                ...options,
                headers: this.getHeaders(options.auth !== false)
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.error || 'API request failed');
            }

            return await response.json();
        } catch (error) {
            console.error('API Error:', error);
            throw error;
        }
    }

    // ===== Authentication =====
    async register(username, email, password) {
        const data = await this.request('/auth/register', {
            method: 'POST',
            auth: false,
            body: JSON.stringify({ username, email, password })
        });
        this.setAuth(data);
        return data;
    }

    async login(username, password) {
        const data = await this.request('/auth/login', {
            method: 'POST',
            auth: false,
            body: JSON.stringify({ username, password })
        });
        this.setAuth(data);
        return data;
    }

    setAuth(data) {
        this.token = data.token;
        this.user = {
            username: data.username,
            email: data.email,
            role: data.role
        };
        localStorage.setItem('token', this.token);
        localStorage.setItem('user', JSON.stringify(this.user));
    }

    logout() {
        this.token = null;
        this.user = null;
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    }

    isAuthenticated() {
        return !!this.token;
    }

    // ===== Players =====
    async getPlayers(page = 1, pageSize = 10) {
        return await this.request(`/players?page=${page}&pageSize=${pageSize}`);
    }

    async getPlayer(id) {
        return await this.request(`/players/${id}`);
    }

    async createPlayer(name) {
        return await this.request('/players', {
            method: 'POST',
            body: JSON.stringify({ name })
        });
    }

    async updatePlayer(id, data) {
        return await this.request(`/players/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deletePlayer(id) {
        return await this.request(`/players/${id}`, {
            method: 'DELETE'
        });
    }

    // ===== Characters =====
    async getCharacters(playerId = null) {
        const query = playerId ? `?playerId=${playerId}` : '';
        return await this.request(`/characters${query}`);
    }

    async getCharacter(id) {
        return await this.request(`/characters/${id}`);
    }

    async createCharacter(name, characterClass, playerId) {
        return await this.request('/characters', {
            method: 'POST',
            body: JSON.stringify({ name, characterClass, playerId })
        });
    }

    async updateCharacter(id, data) {
        return await this.request(`/characters/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deleteCharacter(id) {
        return await this.request(`/characters/${id}`, {
            method: 'DELETE'
        });
    }

    // ===== Items =====
    async getItems(playerId = null) {
        const query = playerId ? `?playerId=${playerId}` : '';
        return await this.request(`/items${query}`);
    }

    async getItem(id) {
        return await this.request(`/items/${id}`);
    }

    async createItem(data) {
        return await this.request('/items', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updateItem(id, data) {
        return await this.request(`/items/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deleteItem(id) {
        return await this.request(`/items/${id}`, {
            method: 'DELETE'
        });
    }

    // ===== Scores =====
    async getScores(playerId = null) {
        const query = playerId ? `?playerId=${playerId}` : '';
        return await this.request(`/scores${query}`);
    }

    async getScore(id) {
        return await this.request(`/scores/${id}`);
    }

    async createScore(data) {
        return await this.request('/scores', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async deleteScore(id) {
        return await this.request(`/scores/${id}`, {
            method: 'DELETE'
        });
    }

    async getLeaderboard(gameMode, top = 10) {
        return await this.request(`/scores/leaderboard/${gameMode}?top=${top}`, {
            auth: false
        });
    }
}

// Export API instance
const api = new GameAPI();
