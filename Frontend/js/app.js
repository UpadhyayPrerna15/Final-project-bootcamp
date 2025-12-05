// Main Application Logic
class GameApp {
    constructor() {
        this.currentSection = 'home';
        this.currentGameMode = 'Arena';
        this.players = [];
        
        this.initEventListeners();
        this.checkAuth();
        this.loadHomeStats();
        this.loadLeaderboard();
    }

    // ===== Initialization =====
    initEventListeners() {
        // Navigation
        document.querySelectorAll('.nav-link').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const section = e.target.getAttribute('href').substring(1);
                this.navigateToSection(section);
            });
        });

        // Auth buttons
        document.getElementById('loginBtn').addEventListener('click', () => this.showModal('loginModal'));
        document.getElementById('registerBtn').addEventListener('click', () => this.showModal('registerModal'));
        document.getElementById('heroRegisterBtn').addEventListener('click', () => this.showModal('registerModal'));
        document.getElementById('heroLeaderboardBtn').addEventListener('click', () => this.navigateToSection('leaderboard'));
        document.getElementById('logoutBtn').addEventListener('click', () => this.logout());

        // Modal switches
        document.getElementById('switchToRegister').addEventListener('click', (e) => {
            e.preventDefault();
            this.hideModal('loginModal');
            this.showModal('registerModal');
        });
        document.getElementById('switchToLogin').addEventListener('click', (e) => {
            e.preventDefault();
            this.hideModal('registerModal');
            this.showModal('loginModal');
        });

        // Close modals
        document.querySelectorAll('.close').forEach(closeBtn => {
            closeBtn.addEventListener('click', (e) => {
                const modalId = e.target.getAttribute('data-modal');
                this.hideModal(modalId);
            });
        });

        // Close modal on outside click
        window.addEventListener('click', (e) => {
            if (e.target.classList.contains('modal')) {
                this.hideModal(e.target.id);
            }
        });

        // Forms
        document.getElementById('loginForm').addEventListener('submit', (e) => this.handleLogin(e));
        document.getElementById('registerForm').addEventListener('submit', (e) => this.handleRegister(e));
        document.getElementById('createPlayerForm').addEventListener('submit', (e) => this.handleCreatePlayer(e));
        document.getElementById('createCharacterForm').addEventListener('submit', (e) => this.handleCreateCharacter(e));
        document.getElementById('submitScoreForm').addEventListener('submit', (e) => this.handleSubmitScore(e));

        // Dashboard actions
        document.getElementById('createPlayerBtn').addEventListener('click', () => this.showModal('createPlayerModal'));
        document.getElementById('createCharacterBtn').addEventListener('click', () => {
            this.loadPlayerDropdown();
            this.showModal('createCharacterModal');
        });
        document.getElementById('submitScoreBtn').addEventListener('click', () => {
            this.loadPlayerDropdown();
            this.showModal('submitScoreModal');
        });

        // Leaderboard tabs
        document.querySelectorAll('.tab-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
                e.target.classList.add('active');
                this.currentGameMode = e.target.getAttribute('data-mode');
                this.loadLeaderboard();
            });
        });

        // Score difficulty slider
        const difficultySlider = document.getElementById('scoreDifficulty');
        const difficultyValue = document.getElementById('difficultyValue');
        difficultySlider.addEventListener('input', (e) => {
            difficultyValue.textContent = e.target.value;
        });
    }

    // ===== Authentication =====
    checkAuth() {
        if (api.isAuthenticated()) {
            this.showAuthenticatedUI();
        } else {
            this.showUnauthenticatedUI();
        }
    }

    showAuthenticatedUI() {
        document.getElementById('loginBtn').style.display = 'none';
        document.getElementById('registerBtn').style.display = 'none';
        document.getElementById('userMenu').style.display = 'flex';
        document.getElementById('dashboardLink').style.display = 'block';
        document.getElementById('userName').textContent = api.user.username;
    }

    showUnauthenticatedUI() {
        document.getElementById('loginBtn').style.display = 'block';
        document.getElementById('registerBtn').style.display = 'block';
        document.getElementById('userMenu').style.display = 'none';
        document.getElementById('dashboardLink').style.display = 'none';
    }

    async handleLogin(e) {
        e.preventDefault();
        const username = document.getElementById('loginUsername').value;
        const password = document.getElementById('loginPassword').value;

        try {
            await api.login(username, password);
            this.showToast('Login successful!', 'success');
            this.hideModal('loginModal');
            this.checkAuth();
            this.navigateToSection('dashboard');
            this.loadDashboard();
        } catch (error) {
            this.showToast(error.message, 'error');
        }
    }

    async handleRegister(e) {
        e.preventDefault();
        const username = document.getElementById('registerUsername').value;
        const email = document.getElementById('registerEmail').value;
        const password = document.getElementById('registerPassword').value;

        try {
            await api.register(username, email, password);
            this.showToast('Registration successful!', 'success');
            this.hideModal('registerModal');
            this.checkAuth();
            this.navigateToSection('dashboard');
            this.loadDashboard();
        } catch (error) {
            this.showToast(error.message, 'error');
        }
    }

    logout() {
        api.logout();
        this.checkAuth();
        this.navigateToSection('home');
        this.showToast('Logged out successfully', 'success');
    }

    // ===== Navigation =====
    navigateToSection(sectionId) {
        document.querySelectorAll('.section').forEach(section => {
            section.classList.remove('active');
        });
        document.querySelectorAll('.nav-link').forEach(link => {
            link.classList.remove('active');
        });

        document.getElementById(sectionId).classList.add('active');
        document.querySelector(`[href="#${sectionId}"]`)?.classList.add('active');
        this.currentSection = sectionId;

        if (sectionId === 'dashboard' && api.isAuthenticated()) {
            this.loadDashboard();
        } else if (sectionId === 'leaderboard') {
            this.loadLeaderboard();
        } else if (sectionId === 'dashboard' && !api.isAuthenticated()) {
            this.showToast('Please login to access dashboard', 'error');
            this.navigateToSection('home');
        }
    }

    // ===== Dashboard =====
    async loadDashboard() {
        await Promise.all([
            this.loadPlayers(),
            this.loadCharacters(),
            this.loadItems(),
            this.loadScores()
        ]);
    }

    async loadPlayers() {
        try {
            const players = await api.getPlayers();
            this.players = players;
            this.renderPlayers(players);
        } catch (error) {
            console.error('Error loading players:', error);
        }
    }

    renderPlayers(players) {
        const container = document.getElementById('playersList');
        if (players.length === 0) {
            container.innerHTML = '<p class="empty-state">No players yet. Create one to get started!</p>';
            return;
        }

        container.innerHTML = players.map(player => `
            <div class="item">
                <div class="item-header">
                    <span class="item-title">${player.name}</span>
                    <span class="item-badge">Lvl ${player.level}</span>
                </div>
                <div class="item-stats">
                    <span>⚔️ XP: ${player.experience}</span>
                    <span>💰 ${player.gold}</span>
                    <span>❤️ ${player.health}/${player.maxHealth}</span>
                    <span>✨ ${player.mana}/${player.maxMana}</span>
                </div>
            </div>
        `).join('');
    }

    async loadCharacters() {
        try {
            const characters = await api.getCharacters();
            this.renderCharacters(characters);
        } catch (error) {
            console.error('Error loading characters:', error);
        }
    }

    renderCharacters(characters) {
        const container = document.getElementById('charactersList');
        if (characters.length === 0) {
            container.innerHTML = '<p class="empty-state">No characters yet.</p>';
            return;
        }

        container.innerHTML = characters.map(char => `
            <div class="item">
                <div class="item-header">
                    <span class="item-title">${char.name}</span>
                    <span class="item-badge badge-${char.characterClass.toLowerCase()}">${char.characterClass}</span>
                </div>
                <div class="item-stats">
                    <span>Lvl ${char.level}</span>
                    <span>💪 ${char.strength}</span>
                    <span>🧠 ${char.intelligence}</span>
                    <span>🎯 ${char.dexterity}</span>
                    <span>❤️ ${char.health}/${char.maxHealth}</span>
                </div>
            </div>
        `).join('');
    }

    async loadItems() {
        try {
            const items = await api.getItems();
            this.renderItems(items);
        } catch (error) {
            console.error('Error loading items:', error);
        }
    }

    renderItems(items) {
        const container = document.getElementById('itemsList');
        if (items.length === 0) {
            container.innerHTML = '<p class="empty-state">No items in inventory.</p>';
            return;
        }

        container.innerHTML = items.map(item => `
            <div class="item">
                <div class="item-header">
                    <span class="item-title">${item.name}</span>
                    <span class="item-badge">⭐ ${item.rarity}</span>
                </div>
                <div class="item-stats">
                    <span>${item.itemType}</span>
                    ${item.attackBonus > 0 ? `<span>⚔️ +${item.attackBonus}</span>` : ''}
                    ${item.defenseBonus > 0 ? `<span>🛡️ +${item.defenseBonus}</span>` : ''}
                    <span>x${item.quantity}</span>
                </div>
            </div>
        `).join('');
    }

    async loadScores() {
        try {
            const scores = await api.getScores();
            this.renderScores(scores);
        } catch (error) {
            console.error('Error loading scores:', error);
        }
    }

    renderScores(scores) {
        const container = document.getElementById('scoresList');
        if (scores.length === 0) {
            container.innerHTML = '<p class="empty-state">No scores yet.</p>';
            return;
        }

        container.innerHTML = scores.slice(0, 5).map(score => `
            <div class="item">
                <div class="item-header">
                    <span class="item-title">${score.gameMode}</span>
                    ${score.isHighScore ? '<span class="item-badge" style="background: gold;">🏆 High Score</span>' : ''}
                </div>
                <div class="item-stats">
                    <span>📊 ${score.points} pts</span>
                    <span>⚔️ ${score.kills}K</span>
                    <span>💀 ${score.deaths}D</span>
                    <span>⏱️ ${Math.floor(score.timePlayed / 60)}m</span>
                </div>
            </div>
        `).join('');
    }

    // ===== Create Actions =====
    async handleCreatePlayer(e) {
        e.preventDefault();
        const name = document.getElementById('playerName').value;

        try {
            await api.createPlayer(name);
            this.showToast('Player created successfully!', 'success');
            this.hideModal('createPlayerModal');
            document.getElementById('createPlayerForm').reset();
            await this.loadPlayers();
        } catch (error) {
            this.showToast(error.message, 'error');
        }
    }

    async handleCreateCharacter(e) {
        e.preventDefault();
        const name = document.getElementById('characterName').value;
        const characterClass = document.getElementById('characterClass').value;
        const playerId = parseInt(document.getElementById('characterPlayerId').value);

        try {
            await api.createCharacter(name, characterClass, playerId);
            this.showToast('Character created successfully!', 'success');
            this.hideModal('createCharacterModal');
            document.getElementById('createCharacterForm').reset();
            await this.loadCharacters();
        } catch (error) {
            this.showToast(error.message, 'error');
        }
    }

    async handleSubmitScore(e) {
        e.preventDefault();
        const playerId = parseInt(document.getElementById('scorePlayerId').value);
        const gameMode = document.getElementById('scoreGameMode').value;
        const points = parseInt(document.getElementById('scorePoints').value);
        const kills = parseInt(document.getElementById('scoreKills').value);
        const deaths = parseInt(document.getElementById('scoreDeaths').value);
        const timePlayed = parseFloat(document.getElementById('scoreTime').value);
        const difficultyLevel = parseInt(document.getElementById('scoreDifficulty').value);

        try {
            await api.createScore({ playerId, gameMode, points, kills, deaths, timePlayed, difficultyLevel });
            this.showToast('Score submitted successfully!', 'success');
            this.hideModal('submitScoreModal');
            document.getElementById('submitScoreForm').reset();
            await this.loadScores();
            if (this.currentSection === 'leaderboard') {
                await this.loadLeaderboard();
            }
        } catch (error) {
            this.showToast(error.message, 'error');
        }
    }

    async loadPlayerDropdown() {
        try {
            const players = await api.getPlayers();
            const selects = ['characterPlayerId', 'scorePlayerId'];
            
            selects.forEach(selectId => {
                const select = document.getElementById(selectId);
                select.innerHTML = '<option value="">-- Select a Player --</option>' +
                    players.map(p => `<option value="${p.id}">${p.name} (Lvl ${p.level})</option>`).join('');
            });
        } catch (error) {
            console.error('Error loading players:', error);
        }
    }

    // ===== Leaderboard =====
    async loadLeaderboard() {
        try {
            const leaderboard = await api.getLeaderboard(this.currentGameMode, 20);
            this.renderLeaderboard(leaderboard);
        } catch (error) {
            console.error('Error loading leaderboard:', error);
        }
    }

    renderLeaderboard(leaderboard) {
        const container = document.getElementById('leaderboardList');
        if (leaderboard.length === 0) {
            container.innerHTML = '<p class="empty-state" style="text-align: center; padding: 2rem;">No scores for this game mode yet.</p>';
            return;
        }

        container.innerHTML = leaderboard.map(entry => `
            <div class="leaderboard-row">
                <div class="rank rank-${entry.rank}">#${entry.rank}</div>
                <div class="player-name">${entry.playerName}</div>
                <div class="score">${entry.points.toLocaleString()}</div>
                <div>${entry.kills}/${entry.deaths}</div>
                <div>${Math.floor(entry.timePlayed / 60)}m ${Math.floor(entry.timePlayed % 60)}s</div>
                <div>Difficulty ${entry.difficultyLevel}</div>
            </div>
        `).join('');
    }

    // ===== Home Stats =====
    async loadHomeStats() {
        try {
            // These endpoints would need to be public or we'd load them differently
            // For now, showing placeholder data
            document.getElementById('totalPlayers').textContent = '100+';
            document.getElementById('totalCharacters').textContent = '250+';
            document.getElementById('totalScores').textContent = '1000+';
        } catch (error) {
            console.error('Error loading stats:', error);
        }
    }

    // ===== Utility Methods =====
    showModal(modalId) {
        document.getElementById(modalId).classList.add('active');
    }

    hideModal(modalId) {
        document.getElementById(modalId).classList.remove('active');
    }

    showToast(message, type = 'success') {
        const toast = document.getElementById('toast');
        toast.textContent = message;
        toast.className = `toast ${type} show`;
        
        setTimeout(() => {
            toast.classList.remove('show');
        }, 3000);
    }
}

// Initialize app when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    window.gameApp = new GameApp();
});
