# Deploy H2 Emblem Service to VPS

## Quick Deploy

SSH into your VPS:
```bash
ssh root@104.207.143.249
```

Check what ports are in use:
```bash
netstat -tlnp | grep LISTEN
# or
ss -tlnp
```

Create the service directory:
```bash
mkdir -p /opt/h2emblem
cd /opt/h2emblem
```

Copy the files (server.js and package.json) to the server, or create them:
```bash
nano server.js   # paste the server.js content
nano package.json  # paste package.json content
```

Install and run:
```bash
# Install Node.js if not present
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
apt-get install -y nodejs

# Start the service
cd /opt/h2emblem
npm start
```

## Run as systemd service (recommended)

Create service file:
```bash
cat > /etc/systemd/system/h2emblem.service << 'EOF'
[Unit]
Description=Halo 2 Emblem Service
After=network.target

[Service]
Type=simple
User=root
WorkingDirectory=/opt/h2emblem
ExecStart=/usr/bin/node server.js
Restart=on-failure
Environment=PORT=3001

[Install]
WantedBy=multi-user.target
EOF
```

Enable and start:
```bash
systemctl daemon-reload
systemctl enable h2emblem
systemctl start h2emblem
systemctl status h2emblem
```

## Open firewall port

```bash
# UFW
ufw allow 3001/tcp

# or iptables
iptables -A INPUT -p tcp --dport 3001 -j ACCEPT
```

## Test

```bash
curl "http://104.207.143.249:3001/health"
curl "http://104.207.143.249:3001/?emblem=1&fg=1&bg=1&pri=red&sec=blue"
```

## Update Entity to use your service

In BSPViewer.cs, change the emblem URL from:
```
https://h2emblem.carnagereport.workers.dev/
```
to:
```
http://104.207.143.249:3001/
```
