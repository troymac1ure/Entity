// WebSocket client for HaloCaster telemetry
class websocket_client {
    constructor() {
        this.socket = null;
        this.callbacks = {};
        this.connected = false;
    }

    async connect(url) {
        return new Promise((resolve, reject) => {
            try {
                this.socket = new WebSocket(url);

                this.socket.onopen = () => {
                    this.connected = true;
                    console.log('WebSocket connected to', url);
                    resolve();
                };

                this.socket.onclose = () => {
                    this.connected = false;
                    console.log('WebSocket disconnected');
                    if (this.callbacks['close']) {
                        this.callbacks['close']();
                    }
                };

                this.socket.onerror = (error) => {
                    console.error('WebSocket error:', error);
                    reject(error);
                };

                this.socket.onmessage = (event) => {
                    try {
                        const data = JSON.parse(event.data);
                        const messageType = data.type || data.message_type;

                        if (messageType && this.callbacks[messageType]) {
                            this.callbacks[messageType](data);
                        }
                    } catch (e) {
                        console.error('Error parsing message:', e);
                    }
                };
            } catch (error) {
                reject(error);
            }
        });
    }

    disconnect() {
        if (this.socket) {
            this.socket.close();
            this.socket = null;
            this.connected = false;
        }
    }

    send(data) {
        if (this.socket && this.connected) {
            this.socket.send(JSON.stringify(data));
        }
    }

    add_message_recieved_callback(messageType, callback) {
        this.callbacks[messageType] = callback;
    }

    remove_message_recieved_callback(messageType) {
        delete this.callbacks[messageType];
    }

    // Request team scoreboard data
    request_team_scoreboard(teamIndex) {
        this.send({
            type: 'get_team_scoreboard',
            team_index: teamIndex
        });
    }

    // Request player telemetry
    request_player_telemetry(playerName) {
        this.send({
            type: 'get_player_telemetry',
            player_name: playerName
        });
    }

    // Request all players
    request_all_players() {
        this.send({
            type: 'get_all_players'
        });
    }

    // Request kill feed
    request_kill_feed() {
        this.send({
            type: 'get_kill_feed'
        });
    }
}

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = websocket_client;
}
